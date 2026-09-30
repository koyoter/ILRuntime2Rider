using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Client.AppDomainsInfo;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.References;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values;

public class ILRuntimeFramePhysicalAdapter :
    IFramePhysicalAdapter<ILRuntimeProperty>,
    IFramePhysicalAdapter
{
    private readonly IStackFrame<ILRuntimeStackFrame> frame;
    private readonly ILRuntimeStackFrame runtimeStackFrame;
    private readonly IValueServicesFacade<ILRuntimeProperty> valueServices;
    private readonly ILRuntimeTypeUniverse typeUniverse;
    private IReadOnlyList<IReifiedType> typeGenericArguments;

    public ILRuntimeFramePhysicalAdapter(
        IStackFrame<ILRuntimeStackFrame> frame,
        IValueServicesFacade<ILRuntimeProperty> valueServices)
    {
        this.frame = frame;
        runtimeStackFrame = this.frame.Frame;
        this.valueServices = valueServices;
        typeUniverse = (ILRuntimeTypeUniverse)valueServices.TypeUniverse;
    }

    IEnumerable<IValueReference> IFramePhysicalAdapter.GetLocalVariables()
    {
        return GetLocalVariables();
    }

    public IValueReference<ILRuntimeProperty> GetThis()
    {
        return !runtimeStackFrame.HasThis
            ? null
            : (IValueReference<ILRuntimeProperty>)new ILRuntimeThisReference(frame, valueServices.RoleFactory);
    }

    public IEnumerable<IValueReference<ILRuntimeProperty>> GetArguments()
    {
        // The ILRuntime protocol folds method arguments into the local variable list.
        return Array.Empty<IValueReference<ILRuntimeProperty>>();
    }

    public IEnumerable<IValueReference<ILRuntimeProperty>> GetLocalVariables()
    {
        return frame.Frame.LocalVariables.Select(property =>
            new ILRuntimeLocalVariableReference(frame, property, valueServices.RoleFactory));
    }

    IEnumerable<IValueReference> IFramePhysicalAdapter.GetArguments()
    {
        return GetArguments();
    }

    IValueReference IFramePhysicalAdapter.GetThis() => GetThis();

    public IEnumerable<IValueReference<ILRuntimeProperty>> GetCompilerGeneratedVariables()
    {
        throw new NotSupportedException();
    }

    IEnumerable<IValueReference> IFramePhysicalAdapter.GetCompilerGeneratedVariables()
    {
        return GetCompilerGeneratedVariables();
    }

    public bool ScopeMatchesCurrentIP(int fieldScopeIndex0Based)
    {
        throw new NotSupportedException();
    }

    public int IP { get; }

    public int LocalVariableCount => runtimeStackFrame.StackFrameInfo.LocalVariables.Length;

    public int ArgumentCount => 0;

    public ulong AppDomainId { get; }

    public bool IsManaged => runtimeStackFrame.IsManaged;

    public IModuleInfo ModuleInfo { get; }

    public IMetadataMethodLite Method =>
        runtimeStackFrame.IsManaged || !runtimeStackFrame.HasThis ? null : frame.Frame.ToLite();

    public IReadOnlyList<IReifiedType> TypeGenericArguments
    {
        get
        {
            typeGenericArguments ??= frame.Frame.Properties
                .Select(property => typeUniverse.GetReifiedType(property.Value))
                .ToList();
            return typeGenericArguments;
        }
    }

    public IReadOnlyList<IReifiedType> MethodGenericArguments => new Collection<IReifiedType>();

    public IReifiedType ContainingType { get; }
}
