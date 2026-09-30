using Mono.Debugging.MetadataLite.API;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

public class ILRuntimeMethod(string name, ILRuntimeProperty returnValue)
{
    public string Name { get; } = name;

    public ILRuntimeProperty ReturnValue { get; set; } = returnValue;

    public IMetadataTypeLite DeclaringType { get; set; }
}
