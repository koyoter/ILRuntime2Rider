using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.ModulesView;
using Mono.Debugging.ModuleSymbols;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Views;

/// <summary>
/// Registered so the session's IModulesView service resolves; also disables
/// decompiled sources for modules whose symbols load successfully.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeModulesView : ModulesView<ILRuntimeModuleInfo>
{
    protected override void OnSymbolsChanged(
        ILRuntimeModuleInfo moduleInfo,
        SymbolsLoadingResult result)
    {
        if (!result.IsSuccess)
            return;
        moduleInfo.Symbols.DisableDecompiledSources();
    }

    protected override void OnDecompiledSourcesToggled(ILRuntimeModuleInfo moduleInfo)
    {
        throw new NotSupportedException("Toggling decompiled sources is not supported for ILRuntime targets.");
    }
}
