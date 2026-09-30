using System;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.Utils;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;

public class ILRuntimeLocalVariableReference :
    ICachedValueReference<ILRuntimeProperty>,
    IValueReference<ILRuntimeProperty>,
    IValueReference,
    IDebuggerHierarchyObject
{
    private readonly RollbackIfFailCachedValue<ILRuntimeProperty, IValueFetchOptions> cachedValue;
    private readonly IStackFrame<ILRuntimeStackFrame> frame;
    private readonly ILRuntimeProperty propertyValue;
    private readonly IValueRoleFactory<ILRuntimeProperty> roleFactory;

    public ILRuntimeLocalVariableReference(
        IStackFrame<ILRuntimeStackFrame> frame,
        ILRuntimeProperty propertyValue,
        IValueRoleFactory<ILRuntimeProperty> roleFactory)
    {
        this.frame = frame;
        this.propertyValue = propertyValue;
        this.roleFactory = roleFactory;
        cachedValue = this.CreateRollbackIfFailCacheValue(property => true);
    }

    public IDebuggerHierarchyObject Parent => frame;

    public IValueRole GetPrimaryRole(IValueFetchOptions options)
    {
        return roleFactory.GetPrimaryRole(this, options);
    }

    public IMetadataTypeLite DeclaredType => null;

    public string DefaultName => propertyValue.Name;

    public ILRuntimeProperty GetValue(IValueFetchOptions options)
    {
        return propertyValue;
    }

    public void SetValue(ILRuntimeProperty value, IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime protocol cannot modify debuggee values.");
    }

    public bool IsWriteable => false;

    public ValueOriginKind OriginKind => ValueOriginKind.LocalVariable;

    public ValueFlags DefaultFlags => ValueFlags.PinnableToTop;

    public IStackFrame OriginatingFrame => frame;

    public ILRuntimeProperty GetValueNoCache(IValueFetchOptions options)
    {
        return propertyValue;
    }

    public void ResetCache()
    {
        throw new NotSupportedException("Cache invalidation is not supported for this reference type.");
    }
}
