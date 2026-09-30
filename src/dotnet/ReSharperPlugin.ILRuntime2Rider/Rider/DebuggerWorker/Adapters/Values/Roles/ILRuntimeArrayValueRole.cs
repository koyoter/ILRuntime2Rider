using System;
using System.Linq;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

public class ILRuntimeArrayValueRole : ArrayValueRoleBase<ILRuntimeProperty>
{
    private readonly ILRuntimeProperty property;
    private readonly int[] lengths;
    private readonly int[] lowerBounds;

    public ILRuntimeArrayValueRole(
        IValueReference<ILRuntimeProperty> valueReference,
        IReifiedType reifiedType,
        IValueServicesFacade<ILRuntimeProperty> valueServices,
        IValueFetchOptions creationOptions)
        : base(valueReference, reifiedType, valueServices, creationOptions)
    {
        property = ValueReference.GetValue(CreationOptions);
        lengths = new int[1] { property.Children.Length };
        lowerBounds = new int[1];
    }

    private ILRuntimeProperty[] Elements => property.Children;

    public override int Rank => property.Rank;

    public override int[] Dimensions => lengths;

    public override int[] BaseIndices => lowerBounds;

    public override IMetadataTypeLite ElementType => property.Children.First().ToLite();

    public override ILRuntimeProperty GetElement(params int[] indices)
    {
        var absoluteIndex = GetAbsoluteIndex(indices);
        return Elements.FirstOrDefault(e => e.Parameters[0].Offset == absoluteIndex);
    }

    public override void SetElement(ILRuntimeProperty newElementValue, params int[] indices)
    {
        throw new NotSupportedException("The ILRuntime protocol cannot modify debuggee values.");
    }

    private int GetAbsoluteIndex(int[] indices)
    {
        return indices.Length == 1
            ? indices[0]
            : throw new NotSupportedException("Multi-dimensional arrays are not supported.");
    }
}
