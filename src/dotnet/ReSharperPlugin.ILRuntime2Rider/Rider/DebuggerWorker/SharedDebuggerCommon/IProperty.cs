using ILRuntime.Runtime.Debugger;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

public interface IProperty
{
    IProperty Parent { get; set; }
    VariableInfo Info { get; }
    VariableReference[] Parameters { get; set; }
    VariableReference GetVariableReference();
}
