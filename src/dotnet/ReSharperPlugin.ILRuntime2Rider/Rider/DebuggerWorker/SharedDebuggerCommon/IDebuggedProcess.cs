using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ILRuntime.Runtime.Debugger;
using ILRuntime.Runtime.Debugger.Expressions;
using ILRuntime.Runtime.Debugger.Protocol;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

public interface IDebuggedProcess
{
    ConcurrentDictionary<int, IThread> Threads { get; }

    Action OnDisconnected { get; set; }

    bool Connected { get; set; }

    bool ConnectFailed { get; set; }

    bool Connecting { get; set; }

    void Close();

    bool CheckDebugServerVersion();

    T AwaitRPCRequest<T>(out bool aborted, int timeout = 0);

    void AddPendingBreakpoint(ILRuntimeBreakpointBase bp);

    void SendBindBreakpoint(CSBindBreakpoint msg);

    void SendSetBreakpointCondition(int bpHash, BreakpointConditionStyle style, string expression);

    void SendSetBreakpointEnabled(int bpHash, bool enabled);

    void SendDeleteBreakpoint(int bpHash);

    void SendExecute(int threadHash);

    void SendStep(int threadHash, StepTypes type);

    VariableInfo ResolveVariable(
        VariableReference parent,
        string name,
        int threadId,
        int frameId,
        uint timeout);

    VariableInfo ResolveIndexAccess(
        VariableReference body,
        VariableReference index,
        int threadId,
        int frameId,
        uint timeout);

    VariableInfo[] EnumChildren(
        VariableReference parent,
        int threadId,
        int frameId,
        uint timeout);

    IProperty Resolve(IStackFrame frame, EvalExpression expression, uint timeout);
}
