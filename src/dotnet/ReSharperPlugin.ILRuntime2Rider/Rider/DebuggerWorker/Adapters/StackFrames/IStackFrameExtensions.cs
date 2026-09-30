using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Client.CallStacks;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;

public static class IStackFrameExtensions
{
    public static ILRuntimeStackFrame GetILRuntimeFrame(this IStackFrame frame)
    {
        return ((IStackFrame<ILRuntimeStackFrame>)frame).Frame;
    }
}
