using System.Collections.Generic;
using Mono.Debugging.Autofac;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;

#if RS_262_OR_LATER
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Filtering;

// JetBrains.Debugger.Worker.PausedContexts (activated unconditionally by CreateSession since the
// 2026.2 patch stream) depends on Mono.Debugging.Filtering.IFramesFilterer, but JetBrains only
// ships implementors gated to CorDebuggerType / SoftDebuggerType (CorFramesFilterer /
// SoftFramesFilterer), so nothing provides the service for the ILRuntime session and the session
// container fails to activate. The ILRuntime protocol has no frame-filtering concept, so an empty
// result is the correct behavior.
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeFramesFilterer : IFramesFilterer
{
    public IReadOnlyCollection<int> GetFramesToFilterOut(ICallStack callStack) => new int[0];
}
#endif
