using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace NCoreUtils.ObservableProperties;

[Generator(LanguageNames.CSharp)]
public class ObservableGenerator : IIncrementalGenerator
{
    private const string attributeSource = @"#nullable enable
namespace NCoreUtils
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class HasObservablePropertiesAttribute : System.Attribute
    {
        public HasObservablePropertiesAttribute() { /* noop */ }
    }

    internal sealed class HasObservablePropertyAttribute : System.Attribute
    {
        public System.Type? EqualityComparer { get; set; }

        public NCoreUtils.ChangeTriggerStrategy Strategy { get; set; }

        public HasObservablePropertyAttribute() { /* noop */ }
    }
}";

    private static UTF8Encoding Utf8 { get; } = new(false);

    private static bool IsFieldVariableDeclation(SyntaxNode node)
        => node is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Parent: FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0 } } };

    private static readonly SymbolDisplayFormat qualifiedFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context => context.AddSource("HasObservablePropertyAttribute.g.cs", SourceText.From(attributeSource, Utf8)));

        IncrementalValuesProvider<ObservableFieldTarget> fields = context.SyntaxProvider.ForAttributeWithMetadataName(
            "NCoreUtils.HasObservablePropertyAttribute",
            (node, _) => IsFieldVariableDeclation(node),
            (ctx, cancellationToken) =>
            {
                if (!ctx.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                {
                    return default;
                }

                FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)ctx.TargetNode.Parent!.Parent!;
                IFieldSymbol fieldSymbol = (IFieldSymbol)ctx.TargetSymbol;
                if (fieldSymbol.ContainingSymbol is INamedTypeSymbol host)
                {
                    var attr = ctx.Attributes.FirstOrDefault(a => a.AttributeClass?.Name == "HasObservablePropertyAttribute");
                    INamedTypeSymbol? equalityComparer = null;
                    FieldChangeTriggerStrategy strategy = FieldChangeTriggerStrategy.Default;
                    if (attr is not null)
                    {
                        equalityComparer = attr.NamedArguments.TryGetFirst("EqualityComparer", out var eq)
                            ? eq.Value as INamedTypeSymbol
                            : default;
                        strategy = attr.NamedArguments.TryGetFirst("Strategy", out var strat) && strat.Value is int istrat
                            ? (FieldChangeTriggerStrategy)istrat
                            : FieldChangeTriggerStrategy.Default;
                    }
                    return new ObservableFieldTarget(ctx.SemanticModel, host, fieldSymbol, equalityComparer, strategy);
                }
                return default;
            }
        )
        .Where(t => t is not null)!;

        var targets = fields.Collect().SelectMany((items, cancellationToken) =>
        {
            var types = new ConcurrentDictionary<ITypeSymbol, TypeDescriptor>(SymbolEqualityComparer.Default);
            TypeDescriptor GetOrCreateTypeDescriptor(ITypeSymbol ty)
                => types.GetOrAdd(ty, static ty => new TypeDescriptor(
                    @namespace: ty.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)),
                    name: ty.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    qualifiedName: ty.ToDisplayString(qualifiedFormat)
                ));
            return items
                .GroupBy(item => GetOrCreateTypeDescriptor(item.Host))
                .Select(g => new TargetDescriptor(
                    g.Key,
                    g.Select(item => new PropertyDescriptor(
                        ownerType: g.Key,
                        valueType: GetOrCreateTypeDescriptor(item.Field.Type),
                        fieldName: item.Field.Name,
                        propertyName: item.Field.Name.Capitalize(),
                        equalityComparer: item.EqualityComparer is ITypeSymbol equalityComparer
                            ? new EqualityComparerDescriptor(
                                type: GetOrCreateTypeDescriptor(equalityComparer),
                                singletonMember: equalityComparer.GetMembers().FirstOrDefault(m => m is IPropertySymbol p && p.IsStatic && p.Name == "Singleton")?.Name
                            )
                            : default,
                        strategy: item.Strategy
                    )).ToImmutableArray()
                ));
        });

        context.RegisterSourceOutput(targets, (ctx, target) =>
        {
            var unit = ObservableEmitter.EmitUnit(target);
            ctx.AddSource($"{target.Type.Name}.g.cs", unit.GetText(Utf8));
        });
    }
}