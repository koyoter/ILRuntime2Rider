using System;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;

/// <summary>
/// Unused today: the ILRuntime protocol folds method arguments into the local
/// variable list, so no argument references are ever created.
/// </summary>
public class ILRuntimeArgumentReference :
    ICachedValueReference<ILRuntimeProperty>,
    IValueReference<ILRuntimeProperty>,
    IValueReference,
    IDebuggerHierarchyObject
{
    public IDebuggerHierarchyObject Parent { get; }

    public IValueRole GetPrimaryRole(IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime debug protocol does not expose method arguments.");
    }

    public IMetadataTypeLite DeclaredType { get; }

    public string DefaultName { get; }

    public ILRuntimeProperty GetValue(IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime debug protocol does not expose method arguments.");
    }

    public void SetValue(ILRuntimeProperty value, IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime debug protocol does not expose method arguments.");
    }

    public bool IsWriteable { get; }

    public ValueOriginKind OriginKind { get; }

    public ValueFlags DefaultFlags { get; }

    public IStackFrame OriginatingFrame { get; }

    public ILRuntimeProperty GetValueNoCache(IValueFetchOptions options)
    {
        throw new NotSupportedException("The ILRuntime debug protocol does not expose method arguments.");
    }

    public void ResetCache()
    {
        throw new NotSupportedException("The ILRuntime debug protocol does not expose method arguments.");
    }
}
