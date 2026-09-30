namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;

/// <summary>
/// How an <see cref="ILRuntimeProperty"/>'s raw value string should be presented.
/// </summary>
public enum ILRuntimeValueRoleKind
{
    Object,
    Primitive,
    String,
    Array
}
