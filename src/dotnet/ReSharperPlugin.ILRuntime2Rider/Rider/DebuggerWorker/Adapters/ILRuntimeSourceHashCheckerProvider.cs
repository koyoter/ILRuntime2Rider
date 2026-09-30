using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using JetBrains.Diagnostics;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeSourceHashCheckerProvider : ISourceHashCheckerProvider
{
    private readonly ILogger logger;

    public ILRuntimeSourceHashCheckerProvider(ILogger logger)
    {
        this.logger = logger;
    }

    public SourceHashChecker GetSourceHashChecker()
    {
        return new SourceHashChecker(logger, SourceHashChecker.ExtendedAlgoTypes);
    }
}
