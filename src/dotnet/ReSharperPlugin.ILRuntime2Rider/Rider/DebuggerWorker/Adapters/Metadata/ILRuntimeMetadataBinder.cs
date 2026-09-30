using System;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.Evaluation.Metadata;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Metadata;

/// <summary>
/// The ILRuntime protocol cannot map reified types back to CLR metadata types,
/// so metadata-based binding is unsupported.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeMetadataBinder : IDebuggerMetadataBinder<TypeMirror>
{
    public TypeMirror ToMetadataType(IStackFrame stackFrame, IReifiedType type)
    {
        throw new NotSupportedException("Metadata type binding is not supported by the ILRuntime debug protocol.");
    }
}
