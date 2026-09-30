using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeAssemblyLoadHelper : DebuggerAssemblyLoadHelper<ILRuntimeProperty>
{
    public ILRuntimeAssemblyLoadHelper(
        ILRuntimeAppDomainsManager appDomainsManager,
        DebuggingHelper<ILRuntimeProperty> debuggingHelper)
        : base(appDomainsManager, debuggingHelper)
    {
    }
}
