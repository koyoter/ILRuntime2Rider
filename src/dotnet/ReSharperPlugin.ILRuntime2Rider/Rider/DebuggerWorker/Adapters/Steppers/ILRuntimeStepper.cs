using ILRuntime.Runtime.Debugger;
using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.Steppers;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Steppers;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeStepper :
    Stepper<ILRuntimeThread, ILRuntimeAfterStepState, ILRuntimeStepContext>
{
    private readonly ILRuntimeDebuggerSession session;

    public ILRuntimeStepper(ILogger logger, ILRuntimeDebuggerSession session)
        : base(logger, session, ILRuntimeThreadEqualityComparer<ILRuntimeThread>.Instance)
    {
        this.session = session;
    }

    protected override void OnDeactivateThread(ILRuntimeThread thread)
    {
    }

    protected override void DoStepInto(ILRuntimeStepContext stepContext)
    {
        session.Resume();
        session.DebuggedProcess.SendStep((int)stepContext.GetThreadId(), StepTypes.Into);
    }

    protected override void DoStepOver(ILRuntimeStepContext stepContext)
    {
        session.Resume();
        session.DebuggedProcess.SendStep((int)stepContext.GetThreadId(), StepTypes.Over);
    }

    protected override void DoStepOut(ILRuntimeStepContext stepContext)
    {
        session.Resume();
        session.DebuggedProcess.SendStep((int)stepContext.GetThreadId(), StepTypes.Out);
    }

    protected override void TryLeaveIgnoredFrame(ILRuntimeStepContext stepContext)
    {
        session.DebuggedProcess.SendStep((int)stepContext.GetThreadId(), StepTypes.Into);
    }
}
