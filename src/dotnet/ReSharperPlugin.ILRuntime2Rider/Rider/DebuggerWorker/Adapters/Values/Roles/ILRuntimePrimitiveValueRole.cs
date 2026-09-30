using System;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

public class ILRuntimePrimitiveValueRole :
    IPrimitiveValueRole<ILRuntimeProperty>,
    IPrimitiveValueRole,
    IMutableToObjectValueRole,
    IValueRole,
    IMutableToObjectValueRole<ILRuntimeProperty>,
    IValueRole<ILRuntimeProperty>
{
    private readonly IValueReference<ILRuntimeProperty> valueReference;
    private readonly ILRuntimeReifiedType reifiedType;
    private readonly IValueFetchOptions creationOptions;
    private readonly IValueRoleFactory<ILRuntimeProperty> roleFactory;

    public ILRuntimePrimitiveValueRole(
        IValueReference<ILRuntimeProperty> valueReference,
        ILRuntimeReifiedType reifiedType,
        IValueFetchOptions creationOptions,
        IValueRoleFactory<ILRuntimeProperty> roleFactory)
    {
        this.valueReference = valueReference;
        this.reifiedType = reifiedType;
        this.creationOptions = creationOptions;
        this.roleFactory = roleFactory;
    }

    public IObjectValueRole<ILRuntimeProperty> ToObject()
    {
        return roleFactory.CreateObjectRole(ValueReference, reifiedType, CreationOptions, true);
    }

    public IReifiedType ReifiedType => reifiedType;

    IObjectValueRole IMutableToObjectValueRole.ToObject() => ToObject();

    public object GetPrimitive()
    {
        var property = valueReference.GetValue(creationOptions);
        return Convert.ChangeType(property.Info.Value, property.CompileType);
    }

    public bool IsBoxed => false;

    public IValueReference<ILRuntimeProperty> ValueReference => valueReference;

    public IValueFetchOptions CreationOptions => creationOptions;
}
