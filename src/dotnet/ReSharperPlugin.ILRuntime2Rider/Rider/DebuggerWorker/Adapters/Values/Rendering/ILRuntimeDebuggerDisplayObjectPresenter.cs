using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.Values.Render.ValuePresenters;
using Mono.Debugging.Backend.Values.ValueRoles;
using Mono.Debugging.Client.Values.Render;
using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Rendering;

/// <summary>
/// Declares that ILRuntime values never carry DebuggerDisplay metadata.
/// </summary>
[DebuggerSessionComponent()]
public class ILRuntimeDebuggerDisplayObjectPresenter<TValue> :
    DebuggerDisplayObjectPresenter<TValue>
    where TValue : class
{
    public override bool IsApplicable(
        IObjectValueRole<TValue> role,
        IMetadataTypeLite instanceType,
        IPresentationOptions options,
        IUserDataHolder dataHolder)
    {
        return false;
    }
}
