using System;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.Values;
using Mono.Debugging.Client.Values.Render;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// The ILRuntime debug protocol has no data breakpoint support; this stub is
/// registered so the session component graph resolves uniformly.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeDataBreakpoints : IDataBreakpoints
{
    public DataBreakpoint SetDataBreakpoint(IValueEntity value)
    {
        throw new NotSupportedException("Data breakpoints are not supported by the ILRuntime debug protocol.");
    }

    public void RemoveDataBreakpoint(IValueEntity value)
    {
        throw new NotSupportedException("Data breakpoints are not supported by the ILRuntime debug protocol.");
    }

    public DataBreakpoint GetDataBreakpointForValue(IValueEntity value, IValueFetchOptions options)
    {
        return null;
    }

    public bool CanSetDataBreakpoint(IValueEntity value)
    {
        return false;
    }

    public bool CanSetDataBreakpoints => false;

    public DataBreakpoint GetDataBreakpointForValue(IValueEntity value)
    {
        return null;
    }
}
