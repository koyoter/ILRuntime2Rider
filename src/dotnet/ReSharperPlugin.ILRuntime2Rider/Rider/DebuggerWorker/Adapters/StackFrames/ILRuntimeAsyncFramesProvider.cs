using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.CallStacks.Async;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeAsyncFramesProvider :
    AsyncFramesProviderBase<ILRuntimeStackFrame, ILRuntimeProperty>
{
    public ILRuntimeAsyncFramesProvider(ILogger logger)
        : base(logger)
    {
    }

    protected override bool IsFrameMoveNextMethod(IStackFrame<ILRuntimeStackFrame> frame) => false;
}
