using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NCoreUtils;

internal static class BinaryStringExtensions
{
    private static UTF8Encoding Utf8 { get; } = new(false);

    private static UnicodeEncoding Utf16Le { get; } = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);

    private static UnicodeEncoding Utf16Be { get; } = new UnicodeEncoding(bigEndian: true, byteOrderMark: false);

    public static bool TryGetBinaryStringData(this IMethodSymbol method, [MaybeNullWhen(false)] out string text, [MaybeNullWhen(false)] out Encoding encoding)
    {
        var data = method.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "BinaryStringAttribute");
        if (data is not null && data.ConstructorArguments[0].Value is string textValue)
        {
            text = textValue;
            encoding = data.NamedArguments.Length switch
            {
                0 => Utf8,
                _ => data.NamedArguments.Select(GetEncoding).Where(enc => enc is not null).FirstOrDefault() ?? Utf8
            };
            return true;
        }
        text = default;
        encoding = default;
        return false;

        static Encoding? GetEncoding(KeyValuePair<string, TypedConstant> namedArgument)
        {
            if (namedArgument.Key == "Encoding")
            {
                return Convert.ToInt32(namedArgument.Value.Value) switch
                {
                    0 => Utf8,
                    1 => Utf16Le,
                    2 => Utf16Be,
                    _ => default
                };
            }
            return default;
        }
    }

    public static MethodDeclarationSyntax AddMethodAccessModifiers(this MethodDeclarationSyntax mds, Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => mds,
            Accessibility.Private => mds.AddModifiers(Token(SyntaxKind.PrivateKeyword)),
            Accessibility.ProtectedAndInternal => mds.AddModifiers(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword)),
            Accessibility.Protected => mds.AddModifiers(Token(SyntaxKind.ProtectedKeyword)),
            Accessibility.Internal => mds.AddModifiers(Token(SyntaxKind.InternalKeyword)),
            Accessibility.ProtectedOrInternal => throw new NotSupportedException("ProtectedOrInternal accessibility is not supported"),
            Accessibility.Public => mds.AddModifiers(Token(SyntaxKind.PublicKeyword)),
            _ => throw new InvalidOperationException("invalid accessibility")
        };
    }
}