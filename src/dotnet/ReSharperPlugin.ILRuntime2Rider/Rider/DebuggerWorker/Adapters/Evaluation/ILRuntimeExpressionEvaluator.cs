using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.DebuggerOptions;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.Evaluation;
using Mono.Debugging.MetadataLite.API;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;

public abstract class ILRuntimeExpressionEvaluator : IExpressionEvaluator<ILRuntimeProperty>
{
    private const string BaseKeyword = "base";
    private const string ThisKeyword = "this";

    private readonly IValueServicesFacade<ILRuntimeProperty> facade;

    protected ILRuntimeExpressionEvaluator(IValueServicesFacade<ILRuntimeProperty> facade)
    {
        this.facade = facade;
    }

    public abstract bool IsApplicable(IStackFrame frame, IEvaluationOptions evaluationOptions);

    public IValueReference<ILRuntimeProperty> Evaluate(
        IStackFrame frame,
        string expression,
        IEvaluationOptions options)
    {
        return new ILRuntimeRefactoryValueReference(facade.RoleFactory, frame, this, expression);
    }

    public IReadOnlyList<IValuePresentation> EvaluateDebuggerDisplay(IStackFrame frame, ILRuntimeProperty thisObject,
        IMetadataTypeLite thisTypeLite,
        string expression, IEvaluationOptions options, CancellationToken token)
    {
        throw new NotSupportedException();
    }

    public abstract int Priority { get; }

    public IReadOnlyList<IValuePresentation> EvaluateDebuggerDisplay(
        IStackFrame frame,
        ILRuntimeProperty thisObject,
        string expression,
        IEvaluationOptions options,
        CancellationToken token)
    {
        throw new NotSupportedException();
    }

    protected abstract ILRuntimeProperty Evaluate(
        IStackFrame frame,
        string expression,
        string[] expressions);

    public ILRuntimeProperty Evaluate(IStackFrame frame, string expression)
    {
        // ILRuntime has no base-call concept; map the C# `base` keyword onto
        // `this`. Word boundaries keep identifiers like `myBaseField` intact.
        expression = Regex.Replace(expression, $@"\b{BaseKeyword}\b", ThisKeyword);
        var parts = expression.Split('.');
        var result = Evaluate(frame, expression, parts);
        var lastName = parts.Last();
        if (result != null && result.Name != lastName && result.IsNull)
            throw new EvaluatorException($"This {result.Name} is Null", Array.Empty<object>());
        if (result == null)
        {
            throw new EvaluatorException(
                $"This {expression} is Unknown, which is not valid in the given context");
        }

        return result;
    }
}
