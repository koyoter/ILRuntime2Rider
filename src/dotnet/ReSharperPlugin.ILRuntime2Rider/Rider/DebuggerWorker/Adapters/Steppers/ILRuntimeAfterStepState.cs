using System;
using System.Collections.Generic;
using Mono.Debugging.Backend.Threads;
using Mono.Debugging.Client.Steppers;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Steppers;

/// <summary>
/// Post-step bookkeeping. The ILRuntime protocol reports step completion
/// directly, so these bookkeeping hooks are never exercised on the happy path.
/// </summary>
public class ILRuntimeAfterStepState : AfterStepState<ILRuntimeThread, ILRuntimeStepContext>
{
    public ILRuntimeAfterStepState(
        IThread<ILRuntimeThread> thread,
        ILRuntimeDebuggerSession session)
#if RS_261_OR_LATER
        : base(thread, session)
#else
        : base(thread)
#endif
    {
    }

    public override bool HasQueuedCallbacks(ILRuntimeThread thread) => false;

    public override bool IsValid() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override bool ContinueOnStepIn() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

#if !RS_261_OR_LATER
    // Folded into ShouldStepThroughStepHandler.IsDebuggerStepThrough() in 2026.1.
    public override bool ShouldStepThrough() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");
#endif

    public override long GetThreadId() => Thread.Id;

    public override IEnumerable<CallStackElementShortInfo> EnumerateCallStackElements()
    {
        throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");
    }

    public override bool IsStepOut() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override bool IsStepIn() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override ILRuntimeStepContext CreateStepContext() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

#if RS_251_OR_LATER
    /// <summary>
    /// Only consumed by the base class's optional stepper logger (see SetStepLoggingEnabled),
    /// never by the stepping flow itself; the ILRuntime protocol carries no method position.
    /// </summary>
    public override string GetMethodPositionPresentation() => "ILRuntime method";
#endif

#if RS_261_OR_LATER
    // Neutral answers: no managed call stack, no debugger-hidden frames, and no sequence
    // points, so the base stepping handlers all short-circuit to ContinueBreaking.
    public override int CallStackFrameCount => 0;

    public override bool IsUserCode() => true;

    public override bool IsDebuggerStepThrough() => false;

    public override bool IsDebuggerHidden() => false;

    public override SequencePointState GetSequencePointState() => null;
#endif
}
