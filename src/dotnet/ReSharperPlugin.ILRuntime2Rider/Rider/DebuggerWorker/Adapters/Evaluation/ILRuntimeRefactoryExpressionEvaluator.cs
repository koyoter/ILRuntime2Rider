using ILRuntime.Runtime.Debugger.Expressions;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

/// <summary>
/// Default evaluator: resolves the expression root from locals, then walks
/// member children; falls back to the protocol-level variable lookup by name.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeRefactoryExpressionEvaluator : ILRuntimeExpressionEvaluator
{
    public ILRuntimeRefactoryExpressionEvaluator(IValueServicesFacade<ILRuntimeProperty> facade)
        : base(facade)
    {
    }

    public override int Priority => 10;

    public override bool IsApplicable(IStackFrame frame, IEvaluationOptions evaluationOptions)
    {
        return true;
    }

    protected override ILRuntimeProperty Evaluate(
        IStackFrame frame,
        string expression,
        string[] expressions)
    {
        var stackFrame = frame.GetILRuntimeFrame();
        // Method calls (foo.bar(x)) are parsed by the ILRuntime protocol parser
        // and resolved over the wire; the dotted walk below can't handle them.
        if (expression.Contains("("))
        {
            return (ILRuntimeProperty)stackFrame.Thread.DebuggedProcess.Resolve(
                stackFrame, new Parser(expression).Parse(), 3000);
        }

        ILRuntimeProperty property = null;
        var index = 0;
        var rootName = expressions[index];
        if (rootName == "this")
        {
            index = 1;
            property = stackFrame.ThisProperty;
        }

        property ??= stackFrame.GetPropertyInLocalVariables(rootName);

        if (property == null)
        {
            expression = expression.Insert(0, "this.");
        }

        if (property != null)
        {
            for (var i = index; i < expressions.Length; i++)
            {
                var child = property.GetPropertyInChild(expressions[i]);
                if (child == null)
                {
                    break;
                }

                property = child;
            }
        }

        return (ILRuntimeProperty)stackFrame.GetPropertyByName(expression);
    }
}
