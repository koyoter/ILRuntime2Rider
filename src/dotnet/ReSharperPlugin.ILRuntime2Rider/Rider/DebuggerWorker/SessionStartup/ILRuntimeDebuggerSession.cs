using System;
using Debugger.Common;
using Debugger.Common.ManagedSymbols;
using Debugger.Common.ManagedSymbols.PdbSymbols;
using JetBrains.Debugger.Model.Plugins.ILRuntime2Rider;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.Events;
using Mono.Debugging.Debugee;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Breakpoints;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Steppers;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeDebuggerSession(
    Lifetime lifetime,
    ILogger logger,
    IExpressionRangeProvider expressionRangeProvider,
    ISourceHashCheckerProvider hashCheckerProvider,
    ILRuntimeDebuggerWorkerHost host,
    DebugSymbolsKind symbolsKind = 0)
    : DebuggerSession<ILRuntimeProperty>(lifetime, logger, expressionRangeProvider, hashCheckerProvider, symbolsKind)
{
    private ILRuntimeThread CurrentThread => ThreadsManager.ActiveThread.Thread;

    public ILRuntimeThreadsManager ThreadsManager => (ILRuntimeThreadsManager)base.ThreadsManager;

    public IDebuggedProcess DebuggedProcess { get; private set; }

    private ILRuntimeBreakpointManager BreakpointManager => (ILRuntimeBreakpointManager)base.BreakpointsManager;

    public ILRuntimeAppDomainsManager AppDomainsManager =>
        (ILRuntimeAppDomainsManager)base.AppDomainsManager;

    [Injected] private ILRuntimeStepper Stepper { get; set; }

    [Injected] public ILRuntimeMetadataStorage MetadataStorage { get; set; }

    protected override void OnSmartStepLine(SmartStepIntoInfo smartStepIntoInfo)
    {
        Stepper.SmartStepInto(smartStepIntoInfo);
    }

    /// <summary>
    /// Smart step-into is a C#-specific feature the ILRuntime protocol does not
    /// implement; the base implementation would dereference a missing
    /// ISmartStepIntoProvider and its NRE kills the worker's queued task pump.
    /// </summary>
    protected override void DoCancelStatefulOperations() => Stepper.OnDebuggerBreak();

    protected override void OnAttachToProcess(ProcessInfo processInfo)
    {
        throw new NotSupportedException("ILRuntime debugging only supports the attach workflow via OnRun.");
    }

    protected override void OnRun(DebuggerStartInfo startInfo)
    {
        SmartStepIntoProvider = new Adapters.Evaluation.ILRuntimeSmartStepIntoProvider();
        LogEx.Info(Logger, "ILRuntime debugger session started");
        OnDebuggerOutput(false, "[ILRuntime2Rider] worker build 2026-09-29-4, step diagnostics ON\n");

        var service = ILRuntimeAttachService.Instance;
        service.OnMessage += RaiseTargetNotificationSent;
        service.OnDisconnect += Stepper.Reset;
        service.OnConnectResult += (result, session) =>
        {
            LogEx.Info(Logger, $"ILRuntime debugger connect result: {result}");
            if (result)
            {
                DebuggedProcess = session;
                BreakpointManager.RegisterAllBreakpoints();
            }
        };

        var reconnectLifetime = Lifetime.CreateNested().Lifetime;
        host.Model.Signal.Advise(reconnectLifetime, process =>
        {
            if (process != null && service.IsNone && service.IsSameProcess(process))
            {
                LogEx.Info(Logger, $"ILRuntime debugger reconnect: {process}");
                service.Connect(this, process);
            }
        });

        var ilStartInfo = (ILRuntimeDebuggerStartInfo)startInfo;
        service.Connect(this, new Process(
            ilStartInfo.Address.ToString(), ilStartInfo.Port, ilStartInfo.ProjectName));
        OnStarted();
    }

    protected override void OnDetach()
    {
        LogEx.Info(Logger, "ILRuntime debugger session detached");
        HasExited = true;
    }

    protected override void OnStop()
    {
        throw new NotSupportedException("Pausing a running ILRuntime target is not supported.");
    }

    protected override void OnExit()
    {
        LogEx.Info(Logger, "ILRuntime debugger session exited");
        HasExited = true;
    }

    protected override void OnStepLine() => Stepper.StepInto(new ILRuntimeStepContext(CurrentThread));

    protected override void OnStepInstruction() => Stepper.StepInto(new ILRuntimeStepContext(CurrentThread));

    protected override void OnNextLine() => Stepper.StepOver(new ILRuntimeStepContext(CurrentThread));

    protected override void OnNextInstruction() => Stepper.StepOver(new ILRuntimeStepContext(CurrentThread));

    protected override void OnStepOut() => Stepper.StepOut(new ILRuntimeStepContext(CurrentThread));

    protected override void OnContinue()
    {
        Resume();
        DebuggedProcess?.SendExecute(CurrentThread.ThreadID);
    }

    protected override IManagedSymbolReaderFactory CreateSymbolReaderFactory()
    {
        return new ManagedPdbSymbolReaderFactory(PathResolutionManager, FileExistenceCache);
    }

    protected override DebugeeRuntime CalculateDebugeeRuntime()
    {
        return new ILDebugeeRuntime();
    }

    protected override ProcessInfo OnGetProcessInfo()
    {
        return new ProcessInfo(-1L, "ILRuntime");
    }

    public void Resume() => IncrementDebuggeeTimestamp();

    private void EndSession()
    {
        if (HasExited)
            return;
        OnContinue();
        QueuedTasks.QueueRuntimeEventToMainThread(nameof(EndSession),
            () => OnTargetEvent(new TargetEventArgs(TargetEventType.TargetExited)));
    }
}
