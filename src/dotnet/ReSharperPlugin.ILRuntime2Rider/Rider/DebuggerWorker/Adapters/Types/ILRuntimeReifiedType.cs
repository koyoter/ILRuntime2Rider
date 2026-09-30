using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Client;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.Services;
using Mono.Debugging.TypeSystem;
using JetBrains.Annotations;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;

public class ILRuntimeReifiedType : ReifiedTypeBase<ILRuntimeProperty>
{
    public ILRuntimeReifiedType(
        IMetadataTypeLite metadataType,
        ITypeUniverse typeUniverse,
        IValueServicesFacade<ILRuntimeProperty> valueServices,
        IMetadataLiteInvocator<ILRuntimeProperty> invocator,
        IDebuggingHelper<ILRuntimeProperty> debuggingHelper)
        : base(metadataType, typeUniverse, valueServices, invocator, debuggingHelper)
    {
    }

    public ILRuntimeReifiedType(
        ILRuntimeProperty property,
        ITypeUniverse typeUniverse,
        IValueServicesFacade<ILRuntimeProperty> valueServices,
        IMetadataLiteInvocator<ILRuntimeProperty> invocator,
        IDebuggingHelper<ILRuntimeProperty> debuggingHelper)
        : base(property.ToLite(), typeUniverse, valueServices, invocator, debuggingHelper)
    {
        Property = property;
    }

    public ILRuntimeProperty Property { get; }

    [UsedImplicitly]
    private ILRuntimeTypeUniverse TypeUniverse => (ILRuntimeTypeUniverse)base.TypeUniverse;

    [Injected] public ILRuntimeDebuggerSession Session { get; set; }

    public override IReifiedType Base => null;

    protected override ILRuntimeProperty DoReadStaticField(
        IMetadataFieldLite field,
        IStackFrame frame,
        IValueFetchOptions options)
    {
        throw new NotSupportedException("Static field access is not supported for ILRuntime targets.");
    }

    protected override void WriteStaticField(
        IMetadataFieldLite field,
        ILRuntimeProperty newValue,
        IStackFrame originatingFrame,
        IValueFetchOptions options)
    {
        throw new NotSupportedException("Static field access is not supported for ILRuntime targets.");
    }
}
