using Autofac;
using JetBrains.Debugger.Model.Plugins.ILRuntime2Rider;
using JetBrains.Debugger.Worker;
using JetBrains.Lifetimes;
using Mono.Debugging.Autofac;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

/// <summary>
/// Exposes the ILRuntime debugger-worker RD model to session components.
/// </summary>
[DebuggerGlobalComponent]
public class ILRuntimeDebuggerWorkerHost(RiderDebuggerWorker debuggerWorker) : IStartable
{
    public ILRuntimeDebuggerWorkerModel Model { get; } =
        debuggerWorker.FrontendModel.GetILRuntimeDebuggerWorkerModel();

    public void Start()
    {
    }
}
