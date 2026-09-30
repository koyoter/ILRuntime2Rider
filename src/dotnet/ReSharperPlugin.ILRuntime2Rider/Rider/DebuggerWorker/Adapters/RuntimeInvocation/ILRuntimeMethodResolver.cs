using System;
using System.Collections.Generic;
using JetBrains.Util;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Evaluation.Metadata;
using Mono.Debugging.Evaluation.OverloadResolution;
using Mono.Debugging.Evaluation.RuntimeInvocation;
using Mono.Debugging.Tools;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.RuntimeInvocation;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMethodResolver :
    DebuggerMethodResolver<TypeMirror, TypeMirror, MethodMirror, ParameterInfoMirror, TypeMirror>
{
    private readonly ExtensionMethodsCache extensionMethodsCache;

    public ILRuntimeMethodResolver(
        IOverloadResolutionEngine<TypeMirror, MethodMirror> overloadResolutionEngine,
        IDebuggerMetadataBinder<TypeMirror> metadataBinder,
        IMetadataTools<TypeMirror, MethodMirror> tools,
        ILogger logger)
        : base(overloadResolutionEngine, metadataBinder, tools)
    {
        extensionMethodsCache = new ExtensionMethodsCache(logger);
    }

    protected override IReadOnlyList<MethodMirror> CollectExtensionMethods(
        IStackFrame stackFrame,
        string methodName)
    {
        throw new NotSupportedException("Method invocation is not supported by the ILRuntime debug protocol.");
    }
}
