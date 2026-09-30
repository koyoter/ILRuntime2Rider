using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILRuntime.Runtime.Debugger.Protocol;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ReSharperPlugin.ILRuntime2Rider.Rider.DebuggerWorker.SharedDebuggerCommon;

/// <summary>
/// Base class for ILRuntime breakpoints. Builds the CSBindBreakpoint request by
/// re-parsing the source document around the breakpoint line, so the ILRuntime
/// debug server can resolve the containing type/method/namespace.
/// Named ...Base to avoid clashing with Mono.Debugging.Client.Breakpoint.
/// </summary>
public abstract class ILRuntimeBreakpointBase
{
    public int EndLine { get; protected set; }
    public int EndColumn { get; protected set; }
    public string DocumentName { get; protected set; }
    public int StartLine { get; protected set; }
    public int StartColumn { get; protected set; }

    public abstract string ConditionExpression { get; }
    public abstract bool IsBound { get; }
    public abstract bool TryBind();
    public abstract void Bound(BindBreakpointResults res);

    protected CSBindBreakpoint CreateBindRequest(bool enabled, BreakpointConditionStyle style, string condition)
    {
        using (var stream = File.OpenRead(DocumentName))
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: DocumentName);
            var textLine = syntaxTree.GetText().Lines[StartLine];
            var location = syntaxTree.GetLocation(textLine.Span);
            var node = location.SourceTree.GetRoot().FindNode(location.SourceSpan, true, true);

            var isLambda = GetAncestorNode<LambdaExpressionSyntax>(node.Parent) != null;
            BaseMethodDeclarationSyntax method = GetAncestorNode<MethodDeclarationSyntax>(node.Parent);
            string methodName = null;
            if (method != null)
            {
                methodName = ((MethodDeclarationSyntax)method).Identifier.Text;
            }
            else
            {
                method = GetAncestorNode<ConstructorDeclarationSyntax>(node.Parent);
                if (method != null)
                {
                    var isStatic = method.Modifiers.Any(m => m.Text == "static");
                    methodName = isStatic ? ".cctor" : ".ctor";
                }
            }

            var className = GetClassName(method);
            var nameSpaceStack = new Stack<string>();
            var usingSyntaxList = new List<UsingDirectiveSyntax>(syntaxTree.GetCompilationUnitRoot().Usings);
            CollectNamespaces(node.Parent, nameSpaceStack, usingSyntaxList);

            return new CSBindBreakpoint
            {
                BreakpointHashCode = GetHashCode(),
                IsLambda = isLambda,
                NamespaceName = string.Join(".", nameSpaceStack),
                TypeName = className,
                MethodName = methodName,
                StartLine = StartLine,
                EndLine = EndLine,
                Enabled = enabled,
                Condition = new BreakpointCondition
                {
                    Style = style,
                    Expression = condition
                },
                UsingInfos = usingSyntaxList.Select(n => new UsingInfo
                {
                    Alias = n.Alias != null ? n.Alias.Name.ToString() : "",
                    Name = n.Name.ToString()
                }).ToArray()
            };
        }
    }

    protected string GetClassName(BaseMethodDeclarationSyntax method)
    {
        var current = GetAncestorNode<ClassDeclarationSyntax>(method);
        string className = null;
        while (current != null)
        {
            className = className == null
                ? current.Identifier.Text
                : $"{current.Identifier.Text}/{className}";
            current = GetAncestorNode<ClassDeclarationSyntax>(current.Parent);
        }

        return className;
    }

    protected T GetAncestorNode<T>(SyntaxNode node) where T : SyntaxNode
    {
        while (node != null)
        {
            if (node is T match)
                return match;
            node = node.Parent;
        }

        return null;
    }

    private static void CollectNamespaces(SyntaxNode node, Stack<string> namespaceList,
        List<UsingDirectiveSyntax> usingSyntaxList)
    {
        if (node == null)
            return;

        if (node is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            namespaceList.Push(namespaceDeclaration.Name.ToString());
            usingSyntaxList.AddRange(namespaceDeclaration.Usings);
        }
        else if (node is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
        {
            namespaceList.Push(fileScopedNamespace.Name.ToString());
            usingSyntaxList.AddRange(fileScopedNamespace.Usings);
        }

        CollectNamespaces(node.Parent, namespaceList, usingSyntaxList);
    }
}
