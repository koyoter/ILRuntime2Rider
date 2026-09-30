using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Debugger;
using JetBrains.Diagnostics.StringInterpolation;
using JetBrains.Util.Logging;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.Adapters.Values.Roles;
using ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerImpl;

/// <summary>
/// A single watchable value (variable, field, array element...) reported by the
/// ILRuntime debug server, plus lazy child enumeration through the wire protocol.
/// </summary>
public class ILRuntimeProperty : IProperty
{
    private static readonly HashSet<string> NumericTypeNames = new()
    {
        "System.Byte", "System.Char", "System.SByte",
        "System.Int16", "System.Int32", "System.Int64",
        "System.UInt16", "System.UInt32", "System.UInt64",
        "System.Single", "System.Double", "System.Decimal"
    };

    private readonly ILRuntimeStackFrame frame;
    private ILRuntimeValueRoleKind valueRole;
    private bool isArray;
    private ILRuntimeProperty[] children;

    public ILRuntimeCustomAttribute[] CustomAttributes;

    private ILRuntimeProperty(ILRuntimeStackFrame frame, VariableInfo info)
    {
        this.frame = frame;
        Info = info;
        ClassifyValue(info);
    }

    /// <summary>
    /// Decides how the raw value string should be presented: primitives get a
    /// compiled CLR type for conversions, strings render as-is, everything
    /// else is treated as an expandable object.
    /// </summary>
    private void ClassifyValue(VariableInfo info)
    {
        var typeName = info.TypeName;
        if (typeName == null)
        {
            valueRole = ILRuntimeValueRoleKind.Object;
            return;
        }

        if (typeName == "System.String")
        {
            valueRole = ILRuntimeValueRoleKind.String;
            return;
        }

        if (typeName == "System.Boolean")
        {
            info.Value = info.Value == "0" || info.Value.ToLower().EndsWith("false") ? "False" : "True";
            valueRole = ILRuntimeValueRoleKind.Primitive;
            CompileType = Type.GetType(info.TypeName);
            return;
        }

        if (NumericTypeNames.Contains(typeName))
        {
            CompileType = Type.GetType(info.TypeName);
            try
            {
                Convert.ChangeType(info.Value, CompileType);
                valueRole = ILRuntimeValueRoleKind.Primitive;
            }
            catch
            {
                valueRole = ILRuntimeValueRoleKind.String;
            }

            return;
        }

        valueRole = ILRuntimeValueRoleKind.Object;
    }

    public static ILRuntimeProperty CreateInstance(ILRuntimeStackFrame frame, VariableInfo info)
    {
        return new ILRuntimeProperty(frame, info);
    }

    public Type CompileType { get; private set; }

    public bool IsString => valueRole == ILRuntimeValueRoleKind.String;

    public bool IsPrimitive => valueRole == ILRuntimeValueRoleKind.Primitive;

    public bool IsArray => isArray;

    public int Rank => 1;

    public bool IsField => Info.Type == VariableTypes.FieldReference;

    public bool IsProperty => Info.Type == VariableTypes.PropertyReference;

    public bool IsNull => Info.Value == "null";

    public IProperty Parent { get; set; }

    public VariableInfo Info { get; }

    public VariableReference[] Parameters { get; set; }

    public string Name => Info.Name;

    public string FullName
    {
        get
        {
            if (Parent != null)
            {
                switch (Info.Type)
                {
                    case VariableTypes.FieldReference:
                    case VariableTypes.PropertyReference:
                    case VariableTypes.Invocation:
                        return $"{((ILRuntimeProperty)Parent).FullName}.{Name}";
                    case VariableTypes.IndexAccess:
                        return $"{((ILRuntimeProperty)Parent).FullName}[{Parameters[0].FullName}]";
                    case VariableTypes.Error:
                    case VariableTypes.NotFound:
                    case VariableTypes.Timeout:
                        return Name;
                    default:
                        throw new NotSupportedException();
                }
            }

            switch (Info.Type)
            {
                case VariableTypes.Integer:
                    return Info.Offset.ToString();
                case VariableTypes.Boolean:
                    return (Info.Offset == 1).ToString();
                case VariableTypes.String:
                    return $"\"{Name}\"";
                default:
                    return Name;
            }
        }
    }

    public ILRuntimeProperty[] Children
    {
        get
        {
            children ??= EnumChildren();
            return children;
        }
    }

    public VariableReference GetVariableReference()
    {
        if (Info == null)
            return null;
        var reference = new VariableReference();
        reference.Address = Info.Address;
        reference.Name = Info.Name;
        reference.Type = Info.Type;
        reference.Offset = Info.Offset;
        if (Parent != null)
            reference.Parent = Parent.GetVariableReference();
        reference.Parameters = Parameters;
        return reference;
    }

    public ILRuntimeProperty[] EnumChildren() => EnumChildren(3000);

    public ILRuntimeProperty[] EnumChildren(int timeout)
    {
        var infos = frame.Thread.DebuggedProcess.EnumChildren(GetVariableReference(),
            frame.ThreadID, frame.Index, (uint)timeout);
        var result = new ILRuntimeProperty[infos.Length];
        for (var index = 0; index < infos.Length; ++index)
        {
            var info = infos[index];
            var instance = CreateInstance(frame, info);
            if (info.Type == VariableTypes.IndexAccess)
            {
                isArray = true;
                instance.Parameters = new VariableReference[1]
                {
                    VariableReference.GetInteger(info.Offset)
                };
            }

            instance.Parent = this;
            result[index] = instance;
            frame.RegisterVariable(instance);
        }

        if (isArray)
        {
            var lengthProperty = CreateArrayProperty(Info.TypeName.Contains("[]") ? "Length" : "Count",
                result.Length);
            lengthProperty.Parent = this;
            frame.RegisterVariable(lengthProperty);
        }

        return result;
    }

    private ILRuntimeProperty CreateArrayProperty(string name, int value)
    {
        return CreateInstance(frame, new VariableInfo
        {
            Name = name,
            Value = value.ToString(),
            ValueType = ValueTypes.Integer,
            Type = VariableTypes.PropertyReference,
            TypeName = "System.Int32"
        });
    }
}
