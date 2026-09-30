using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.Services;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

/// <summary>
/// Cannot invoke methods on the debuggee; a "call" surfaces the declared return
/// type so the evaluation pipeline can render a result placeholder.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMetadataLiteInvocator : IMetadataLiteInvocator<ILRuntimeProperty>
{
    public ILRuntimeProperty CallMethod(
        IStackFrame frame,
        IValueFetchOptions options,
        IMetadataTypeLite constructedType,
        IMetadataMethodLite method,
        IMetadataTypeLite[] methodTypeArguments,
        ILRuntimeProperty thisObject,
        params ILRuntimeProperty[] arguments)
    {
        return ((ILRuntimeMetadataTypeLite)((ILRuntimeMetadataMethodLite)method).ReturnType).Property;
    }
}
