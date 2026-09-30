using System;
using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.Evaluation.RuntimeInvocation;
using Mono.Debugging.MetadataLite.Services;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.RuntimeInvocation;

/// <summary>
/// The ILRuntime protocol cannot invoke methods on the debuggee target.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeRuntimeInvocator : RuntimeInvocator<ILRuntimeProperty>
{
    public ILRuntimeRuntimeInvocator(
        IDebuggerMethodResolver debuggerMethodResolver,
        IValueMetadataProvider<ILRuntimeProperty> valueMetadataProvider,
        ILogger logger)
        : base(debuggerMethodResolver, valueMetadataProvider, logger)
    {
    }

    public override InvocationResult<ILRuntimeProperty> RuntimeInvoke(
        IStackFrame stackFrame,
        IValueFetchOptions options,
        InvocationInfo<ILRuntimeProperty> invocationInfo)
    {
        throw new NotSupportedException();
    }
}
