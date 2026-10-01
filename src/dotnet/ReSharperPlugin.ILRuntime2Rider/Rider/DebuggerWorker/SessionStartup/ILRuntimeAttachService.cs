using System;
using JetBrains.Debugger.Model.Plugins.ILRuntime2Rider;
using Mono.Debugging.Client.Events;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

/// <summary>
/// Owns the single attach connection to an ILRuntime host. A worker process attaches
/// exactly once; <see cref="Connect"/> is idempotent while a connection attempt or an
/// established session is in flight.
/// </summary>
public class ILRuntimeAttachService
{
    private enum State
    {
        None,
        Connecting,
        Connected,
    }

    public static readonly ILRuntimeAttachService Instance = new();

    private readonly object _stateLock = new();
    private State _state = State.None;
    private Process _process = null!;

    public event Action? OnDisconnect;
    public event Action<bool, IDebuggedProcess?>? OnConnectResult;
    public event Action<string, NotificationType, NotificationSubject>? OnMessage;

    public bool IsNone
    {
        get
        {
            lock (_stateLock) return _state == State.None;
        }
    }

    // Port is deliberately NOT compared: killing and restarting Unity yields a new
    // debug port (56000 + PID), and the session should reattach to it. A hot-reload
    // domain reload keeps the same process and port, so it matches either way.
    public bool IsSameProcess(Process process)
    {
        lock (_stateLock)
        {
            return _process != null
                && _process.Host == process.Host
                && _process.ProjectName == process.ProjectName;
        }
    }

    public void Connect(ILRuntimeDebuggerSession debuggerSession, Process process)
    {
        lock (_stateLock)
        {
            if (_state != State.None) return;
            _process = process;
            _state = State.Connecting;
        }

        var hostInfo = $"{process.Host}:{process.Port}";
        DebuggedProcessRider debuggedProcess;
        try
        {
            debuggedProcess = new DebuggedProcessRider(debuggerSession, process.Host, process.Port);
            debuggedProcess.OnDisconnected = () =>
            {
                lock (_stateLock) _state = State.None;
                OnDisconnect?.Invoke();
                OnMessage?.Invoke($"Disconnected {hostInfo}", NotificationType.Error,
                    NotificationSubject.ConnectionError);
            };

            OnMessage?.Invoke($"Session connecting {hostInfo}...", NotificationType.Info,
                NotificationSubject.Default);
            debuggedProcess.WaitForConnect();
        }
        catch (Exception ex)
        {
            // Defense in depth: the bundled DebugSocket is race-free, but any leftover
            // construction failure must not escape into session startup (the worker would
            // exit gracefully and the frontend would report the session as terminated).
            lock (_stateLock)
            {
                _state = State.None;
                _process = null; // stop Signal-driven auto reconnect after a failed attach
            }
            OnMessage?.Invoke($"ILRuntime attach failed: {hostInfo} ({ex.Message})",
                NotificationType.Error, NotificationSubject.ConnectionError);
            OnConnectResult?.Invoke(false, null);
            return;
        }

        if (!debuggedProcess.Connected)
        {
            lock (_stateLock)
            {
                _state = State.None;
                _process = null;
            }
            OnMessage?.Invoke($"ILRuntime attach failed: {hostInfo}", NotificationType.Error,
                NotificationSubject.ConnectionError);
            OnConnectResult?.Invoke(false, null);
            return;
        }

        if (!debuggedProcess.CheckDebugServerVersion())
        {
            // The ILRuntime server accepts a single connection; leaving this one open
            // would block every later attach (30s attach-reply timeouts) until Unity
            // restarts. Close so the slot frees immediately.
            debuggedProcess.Close();
            lock (_stateLock)
            {
                _state = State.None;
                _process = null;
            }
            OnMessage?.Invoke($"ILRuntime attach failed: {hostInfo}", NotificationType.Error,
                NotificationSubject.ConnectionError);
            OnConnectResult?.Invoke(false, null);
        }
        else
        {
            lock (_stateLock) _state = State.Connected;
            OnMessage?.Invoke($"ILRuntime attach succeeded: {hostInfo}", NotificationType.Info,
                NotificationSubject.Default);
            OnConnectResult?.Invoke(true, debuggedProcess);
        }
    }
}
