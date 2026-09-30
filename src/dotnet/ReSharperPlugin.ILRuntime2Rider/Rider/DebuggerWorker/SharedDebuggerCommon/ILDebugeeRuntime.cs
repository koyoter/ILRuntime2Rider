using Mono.Debugging.Debugee;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// Declares the debugged target as a Mono runtime flavour so the Mono.Debugging
/// pipeline treats it accordingly.
/// </summary>
public class ILDebugeeRuntime : DebugeeRuntime
{
    protected override DebugeeRuntimeFlavour GetFlavour()
    {
        return DebugeeRuntimeFlavour.Mono;
    }
}
