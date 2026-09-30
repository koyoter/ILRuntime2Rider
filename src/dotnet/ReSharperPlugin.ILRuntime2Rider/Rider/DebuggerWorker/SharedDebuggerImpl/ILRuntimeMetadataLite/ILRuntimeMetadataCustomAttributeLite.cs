using System.Collections.Generic;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

public class ILRuntimeMetadataCustomAttributeLite : IMetadataCustomAttributeLite
{
    public ILRuntimeMetadataCustomAttributeLite(ILRuntimeCustomAttribute customAttribute)
    {
    }

    public IMetadataMethodLite UsedConstructor { get; }

    public IReadOnlyList<IMetadataAttributeValueLite> ConstructorArguments { get; }

    public IReadOnlyList<IMetadataFieldInitializationLite> InitializedFields { get; }

    public IReadOnlyList<IMetadataPropertyInitializationLite> InitializedProperties { get; }
}
