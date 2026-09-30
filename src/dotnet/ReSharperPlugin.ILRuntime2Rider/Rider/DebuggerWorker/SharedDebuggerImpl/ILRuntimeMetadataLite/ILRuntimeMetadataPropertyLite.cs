using System;
using System.Collections.Generic;
using JetBrains.Metadata.Access;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

public class ILRuntimeMetadataPropertyLite : IMetadataPropertyLite
{
    private readonly ILRuntimeProperty property;

    public ILRuntimeMetadataPropertyLite(ILRuntimeProperty property) => this.property = property;

    public bool HasCustomAttribute(string attributeName) => false;

    public IReadOnlyList<IMetadataCustomAttributeLite> CustomAttributes =>
        Array.Empty<IMetadataCustomAttributeLite>();

    public string Name => property.Info.Name;

    public bool IsSpecialName { get; }

    public bool IsRTSpecialName { get; }

    public IMetadataTypeLite DeclaringType => property.ToLite();

    public bool IsIndexer() => false;

    public IMetadataTypeLite Type => property.ToLite();
    public IMetadataTypeLite SubstitutedType { get; }

    public IMetadataMethodLite Getter => property.ToMethodLite();

    public IMetadataMethodLite Setter { get; }
    public MetadataToken MetadataToken { get; }
}
