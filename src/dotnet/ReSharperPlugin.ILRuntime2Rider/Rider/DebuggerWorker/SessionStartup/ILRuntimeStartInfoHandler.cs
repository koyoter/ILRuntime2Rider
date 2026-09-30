using JetBrains.Debugger.Model.Plugins.ILRuntime2Rider;
using Mono.Debugging.Autofac;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

[DebuggerGlobalComponent]
public class ILRuntimeStartInfoHandler()
    : ILRuntimeStartInfoHandlerBase<ILRuntimeStartInfo>(ILRuntimeDebuggerType.Instance);
