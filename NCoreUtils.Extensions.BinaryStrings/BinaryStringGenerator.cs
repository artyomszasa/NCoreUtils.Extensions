using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NCoreUtils;

[Generator(LanguageNames.CSharp)]
public class BinaryStringGenerator : IIncrementalGenerator
{
    private const string attributeSource = @"#nullable enable
using System;

namespace NCoreUtils
{
    internal enum BinaryStringEncoding
    {
        Utf8 = 0,
        Utf16LE = 1,
        Utf16BE = 2
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class BinaryStringAttribute : Attribute
    {
        public string Text { get; }

        public BinaryStringEncoding Encoding { get; }

        public BinaryStringAttribute(string text, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            Text = text;
            Encoding = encoding;
        }
    }
}";

    private static UTF8Encoding Utf8 { get; } = new(false);

    internal static string GeneratorName { get; } = typeof(BinaryStringGenerator).Namespace + "." + nameof(BinaryStringGenerator);

    internal static string Version { get; } = typeof(BinaryStringGenerator).Assembly.GetName().Version.ToString();

    private static bool IsBinaryStringAttribute(string? fullName)
        => fullName == "NCoreUtils.BinaryStringAttribute"
            || fullName == "global::NCoreUtils.BinaryStringAttribute";

    private static string? GetConstantAsMaybeString(SemanticModel semanticModel, ExpressionSyntax expression)
    {
        return semanticModel.GetConstantValue(expression) switch
        {
            { HasValue: true, Value: var value } => value?.ToString(),
            _ => throw new InvalidOperationException($"Unable to get string? value from {expression}")
        };
    }

    private static BinaryStringTarget? ReadTarget(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (ctx.Node is MethodDeclarationSyntax node)
        {
            var semanticModel = ctx.SemanticModel;
            var attr = node.AttributeLists.SelectMany(list => list.Attributes).FirstOrDefault(attribute =>
            {
                var attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                var fullName = attributeSymbol?.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return IsBinaryStringAttribute(fullName);
            });
            if (attr is not null)
            {
                var args = (IReadOnlyList<AttributeArgumentSyntax>?)attr.ArgumentList?.Arguments ?? Array.Empty<AttributeArgumentSyntax>();
                string? text = null;
                Encoding? encoding = null;
                for (var i = 0; i < args.Count; ++i)
                {
                    var arg = args[i];
                    switch (i)
                    {
                        case 0:
                            text = GetConstantAsMaybeString(semanticModel, arg.Expression) ?? string.Empty;
                            break;
                        case 1:
                            var enc = GetConstantAsMaybeString(semanticModel, arg.Expression);
                            encoding = (enc?.ToLowerInvariant()) switch
                            {
                                "utf8" => Utf8,
                                "utf16le" => new UnicodeEncoding(bigEndian: false, byteOrderMark: false),
                                "utf16be" => new UnicodeEncoding(bigEndian: true, byteOrderMark: false),
                                _ => Utf8,
                            };
                            break;
                        default:
                            break;
                    }
                }
                var cds = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                return text is null || cds is null ? null : new BinaryStringTarget(semanticModel, cds, node, text, encoding ?? Utf8);
            }
        }
        return default;
    }

    private static bool IsMethodDeclaration(SyntaxNode node)
        => node is MethodDeclarationSyntax { AttributeLists.Count: >0 } methodSyntax
            && methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.StaticKeyword))
            && methodSyntax.Modifiers.Any(token => token.IsKind(SyntaxKind.PartialKeyword));

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context => context.AddSource("BinaryStringAttribute.g.cs", SourceText.From(attributeSource, Utf8)));

        IncrementalValuesProvider<BinaryStringMethodTarget> methodTargets = context.SyntaxProvider.ForAttributeWithMetadataName(
            "NCoreUtils.BinaryStringAttribute",
            (node, _) => IsMethodDeclaration(node),
            (ctx, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (ctx.TargetSymbol is IMethodSymbol method
                    && method.TryGetBinaryStringData(out var text, out var encoding)
                    && method.ContainingType is INamedTypeSymbol containingType)
                {
                    return new BinaryStringMethodTarget(containingType, method, text, encoding);
                }
                return null;
            }
        ).Where(o => o is not null)!;

        var classTargets = methodTargets.Collect().SelectMany((methods, cancellationToken) =>
        {
            return methods.GroupBy(e => e.ContainingType, SymbolEqualityComparer.Default)
                .Select(g => new BinaryStringClassTarget((INamedTypeSymbol)g.Key!, g.ToImmutableArray()))
                .ToImmutableArray();
        });

        context.RegisterSourceOutput(classTargets, (ctx, target) =>
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            var syntax = BinaryStringEmitter.EmitCompilationUnit(target);
            ctx.AddSource(
                $"{target.ContainingType.Name}.g.cs",
                syntax.GetText(Utf8)
            );
        });
    }
}