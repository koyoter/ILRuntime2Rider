using System.Collections.Generic;
using ILRuntime.Runtime.Debugger;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

public class ILRuntimeStackFrame : IStackFrame
{
    public ILRuntimeStackFrame(ILRuntimeThread thread, StackFrameInfo stackFrameInfo, int index)
    {
        Thread = thread;
        StackFrameInfo = stackFrameInfo;
        Index = index;
        if (!string.IsNullOrEmpty(stackFrameInfo.DocumentName))
        {
            StartLine = stackFrameInfo.StartLine + 1;
            EndLine = stackFrameInfo.EndLine + 1;
            StartColumn = stackFrameInfo.StartColumn + 1;
            EndColumn = stackFrameInfo.EndColumn + 1;
        }

        if (stackFrameInfo.LocalVariables == null)
            stackFrameInfo.LocalVariables = new VariableInfo[0];
        var variables = new List<ILRuntimeProperty>();
        foreach (var localVariable in stackFrameInfo.LocalVariables)
        {
            var instance = ILRuntimeProperty.CreateInstance(this, localVariable);
            RegisterVariable(instance);
            if (localVariable.Name == "this")
                ThisProperty = instance;
            else
                variables.Add(instance);
        }

        LocalVariables = variables;
        // ILRuntime coroutine frames are compiler-generated MoveNext methods whose
        // names contain the ">d" state-machine marker.
        IsCoroutine = stackFrameInfo.MethodName.Contains(">d");
    }

    public Dictionary<string, ILRuntimeProperty> Properties { get; } = new();

    public bool HasThis => ThisProperty != null;

    public bool IsManaged => StackFrameInfo.DocumentName == null;

    public bool IsCoroutine { get; }

    public ILRuntimeProperty ThisProperty { get; }

    public IEnumerable<ILRuntimeProperty> LocalVariables { get; }

    public int StartLine { get; }

    public int StartColumn { get; }

    public int EndLine { get; }

    public int EndColumn { get; }

    public ILRuntimeThread Thread { get; }

    public StackFrameInfo StackFrameInfo { get; }

    public int ThreadID => Thread.ThreadID;

    public int Index { get; }

    public IProperty GetPropertyByName(string name)
    {
        Properties.TryGetValue(name, out var property);
        return property;
    }

    public void RegisterVariable(ILRuntimeProperty property)
    {
        Properties[property.FullName] = property;
    }
}
