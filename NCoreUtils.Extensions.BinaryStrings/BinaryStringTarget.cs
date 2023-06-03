using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NCoreUtils;

internal sealed class BinaryStringMethodTarget
{
    public INamedTypeSymbol ContainingType { get; }

    public IMethodSymbol Method { get; }

    public string Text { get; }

    public Encoding Encoding { get; }

    public BinaryStringMethodTarget(INamedTypeSymbol containingType, IMethodSymbol method, string text, Encoding encoding)
    {
        ContainingType = containingType ?? throw new ArgumentNullException(nameof(containingType));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
    }
}

internal sealed class BinaryStringClassTarget
{
    public INamedTypeSymbol ContainingType { get; }

    public ImmutableArray<BinaryStringMethodTarget> Methods { get; }

    public BinaryStringClassTarget(INamedTypeSymbol containingType, ImmutableArray<BinaryStringMethodTarget> methods)
    {
        ContainingType = containingType ?? throw new ArgumentNullException(nameof(containingType));
        Methods = methods.IsDefault ? ImmutableArray<BinaryStringMethodTarget>.Empty : methods;
    }
}

internal sealed class BinaryStringTarget
{
    public SemanticModel SemanticModel { get; }

    public ClassDeclarationSyntax Host { get; }

    public MethodDeclarationSyntax Method { get; }

    public string Text { get; }

    public Encoding Encoding { get; }

    public BinaryStringTarget(SemanticModel semanticModel, ClassDeclarationSyntax host, MethodDeclarationSyntax method, string text, Encoding encoding)
    {
        SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
        Host = host ?? throw new ArgumentNullException(nameof(host));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Text = text ?? string.Empty;
        Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
    }
}