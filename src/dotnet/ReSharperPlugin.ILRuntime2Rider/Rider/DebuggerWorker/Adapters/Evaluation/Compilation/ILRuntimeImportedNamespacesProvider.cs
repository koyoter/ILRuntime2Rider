using System.Collections.Generic;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using Mono.Debugging.Evaluation.NewGen.Compilation;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation.Compilation;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeImportedNamespacesProvider : IImportedNamespacesProvider
{
    public IReadOnlyCollection<string> GetImports(IStackFrame frame, IEvaluationOptions options)
    {
        return options.NamespaceImports;
    }
}
