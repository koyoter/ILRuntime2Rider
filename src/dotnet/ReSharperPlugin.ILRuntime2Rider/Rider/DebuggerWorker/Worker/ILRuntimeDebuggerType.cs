using Mono.Debugging.Autofac;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

/// <summary>
/// Marks the debugger session/worker component scope as the ILRuntime engine.
/// </summary>
public class ILRuntimeDebuggerType : DebuggerType
{
    public static readonly ILRuntimeDebuggerType Instance = new();
}
