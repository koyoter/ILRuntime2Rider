using System;
using System.Collections.Generic;
using System.Threading;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Client.HeapView;
using Mono.Debugging.Client.Values;
using Mono.Debugging.MetadataLite.API;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Views;

/// <summary>
/// Registered so the session's IHeapView service resolves; the ILRuntime
/// protocol cannot inspect the managed heap.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeHeapView : IHeapView
{
    public IValueReference GetAtAddress(ulong address, CancellationToken token)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<ManagedHeapTypeInfo> GetHeapTypeInformation(CancellationToken token)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IValue> GetValuesForInstancesOf(IMetadataTypeLite metadataTypeLite, Paging? paging = null,
        CancellationToken? token = null)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IValueReference> GetReferencesForInstancesOf(IMetadataTypeLite metadataTypeLite,
        Paging? paging = null, CancellationToken? token = null)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IValueReference> GetReferencesForInstancesAndDescendantsOf(IMetadataTypeLite metadataTypeLite,
        Paging? paging = null, CancellationToken? token = null)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IValue> GetValuesForInstancesAndDescendantsOf(IMetadataTypeLite metadataTypeLite,
        Paging? paging = null, CancellationToken? token = null)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IValue> GetInstances(string clrTypeName, int startIndex, int count, CancellationToken token)
    {
        throw new NotSupportedException("Heap inspection is not supported by the ILRuntime debug protocol.");
    }

    public bool IsSupported { get; }
}
