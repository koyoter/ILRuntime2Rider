using System;
using Debugger.Common.ManagedSymbols;
using JetBrains.Util;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Util;

internal static class SymbolUrlExtensions
{
    internal static ISymbolUrl TryGetSymbolUrl(this string url, Guid mvid)
    {
        var moduleInfo = mvid != Guid.Empty ? new SymbolModuleInfo(mvid) : null;
        return !url.IsNullOrWhitespace() ? new SimpleSymbolUrl(url, moduleInfo) : (ISymbolUrl)null;
    }
}
