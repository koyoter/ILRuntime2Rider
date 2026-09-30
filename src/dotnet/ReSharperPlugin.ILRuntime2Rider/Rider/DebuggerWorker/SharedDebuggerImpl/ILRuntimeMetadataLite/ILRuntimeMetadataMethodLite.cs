using System;
using System.Collections.Generic;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

public class ILRuntimeMetadataMethodLite : IMetadataMethodLite
{
    private readonly ILRuntimeMethod method;

    public ILRuntimeMetadataMethodLite(ILRuntimeMethod method)
    {
        this.method = method;
    }

    public bool HasCustomAttribute(string attributeName) => throw new NotSupportedException();

    public IReadOnlyList<IMetadataCustomAttributeLite> CustomAttributes =>
        Array.Empty<IMetadataCustomAttributeLite>();

    public string Name => method.Name;

    public bool IsSpecialName { get; }

    public bool IsRTSpecialName { get; }

    public IMetadataTypeLite DeclaringType => method.DeclaringType;

    public bool IsPrivate { get; }

    public bool IsPublic { get; }

    public bool IsFamily { get; }

    public bool IsFamilyAndAssembly { get; }

    public bool IsFamilyOrAssembly { get; }

    public bool IsAssembly { get; }

    public bool Equals(IMetadataMethodLite other) => throw new NotSupportedException();

    public IMetadataMethodLite GetGenericDefinition() => this;

    public IMetadataMethodLite MakeGenericMethod(params IMetadataTypeLite[] genericArguments)
    {
        throw new NotSupportedException();
    }

    public IMetadataTypeLite OwnerType => DeclaringType;

    public IMetadataTypeLite ReturnType => method.ReturnValue.ToLite();

    public IMetadataParameterLite[] Parameters => Array.Empty<IMetadataParameterLite>();

    public bool IsConstructor { get; }

    public bool IsStatic { get; }

    public bool IsVirtual { get; }

    public bool IsNewSlot { get; }

    public bool IsAbstract { get; }

    public bool IsGenericMethod { get; }

    public bool IsGenericMethodDefinition { get; }

    public IMetadataTypeLite[] GenericArguments { get; }

    public uint MetadataToken { get; }
}
