using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

public class ILRuntimeStringValueRole :
    IStringValueRole<ILRuntimeProperty>,
    IMutableToObjectValueRole<ILRuntimeProperty>,
    IMutableToObjectValueRole,
    IValueRole,
    IValueRole<ILRuntimeProperty>,
    IStringValueRole
{
    private readonly IValueReference<ILRuntimeProperty> value;
    private readonly ILRuntimeReifiedType reifiedType;
    private readonly IValueFetchOptions options;
    private readonly ILRuntimeValueRoleFactory roleFactory;

    private string StringValue => value.GetValue(options).Info.Value;

    public ILRuntimeStringValueRole(
        IValueReference<ILRuntimeProperty> value,
        ILRuntimeReifiedType reifiedType,
        IValueFetchOptions options,
        ILRuntimeValueRoleFactory roleFactory)
    {
        this.value = value;
        this.reifiedType = reifiedType;
        this.options = options;
        this.roleFactory = roleFactory;
    }

    public IObjectValueRole<ILRuntimeProperty> ToObject()
    {
        return roleFactory.CreateObjectRole(ValueReference, reifiedType, options, false);
    }

    public IReifiedType ReifiedType => reifiedType;

    IObjectValueRole IMutableToObjectValueRole.ToObject() => ToObject();

    public IValueReference<ILRuntimeProperty> ValueReference => value;

    public IValueFetchOptions CreationOptions => options;

    public string GetString() => StringValue;

    public string GetSubstring(int startOffset, int length)
    {
        return StringValue.Substring(startOffset, length);
    }

    public int Length => StringValue.Length;
}
