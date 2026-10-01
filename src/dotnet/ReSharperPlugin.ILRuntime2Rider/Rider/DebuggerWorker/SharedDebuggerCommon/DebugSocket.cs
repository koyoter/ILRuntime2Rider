using System;
using System.IO;
using System.Net.Sockets;
using ILRuntime.Runtime.Debugger.Protocol;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// Client side of the ILRuntime debug wire protocol, byte-compatible with the
/// ILRuntime debug server.
/// </summary>
/// <remarks>
/// Replaces <c>ILRuntime.Runtime.Debugger.DebugSocket</c> (kept inside the bundled
/// ILRuntime.dll): the original assigns its socket field <i>after</i> BeginConnect,
/// so on .NET - where the connect callback can fire inline for fast local handshakes -
/// it dereferences the still-null field and throws out of the DebuggedProcess
/// constructor. This implementation assigns before initiating the connect, never
/// throws into the caller and always reports exactly one terminal event
/// (OnConnect, OnConnectFailed or OnClose).
/// </remarks>
public class DebugSocket
{
    private const int HEAD_SIZE = 8;
    private const int MAX_MESSAGE_SIZE = 256 * 1024;
    private const int RECV_BUFFER_SIZE = 16 * 1024;

    private readonly object _syncRoot = new();
    private readonly byte[] _sendBuffer = new byte[64 * 1024];
    private readonly MemoryStream _sendStream = new(_sendBuffer);
    private readonly BinaryWriter _sendWriter;
    private byte[] _receiveAccumulator = new byte[64 * 1024];
    private int _receiveCount;
    private byte[] _socketBuffer = new byte[RECV_BUFFER_SIZE];
    private Socket? _socket;
    private bool _ready;

    public Action? OnConnect { get; set; }
    public Action? OnConnectFailed { get; set; }
    public Action? OnClose { get; set; }
    public Action<DebugMessageType, byte[]>? OnReciveMessage { get; set; }

    public DebugSocket()
    {
        _sendWriter = new BinaryWriter(_sendStream);
    }

    public void Connect(string ip, int port)
    {
        lock (_syncRoot)
        {
            CloseCore();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket = socket; // assign BEFORE BeginConnect: the callback may fire inline
            _ready = false;
            try
            {
                socket.BeginConnect(ip, port, OnConnected, null);
            }
            catch (Exception)
            {
                DisposeSilently();
                OnConnectFailed?.Invoke();
            }
        }
    }

    public void Send(DebugMessageType type, byte[] buffer, int len)
    {
        Socket? socket;
        int total;
        lock (_syncRoot)
        {
            if (!_ready) return;
            socket = _socket;
            if (socket == null) return;
            _sendStream.Position = 0;
            _sendWriter.Write(len + HEAD_SIZE);
            _sendWriter.Write((int)type);
            _sendWriter.Write(buffer, 0, len);
            total = (int)_sendStream.Position;
        }

        try
        {
            socket.Send(_sendBuffer, 0, total, SocketFlags.None);
        }
        catch (Exception)
        {
            Close();
        }
    }

    public void Close()
    {
        lock (_syncRoot)
        {
            CloseCore();
        }
    }

    /// <summary>
    /// Closes the socket. Caller must hold <see cref="_syncRoot"/>. Reports OnClose
    /// only when a live socket actually existed (never-connected sockets stay silent).
    /// </summary>
    private void CloseCore()
    {
        var socket = _socket;
        _socket = null;
        _ready = false;
        _receiveCount = 0;
        if (socket == null) return;
        try
        {
            socket.Dispose();
        }
        catch (Exception)
        {
            // already gone
        }
        OnClose?.Invoke();
    }

    /// <summary>Drops a socket that never became usable; reports no events.</summary>
    private void DisposeSilently()
    {
        var socket = _socket;
        _socket = null;
        _ready = false;
        _receiveCount = 0;
        try
        {
            socket?.Dispose();
        }
        catch (Exception)
        {
            // already gone
        }
    }

    private void OnConnected(IAsyncResult asyncResult)
    {
        Socket? socket;
        lock (_syncRoot)
        {
            socket = _socket;
            if (socket == null) return; // closed while connecting
            try
            {
                socket.EndConnect(asyncResult);
            }
            catch (Exception)
            {
                DisposeSilently();
                OnConnectFailed?.Invoke();
                return;
            }

            _ready = true;
        }

        BeginReceive(socket);
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_socket, socket))
                return; // closed while starting the receive loop; OnClose already reported
        }
        OnConnect?.Invoke();
    }

    private void BeginReceive(Socket socket)
    {
        var buffer = _socketBuffer;
        try
        {
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceived, socket);
        }
        catch (Exception)
        {
            Close();
        }
    }

    private void OnReceived(IAsyncResult asyncResult)
    {
        var socket = (Socket)asyncResult.AsyncState!;
        int received;
        try
        {
            received = socket.EndReceive(asyncResult);
        }
        catch (Exception)
        {
            // Receive failure means the connection died; CloseCore is idempotent, so a
            // concurrent Close already having removed the socket reports nothing extra.
            Close();
            return;
        }

        if (received <= 0)
        {
            Close();
            return;
        }

        try
        {
            ReceivePayload(_socketBuffer, received);
        }
        catch (Exception)
        {
            Close();
            return;
        }

        lock (_syncRoot)
        {
            if (ReferenceEquals(_socket, socket) && _ready)
                BeginReceive(socket);
        }
    }

    /// <summary>
    /// Wire framing: [int32 totalLength][int32 messageType][payload], where
    /// totalLength counts itself. Buffers partial frames between calls.
    /// </summary>
    private void ReceivePayload(byte[] data, int count)
    {
        if (_receiveCount + count > _receiveAccumulator.Length)
            Array.Resize(ref _receiveAccumulator, Math.Max(_receiveAccumulator.Length * 2, _receiveCount + count));
        Buffer.BlockCopy(data, 0, _receiveAccumulator, _receiveCount, count);
        _receiveCount += count;

        var offset = 0;
        while (_receiveCount - offset >= HEAD_SIZE)
        {
            var total = BitConverter.ToInt32(_receiveAccumulator, offset);
            if (total < HEAD_SIZE || total > MAX_MESSAGE_SIZE)
                throw new InvalidDataException($"Invalid ILRuntime debug frame length: {total}");
            if (_receiveCount - offset < total) break;

            var messageType = (DebugMessageType)BitConverter.ToInt32(_receiveAccumulator, offset + 4);
            var payloadLength = total - HEAD_SIZE;
            var payload = new byte[payloadLength];
            Buffer.BlockCopy(_receiveAccumulator, offset + HEAD_SIZE, payload, 0, payloadLength);
            OnReciveMessage?.Invoke(messageType, payload);
            offset += total;
        }

        if (offset <= 0) return;
        Buffer.BlockCopy(_receiveAccumulator, offset, _receiveAccumulator, 0, _receiveCount - offset);
        _receiveCount -= offset;
    }
}
