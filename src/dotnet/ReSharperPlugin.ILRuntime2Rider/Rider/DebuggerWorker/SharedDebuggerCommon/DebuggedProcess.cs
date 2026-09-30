using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ILRuntime.Runtime.Debugger;
using ILRuntime.Runtime.Debugger.Expressions;
using ILRuntime.Runtime.Debugger.Protocol;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// Client side of the ILRuntime debug wire protocol. Owns the debug socket,
/// serializes requests, dispatches server events (breakpoint hits, thread
/// lifetime, module loads) and blocks for synchronous RPC replies.
/// The wire format must stay byte-compatible with the ILRuntime debug server.
/// </summary>
public abstract class DebuggedProcess
{
    private readonly string BreakpointConditionErrorMessage =
        "不能设置下面的断点:" +
        Environment.NewLine +
        "于{0}, 行{1}字符{2}, 条件是\"{3}\"为true" +
        Environment.NewLine +
        "断点的条件未能执行。错误是\"{4}\"。";

    private readonly ConcurrentDictionary<int, ILRuntimeBreakpointBase> breakpoints = new();
    private readonly BinaryWriter writer;
    private bool closed;
    private bool rpcCompleted;
    private object rpcResult;
    private bool rpcStarted;
    private readonly MemoryStream sendStream = new(64 * 1024);
    private readonly DebugSocket socket;

    // Socket thread signals completion; session threads wait on these instead of polling.
    private readonly ManualResetEventSlim connectCompleted = new(false);
    private readonly ManualResetEventSlim attachCompleted = new(false);
    private readonly AutoResetEvent rpcReplyEvent = new(false);

    public DebuggedProcess(string host, int port)
    {
        writer = new BinaryWriter(sendStream);
        socket = new DebugSocket();
        socket.OnConnect = OnConnected;
        socket.OnConnectFailed = OnConnectFailed;
        socket.OnReciveMessage = OnReceiveMessage;
        socket.OnClose = OnClose;
        Connecting = true;
        socket.Connect(host, port);
    }

    public ConcurrentDictionary<int, IThread> Threads { get; } = new();

    public Action OnDisconnected { get; set; }

    public bool Connected { get; set; }

    public bool ConnectFailed { get; set; }

    public bool Connecting { get; set; }

    public int RemoteDebugVersion { get; set; }

    private void OnConnected()
    {
        Connected = true;
        Connecting = false;
        connectCompleted.Set();
    }

    private void OnConnectFailed()
    {
        ConnectFailed = true;
        Connecting = false;
        connectCompleted.Set();
    }

    /// <summary>
    /// Blocks until the socket connect attempt completes (success or failure).
    /// Check <see cref="Connected"/> afterwards to tell which one happened.
    /// </summary>
    public void WaitForConnect()
    {
        connectCompleted.Wait();
    }

    public void Close()
    {
        socket.OnClose = null;
        socket.Close();
    }

    private void OnClose()
    {
        closed = true;
        connectCompleted.Set();
        attachCompleted.Set();
        rpcReplyEvent.Set();
        OnDisconnected?.Invoke();
    }

    public bool CheckDebugServerVersion()
    {
        socket.Send(DebugMessageType.CSAttach, new byte[0], 0);
        // The attach reply also wakes this wait when the connection drops (OnClose).
        if (!attachCompleted.Wait(30000) || !Connected) return false;

        return RemoteDebugVersion == DebuggerServer.Version;
    }

    /// <summary>
    /// Blocks the calling thread until the pending RPC reply arrives, the caller
    /// timeout elapses or the connection dies. Only one RPC may be awaited at a time.
    /// </summary>
    public T AwaitRPCRequest<T>(out bool aborted, int timeout = 0)
    {
        aborted = false;
        if (rpcStarted)
            throw new NotSupportedException("Nested RPC requests are not supported.");

        rpcCompleted = false;
        rpcResult = null;
        rpcStarted = true;
        var rpcStartTime = DateTime.Now;
        while (!rpcCompleted)
        {
            if (timeout > 0 && (DateTime.Now - rpcStartTime).TotalMilliseconds > timeout)
            {
                rpcCompleted = true;
                rpcResult = null;
                rpcStarted = false;
                aborted = true;
                break;
            }

            if (closed)
            {
                rpcCompleted = true;
                rpcResult = null;
                rpcStarted = false;
                aborted = true;
                break;
            }

            // Wakes immediately on the reply; the 10ms cap keeps the closed
            // check responsive without spinning.
            rpcReplyEvent.WaitOne(10);
        }

        return (T)rpcResult;
    }

