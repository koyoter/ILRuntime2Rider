using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Values;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeFramePhysicalAdapterFactory : IFramePhysicalAdapterFactory<ILRuntimeStackFrame>
{
    [Injected]
    public IValueServicesFacade<ILRuntimeProperty> ValueServices { get; set; }

    public IFramePhysicalAdapter CreateAdapter(IStackFrame<ILRuntimeStackFrame> frame)
    {
        return new ILRuntimeFramePhysicalAdapter(frame, ValueServices);
    }
}
