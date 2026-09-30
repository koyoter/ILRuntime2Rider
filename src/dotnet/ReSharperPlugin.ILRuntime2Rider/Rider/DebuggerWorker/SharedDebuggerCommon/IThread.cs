using ILRuntime.Runtime.Debugger;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

public interface IThread
{
    int ThreadID { get; }
    StackFrameInfo[] StackFrames { set; }
}
