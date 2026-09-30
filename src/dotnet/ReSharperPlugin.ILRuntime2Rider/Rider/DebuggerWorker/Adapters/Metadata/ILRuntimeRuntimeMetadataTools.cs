using System;
using System.Collections.Generic;
using Debugger.Common.ManagedSymbols;
using JetBrains.Metadata.Reader.API;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Tools;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;

/// <summary>
/// Registered to satisfy the evaluation pipeline's IRuntimeMetadataTools
/// dependency. Imports and corlib namespaces are not resolvable for ILRuntime
/// targets, so these lookups are unsupported.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeRuntimeMetadataTools :
    IRuntimeMetadataTools<IMetadataMethod>,
    IRuntimeMetadataTools
{
    public ILRuntimeRuntimeMetadataTools(
        ILRuntimeAppDomainsManager appDomainsManager)
    {
        AppDomainsManager = appDomainsManager;
    }

    protected ILRuntimeAppDomainsManager AppDomainsManager { get; }

    public IMetadataMethod GetCurrentMetadataMethod(IStackFrame stackFrame)
    {
        throw new NotSupportedException("Metadata imports are not resolvable for ILRuntime targets.");
    }

    public ISet<IManagedImport> GetAvailableImports(IStackFrame stackFrame)
    {
        throw new NotSupportedException("Metadata imports are not resolvable for ILRuntime targets.");
    }

    public ISet<string> GetNamespacesFromCorLib(IStackFrame stackFrame)
    {
        throw new NotSupportedException("Metadata imports are not resolvable for ILRuntime targets.");
    }
}
