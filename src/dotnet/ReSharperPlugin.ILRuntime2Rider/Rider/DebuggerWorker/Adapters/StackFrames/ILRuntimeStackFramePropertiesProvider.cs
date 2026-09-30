using System;
using Debugger.Common.ManagedSymbols;
using Debugger.Common.Ranges;
using Mono.Debugging.Autofac;
using Mono.Debugging.Backend.CallStacks;
using Mono.Debugging.Backend.Values;
using Mono.Debugging.Client;
using Mono.Debugging.Client.CallStacks;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Util;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SessionStartup;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.StackFrames;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeStackFramePropertiesProvider : IStackFramePropertiesProvider<ILRuntimeStackFrame>
{
    private readonly ILRuntimeDebuggerSession session;

    [Injected]
    public PrioritizedMembersManager PrioritizedMembersManager { get; set; }

    [Injected]
    public IValueServicesFacade<ILRuntimeProperty> ValueServices { get; set; }

    public ILRuntimeStackFramePropertiesProvider(ILRuntimeDebuggerSession session)
    {
        this.session = session;
    }

    public bool IsApplicable(IStackFrame<ILRuntimeStackFrame> frame)
    {
        return frame is StackFrame<ILRuntimeStackFrame>;
    }

    public IStackFrameProperties GetFrameProperties(
        IStackFrame<ILRuntimeStackFrame> frame,
        FramePropertiesCalculationOptions options)
    {
        var runtimeFrame = frame.Frame;
        var stackFrameInfo = runtimeFrame.StackFrameInfo;
        var managedSymbolUrl = stackFrameInfo.DocumentName != null
            ? session.PathResolutionManager.ToManagedSymbolUrl(
                stackFrameInfo.DocumentName.TryGetSymbolUrl(Guid.Empty))
            : null;
#if RS_262_OR_LATER
        // The four name strings collapsed into StackFrameIdentity + presentations in 2026.2.
        return new StackFrameProperties(
            StackFrameOrigin.UserCode,
            new SourceLocation(stackFrameInfo.MethodName, managedSymbolUrl,
                runtimeFrame.StartLine, runtimeFrame.StartColumn, runtimeFrame.EndLine, runtimeFrame.EndColumn),
            0L, runtimeFrame.GetLanguage(), false, false, false, false, false, runtimeFrame.HasDebugInfo(),
            new StackFrameIdentity(string.Empty, string.Empty, stackFrameInfo.MethodName, string.Empty),
            MethodSignaturePresentations.Empty,
            StackFrameKind.ILFrame, false, 0U);
#elif RS_252_OR_LATER
        // 2025.2 inserted the isCompletedAsync flag after isAsync.
        return new StackFrameProperties(
            StackFrameOrigin.UserCode,
            new SourceLocation(stackFrameInfo.MethodName, managedSymbolUrl,
                runtimeFrame.StartLine, runtimeFrame.StartColumn, runtimeFrame.EndLine, runtimeFrame.EndColumn),
            0L, runtimeFrame.GetLanguage(), false, false, false, false, false, runtimeFrame.HasDebugInfo(),
            string.Empty, stackFrameInfo.MethodName, string.Empty, string.Empty,
            StackFrameKind.ILFrame, false, 0U);
#else
        return new StackFrameProperties(
            StackFrameOrigin.UserCode,
            new SourceLocation(stackFrameInfo.MethodName, managedSymbolUrl,
                runtimeFrame.StartLine, runtimeFrame.StartColumn, runtimeFrame.EndLine, runtimeFrame.EndColumn),
            0L, runtimeFrame.GetLanguage(), false, false, false, false, runtimeFrame.HasDebugInfo(),
            string.Empty, stackFrameInfo.MethodName, string.Empty, string.Empty,
            StackFrameKind.ILFrame, false, 0U);
#endif
    }

    public PositionSpan? GetFrameRange(
        IStackFrame<ILRuntimeStackFrame> frame,
        FramePropertiesCalculationOptions options)
    {
        var runtimeFrame = frame.Frame;
        return new PositionSpan(
            new LinePosition(runtimeFrame.StartLine, runtimeFrame.StartColumn),
            new LinePosition(runtimeFrame.EndLine, runtimeFrame.EndColumn));
    }
}
