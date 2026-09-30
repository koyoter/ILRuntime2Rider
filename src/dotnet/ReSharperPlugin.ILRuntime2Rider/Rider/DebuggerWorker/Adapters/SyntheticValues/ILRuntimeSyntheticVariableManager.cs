using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.EvaluateResultValueTracker;
using Mono.Debugging.Backend.SyntheticVariable;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Evaluation;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.SyntheticValues;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeSyntheticVariableManager : SyntheticVariableManager<ILRuntimeProperty>
{
    public ILRuntimeSyntheticVariableManager(IValueRoleFactory<ILRuntimeProperty> valueRoleFactory,
        IEvaluateResultHolder<ILRuntimeProperty> evaluateResultHolder) : base(valueRoleFactory, evaluateResultHolder)
    {
    }

    protected override bool TryGetSyntheticFromEvalException(
        EvaluatorExceptionThrownException<ILRuntimeProperty> evalException,
        out IImmediateSyntheticVariable<ILRuntimeProperty> result)
    {
        throw new NotSupportedException("Synthetic evaluation results are not supported by the ILRuntime debug protocol.");
    }
}
