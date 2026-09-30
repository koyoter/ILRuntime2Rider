using System;
using JetBrains.Lifetimes;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using Mono.Debugging.Evaluation.NewGen.VirtualExecution;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

/// <summary>
/// The ILRuntime protocol has no method-invocation-on-debuggee support.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeILCodeExecutor : IILCodeExecutor<ILRuntimeProperty>
{
    public ILRuntimeProperty ExecuteMethod(
        IStackFrame frame,
        ExecutionDataBase executionData,
        IEvaluationOptions options,
        Lifetime lifetime)
    {
        throw new NotSupportedException("Method invocation is not supported by the ILRuntime debug protocol.");
    }
}
