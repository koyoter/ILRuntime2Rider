using System.Collections.Generic;
using Mono.Debugger.Soft;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Util;

/// <summary>
/// Mirrors are identified by the debug server's numeric object id.
/// </summary>
public class ILRuntimeMirrorEqualityComparer<T> : IEqualityComparer<T> where T : Mirror
{
    public static readonly ILRuntimeMirrorEqualityComparer<T> Instance = new();

    public bool Equals(T x, T y)
    {
        return x != null && y != null && x.Id == y.Id;
    }

    public int GetHashCode(T obj)
    {
        return obj.Id.GetHashCode();
    }
}
