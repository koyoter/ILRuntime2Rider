using System.Collections.Generic;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Threads;

/// <summary>
/// Threads are identified by the hash the ILRuntime debug server assigned them.
/// </summary>
public class ILRuntimeThreadEqualityComparer<T> : IEqualityComparer<T> where T : ILRuntimeThread
{
    public static readonly ILRuntimeThreadEqualityComparer<T> Instance = new();

    public bool Equals(T x, T y)
    {
        return x != null && y != null && x.ThreadID == y.ThreadID;
    }

    public int GetHashCode(T obj)
    {
        return obj.ThreadID.GetHashCode();
    }
}