    private void CompleteRPCRequest(object result)
    {
        if (rpcStarted)
        {
            rpcResult = result;
            rpcCompleted = true;
            rpcStarted = false;
            rpcReplyEvent.Set();
        }
    }

    private void OnReceiveMessage(DebugMessageType type, byte[] buffer)
    {
        using (var ms = new MemoryStream(buffer))
        {
            using (var reader = new BinaryReader(ms))
            {
                switch (type)
                {
                    case DebugMessageType.SCAttachResult:
                    {
                        var result = new SCAttachResult();
                        result.Result = (AttachResults)reader.ReadByte();
                        result.DebugServerVersion = reader.ReadInt32();
                        RemoteDebugVersion = result.DebugServerVersion;
                        attachCompleted.Set();
                    }
                        break;
                    case DebugMessageType.SCBindBreakpointResult:
                    {
                        var msg = new SCBindBreakpointResult();
                        msg.BreakpointHashCode = reader.ReadInt32();
                        msg.Result = (BindBreakpointResults)reader.ReadByte();
                        OnBindBreakpointResult(msg);
                    }
                        break;
                    case DebugMessageType.SCBreakpointHit:
                    {
                        var msg = new SCBreakpointHit();
                        msg.BreakpointHashCode = reader.ReadInt32();
                        msg.ThreadHashCode = reader.ReadInt32();
                        msg.StackFrame = ReadStackFrames(reader);
                        OnBreakpointHit(msg, reader.ReadString());
                    }
                        break;
                    case DebugMessageType.SCStepComplete:
                    {
                        var msg = new SCStepComplete();
                        msg.ThreadHashCode = reader.ReadInt32();
                        msg.StackFrame = ReadStackFrames(reader);
                        OnStepComplete(msg);
                    }
                        break;
                    case DebugMessageType.SCThreadStarted:
                    {
                        var msg = new SCThreadStarted();
                        msg.ThreadHashCode = reader.ReadInt32();
                        OnThreadStarted(msg);
                    }
                        break;
                    case DebugMessageType.SCThreadEnded:
                    {
                        var msg = new SCThreadEnded();
                        msg.ThreadHashCode = reader.ReadInt32();
                        OnThreadEnded(msg);
                    }
                        break;
                    case DebugMessageType.SCModuleLoaded:
                    {
                        var msg = new SCModuleLoaded();
                        msg.ModuleName = reader.ReadString();
                        OnModuleLoaded(msg);
                    }
                        break;
                    case DebugMessageType.SCResolveVariableResult:
                    {
                        CompleteRPCRequest(ReadVariableInfo(reader));
                    }
                        break;
                    case DebugMessageType.SCEnumChildrenResult:
                    {
                        var count = reader.ReadInt32();
                        var result = new VariableInfo[count];
                        for (var i = 0; i < count; i++) result[i] = ReadVariableInfo(reader);
                        CompleteRPCRequest(result);
                    }
                        break;
                }
            }
        }
    }

    private KeyValuePair<int, StackFrameInfo[]>[] ReadStackFrames(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        var result = new KeyValuePair<int, StackFrameInfo[]>[length];
        for (var i = 0; i < length; i++)
        {
            var key = reader.ReadInt32();
            var count = reader.ReadInt32();
            var frames = new StackFrameInfo[count + 1];
            for (var j = 0; j < count; j++)
            {
                var info = new StackFrameInfo();
                info.MethodName = reader.ReadString();
                info.DocumentName = reader.ReadString();
                info.StartLine = reader.ReadInt32();
                info.StartColumn = reader.ReadInt32();
                info.EndLine = reader.ReadInt32();
                info.EndColumn = reader.ReadInt32();
                info.ArgumentCount = reader.ReadInt32();
                var variableCount = reader.ReadInt32();
                info.LocalVariables = new VariableInfo[variableCount];
                for (var k = 0; k < variableCount; k++) info.LocalVariables[k] = ReadVariableInfo(reader);
                frames[j] = info;
            }

            frames[count] = new StackFrameInfo
            {
                MethodName = "Transition to Native methods"
            };
            result[i] = new KeyValuePair<int, StackFrameInfo[]>(key, frames);
        }

        return result;
    }

