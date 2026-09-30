using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.ObjectId;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.ObjectId;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeObjectSupport : IObjectIdSupport<ILRuntimeProperty>
{
    public bool IsValueSupported(ILRuntimeProperty value)
    {
        return true;
    }

    public ulong GetAddress(ILRuntimeProperty value)
    {
        return (ulong)value.Info.Address;
    }

    public IUserVariableReference<ILRuntimeProperty> CreateReference(
        ILRuntimeProperty value,
        string label,
        bool isStrongReference)
    {
        throw new NotSupportedException("Object-id references are not supported by the ILRuntime debug protocol.");
    }
}
