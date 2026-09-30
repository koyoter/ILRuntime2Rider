using System;
using ILRuntime.Runtime.Debugger;
using JetBrains.Util;
using Mono.Debugging.Client;
using Mono.Debugging.Client.Events;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// Bridges raw protocol events from <see cref="DebuggedProcess"/> into the
/// Rider debugger session (target events, output, breakpoint bookkeeping).
/// </summary>
public class DebuggedProcessRider(ILRuntimeDebuggerSession debuggerSession, string host, int port)
    : DebuggedProcess(host, port), IDebuggedProcess
{
    public Action<string> DiagnosticsSink { get; set; } =
        text => debuggerSession.OnDebuggerOutput(false, text + Environment.NewLine);

    private const string BreakpointHitEvent = "Breakpoint Hit";
    private const string StepCompleteEvent = "Step Complete";
    private const string ThreadEndedEvent = "Thread Ended";
    private const string ThreadStartedEvent = "Thread Started";

    protected override void HandleModuleLoaded(string moduleName)
    {
    }

    protected override void HandleBreakpointHit(ILRuntimeBreakpointBase bp, IThread bpThread)
    {
        try
        {
            debuggerSession.QueuedTasks.QueueRuntimeEventToMainThread(BreakpointHitEvent, () =>
            {
                if (bp is not ILRuntimeBreakpoint runtimeBreakpoint) return;

                runtimeBreakpoint.BreakEventInfo.IncrementHitCount();
                if (!runtimeBreakpoint.BreakEventInfo.HitCountReached)
                {
                    debuggerSession.Continue();
                }
                else
                {
                    var ilRuntimeThread = (ILRuntimeThread)bpThread;
                    debuggerSession.ThreadsManager.SetActiveThread(ilRuntimeThread);
                    debuggerSession.OnTargetEvent(new TargetEventArgs(TargetEventType.TargetHitBreakpoint)
                    {
                        Thread = debuggerSession.ThreadsManager.GetThread(ilRuntimeThread),
                        BreakEvent = runtimeBreakpoint.BreakEvent
                    });
                }
            });
        }
        catch (Exception ex)
        {
            debuggerSession.Logger.Info(ex.ToString());
            throw;
        }
    }

    protected override void HandleBreakpointConditionError(string errorMsg)
    {
        // Surfacing the condition failure in the debugger console; the previous
        // behaviour (throwing) killed the whole worker process.
        debuggerSession.QueuedTasks.QueueRuntimeEventToMainThread(BreakpointHitEvent,
            () => debuggerSession.OnDebuggerOutput(true, errorMsg + Environment.NewLine));
    }

    protected override void HandleStepComplete(IThread bpThread)
    {
        debuggerSession.QueuedTasks.QueueRuntimeEventToMainThread(StepCompleteEvent, () =>
        {
            var ilRuntimeThread = (ILRuntimeThread)bpThread;
            debuggerSession.ThreadsManager.SetActiveThread(ilRuntimeThread);
            debuggerSession.OnTargetEvent(new TargetEventArgs(TargetEventType.TargetStepCompleted)
            {
                Thread = debuggerSession.ThreadsManager.GetThread(ilRuntimeThread)
            });
            debuggerSession.OnDebuggerOutput(false, $"{StepCompleteEvent}\n");
        });
    }

    protected override IThread HandleThreadStarted(int threadHash)
    {
        var thread = new ILRuntimeThread(threadHash, this);
        debuggerSession.QueuedTasks.QueueRuntimeEventToMainThread(ThreadStartedEvent, () =>
        {
            debuggerSession.OnTargetEvent(new TargetEventArgs(TargetEventType.ThreadStarted));
            debuggerSession.OnDebuggerOutput(false, $"{ThreadStartedEvent} {thread.ThreadID}\n");
        });
        return thread;
    }

    protected override void HandleThreadEnded(IThread thread)
    {
        debuggerSession.QueuedTasks.QueueRuntimeEventToMainThread(ThreadEndedEvent, () =>
        {
            debuggerSession.OnTargetEvent(new TargetEventArgs(TargetEventType.ThreadStopped));
            debuggerSession.OnDebuggerOutput(false, $"{ThreadEndedEvent} {thread.ThreadID}\n");
        });
    }

    protected override IProperty CreateProperty(IStackFrame frame, VariableInfo info)
    {
        return ILRuntimeProperty.CreateInstance(frame as ILRuntimeStackFrame, info);
    }
}
