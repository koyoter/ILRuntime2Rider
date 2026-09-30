using ILRuntime.Runtime.Debugger;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.Services;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeValueRoleFactory : IValueRoleFactory<ILRuntimeProperty>
{
    [Injected]
    public ILRuntimeTypeUniverse TypeUniverse { get; set; }

    [Injected]
    public IValueServicesFacade<ILRuntimeProperty> ValueServices { get; set; }

    [Injected]
    public IMetadataLiteInvocator<ILRuntimeProperty> MetadataLiteInvocator { get; set; }

    public IValueRole GetPrimaryRole(
        IValueReference<ILRuntimeProperty> value,
        IValueFetchOptions options)
    {
        var property = value.GetValue(options);
        var reifiedType = TypeUniverse.GetReifiedType(property);
        if (property.Info.ValueType == ValueTypes.Null || property.Info.Type == VariableTypes.Null)
            return new NullReferenceRole<ILRuntimeProperty>(value, reifiedType, options);
        if (property.Info.Type == VariableTypes.Error || property.Info.Type == VariableTypes.Timeout)
            return new ILRuntimeStringValueRole(value, reifiedType, options, this);
        if (property.IsString)
            return new ILRuntimeStringValueRole(value, reifiedType, options, this);
        if (property.IsPrimitive)
            return new ILRuntimePrimitiveValueRole(value, reifiedType, options, this);
        return property.IsArray
            ? new ILRuntimeArrayValueRole(value, reifiedType, ValueServices, options)
            : new ILRuntimeObjectValueRole(value, reifiedType, options, MetadataLiteInvocator, this);
    }

    public IObjectValueRole<ILRuntimeProperty> CreateObjectRole(
        IValueReference<ILRuntimeProperty> reference,
        IReifiedType<ILRuntimeProperty> type,
        IValueFetchOptions creationOptions,
        bool boxing)
    {
        return new ILRuntimeObjectValueRole(reference, type, creationOptions, MetadataLiteInvocator, this);
    }

    public bool IsValid(ILRuntimeProperty value) => true;
}
