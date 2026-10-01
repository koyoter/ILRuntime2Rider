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

#if RS_262_OR_LATER
    // Abstract member added to SyntheticVariableManager<T> in the 2026.2 patch stream (absent in the
    // 2026.2.0 SDK, present in the 2026.2.3 runtime). Missing override => TypeLoadException at session
    // container build => attach drops instantly. The ILRuntime protocol has no native active-exception
    // concept (ILRuntimeThreadPhysicalAdapter.GetActiveException() is always null), so returning null
    // makes the base class skip the synthetic gracefully.
    protected override IImmediateSyntheticVariable<ILRuntimeProperty> CreateSyntheticVariableForThrownException(
        ILRuntimeProperty exception,
        Mono.Debugging.Client.CallStacks.IStackFrame stackFrame) => null;
#endif
}
