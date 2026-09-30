using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Threads;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.CallStacks;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeThreadShimsFactory : IThreadShimsFactory<ILRuntimeThread>
{
    [Injected]
    public ILRuntimeDebuggerSession Session { get; set; }

    [Injected]
    public ITypeUniverse TypeUniverse { get; set; }

    [Injected]
    public IValueRoleFactory<ILRuntimeProperty> RoleFactory { get; set; }

    [Injected]
    public IStackFrameServicesFacade<ILRuntimeStackFrame> FrameServices { get; set; }

    public IThreadPhysicalAdapter CreatePhysicalAdapter(IThread<ILRuntimeThread> thread)
    {
        return new ILRuntimeThreadPhysicalAdapter(thread, TypeUniverse, RoleFactory);
    }

    public ICallStack CreateCallStack(IThread<ILRuntimeThread> thread)
    {
        return new CallStack<ILRuntimeThread, ILRuntimeStackFrame>(thread, Session, FrameServices);
    }
}
