using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.CallStacks;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeFrameValuesProvider :
    GenericFrameValuesProvider<ILRuntimeStackFrame, ILRuntimeProperty>
{
    public ILRuntimeFrameValuesProvider(ILogger logger, ILRuntimeDebuggerSession session)
        : base(logger)
    {
    }

    public override int Priority => 0;
}
