using System;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.AppDomainsInfo;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Evaluation.NewGen.Compilation;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation.Compilation;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeRoslynReferencesProvider :
    RoslynReferencesProvider<AppDomainMirror, AssemblyMirror, ModuleMirror>
{
    public ILRuntimeRoslynReferencesProvider(
        IAppDomainsManager<AppDomainMirror, AssemblyMirror, ModuleMirror> appDomainsManager)
        : base(appDomainsManager)
    {
    }

    protected override AppDomainMirror GetDomain(IStackFrame frame)
    {
        throw new NotSupportedException();
    }

    protected override IAssemblyInfo<AssemblyMirror, ModuleMirror> GetAssemblyInfo(IStackFrame frame)
    {
        throw new NotSupportedException();
    }
}
