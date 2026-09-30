using Mono.Debugging.Autofac;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.Services;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMetadataLiteProvider : IValueMetadataProvider<ILRuntimeProperty>
{
    public IMetadataTypeLite GetValueType(ILRuntimeProperty property)
    {
        return property.ToLite();
    }
}
