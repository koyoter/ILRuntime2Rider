namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

public interface IStackFrame
{
    int ThreadID { get; }
    int Index { get; }

    IProperty GetPropertyByName(string name);
}
