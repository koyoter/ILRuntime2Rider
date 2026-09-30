using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.EvaluateResultValueTracker;
using Mono.Debugging.Backend.SyntheticVariable;
using Mono.Debugging.Backend.Values;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.SyntheticValues;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeSyntheticValueProvider : SyntheticAdditionalValuesProvider<ILRuntimeProperty>
{
    public ILRuntimeSyntheticValueProvider(IValueServicesFacade<ILRuntimeProperty> valueServices,
        SyntheticVariableManager<ILRuntimeProperty> syntheticVariableManager,
        IEvaluateResultHolder<ILRuntimeProperty> evaluateResultHolder) : base(valueServices,
        syntheticVariableManager, evaluateResultHolder)
    {
    }
}
