using Mono.Debugging.Autofac;
using Mono.Debugging.Client.AppDomainsInfo;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeDocumentsProvider : DocumentProviderBase
{
    public ILRuntimeDocumentsProvider(
        ILRuntimeAppDomainsManager appDomainsManager)
        : base(appDomainsManager)
    {
    }
}
