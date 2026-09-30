using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Debugger.Common.ManagedSymbols;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.AppDomainsInfo;
using Mono.Debugging.Client.Breakpoints;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Modules;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Breakpoints;

/// <summary>
/// Line breakpoints are bound by re-parsing user sources and sending a
/// CSBindBreakpoint request; catchpoints and method breakpoints are not
/// supported by the ILRuntime protocol.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeBreakpointManager :
    BreakpointsManager<AppDomainMirror, AssemblyMirror, ModuleMirror, TypeMirror>
{
    private static readonly ILogger Log = LogManager.Instance.GetLogger(typeof(ILRuntimeBreakpointManager));

    private readonly ILRuntimeAppDomainsManager appDomainsManager;
    private readonly ILRuntimeDebuggerSession session;
    private readonly ConcurrentDictionary<BreakEvent, ILRuntimeBreakpoint> allBreakpoints = new();

    public ILRuntimeBreakpointManager(
        ILogger logger,
        Lifetime lifetime,
        ILRuntimeAppDomainsManager appDomainsManager,
        ILRuntimeDebuggerSession session,
        HeuristicPathResolver heuristicPathResolver)
        : base(logger, lifetime, appDomainsManager, session, heuristicPathResolver)
    {
        this.session = session;
        this.appDomainsManager = appDomainsManager;
        BreakEventAdded += OnBreakEventAdded;
    }

    /// <summary>
    /// Breakpoints added before the attach completed must be (re-)bound once
    /// the debugged process is connected.
    /// </summary>
    public void RegisterAllBreakpoints()
    {
        LogEx.Info(Log, $"ILRuntime RegisterAllBreakpoints {BreakEvents.Count}");
        foreach (var breakEvent in BreakEvents)
            OnBreakEventAdded(breakEvent);
    }

    protected override InsertionResult<ModuleMirror>[] InsertBreakpoint(
        LineBreakpoint breakpoint,
        IManagedSymbolMethod method,
        IManagedSequencePoint sequencePoint,
        IModuleInfo<ModuleMirror> moduleInfo)
    {
        throw new NotSupportedException();
    }

    protected override InsertionResult<ModuleMirror>[] InsertCatchpoint(Catchpoint catchpoint)
    {
        throw new NotSupportedException();
    }

#if !RS_262_OR_LATER
    // This (catchpoint, moduleInfo) overload was removed in 2026.2 in favour of the
    // (catchpoint, type, moduleInfo) shape below; catchpoints are not supported by the
    // ILRuntime protocol either way.
    protected override InsertionResult<ModuleMirror>[] InsertCatchpoint(
        Catchpoint catchpoint,
        IModuleInfo<ModuleMirror> moduleInfo)
    {
        throw new NotSupportedException();
    }
#endif

    protected override InsertionResult<ModuleMirror>[] InsertCatchpoint(
        Catchpoint catchpoint,
        TypeMirror type,
        IModuleInfo<ModuleMirror> moduleInfo)
    {
        throw new NotSupportedException();
    }

    protected override InsertionResult<ModuleMirror>[] InsertMethodBreakpoint(
        MethodBreakpoint breakpoint)
    {
        throw new NotSupportedException();
    }

    protected override InsertionResult<ModuleMirror>[] InsertMethodBreakpoint(
        MethodBreakpoint breakpoint,
        IModuleInfo<ModuleMirror> moduleInfo)
    {
        throw new NotSupportedException();
    }

    public override bool Remove(BreakEvent breakEvent)
    {
        if (session.DebuggedProcess == null)
        {
            return base.Remove(breakEvent);
        }

        var breakPoint = GetBreakpoint(breakEvent);
        if (session.DebuggedProcess.Connected && breakPoint != null)
        {
            session.QueuedTasks.QueueRuntimeEventToMainThread("Delete Breakpoint",
                () => session.DebuggedProcess.SendDeleteBreakpoint(breakPoint.GetHashCode()));
            allBreakpoints.TryRemove(breakEvent, out _);
        }

        return base.Remove(breakEvent);
    }

    private void OnBreakEventAdded(BreakEvent breakEvent)
    {
        if (session.DebuggedProcess == null)
        {
            return;
        }

        if (breakEvent is not LineBreakpoint lineBreakpoint || !session.DebuggedProcess.Connected)
            return;

        LogEx.Info(Log, $"ILRuntime OnBreakEventAdded {lineBreakpoint}");
        var bp = new ILRuntimeBreakpoint(session.DebuggedProcess, GetBreakEventInfo(breakEvent),
            lineBreakpoint.FileName.Path, lineBreakpoint.Line, lineBreakpoint.Column);
        bp.TryBind();
        session.DebuggedProcess.AddPendingBreakpoint(bp);
        allBreakpoints[breakEvent] = bp;
    }

    private ILRuntimeBreakpoint GetBreakpoint(BreakEvent breakEvent)
    {
        allBreakpoints.TryGetValue(breakEvent, out var runtimeBreakpoint);
        return runtimeBreakpoint;
    }
}
