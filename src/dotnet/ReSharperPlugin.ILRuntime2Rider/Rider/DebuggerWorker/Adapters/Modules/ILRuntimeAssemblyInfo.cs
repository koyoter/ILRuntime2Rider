using System;
using System.Collections.Generic;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.Util;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

public class ILRuntimeAssemblyInfo :
    ObjectInfoHolder<FileSystemPath, ModuleMirror, ILRuntimeModuleInfo>,
    IAssemblyInfo<AssemblyMirror, ModuleMirror>
{
    private readonly ViewableList<IModuleInfo> modules;

    public ILRuntimeAssemblyInfo(
        Lifetime lifetime,
        ILRuntimeDebuggerSession session,
        AssemblyMirror assembly)
        : base(lifetime)
    {
        Lifetime = lifetime;
        Session = session;
        modules = new ViewableList<IModuleInfo>();
        MetadataReferenceForRoslyn =
            new MetadataReferenceForRoslyn(lifetime, FileSystemPath.TryParse(assembly.Location), modules);
    }

    public IModuleInfo<ModuleMirror> GetOrCreateModuleInfo(ModuleMirror module)
    {
        return GetOrCreateObjectInfo(module);
    }

    public IModuleInfo<ModuleMirror> TryGetModuleInfo(ModuleMirror module)
    {
        return TryGetObjectInfo(module);
    }

    public IEnumerable<IModuleInfo<ModuleMirror>> GetAllModules()
    {
        return GetAllObjectsInfo();
    }

    public bool RemoveModuleInfo(ModuleMirror module)
    {
        return RemoveObjectInfo(module);
    }

    public Lifetime Lifetime { get; }

    public IDebuggerSession Session { get; }

    public AssemblyMirror Assembly { get; }

    public MetadataReferenceForRoslyn MetadataReferenceForRoslyn { get; }

    public event Action<IModuleInfo<ModuleMirror>> CorLibLoaded;

    protected override FileSystemPath GetKey(ModuleMirror module)
    {
        return FileSystemPath.TryParse(module.Assembly.Location);
    }

    protected override ILRuntimeModuleInfo CreateObjectInfo(
        Lifetime lifetime,
        ModuleMirror module)
    {
        var moduleInfo = new ILRuntimeModuleInfo(lifetime, module, (ILRuntimeDebuggerSession)Session);
        modules.Add(lifetime, moduleInfo);
        return moduleInfo;
    }
}
