using System;
using System.Linq;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

public static class ILRuntimeStackFrameExtensions
{
    private static Func<ILRuntimeProperty, string, bool> CompareName => (property, name) => property.Name.Equals(name);

    public static bool HasDebugInfo(this ILRuntimeStackFrame stackFrame)
    {
        return stackFrame.IsManaged;
    }

    public static string GetLanguage(this ILRuntimeStackFrame stackFrame)
    {
        return !stackFrame.IsManaged ? "Transition" : "Managed";
    }

    public static ILRuntimeProperty GetPropertyInLocalVariables(
        this ILRuntimeStackFrame stackFrame,
        string name)
    {
        return stackFrame.GetPropertyInLocalVariables(name, CompareName);
    }

    public static ILRuntimeProperty GetPropertyInLocalVariables(
        this ILRuntimeStackFrame stackFrame,
        string name,
        Func<ILRuntimeProperty, string, bool> predicate)
    {
        return stackFrame.LocalVariables.SingleOrDefault(p => predicate(p, name));
    }

    public static ILRuntimeProperty GetPropertyInThisVariables(
        this ILRuntimeStackFrame stackFrame,
        string name)
    {
        return stackFrame.GetPropertyInThisVariables(name, CompareName);
    }

    public static ILRuntimeProperty GetPropertyInThisVariables(
        this ILRuntimeStackFrame stackFrame,
        string name,
        Func<ILRuntimeProperty, string, bool> predicate)
    {
        return stackFrame.ThisProperty.Children.SingleOrDefault(p => predicate(p, name));
    }

    public static ILRuntimeProperty GetPropertyInChild(this ILRuntimeProperty property, string name)
    {
        return property.GetPropertyInChild(name, CompareName);
    }

    public static ILRuntimeProperty GetPropertyInChild(
        this ILRuntimeProperty property,
        string name,
        Func<ILRuntimeProperty, string, bool> predicate)
    {
        return property.Children.SingleOrDefault(p => predicate(p, name));
    }
}
