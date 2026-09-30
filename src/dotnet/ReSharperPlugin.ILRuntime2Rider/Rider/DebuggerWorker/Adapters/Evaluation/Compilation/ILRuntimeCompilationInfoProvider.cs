using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.Evaluation.NewGen.Compilation;
using Mono.Debugging.MetadataLite.API;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation.Compilation;

/// <summary>
/// Roslyn-based expression compilation is not available for ILRuntime targets;
/// this provider feeds the pipeline the (empty) frame info it expects.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeCompilationInfoProvider : ICompilationInfoProvider
{
    public ILRuntimeCompilationInfoProvider(ILRuntimeDebuggerSession session)
    {
        Session = session;
    }

    private ILRuntimeDebuggerSession Session { get; }

    public MethodInfoForCompilation GetFrameInfo(
        IStackFrame frame,
        IValueFetchOptions evaluationOptions)
    {
        return null;
    }

    public TypeInfoForCompilation GetTypeInfo(IStackFrame frame, IMetadataTypeLite type)
    {
        return new TypeInfoForCompilation(null, ((ILRuntimeMetadataTypeLite)type).MetadataToken);
    }
}
