using System;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.Services;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

public class ILRuntimeObjectValueRole : ObjectValueRoleBase<ILRuntimeProperty>
{
    public ILRuntimeObjectValueRole(
        IValueReference<ILRuntimeProperty> valueReference,
        IReifiedType reifiedType,
        IValueFetchOptions creationOptions,
        IMetadataLiteInvocator<ILRuntimeProperty> invocator,
        IValueRoleFactory<ILRuntimeProperty> valueRoleFactory)
        : base(valueReference, reifiedType, creationOptions, invocator, valueRoleFactory)
    {
    }

    public override IObjectValueRole<ILRuntimeProperty> Base
    {
        get
        {
            var baseType = ReifiedType.Base;
            return baseType != null
                ? new ILRuntimeObjectValueRole(ValueReference, baseType, CreationOptions, Invocator, RoleFactory)
                : (IObjectValueRole<ILRuntimeProperty>)null;
        }
    }

    public override long UniqueId => ValueReference.GetValue(CreationOptions).GetHashCode();

    public override ILRuntimeProperty GetFieldValue(IMetadataFieldLite metadataField)
    {
        return ((ILRuntimeMetadataFieldLite)metadataField).Property;
    }

    public override void SetFieldValue(IMetadataFieldLite metadataField, ILRuntimeProperty newValue)
    {
        throw new NotSupportedException("The ILRuntime protocol cannot modify debuggee values.");
    }
}
