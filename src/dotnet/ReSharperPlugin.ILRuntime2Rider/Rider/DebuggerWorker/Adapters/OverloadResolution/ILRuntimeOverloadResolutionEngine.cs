using Mono.Debugger.Soft;
using Mono.Debugging.Autofac;
using Mono.Debugging.Evaluation.OverloadResolution;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Worker;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.OverloadResolution;

[DebuggerSessionComponent(typeof(ILRuntimeDebuggerType))]
public class ILRuntimeOverloadResolutionEngine :
    OverloadResolutionEngine<TypeMirror, MethodMirror, ParameterInfoMirror, TypeMirror>
{
    public ILRuntimeOverloadResolutionEngine(
        IDebuggerEntityProperties<TypeMirror, MethodMirror, ParameterInfoMirror, TypeMirror> props)
        : base(props)
    {
    }
}
