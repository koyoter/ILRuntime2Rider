using System.Collections.Generic;
using System.Linq;
using Mono.Debugging.Backend.Threads;
using Mono.Debugging.Backend.Values.ValueReferences;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.TypeSystem;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;

public class ILRuntimeThreadPhysicalAdapter :
    IThreadPhysicalAdapter<ILRuntimeProperty>,
    IThreadPhysicalAdapter,
    IFrameEnumerator<ILRuntimeStackFrame>
{
    private const string ThreadName = "ILRuntime Thread";

    private readonly IThread<ILRuntimeThread> thread;
    private readonly ITypeUniverse typeUniverse;
    private readonly IValueRoleFactory<ILRuntimeProperty> roleFactory;

    public ILRuntimeThreadPhysicalAdapter(
        IThread<ILRuntimeThread> thread,
        ITypeUniverse typeUniverse,
        IValueRoleFactory<ILRuntimeProperty> roleFactory)
    {
        this.thread = thread;
        this.typeUniverse = typeUniverse;
        this.roleFactory = roleFactory;
    }

    public ILRuntimeProperty GetActiveException() => null;

    public long GetId() => thread.Thread.ThreadID;
    public int? GetManagedId() => thread.Thread.ThreadID;

    public string GetName() => ThreadName;

    public IValueReference GetActiveExceptionReference()
    {
#if RS_262_OR_LATER
        return !HasActiveException
            ? null
            : (IValueReference)new ActiveExceptionValueReference<ILRuntimeProperty>(thread, typeUniverse,
                roleFactory, false);
#else
        return !HasActiveException
            ? null
            : (IValueReference)new ActiveExceptionValueReference<ILRuntimeProperty>(thread, typeUniverse,
                roleFactory);
#endif
    }

    public bool HasActiveException => GetActiveException() != null;

#if RS_262_OR_LATER
    // The ILRuntime VM is not CLR managed code; consumers treat false/null as
    // "stay conservative", which matches every other unexercised hook here.
    public bool? IsStoppedInManagedCode() => false;
#endif

    public IReadOnlyList<ILRuntimeStackFrame> EnumerateFrames()
    {
        return thread.Thread.Frames;
    }

    public ILRuntimeStackFrame GetTopFrame()
    {
        return thread.Thread.Frames.FirstOrDefault();
    }
}
