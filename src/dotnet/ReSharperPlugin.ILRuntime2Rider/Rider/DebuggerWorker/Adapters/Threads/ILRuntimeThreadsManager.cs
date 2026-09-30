using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Threads;
using Mono.Debugging.Client;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;

/// <summary>
/// The ILRuntime protocol only runs threads until the next breakpoint hit;
/// pausing, freezing or selectively resuming individual threads is not
/// supported by the wire protocol, so those operations are rejected here.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeThreadsManager : ThreadsManager<ILRuntimeThread>
{
    private readonly ILRuntimeDebuggerSession session;
    private readonly ILogger logger;

    public ILRuntimeThreadsManager(
        IThreadShimsFactory<ILRuntimeThread> threadShimsFactory,
        ILRuntimeDebuggerSession session,
        ILogger logger)
        : base(threadShimsFactory, session)
    {
        this.session = session;
        this.logger = logger;
    }

    public override void SetOtherThreadsToRunState(ILRuntimeThread thread)
    {
        throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsThreadSuspended(ILRuntimeThread thread)
    {
        throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");
    }

    public override ThreadState GetThreadState(ILRuntimeThread thread)
    {
        return ThreadState.Running;
    }

    public override bool IsFrozen(IThread<ILRuntimeThread> thread)
    {
        return false;
    }

    public override void SetFrozen(IThread<ILRuntimeThread> thread, bool isFrozen)
    {
    }

    public override bool CanFreezeThreads()
    {
        return false;
    }

    protected override IReadOnlyList<ILRuntimeThread> GetEngineThreads()
    {
        return session.DebuggedProcess.Threads
            .Select(thread => thread.Value as ILRuntimeThread)
            .ToList();
    }

    protected override void ContinueAllThreads() => throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");

    protected override void Continue() => throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");

    public override void Pause()
    {
        throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");
    }

    protected override void ContinueSingleThread(IThread<ILRuntimeThread> threadToResume)
    {
        throw new NotSupportedException("Thread state control is not supported by the ILRuntime debug protocol.");
    }
}
