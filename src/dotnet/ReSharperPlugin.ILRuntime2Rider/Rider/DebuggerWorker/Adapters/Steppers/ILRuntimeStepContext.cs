using Mono.Debugging.Client.Steppers;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Steppers;

public class ILRuntimeStepContext : StepContext<ILRuntimeThread>
{
    public ILRuntimeStepContext(ILRuntimeThread thread)
        : base(thread)
    {
    }

    public override long GetThreadId() => Thread.ThreadID;
}
