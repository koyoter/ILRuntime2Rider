using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Tools;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;

/// <summary>
/// Registered to complete the IMetadataTools service set for the session;
/// mirror-based metadata lookup is not supported for ILRuntime targets.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMirrorMetadataTools :
    IMetadataTools<TypeMirror, MethodMirror, FieldInfoMirror, PropertyInfoMirror, DummyEventInfoMirror>,
    IMetadataTools<TypeMirror, MethodMirror>
{
    private readonly ILRuntimeAppDomainsManager appDomainsManager;

    public ILRuntimeMirrorMetadataTools(
        ILRuntimeAppDomainsManager appDomainsManager)
    {
        this.appDomainsManager = appDomainsManager;
    }

    public TypeMirror GetBaseType(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public MethodMirror[] GetMethods(TypeMirror typeInfo, BindingFlags flags = BindingFlags.Default)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public MethodMirror[] GetMethods(
        TypeMirror typeInfo,
        string name,
        bool caseSensitive = true,
        BindingFlags flags = BindingFlags.Default)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public TypeMirror GetDeclaringType(MethodMirror method)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public TypeMirror GetDeclaringType(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public TypeMirror[] GetNestedTypes(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public string GetFullyQualifiedName(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public string GetShortName(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public bool IsStatic(MethodMirror method)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public bool TypeIsAvailable(string typeName, MethodMirror metaMethod)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public TypeMirror GetReturnType(MethodMirror method)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public IEqualityComparer<TypeMirror> TypeComparer { get; }

    public FieldInfoMirror[] GetFields(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public PropertyInfoMirror[] GetProperties(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public DummyEventInfoMirror[] GetEvents(TypeMirror type)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public string GetName(FieldInfoMirror field)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public string GetName(PropertyInfoMirror property)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public string GetName(DummyEventInfoMirror @event)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public bool IsStatic(FieldInfoMirror field)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public bool IsStatic(PropertyInfoMirror property)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }

    public bool IsStatic(DummyEventInfoMirror @event)
    {
        throw new NotSupportedException("Mirror metadata lookup is not supported for ILRuntime targets.");
    }
}
