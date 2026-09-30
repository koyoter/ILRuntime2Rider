using Mono.Debugging.Client.Steppers;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Steppers;

public class ILRuntimeStepContext : StepContext<ILRuntimeThread>
{
    public ILRuntimeStepContext(ILRuntimeThread thread)
#if RS_261_OR_LATER
        // The ILRuntime protocol has no IL metadata; only the thread id is real.
        : base(thread, 0, null, null, 0, thread.ThreadID)
#else
        : base(thread)
#endif
    {
    }

#if !RS_261_OR_LATER
    // Replaced by the base class's ThreadId property in 2026.1.
    public override long GetThreadId() => Thread.ThreadID;
#endif
}