    private VariableInfo ReadVariableInfo(BinaryReader reader)
    {
        var info = new VariableInfo();
        info.Address = reader.ReadInt64();
        info.Type = (VariableTypes)reader.ReadByte();
        info.Offset = reader.ReadInt32();
        info.Name = reader.ReadString();
        info.Value = reader.ReadString();
        info.ValueType = (ValueTypes)reader.ReadByte();
        info.TypeName = reader.ReadString();
        info.Expandable = reader.ReadBoolean();
        info.IsPrivate = reader.ReadBoolean();
        info.IsProtected = reader.ReadBoolean();

        return info;
    }

    public virtual void AddPendingBreakpoint(ILRuntimeBreakpointBase bp)
    {
        breakpoints[bp.GetHashCode()] = bp;
    }

    public void SendBindBreakpoint(CSBindBreakpoint msg)
    {
        sendStream.Position = 0;
        writer.Write(msg.BreakpointHashCode);
        writer.Write(msg.IsLambda);
        writer.Write(msg.NamespaceName);
        writer.Write(msg.TypeName);
        writer.Write(msg.MethodName);
        writer.Write(msg.StartLine);
        writer.Write(msg.EndLine);
        writer.Write(msg.Enabled);
        writer.Write((byte)msg.Condition.Style);
        if (msg.Condition.Style != BreakpointConditionStyle.None)
            writer.Write(msg.Condition.Expression);
        writer.Write(msg.UsingInfos.Length);
        foreach (var usingInfo in msg.UsingInfos)
        {
            writer.Write(usingInfo.Alias);
            writer.Write(usingInfo.Name);
        }

        socket.Send(DebugMessageType.CSBindBreakpoint, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public void SendSetBreakpointCondition(int bpHash, BreakpointConditionStyle style, string expression)
    {
        sendStream.Position = 0;
        writer.Write(bpHash);
        writer.Write((byte)style);
        if (style != BreakpointConditionStyle.None)
            writer.Write(expression);
        socket.Send(DebugMessageType.CSSetBreakpointCondition, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public void SendSetBreakpointEnabled(int bpHash, bool enabled)
    {
        sendStream.Position = 0;
        writer.Write(bpHash);
        writer.Write(enabled);
        socket.Send(DebugMessageType.CSSetBreakpointEnabled, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public void SendDeleteBreakpoint(int bpHash)
    {
        breakpoints.TryRemove(bpHash, out _);
        sendStream.Position = 0;
        writer.Write(bpHash);
        socket.Send(DebugMessageType.CSDeleteBreakpoint, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public void SendExecute(int threadHash)
    {
        sendStream.Position = 0;
        writer.Write(threadHash);
        socket.Send(DebugMessageType.CSExecute, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public void SendStep(int threadHash, StepTypes type)
    {
        sendStream.Position = 0;
        writer.Write(threadHash);
        writer.Write((byte)type);
        socket.Send(DebugMessageType.CSStep, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    private void OnBindBreakpointResult(SCBindBreakpointResult msg)
    {
        if (breakpoints.TryGetValue(msg.BreakpointHashCode, out var bp)) bp.Bound(msg.Result);
    }

    private void OnModuleLoaded(SCModuleLoaded msg)
    {
        HandleModuleLoaded(msg.ModuleName);
        foreach (var pair in breakpoints)
            if (!pair.Value.IsBound)
                pair.Value.TryBind();
    }

    protected abstract void HandleModuleLoaded(string moduleName);

    public VariableInfo ResolveVariable(VariableReference parent, string name, int threadId, int frameId,
        uint timeout)
    {
        return ResolveVariable(VariableReference.GetMember(name, parent), threadId, frameId, timeout);
    }

    public VariableInfo ResolveVariable(VariableReference variable, int threadId, int frameId, uint timeout)
    {
        var msg = new CSResolveVariable();
        msg.ThreadHashCode = threadId;
        msg.FrameIndex = frameId;
        msg.Variable = variable;
        SendResolveVariable(msg);

        var result = AwaitRPCRequest<VariableInfo>(out var aborted, (int)timeout);
        if (aborted)
            return VariableInfo.RequestTimeout;
        return result;
    }

    private void SendResolveVariable(CSResolveVariable msg)
    {
        sendStream.Position = 0;
        writer.Write(msg.ThreadHashCode);
        writer.Write(msg.FrameIndex);
        WriteVariableReference(msg.Variable);
        socket.Send(DebugMessageType.CSResolveVariable, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public VariableInfo ResolveIndexAccess(VariableReference body, VariableReference index, int threadId,
        int frameId, uint timeout)
    {
        var msg = new CSResolveIndexer();
        msg.Body = body;
        msg.Index = index;
        msg.ThreadHashCode = threadId;
        msg.FrameIndex = frameId;
        SendResolveIndexAccess(msg);

        var result = AwaitRPCRequest<VariableInfo>(out var aborted, (int)timeout);
        if (aborted)
            return VariableInfo.RequestTimeout;
        return result;
    }

    private void SendResolveIndexAccess(CSResolveIndexer msg)
    {
        sendStream.Position = 0;
        writer.Write(msg.ThreadHashCode);
        writer.Write(msg.FrameIndex);
        WriteVariableReference(msg.Body);
        WriteVariableReference(msg.Index);
        socket.Send(DebugMessageType.CSResolveIndexAccess, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    public VariableInfo[] EnumChildren(VariableReference parent, int threadId, int frameId, uint timeout)
    {
        var msg = new CSEnumChildren();
        msg.ThreadHashCode = threadId;
        msg.FrameIndex = frameId;
        msg.Parent = parent;
        SendEnumChildren(msg);

        var result = AwaitRPCRequest<VariableInfo[]>(out var aborted, (int)timeout);
        if (aborted)
            return new[] { VariableInfo.RequestTimeout };
        return result;
    }

    private void SendEnumChildren(CSEnumChildren msg)
    {
        sendStream.Position = 0;
        writer.Write(msg.ThreadHashCode);
        writer.Write(msg.FrameIndex);
        WriteVariableReference(msg.Parent);
        socket.Send(DebugMessageType.CSEnumChildren, sendStream.GetBuffer(), (int)sendStream.Position);
    }

    private void WriteVariableReference(VariableReference reference)
    {
        writer.Write(reference != null);
        if (reference != null)
        {
            writer.Write(reference.Address);
            writer.Write((byte)reference.Type);
            writer.Write(reference.Offset);
            writer.Write(reference.Name);
            WriteVariableReference(reference.Parent);
            if (reference.Parameters != null)
            {
                writer.Write(reference.Parameters.Length);
                foreach (var parameter in reference.Parameters) WriteVariableReference(parameter);
            }
            else
            {
                writer.Write(0);
            }
        }
    }

    protected abstract void HandleBreakpointHit(ILRuntimeBreakpointBase bp, IThread bpThread);

    protected abstract void HandleBreakpointConditionError(string errorMsg);

    protected abstract void HandleStepComplete(IThread bpThread);

    protected abstract IThread HandleThreadStarted(int threadHash);

    protected abstract void HandleThreadEnded(IThread thread);

    private void OnBreakpointHit(SCBreakpointHit msg, string error)
    {
        ILRuntimeBreakpointBase bp;
        IThread t, bpThread = null;
        if (breakpoints.TryGetValue(msg.BreakpointHashCode, out bp))
        {
            foreach (var i in msg.StackFrame)
                if (Threads.TryGetValue(i.Key, out t))
                {
                    t.StackFrames = i.Value;
                    if (i.Key == msg.ThreadHashCode)
                        bpThread = t;
                }

            if (bpThread != null)
            {
                HandleBreakpointHit(bp, bpThread);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    var errorMsg = string.Format(BreakpointConditionErrorMessage,
                        Path.GetFileName(bp.DocumentName),
                        bp.StartLine + 1, bp.StartColumn, bp.ConditionExpression, error);
                    HandleBreakpointConditionError(errorMsg);
                }
            }
        }
    }

    /// <summary>
    /// Diagnostic output routed to the debugger console (Debug Output tab) by
    /// the session-level subclass; worker stdout is not forwarded anywhere.
    /// </summary>
    public Action<string> DiagnosticsSink { get; set; }

    private void ReportDiagnostics(string message)
    {
        Console.WriteLine("[ILRuntime] " + message);
        DiagnosticsSink?.Invoke("[ILRuntime] " + message);
    }

    private void OnStepComplete(SCStepComplete msg)
    {
        IThread t, bpThread = null;

        ReportDiagnostics($"SCStepComplete thread={msg.ThreadHashCode} frames=[{string.Join(",", msg.StackFrame.Select(f => f.Key))}] knownThreads=[{string.Join(",", Threads.Keys)}]");

        foreach (var i in msg.StackFrame)
            if (Threads.TryGetValue(i.Key, out t))
            {
                t.StackFrames = i.Value;
                if (i.Key == msg.ThreadHashCode)
                    bpThread = t;
            }

        if (bpThread != null)
        {
            ReportDiagnostics($"Step complete on thread {bpThread.ThreadID}, dispatching to session");
            HandleStepComplete(bpThread);
        }
        else
        {
            ReportDiagnostics($"Step complete DROPPED: thread {msg.ThreadHashCode} is not registered");
        }
    }

    private void OnThreadStarted(SCThreadStarted msg)
    {
        var thread = HandleThreadStarted(msg.ThreadHashCode);
        Threads[msg.ThreadHashCode] = thread;
    }

    private void OnThreadEnded(SCThreadEnded msg)
    {
        if (Threads.TryGetValue(msg.ThreadHashCode, out var endedThread))
        {
            HandleThreadEnded(endedThread);
            Threads.TryRemove(msg.ThreadHashCode, out _);
        }
    }

    protected abstract IProperty CreateProperty(IStackFrame frame, VariableInfo info);

    public IProperty Resolve(IStackFrame frame, EvalExpression exp, uint timeout)
    {
        if (exp != null)
        {
            if (exp is NameExpression) return ResolveNameExpression(frame, (NameExpression)exp, timeout);
            if (exp is MemberAcessExpression)
                return ResolveMemberAccessExpression(frame, (MemberAcessExpression)exp, timeout);
            if (exp is IndexAccessExpression)
                return ResolveIndexAccessExpression(frame, (IndexAccessExpression)exp, timeout);
            if (exp is InvocationExpression)
                return ResolveInvocationExpression(frame, (InvocationExpression)exp, timeout);
        }

        return null;
    }

    private IProperty ResolveNameExpression(IStackFrame frame, NameExpression exp, uint timeout)
    {
        var res = frame.GetPropertyByName(exp.Content);
        if (res != null)
            return res;
        if (exp.IsRoot)
        {
            var info = ResolveVariable(null, exp.Content, frame.ThreadID, frame.Index, timeout);
            if (info == null)
            {
                info = new VariableInfo();
                info.Name = exp.Content;
                info.Value = "null";
                info.TypeName = "null";
            }

            return CreateProperty(frame, info);
        }
        else
        {
            var info = VariableInfo.FromObject(null);
            info.Type = VariableTypes.FieldReference;
            info.Name = exp.Content;
            return CreateProperty(frame, info);
        }
    }

    private IProperty ResolveMemberAccessExpression(IStackFrame frame, MemberAcessExpression exp, uint timeout)
    {
        var body = Resolve(frame, exp.Body, timeout);
        var member = exp.Member;
        IProperty prop = null;
        if (body != null)
        {
            var reference = body.GetVariableReference();
            if (reference != null)
            {
                if (reference.Type < VariableTypes.Error)
                {
                    if (exp.IsRoot)
                    {
                        var info = ResolveVariable(reference, member, frame.ThreadID, frame.Index, timeout);
                        prop = CreateProperty(frame, info);
                        prop.Parent = body;
                    }
                    else
                    {
                        var info = VariableInfo.FromObject(null);
                        info.Type = VariableTypes.FieldReference;
                        info.Name = member;
                        prop = CreateProperty(frame, info);
                        prop.Parent = body;
                    }
                }
                else if (reference.Type == VariableTypes.NotFound)
                {
                    if (exp.IsRoot)
                    {
                        var info = ResolveVariable(null, reference.Name + "." + member, frame.ThreadID, frame.Index,
                            timeout);
                        prop = CreateProperty(frame, info);
                    }
                    else
                    {
                        var info = VariableInfo.FromObject(null);
                        info.Type = VariableTypes.FieldReference;
                        info.Name = reference.Name + "." + member;
                        prop = CreateProperty(frame, info);
                    }
                }
            }
        }
        else
        {
            prop = CreateProperty(frame, VariableInfo.NullReferenceExeption);
        }

        return prop;
    }

    private IProperty ResolveIndexAccessExpression(IStackFrame frame, IndexAccessExpression exp, uint timeout)
    {
        var body = Resolve(frame, exp.Body, timeout);
        IProperty prop = null;
        if (body != null)
        {
            var reference = body.GetVariableReference();
            if (reference != null)
            {
                if (reference.Type < VariableTypes.Error)
                {
                    var indexExp = exp.Index;
                    VariableReference index = null;
                    if (indexExp is NameExpression)
                    {
                        var content = ((NameExpression)indexExp).Content;
                        if (content == "true")
                        {
                            index = VariableReference.True;
                        }
                        else if (content == "false")
                        {
                            index = VariableReference.False;
                        }
                        else if (content == "null")
                        {
                            index = VariableReference.Null;
                        }
                        else if (int.TryParse(content, out var indexInt))
                        {
                            index = VariableReference.GetInteger(indexInt);
                        }
                        else
                        {
                            var info = ResolveNameExpression(frame, (NameExpression)indexExp, timeout);
                            index = info.GetVariableReference();
                        }
                    }
                    else if (indexExp is StringLiteralExpression)
                    {
                        index = VariableReference.GetString(((StringLiteralExpression)indexExp).Content);
                    }
                    else
                    {
                        var info = Resolve(frame, indexExp, timeout);
                        index = info.GetVariableReference();
                    }

                    if (index != null && index.Type < VariableTypes.Error)
                    {
                        if (exp.IsRoot)
                        {
                            var info = ResolveIndexAccess(reference, index, frame.ThreadID, frame.Index, timeout);
                            prop = CreateProperty(frame, info);
                            prop.Parent = body;
                            prop.Parameters = new[] { index };
                        }
                        else
                        {
                            var info = VariableInfo.FromObject(null);
                            info.Type = VariableTypes.IndexAccess;
                            prop = CreateProperty(frame, info);
                            prop.Parent = body;
                            prop.Parameters = new[] { index };
                        }
                    }
                }
            }
        }
        else
        {
            prop = CreateProperty(frame, VariableInfo.NullReferenceExeption);
        }

        return prop;
    }

    private IProperty ResolveInvocationExpression(IStackFrame frame, InvocationExpression exp, uint timeout)
    {
        var body = Resolve(frame, exp.Body, timeout);
        if (body == null)
            return CreateProperty(frame, VariableInfo.NullReferenceExeption);

        var reference = body.GetVariableReference();
        if (reference == null || reference.Type >= VariableTypes.Error)
            return body;

        // The resolved member chain's outermost node names the method; flipping
        // its type to Invocation turns the server's member lookup into a call.
        reference.Type = VariableTypes.Invocation;
        reference.Parameters = BuildParameters(frame, exp.Parameters, timeout);
        var info = ResolveVariable(reference, frame.ThreadID, frame.Index, timeout);
        var prop = CreateProperty(frame, info);
        prop.Parent = body;
        return prop;
    }

    private VariableReference[] BuildParameters(IStackFrame frame, List<EvalExpression> parameters, uint timeout)
    {
        var result = new VariableReference[parameters.Count];
        for (var i = 0; i < parameters.Count; i++)
        {
            var param = parameters[i];
            if (param is StringLiteralExpression)
            {
                result[i] = VariableReference.GetString(((StringLiteralExpression)param).Content);
            }
            else if (param is NameExpression)
            {
                var content = ((NameExpression)param).Content;
                if (content == "true")
                    result[i] = VariableReference.True;
                else if (content == "false")
                    result[i] = VariableReference.False;
                else if (content == "null")
                    result[i] = VariableReference.Null;
                else if (int.TryParse(content, out var intValue))
                    result[i] = VariableReference.GetInteger(intValue);
                else
                    result[i] = ResolveNameExpression(frame, (NameExpression)param, timeout).GetVariableReference();
            }
            else
            {
                var resolved = Resolve(frame, param, timeout);
                result[i] = resolved?.GetVariableReference();
            }
        }

        return result;
    }
}
