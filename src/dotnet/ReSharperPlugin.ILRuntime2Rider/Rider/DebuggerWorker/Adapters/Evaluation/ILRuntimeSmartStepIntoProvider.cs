using JetBrains.Util.Logging;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Utils;
using JetBrains.Diagnostics;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

/// <summary>
/// Registered so <see cref="DebuggerSession`1.SmartStepIntoProvider"/> resolves.
/// Smart step-into is a C#-specific feature the ILRuntime protocol does not
/// implement; the provider always reports "not available" (null info).
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeSmartStepIntoProvider : ISmartStepIntoProvider
{
    public SmartStepIntoInfo GetSmartStepIntoInfo(long threadId)
    {
        return null;
    }
}
