using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

/// <summary>
/// Coroutine frames are compiler-generated MoveNext methods: locals live as
/// fields of the hidden state machine "this", so member chains are resolved
/// against the state machine's fields instead.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeCoroutineExpressionEvaluator : ILRuntimeExpressionEvaluator
{
    private static readonly Func<ILRuntimeProperty, string, bool> ContainsName =
        (property, name) => property.Name.Contains(name);

    public ILRuntimeCoroutineExpressionEvaluator(IValueServicesFacade<ILRuntimeProperty> facade)
        : base(facade)
    {
    }

    public override int Priority => 11;

    public override bool IsApplicable(IStackFrame frame, IEvaluationOptions evaluationOptions)
    {
        return frame.GetILRuntimeFrame().IsCoroutine;
    }

    protected override ILRuntimeProperty Evaluate(
        IStackFrame frame,
        string expression,
        string[] expressions)
    {
        var ilRuntimeFrame = frame.GetILRuntimeFrame();
        var index = 0;
        var rootName = expressions[index];
        ILRuntimeProperty property;
        if (rootName == "this")
        {
            index = 1;
            property = ilRuntimeFrame.GetPropertyInThisVariables(rootName, ContainsName);
        }
        else
        {
            property = ilRuntimeFrame.GetPropertyInThisVariables(rootName);
        }

        if (property == null)
        {
            property = ilRuntimeFrame.GetPropertyInThisVariables(rootName, ContainsName);
            if (property != null)
                expression = expression.Replace(rootName, property.Name);
        }

        if (property == null)
        {
            property = ilRuntimeFrame.GetPropertyInThisVariables("this", ContainsName);
            if (property != null)
                expression = expression.Insert(0, property.Name + ".");
        }

        if (property != null)
            for (var i = index; i < expressions.Length; ++i)
            {
                var child = property.GetPropertyInChild(expressions[i]);
                if (child != null)
                    property = child;
                else
                    break;
            }

        expression = expression.Insert(0, "this.");
        return (ILRuntimeProperty)ilRuntimeFrame.GetPropertyByName(expression);
    }
}
