using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Client.Values;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.Utils;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Evaluation;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;

/// <summary>
/// A watch/expression reference: re-evaluates the expression string through the
/// ILRuntime expression evaluator when the cached value is invalidated.
/// </summary>
public class ILRuntimeRefactoryValueReference : ICachedValueReference<ILRuntimeProperty>, IExpressionValueReference
{
    private readonly RollbackIfFailCachedValue<ILRuntimeProperty, IValueFetchOptions> cachedValue;
    private readonly ILRuntimeExpressionEvaluator expressionEvaluator;
    private readonly IValueRoleFactory<ILRuntimeProperty> roleFactory;

    public ILRuntimeRefactoryValueReference(
        IValueRoleFactory<ILRuntimeProperty> roleFactory,
        IStackFrame originatingFrame,
        ILRuntimeExpressionEvaluator expressionEvaluator,
        string expression)
    {
        this.roleFactory = roleFactory;
        this.expressionEvaluator = expressionEvaluator;
        cachedValue = this.CreateRollbackIfFailCacheValue(roleFactory.IsValid);
        Expression = expression;
        OriginatingFrame = originatingFrame;
    }

    public IDebuggerHierarchyObject Parent => OriginatingFrame;

    public IValueRole GetPrimaryRole(IValueFetchOptions options)
    {
        return roleFactory.GetPrimaryRole(this, options);
    }

    public IMetadataTypeLite DeclaredType { get; }

    public string DefaultName => Expression;

    public ILRuntimeProperty GetValue(IValueFetchOptions options)
    {
        return cachedValue.Get(options);
    }

    public void SetValue(ILRuntimeProperty value, IValueFetchOptions options)
    {
        throw ValueErrors.ReadOnlyReference();
    }

    public bool IsWriteable { get; }

    public ValueOriginKind OriginKind => ValueOriginKind.Expression;

    public ValueFlags DefaultFlags => ValueFlags.None;

    public IStackFrame OriginatingFrame { get; }

    public ILRuntimeProperty GetValueNoCache(IValueFetchOptions options)
    {
        return expressionEvaluator.Evaluate(OriginatingFrame, Expression);
    }

    public void ResetCache()
    {
        cachedValue.Reset();
    }

    public string Expression { get; }
}
