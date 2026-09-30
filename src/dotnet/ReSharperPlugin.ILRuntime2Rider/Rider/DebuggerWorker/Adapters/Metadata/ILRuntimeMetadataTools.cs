using Mono.Debugging.Autofac;
using Mono.Debugging.Tools.Impl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMetadataTools : MetadataTools
{
}
