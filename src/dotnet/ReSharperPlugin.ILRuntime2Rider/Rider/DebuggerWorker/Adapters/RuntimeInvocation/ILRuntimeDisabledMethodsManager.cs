using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.DisabledToInvokeMethods;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.RuntimeInvocation;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeDisabledMethodsManager : DisabledMethodsManager<ILRuntimeProperty>
{
    public ILRuntimeDisabledMethodsManager(ILogger logger, IDebuggerSession session)
        : base(logger, session)
    {
    }
}
