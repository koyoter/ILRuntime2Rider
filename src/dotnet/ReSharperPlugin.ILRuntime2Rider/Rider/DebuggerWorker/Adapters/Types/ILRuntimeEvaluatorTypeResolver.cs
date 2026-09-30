using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Evaluation.TypeResolution;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeEvaluatorTypeResolver : EvaluatorTypeResolver
{
    public ILRuntimeEvaluatorTypeResolver(
        ILRuntimeMetadataTools tools,
        ILRuntimeRuntimeMetadataTools runtimeTools,
        IExternalTypeResolver typeResolver,
        ILogger logger)
        : base(tools, runtimeTools, typeResolver, logger)
    {
    }

    protected override bool NeedUseExternalResolver(IStackFrame stackFrame)
    {
        return true;
    }
}
