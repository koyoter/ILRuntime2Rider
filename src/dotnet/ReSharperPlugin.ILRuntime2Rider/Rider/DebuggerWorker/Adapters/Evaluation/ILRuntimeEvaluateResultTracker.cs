using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.EvaluateResultValueTracker;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeEvaluateResultTracker(ILogger logger) : BaseEvaluateResultTracker<ILRuntimeProperty>(logger)
{
    // Registered as a session component so it provides the
    // IEvaluateResultHolder<ILRuntimeProperty> service consumed by the
    // synthetic variable providers.
    [DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
    private class ILRuntimeEvaluateResultHolder : EvaluateResultHolder
    {
    }
}
