using System;
using System.Collections.Generic;
using JetBrains.Metadata.Access;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

public class ILRuntimeMetadataFieldLite : IMetadataFieldLite
{
    public ILRuntimeProperty Property { get; }

    public ILRuntimeMetadataFieldLite(ILRuntimeProperty property) => Property = property;

    public bool HasCustomAttribute(string attributeName) => false;

    public IReadOnlyList<IMetadataCustomAttributeLite> CustomAttributes =>
        Array.Empty<IMetadataCustomAttributeLite>();

    public string Name => Property.Name;

    public bool IsSpecialName { get; }

    public bool IsRTSpecialName { get; }

    public IMetadataTypeLite DeclaringType => (Property.Parent as ILRuntimeProperty).ToLite();

    public bool IsPrivate { get; }

    public bool IsPublic { get; }

    public bool IsFamily { get; }

    public bool IsFamilyAndAssembly { get; }

    public bool IsFamilyOrAssembly { get; }

    public bool IsAssembly { get; }

    public object GetLiteralValue() => throw new NotSupportedException();

    public IMetadataTypeLite Type => Property.ToLite();

    public bool IsStatic { get; }

    public bool IsLiteral { get; }
    public MetadataToken MetadataToken { get; }
}
