using System;
using System.Collections.Generic;
using Debugger.Common.MetadataAndPdb;
using Debugger.Common.SymReaders;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Access;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util;
using Microsoft.DiaSymReader.PortablePdb;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using Mono.Debugging.ModuleSymbols;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;

/// <summary>
/// Module metadata is loaded lazily through <see cref="ILRuntimeMetadataStorage"/>
/// once the ILRuntime runtime reports the module as loaded.
/// </summary>
public class ILRuntimeModuleInfo : IModuleInfo<ModuleMirror>
{
    private readonly ILRuntimeDebuggerSession session;

    public ILRuntimeModuleInfo(
        Lifetime lifetime,
        ModuleMirror module,
        ILRuntimeDebuggerSession session)
    {
        Module = module;
        this.session = session;
        MetadataProperty =
            new ViewableProperty<DebuggerModuleMetadata>(session.MetadataStorage.GetModuleMetadata(module));
    }

    public Guid Mvid { get; }

    public IEnumerable<IMetadataCustomAttribute> GetAssemblyAttributes(string attributeFullName)
    {
        throw new NotSupportedException("Custom attributes are not reported by the ILRuntime debug protocol.");
    }

    public IDebuggerSession Session => session;

    public IModuleSymbols Symbols { get; }

    public FileSystemPath AssemblyPath { get; }

    public IReadonlyProperty<DebuggerModuleMetadata> MetadataProperty { get; }

    public DebuggerModuleMetadata Metadata => MetadataProperty.Maybe.ValueOrDefault;

    public bool IsCorLib { get; }

    public bool IsProjectOutputAssembly { get; }

    public IAppDomainInfo AppDomain { get; }

    Lifetime IMetadataImportHolder.Lifetime { get; }

    public IMetadataImport MetadataImport { get; }

    public bool IsValid { get; }

    public ModuleMirror Module { get; }
}
