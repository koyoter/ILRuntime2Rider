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
        : base(thread)
    {
    }

    public override bool HasQueuedCallbacks(ILRuntimeThread thread) => false;

    public override bool IsValid() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override bool ContinueOnStepIn() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override bool ShouldStepThrough() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override long GetThreadId() => Thread.Id;

    public override IEnumerable<CallStackElementShortInfo> EnumerateCallStackElements()
    {
        throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");
    }

    public override bool IsStepOut() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override bool IsStepIn() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");

    public override ILRuntimeStepContext CreateStepContext() => throw new NotSupportedException("Step bookkeeping is not exercised by the ILRuntime stepping flow.");
}
