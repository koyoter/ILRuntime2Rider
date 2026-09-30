using System;
using System.Collections.Generic;
using System.Linq;
using ILRuntime.Runtime.Debugger;
using JetBrains.Diagnostics;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

public class ILRuntimeMetadataTypeLite : IMetadataTypeLite
{
    private static ILogger logger = LogManager.Instance.GetLogger(typeof(MirrorsToLite));

    private readonly VariableInfo variableInfo;

    public ILRuntimeMetadataTypeLite(ILRuntimeProperty property)
    {
        Property = property;
        variableInfo = property.Info;
    }

    public ILRuntimeProperty Property { get; }

    public bool HasCustomAttribute(string attributeName)
    {
        return false;
    }

    public IReadOnlyList<IMetadataCustomAttributeLite> CustomAttributes =>
        Property.CustomAttributes == null
            ? Array.Empty<IMetadataCustomAttributeLite>()
            : Property.CustomAttributes
                .Select(customAttribute => customAttribute.ToLite())
                .ToArray();

    public string Name => variableInfo.TypeName;

    public bool IsSpecialName { get; }

    public bool IsRTSpecialName { get; }

    IMetadataTypeLite IMetadataTypeLite.DeclaringType { get; }

    public int MetadataToken { get; }

    public string FullName => Property.FullName;

    public string ShortName =>
        (Property.IsPrimitive || Property.IsString
            ? variableInfo.TypeName
            : variableInfo.Value).Replace(">d", string.Empty);

    public string AssemblyQualifiedName { get; }

    public bool IsEnum { get; }

    public bool IsClass => !Property.IsPrimitive && !Property.IsString;

    public bool IsClassLike => false;

    public bool IsArray => Property.IsArray;

    public int ArrayRank => Property.Rank;

    public bool IsPointer { get; }

    public bool IsByRef { get; }
    public bool IsInterface { get; }

    public IMetadataTypeLite ElementType =>
        Property.Children
            .First().ToLite();

    public IMetadataTypeLite EnumUnderlyingType { get; }

    public bool IsCorLibType { get; }

    public IMetadataAssemblyLite Assembly { get; }

    public bool IsGenericType { get; }

    public bool IsConstructedGenericType { get; }

    public bool IsGenericTypeDefinition { get; }

    public bool IsTypeParameter { get; }

    public IEnumerable<IMetadataFieldLite> GetFields()
    {
        if (Property.IsNull)
            return Array.Empty<IMetadataFieldLite>();

        var fields = new List<IMetadataFieldLite>();
        foreach (var child in Property.Children)
        {
            if (child.IsProperty)
                continue;
            fields.Add(new ILRuntimeMetadataFieldLite(child));
        }

        return fields;
    }

    public IEnumerable<IMetadataPropertyLite> GetProperties()
    {
        if (Property.IsNull)
            return Array.Empty<IMetadataPropertyLite>();

        var properties = new List<IMetadataPropertyLite>();
        foreach (var child in Property.Children)
        {
            if (child.IsProperty)
                properties.Add(new ILRuntimeMetadataPropertyLite(child));
        }

        return properties;
    }

    public IEnumerable<IMetadataMethodLite> GetMethods()
    {
        return Array.Empty<IMetadataMethodLite>();
    }

    public ElementType GetMetadataElementType()
    {
        throw new NotSupportedException();
    }

    public IReadOnlyList<(string EnumName, ValueType EnumValue)> GetEnumElements()
    {
        throw new NotSupportedException();
    }

    public IMetadataTypeLite MakeGenericType(IMetadataTypeLite[] genericArguments)
    {
        return genericArguments.Length == 0 ? this : (IMetadataTypeLite)null;
    }

    public IMetadataTypeLite GetGenericTypeDefinition()
    {
        return this;
    }

    public IMetadataTypeLite BaseType => null;

    public IReadOnlyCollection<IMetadataTypeLite> ImplementedInterfaces => Array.Empty<IMetadataTypeLite>();

    public IMetadataTypeLite[] GenericArguments => Array.Empty<IMetadataTypeLite>();

    public int GenericParameterCount { get; }

    IMetadataTypeLite IMetadataTypeMemberLite.DeclaringType { get; }

    public bool IsPrivate => variableInfo.IsPrivate;

    public bool IsPublic => !variableInfo.IsPrivate && variableInfo.IsProtected;

    public bool IsFamily { get; }

    public bool IsFamilyAndAssembly { get; }

    public bool IsFamilyOrAssembly { get; }

    public bool IsAssembly { get; }

    public bool Equals(IMetadataTypeLite other)
    {
        return other is ILRuntimeMetadataTypeLite metadataTypeLite &&
               Property.Equals(metadataTypeLite.Property);
    }
}
