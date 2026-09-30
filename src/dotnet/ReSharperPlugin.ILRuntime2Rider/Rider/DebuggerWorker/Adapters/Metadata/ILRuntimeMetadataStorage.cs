using JetBrains.Lifetimes;
using JetBrains.Metadata.Access;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Tools;
using JetBrains.Diagnostics;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Util;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;

/// <summary>
/// Loads module metadata by reading the assembly from disk, keyed by mirror identity.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMetadataStorage : MetadataStorage<ModuleMirror>
{
    public ILRuntimeMetadataStorage(
        ILogger logger,
        Lifetime lifetime,
        ILRuntimeDebuggerSession session)
        : base(logger, lifetime, ILRuntimeMirrorEqualityComparer<ModuleMirror>.Instance, session)
    {
    }

    protected override string GetModulePathImpl(ModuleMirror module)
    {
        return module.Assembly.Location;
    }

    protected override MetadataToken GetModuleMetadataTokenImpl(ModuleMirror module)
    {
        return MetadataToken.Nil;
    }

    protected override MetadataHolder GetMetadataFromMemoryImpl(ModuleMirror module)
    {
        return new MetadataHolder(FileSystemPath.TryParse(module.Assembly.Location).ReadAllBytes(), true);
    }
}
