using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Util.DataStructures;
using Mono.Debugging.Async;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client.TasksView;
using Mono.Debugging.Client.TasksView.Nodes;
using Mono.Debugging.Client.Values.Render;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Views;

/// <summary>
/// Registered so the session's ITasksView service resolves; the ILRuntime
/// protocol cannot enumerate managed tasks.
/// </summary>
[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeTasksView : ITasksView
{
    public OneOf<IReadOnlyCollection<ITasksViewGraphNode>, TasksViewFailureReason> GetTasksInfo(
        IReadOnlyCollection<CallStack> callStacks, IValueFetchOptions options, CancellationToken token)
    {
        throw new NotSupportedException("Tasks view is not supported by the ILRuntime debug protocol.");
    }

    public bool IsSupported { get; }
}
