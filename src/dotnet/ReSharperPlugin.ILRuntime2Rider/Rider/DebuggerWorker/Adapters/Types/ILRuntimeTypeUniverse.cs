using System;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Evaluation;
using Mono.Debugging.MetadataLite.API;
using Mono.Debugging.MetadataLite.JetMetadataLite;
using Mono.Debugging.Tools;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Types;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeTypeUniverse : TypeUniverseBase<ILRuntimeProperty>
{
    public override IMetadataTypeLite GetTypeByAssemblyQualifiedName(
        ulong appDomainId,
        IMetadataType parsedTypeName)
    {
        var type = parsedTypeName.Transform(new ToRealMetadataTransformer());
        return type == null ? null : type.ToLite();
    }

    public override IMetadataTypeLite GetTypeByAssemblyQualifiedName(
        IStackFrame frame,
        IMetadataType parsedTypeName)
    {
        return null;
    }

    public override IMetadataTypeLite GetTypeByFullName(IStackFrame frame, string fullName)
    {
        return ((ILRuntimeProperty)frame.GetILRuntimeFrame().GetPropertyByName(fullName) ??
                throw new EvaluatorException("Evaluator Fail")).ToLite();
    }

    public override IReifiedType GetReifiedType(ulong appDomain, IMetadataTypeLite metadataType)
    {
        return new ILRuntimeReifiedType(((ILRuntimeMetadataTypeLite)metadataType).Property, this, ValueServices,
            Invocator, DebuggingHelper);
    }

    protected override bool IsTypeInitialized(IReifiedType reifiedType)
    {
        throw new NotSupportedException("Static type initialization tracking is not supported for ILRuntime targets.");
    }

    protected override void MarkTypeInitialized(IReifiedType reifiedType)
    {
        throw new NotSupportedException("Static type initialization tracking is not supported for ILRuntime targets.");
    }

    public ILRuntimeReifiedType GetReifiedType(ILRuntimeProperty property)
    {
        return new ILRuntimeReifiedType(property, this, ValueServices, Invocator, DebuggingHelper);
    }

    private class ToRealMetadataTransformer : MetadataTypeTransformerBase<IMetadataType>
    {
        public ToRealMetadataTransformer()
            : base(EmptyArray<IMetadataType>.Instance, EmptyArray<IMetadataType>.Instance)
        {
        }

        protected override IMetadataType CreateArray(
            IMetadataType elementType,
            uint rank,
            int[] arrayTypeLBounds,
            uint[] arrayTypeSizes,
            IMetadataTypeModifier[] arrayTypeTypeModifiers,
            bool arrayTypePinned)
        {
            return MetadataTypeFactory.CreateArrayType(elementType, rank, arrayTypeSizes, arrayTypeLBounds,
                arrayTypeTypeModifiers, arrayTypePinned);
        }

        protected override IMetadataType CreatePointer(
            IMetadataType underlyingType,
            IMetadataTypeModifier[] pointerTypeTypeModifiers,
            bool pointerTypePinned)
        {
            return MetadataTypeFactory.CreatePointerType(underlyingType, pointerTypeTypeModifiers, pointerTypePinned);
        }

        protected override IMetadataType CreateReference(
            IMetadataType underlyingType,
            IMetadataTypeModifier[] referenceTypeTypeModifiers,
            bool referenceTypePinned)
        {
            return underlyingType == null
                ? null
                : (IMetadataType)MetadataTypeFactory.CreateReferenceType(underlyingType, referenceTypeTypeModifiers,
                    referenceTypePinned);
        }

        protected override IMetadataType CreateClass(
            IMetadataTypeInfo classInfo,
            IMetadataType[] genericTypeArguments)
        {
            return MetadataTypeFactory.CreateClassType(new UnresolvedTypeInfo(classInfo.AssemblyQualifiedName),
                genericTypeArguments);
        }
    }
}
