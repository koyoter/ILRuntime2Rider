using System.Net;
using Mono.Debugging.Client;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

public class ILRuntimeDebuggerStartInfo(IPAddress address, int port, string projectName) : DebuggerStartInfo
{
    public IPAddress Address { get; } = address;
    public int Port { get; } = port;
    public string ProjectName { get; } = projectName;
}
