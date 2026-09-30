using System;
using ILRuntime.Runtime.Debugger.Protocol;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

public class ILRuntimeBreakpoint : ILRuntimeBreakpointBase
{
    private readonly IDebuggedProcess process;
    private CSBindBreakpoint bindRequest;
    private bool bound;

    public ILRuntimeBreakpoint(
        IDebuggedProcess process,
        BreakEventInfo<ModuleMirror> breakEventInfo,
        string documentName,
        int line,
        int column)
    {
        BreakEventInfo = breakEventInfo;
        this.process = process;
        BreakEvent = breakEventInfo.BreakEvent;
        DocumentName = documentName;
        StartLine = line - 1;
        StartColumn = column;
        breakEventInfo.BreakEvent.EnableStatusChanged += EnableStatusChanged;
    }

    public BreakEventInfo<ModuleMirror> BreakEventInfo { get; }

    public BreakEvent BreakEvent { get; }

    public override string ConditionExpression => BreakEvent.ConditionExpression?.Expression;

    public override bool IsBound => bound;

    public override bool TryBind()
    {
        try
        {
            bindRequest ??= CreateBindRequest(true, BreakpointConditionStyle.None, null);
            process.SendBindBreakpoint(bindRequest);
            return true;
        }
        catch (Exception)
        {
            // Source file may be unavailable/changed since the request was created;
            // the breakpoint simply stays unbound until the next module load.
            return false;
        }
    }

    public override void Bound(BindBreakpointResults res)
    {
        switch (res)
        {
            case BindBreakpointResults.OK:
                BreakEventInfo.SetStatus(BreakEventStatus.Bound, "BreakpointBound");
                SetCondition();
                bound = true;
                break;
            case BindBreakpointResults.TypeNotFound:
                BreakEventInfo.SetStatus(BreakEventStatus.BindError, "BreakpointBindError:TypeNotFound");
                break;
            case BindBreakpointResults.CodeNotFound:
                BreakEventInfo.SetStatus(BreakEventStatus.BindError, "BreakpointBindError:CodeNotFound");
                break;
        }
    }

    private void SetCondition()
    {
        var hasCondition = !string.IsNullOrEmpty(ConditionExpression);
        if (hasCondition)
            process.SendSetBreakpointCondition(GetHashCode(),
                BreakpointConditionStyle.WhenTrue, ConditionExpression);
    }

    private void EnableStatusChanged(bool enable)
    {
        process.SendSetBreakpointEnabled(GetHashCode(), enable);
    }
}
