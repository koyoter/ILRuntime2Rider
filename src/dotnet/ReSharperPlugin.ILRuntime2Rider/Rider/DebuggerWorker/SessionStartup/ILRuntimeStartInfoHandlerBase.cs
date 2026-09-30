using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JetBrains.Debugger.Model.Plugins.ILRuntime2Rider;
using JetBrains.Debugger.Worker;
using JetBrains.Debugger.Worker.SessionStartup;
using JetBrains.Lifetimes;
using JetBrains.Rider.Model.DebuggerWorker;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.DebuggerOptions;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;

public abstract class ILRuntimeStartInfoHandlerBase<T>(DebuggerType debuggerType)
    : ModelStartInfoHandlerBase<T>(debuggerType)
    where T : ILRuntimeStartInfoBase
{
    protected override IDebuggerSessionOptions CreateSessionOptions(
        Lifetime lifetime,
        T startInfo,
        SessionProperties properties)
    {
        properties.ShowTypeVariables.Value = false;
        return base.CreateSessionOptions(lifetime, startInfo, properties);
    }

    protected override IDebuggerSessionStarter GetSessionStarter(T startInfo,
        IDebuggerSessionOptions debuggerSessionOptions)
    {
        var host = startInfo.Host!;
        IPAddress address;
        try
        {
            address = Dns.GetHostAddresses(host)
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
        }
        catch (Exception e)
        {
            throw new DebuggerStartupException(e.Message, e);
        }

        if (address == null)
        {
            throw new DebuggerStartupException(
                $"Host {host} cannot be resolved into any IPv4 address");
        }

        return new ILRuntimeSessionStarter(
            new ILRuntimeDebuggerStartInfo(address, startInfo.Port, startInfo.ProjectName),
            debuggerSessionOptions);
    }

    private class ILRuntimeSessionStarter(
        ILRuntimeDebuggerStartInfo debuggerStartInfo,
        IDebuggerSessionOptions debuggerSessionOptions)
        : IDebuggerSessionStarter
    {
        public void StartSession(IDebuggerSession session)
        {
            session.Run(debuggerStartInfo, debuggerSessionOptions);
        }

        public IDebuggerSessionOptions Options => debuggerSessionOptions;
    }
}
