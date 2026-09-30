using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

public class ILRuntimeAppDomainInfo :
    ObjectInfoHolder<long, AssemblyMirror, ILRuntimeAssemblyInfo>,
    IAppDomainInfo<AppDomainMirror, AssemblyMirror, ModuleMirror>,
    IAppDomainInfo
{
    public ILRuntimeAppDomainInfo(
        Lifetime lifetime,
        ILRuntimeDebuggerSession session,
        AppDomainMirror appDomain)
        : base(lifetime)
    {
        Lifetime = lifetime;
        Session = session;
        AppDomain = appDomain;
    }

    public IAssemblyInfo<AssemblyMirror, ModuleMirror> GetOrCreateAssemblyInfo(
        AssemblyMirror assembly)
    {
        return GetOrCreateObjectInfo(assembly);
    }

    public IModuleInfo<ModuleMirror> GetOrCreateModuleInfo(ModuleMirror module)
    {
        throw new NotSupportedException("Custom attributes are not reported by the ILRuntime debug protocol.");
    }

    public IAssemblyInfo<AssemblyMirror, ModuleMirror> TryGetAssemblyInfo(AssemblyMirror assembly)
    {
        return TryGetObjectInfo(assembly);
    }

    public IModuleInfo<ModuleMirror> TryGetModuleInfo(ModuleMirror module)
    {
        throw new NotSupportedException("Custom attributes are not reported by the ILRuntime debug protocol.");
    }

    public IEnumerable<IAssemblyInfo<AssemblyMirror, ModuleMirror>> GetAllAssemblies()
    {
        return GetAllObjectsInfo();
    }

    public bool RemoveAssemblyInfo(AssemblyMirror assembly)
    {
        return RemoveObjectInfo(assembly);
    }

    public IEnumerable<IModuleInfo<ModuleMirror>> GetAllModules()
    {
        return GetAllAssemblies().SelectMany(x => x.GetAllModules());
    }

    public IEnumerable<IMetadataCustomAttribute> GetAssemblyAttributes(string attributeFullName)
    {
        throw new NotSupportedException("Custom attributes are not reported by the ILRuntime debug protocol.");
    }

    public AppDomainMirror AppDomain { get; }

    public IModuleInfo<ModuleMirror> CorLib { get; }

    IEnumerable<IModuleInfo> IAppDomainInfo.GetAllModules()
    {
        return GetAllModules();
    }

    public ulong Id { get; }

    public string Name { get; }

    public Lifetime Lifetime { get; }

    public IDebuggerSession Session { get; }

    protected override long GetKey(AssemblyMirror obj)
    {
        return obj.Id;
    }

    protected override ILRuntimeAssemblyInfo CreateObjectInfo(
        Lifetime lifetime,
        AssemblyMirror obj)
    {
        return new ILRuntimeAssemblyInfo(lifetime, (ILRuntimeDebuggerSession)Session, obj);
    }
}
