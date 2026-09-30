using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.Services;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

/// <summary>
/// Evaluation cannot fabricate values on an ILRuntime target (there is no code
/// execution path back into the interpreter), so every factory method is
/// intentionally unsupported. Registered to complete the session component graph.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeValueFactory : IValueFactory<ILRuntimeProperty>
{
    public ILRuntimeProperty CreatePrimitive(
        IStackFrame frame,
        IValueFetchOptions options,
        object value)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateString(IStackFrame frame, IValueFetchOptions options, string value)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateSzArray(
        IStackFrame frame,
        IValueFetchOptions options,
        IMetadataTypeLite elementType,
        int length)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateMdArray(
        IStackFrame frame,
        IValueFetchOptions options,
        IMetadataTypeLite elementType,
        int[] lengths,
        int[] lowerBounds)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateNullReference(IStackFrame frame, IValueFetchOptions options)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateNullReference(
        IStackFrame frame,
        IValueFetchOptions options,
        IMetadataTypeLite type)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateObjectNoConstructor(
        IStackFrame frame,
        IValueFetchOptions options,
        IMetadataTypeLite type)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateObjectWithConstructorIfNeeded(
        IStackFrame stackFrame,
        IEvaluationOptions options,
        IMetadataTypeLite type,
        ILRuntimeProperty[] args)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateByteArray(
        IStackFrame frame,
        IValueFetchOptions options,
        byte[] values)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }

    public ILRuntimeProperty CreateDecimal(
        IStackFrame frame,
        IValueFetchOptions options,
        decimal value)
    {
        throw new NotSupportedException("Value fabrication is not supported by the ILRuntime debug protocol.");
    }
}
