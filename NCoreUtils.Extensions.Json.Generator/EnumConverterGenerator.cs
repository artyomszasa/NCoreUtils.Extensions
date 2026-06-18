using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace NCoreUtils.Json;

[Generator(LanguageNames.CSharp)]
public class EnumConverterGenerator : IIncrementalGenerator
{
    private const string attributeSource = @"#nullable enable
namespace NCoreUtils
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class JsonEnumConverterAttribute : System.Attribute
    {
        public System.Type EnumType { get; }

        public JsonEnumConverterAttribute(System.Type enumType)
        {
            EnumType = enumType;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class JsonEnumValueAttribute : System.Attribute
    {
        public string Value { get; }

        public JsonEnumValueAttribute(string value)
        {
            Value = value;
        }
    }
}";

    private static UTF8Encoding Utf8 { get; } = new(false);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context => context.AddSource("BinaryStringAttribute.g.cs", SourceText.From(attributeSource, Utf8)));

        IncrementalValuesProvider<EnumConverterTarget> targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            "NCoreUtils.JsonEnumConverterAttribute",
            (node, _) => node is ClassDeclarationSyntax,
            (ctx, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var attr = ctx.TargetSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "JsonEnumConverterAttribute")
                    ?? throw new InvalidOperationException("Attribute not foound");

                var enumType = (ITypeSymbol)attr.ConstructorArguments[0].Value!;
                var fields = enumType.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(f =>
                    {
                        var value = f.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "JsonEnumValueAttribute") is AttributeData valueAttribute
                            ? (string)valueAttribute.ConstructorArguments[0].Value!
                            : f.Name;
                        return new EnumField(f.Name, value);
                    })
                    .ToImmutableArray();


                return new EnumConverterTarget(
                    ctx.TargetSymbol.ContainingNamespace.ToDisplayString(),
                    ctx.TargetSymbol.Name,
                    enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    fields
                );
            }
        ).Where(o => o is not null)!;

        context.RegisterSourceOutput(targets, (ctx, target) =>
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                var unit = EnumConverterEmitter.EmitUnit(target);
                ctx.AddSource(
                    $"{target.ConverterName}.g.cs",
                    unit.GetText(Utf8)
                );
            }
            catch (Exception exn)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    descriptor: DiagnosticDescriptors.GenericError,
                    location: default,
                    messageArgs: [exn.GetType().Name, exn.Message, exn.StackTrace.Replace('\n', ' ').Replace('\r', ' ')]
                ));
            }
        });
    }
}