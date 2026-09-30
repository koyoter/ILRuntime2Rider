using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.Common;
using Debugger.Common.ManagedSymbols;
using Debugger.Common.PathResolution;
using JetBrains.Lifetimes;
using JetBrains.Util;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

/// <summary>
/// Presents a single synthetic AppDomain to the debugger pipeline and caches
/// assembly/module info objects per mirror identity. Symbol document lookups
/// are not supported because the ILRuntime protocol does not expose PDBs.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeAppDomainsManager :
    ObjectInfoHolder<long, AppDomainMirror, ILRuntimeAppDomainInfo>,
    IAppDomainsManager<AppDomainMirror, AssemblyMirror, ModuleMirror>
{
    private readonly ILRuntimeDebuggerSession session;

    public ILRuntimeAppDomainsManager(Lifetime lifetime, ILRuntimeDebuggerSession session)
        : base(lifetime)
    {
        this.session = session;
    }

    IEnumerable<IModuleInfo> IAppDomainsManager.GetAllModules()
    {
        return GetAllModules();
    }

    public IEnumerable<Tuple<IManagedSymbolDocument, IModuleInfo<ModuleMirror>>> GetSymbolDocumentsWithModules(
        IUserDebugPath userPath,
        SymbolsSourceType symbolsSourceType)
    {
        yield break;
    }

    public IEnumerable<Tuple<IManagedSymbolDocument, IModuleInfo<ModuleMirror>>> GetSymbolDocumentsWithModules(
        ISymbolUrl url,
        SymbolsSourceType symbolsSourceType)
    {
        yield break;
    }

    public IEnumerable<IManagedSymbolDocument> GetSymbolDocuments(
        IUserDebugPath userPath,
        SymbolsSourceType symbolsSourceType)
    {
        yield break;
    }

    public IEnumerable<IManagedSymbolDocument> GetSymbolDocuments(
        ISymbolUrl url,
        SymbolsSourceType symbolsSourceType)
    {
        yield break;
    }

    public IEnumerable<IModuleInfo<ModuleMirror>> GetAllModules()
    {
        return GetAllAppDomains().SelectMany(x => x.GetAllModules());
    }

    public IAppDomainInfo GetAppDomainInfoById(ulong id)
    {
        return GetAllAppDomains().FirstOrDefault(x => (long)x.Id == (long)id);
    }

    public IAppDomainInfo<AppDomainMirror, AssemblyMirror, ModuleMirror> GetOrCreateAppDomainInfo(
        AppDomainMirror appDomain)
    {
        return GetOrCreateObjectInfo(appDomain);
    }

    public IAssemblyInfo<AssemblyMirror, ModuleMirror> GetOrCreateAssemblyInfo(
        AssemblyMirror assembly)
    {
        return GetOrCreateAppDomainInfo(assembly.Domain).GetOrCreateAssemblyInfo(assembly);
    }

    public IModuleInfo<ModuleMirror> GetOrCreateModuleInfo(ModuleMirror module)
    {
        return GetOrCreateAssemblyInfo(module.Assembly).GetOrCreateModuleInfo(module);
    }

    public IAppDomainInfo<AppDomainMirror, AssemblyMirror, ModuleMirror> TryGetAppDomainInfo(
        AppDomainMirror appDomain)
    {
        return TryGetObjectInfo(appDomain);
    }

    public IAssemblyInfo<AssemblyMirror, ModuleMirror> TryGetAssemblyInfo(AssemblyMirror assembly)
    {
        throw new NotSupportedException();
    }

    public IModuleInfo<ModuleMirror> TryGetModuleInfo(ModuleMirror module)
    {
        throw new NotSupportedException();
    }

    public IEnumerable<IAppDomainInfo<AppDomainMirror, AssemblyMirror, ModuleMirror>> GetAllAppDomains()
    {
        return GetAllObjectsInfo();
    }

    public bool RemoveAppDomainInfo(AppDomainMirror appDomain)
    {
        return RemoveObjectInfo(appDomain);
    }

    public IDebuggerSession Session => session;

    protected override long GetKey(AppDomainMirror obj)
    {
        return obj.Id;
    }

    protected override ILRuntimeAppDomainInfo CreateObjectInfo(
        Lifetime lifetime,
        AppDomainMirror obj)
    {
        return new ILRuntimeAppDomainInfo(lifetime, session, obj);
    }
}
