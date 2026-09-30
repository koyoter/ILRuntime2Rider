using System;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.Utils;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;

public class ILRuntimeThisReference :
    ICachedValueReference<ILRuntimeProperty>,
    IValueReference<ILRuntimeProperty>,
    IValueReference,
    IDebuggerHierarchyObject
{
#if RS_252_OR_LATER
    private readonly FetchedCachedValue<ILRuntimeProperty> cachedValue;
#else
    private readonly RollbackIfFailCachedValue<ILRuntimeProperty, IValueFetchOptions> cachedValue;
#endif
    private readonly IStackFrame<ILRuntimeStackFrame> frame;
    private readonly IValueRoleFactory<ILRuntimeProperty> roleFactory;

    public ILRuntimeThisReference(
        IStackFrame<ILRuntimeStackFrame> frame,
        IValueRoleFactory<ILRuntimeProperty> roleFactory)
    {
        this.frame = frame;
        this.roleFactory = roleFactory;
#if RS_252_OR_LATER
        cachedValue = new FetchedCachedValue<ILRuntimeProperty>(GetValueNoCache, property => true);
#else
        cachedValue = this.CreateRollbackIfFailCacheValue(property => true);
#endif
    }

    public IDebuggerHierarchyObject Parent => frame;

    public IValueRole GetPrimaryRole(IValueFetchOptions options)
    {
        return roleFactory.GetPrimaryRole(this, options);
    }

    public IMetadataTypeLite DeclaredType => frame.Frame.ThisProperty.ToLite();

    public string DefaultName => "this";

    public ILRuntimeProperty GetValue(IValueFetchOptions options)
    {
        return cachedValue.Get(options);
    }

    public void SetValue(ILRuntimeProperty value, IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime protocol cannot modify debuggee values.");
    }

    public bool IsWriteable { get; }

    public ValueOriginKind OriginKind => ValueOriginKind.This;

    public ValueFlags DefaultFlags => ValueFlags.PinnableToTop;

    public IStackFrame OriginatingFrame => frame;

    public ILRuntimeProperty GetValueNoCache(IValueFetchOptions options)
    {
        return frame.Frame.ThisProperty;
    }

    public void ResetCache()
    {
        cachedValue.Reset();
    }
}
