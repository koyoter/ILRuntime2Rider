using ILRuntime.Runtime.Debugger;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

public class ILRuntimeThread : IThread
{
    public ILRuntimeThread(int threadHash, IDebuggedProcess debuggedProcess)
    {
        ThreadID = threadHash;
        DebuggedProcess = debuggedProcess;
    }

    public IDebuggedProcess DebuggedProcess { get; }

    public ILRuntimeStackFrame[] Frames { get; private set; }

    public int ThreadID { get; }

    public StackFrameInfo[] StackFrames
    {
        set
        {
            Frames = new ILRuntimeStackFrame[value.Length];
            for (var index = 0; index < value.Length; ++index)
                Frames[index] = new ILRuntimeStackFrame(this, value[index], index);
        }
    }
}
