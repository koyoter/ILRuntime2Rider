using System;
using System.Collections.Generic;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Evaluation.OverloadResolution;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.OverloadResolution;

/// <summary>
/// Method invocation (and therefore overload resolution) is unsupported for
/// ILRuntime targets; every hook is rejected. Registered so the session
/// component graph resolves.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeEntityProperties :
    DebuggerEntityProperties<TypeMirror, MethodMirror, ParameterInfoMirror, TypeMirror>
{
    public override ISet<TypeMirror> NewTypeSet()
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override ISet<TypeMirror> NewTypeSetIgnoreSubstitution()
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool Equals(TypeMirror t1, TypeMirror t2)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool TypeDefinitionEquals(TypeMirror t1, TypeMirror t2)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override string GetTypeName(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsImplicitlyConvertibleTo(TypeMirror from, TypeMirror to)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsSByte(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsShort(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsInt(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsLong(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsByte(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsUShort(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsUInt(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsULong(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsFloat(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsDouble(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsDecimal(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsChar(TypeMirror t)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsGenericMethod(MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override ParameterInfoMirror[] GetParameters(MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetOpenParameterType(MethodMirror method, int index)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror[] GetMethodGenericParameters(MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsExplicitImplementation(MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetDeclaringType(MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsParams(ParameterInfoMirror parameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsOptional(ParameterInfoMirror parameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override string GetParameterName(ParameterInfoMirror parameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetParameterType(ParameterInfoMirror parameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror SubstituteClassTypeParameters(
        ResolutionMethodInfo<TypeMirror, MethodMirror> candidate,
        TypeMirror formalParameterType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror SubstituteAllTypeParameters(
        ResolutionMethodInfo<TypeMirror, MethodMirror> candidate,
        TypeMirror formalParameterType,
        TypeMirror[] methodTypeArgs)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsArrayType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetArrayElementType(TypeMirror arrayType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override int GetArrayRank(TypeMirror arrayType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsArrayGenericInterface(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsTypeParameter(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsMethodTypeParameter(TypeMirror type, MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsPointerType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetPointerElementType(TypeMirror pointerType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsByRefType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetByRefElementType(TypeMirror refType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsConstructedType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror[] GetConstructedTypeArguments(TypeMirror constructedType)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsNullable(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetUnderlyingNullableType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsReferenceType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override uint GetMethodTypeParameterIndex(TypeMirror type, MethodMirror method)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsInterface(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror GetBaseType(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override ICollection<TypeMirror> GetInterfaces(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override TypeMirror[] GetTypeGenericParameters(TypeMirror type)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsCovariant(TypeMirror genericParameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }

    public override bool IsContravariant(TypeMirror genericParameter)
    {
        throw new NotSupportedException("Overload resolution is not supported by the ILRuntime debug protocol.");
    }
}
