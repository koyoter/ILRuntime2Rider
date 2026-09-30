using System.Collections.Generic;
using JetBrains.Util;
using Mono.Debugger.Soft;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.RuntimeInvocation;

/// <summary>
/// Extension method lookup is not supported for ILRuntime targets; the cache
/// intentionally resolves to no candidates.
/// </summary>
internal class ExtensionMethodsCache
{
    public ExtensionMethodsCache(ILogger logger)
    {
    }

    public IReadOnlyList<MethodMirror> GetExtensionsMethods(string methodName)
    {
        return null;
    }
}
