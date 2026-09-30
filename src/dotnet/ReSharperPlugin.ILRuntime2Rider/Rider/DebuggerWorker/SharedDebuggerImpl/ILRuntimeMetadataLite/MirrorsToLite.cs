using System;
using JetBrains.Diagnostics;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Mono.Debugging.MetadataLite.API;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl.ILRuntimeMetadataLite;

/// <summary>
/// Converts ILRuntime runtime mirrors (properties, stack frames) into the
/// Mono.Debugging MetadataLite model used by the evaluation pipeline.
/// </summary>
public static class MirrorsToLite
{
    private static ILogger logger = LogManager.Instance.GetLogger(typeof(MirrorsToLite));

    public static ILRuntimeMetadataTypeLite ToLite(this ILRuntimeProperty property)
    {
        return new ILRuntimeMetadataTypeLite(property);
    }

    public static IMetadataMethodLite ToMethodLite(this ILRuntimeProperty property)
    {
        var method = new ILRuntimeMethod("get", property);
        var parent = (ILRuntimeProperty)property.Parent;
        method.DeclaringType = parent != null ? parent.ToLite() : (IMetadataTypeLite)null;
        return new ILRuntimeMetadataMethodLite(method);
    }

    public static IMetadataFieldLite ToFieldLite(ILRuntimeProperty property)
    {
        LogEx.Info(logger,
            $"ToFieldLite {property.FullName} IsProperty:{property.IsProperty} IsField:{property.IsField}");
        try
        {
            return new ILRuntimeMetadataFieldLite(property);
        }
        catch (Exception e)
        {
            LogEx.Error(logger, e);
            throw;
        }
    }

    public static IMetadataPropertyLite ToPropertyLite(ILRuntimeProperty property)
    {
        LogEx.Info(logger,
            $"ToPropertyLite {property.FullName} IsProperty:{property.IsProperty} IsField:{property.IsField}");
        try
        {
            return new ILRuntimeMetadataPropertyLite(property);
        }
        catch (Exception e)
        {
            LogEx.Error(logger, e);
            throw;
        }
    }

    public static IMetadataMethodLite ToLite(this ILRuntimeStackFrame stackFrame)
    {
        return new ILRuntimeMetadataMethodLite(new ILRuntimeMethod(stackFrame.StackFrameInfo.MethodName, null)
        {
            DeclaringType = stackFrame.ThisProperty.ToLite()
        });
    }

    public static IMetadataCustomAttributeLite ToLite(this ILRuntimeCustomAttribute customAttribute)
    {
        return new ILRuntimeMetadataCustomAttributeLite(customAttribute);
    }
}
