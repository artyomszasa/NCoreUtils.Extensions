using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        var classes = fields.Collect().SelectMany((targets, cancellationToken) =>
        {
            return targets
                .GroupBy(static t => t.Host, SymbolEqualityComparer.Default)
                .Select(static g => new ObservableClassTarget((INamedTypeSymbol)g.Key!, g.Select(static e => new ObservableFieldSettings(e.Field, e.EqualityComparer, e.Strategy)).ToList()));
        });

        context.RegisterSourceOutput(classes, (ctx, target) =>
        {
            var @namespace = target.Host.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

            SyntaxTriviaList syntaxTriviaList = TriviaList(
                Comment("// <auto-generated/>"),
                Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))
            );

            var typeDeclarationSyntax = ClassDeclaration(target.Host.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                .AddModifiers(Token(TriviaList(Comment("/// <inheritdoc/>")), SyntaxKind.PartialKeyword, TriviaList()))
                .AddMembers(target.Fields.Select<ObservableFieldSettings, MemberDeclarationSyntax>(field =>
                {
                    ExpressionSyntax fieldExpression = field.Name switch
                    {
                        "value" => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("value")),
                        string name => IdentifierName(name)
                    };
                    var propertyName = field.Name.Capitalize();
                    TypeSyntax propertyType = IdentifierName(field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier)));

                    ExpressionSyntax neq = field.EqualityComparer switch
                    {
                        null => BinaryExpression(
                            SyntaxKind.NotEqualsExpression,
                            fieldExpression,
                            IdentifierName("value")
                        ),
                        var equalityComparer => PrefixUnaryExpression(
                            kind: SyntaxKind.LogicalNotExpression,
                            operatorToken: Token(SyntaxKind.ExclamationToken),
                            operand: InvocationExpression(
                                expression: MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, field.EqualityComparerInstanceSyntax, IdentifierName("Equals")),
                                argumentList: ArgumentList(SeparatedList(new ArgumentSyntax[]
                                {
                                    Argument(fieldExpression),
                                    Argument(IdentifierName("value"))
                                }))
                            )
                        )
                    };

                    ExpressionSyntax condition = field.Strategy switch
                    {
                        FieldChangeTriggerStrategy.Default => neq,
                        FieldChangeTriggerStrategy.Always => LiteralExpression(SyntaxKind.TrueLiteralExpression),
                        // if ((0 == _isSet && 0 == Interlocked.CompareExchange(ref _isSet, 1, 0)) || neq([oldvalue], value))
                        FieldChangeTriggerStrategy.InitialSet => BinaryExpression(
                            kind: SyntaxKind.LogicalOrExpression,
                            left: ParenthesizedExpression(BinaryExpression(
                                kind: SyntaxKind.LogicalAndExpression,
                                left: BinaryExpression(
                                    kind: SyntaxKind.EqualsExpression,
                                    left: LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)),
                                    right: IdentifierName($"_{field.Name}IsSet")
                                ),
                                right: BinaryExpression(
                                    kind: SyntaxKind.EqualsExpression,
                                    left: LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)),
                                    right: InvocationExpression(
                                        expression: MemberAccessExpression(
                                            kind: SyntaxKind.SimpleMemberAccessExpression,
                                            expression: ParseTypeName("System.Threading.Interlocked"),
                                            name: IdentifierName("CompareExchange")
                                        ),
                                        argumentList: ArgumentList(SeparatedList(new ArgumentSyntax[]
                                        {
                                            Argument(nameColon: default, refKindKeyword: Token(SyntaxKind.RefKeyword), expression: IdentifierName($"_{field.Name}IsSet")),
                                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1))),
                                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)))
                                        }))
                                    )
                                )
                            )),
                            right: neq
                        ),
                        _ => throw new InvalidOperationException("Invalid field change trigger strategy.")
                    };

                    var setterBody = IfStatement(
                        condition,
                        Block(new StatementSyntax[]
                        {
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    fieldExpression,
                                    IdentifierName("value"))
                            ),
                            ExpressionStatement(
                                InvocationExpression(IdentifierName("OnPropertyChanged"))
                                    .AddArgumentListArguments(
                                        Argument(
                                            InvocationExpression(
                                                IdentifierName(
                                                    Identifier(
                                                        TriviaList(),
                                                        SyntaxKind.NameOfKeyword,
                                                        "nameof",
                                                        "nameof",
                                                        TriviaList()
                                                    )
                                                )
                                            )
                                            .AddArgumentListArguments(
                                                Argument(IdentifierName(propertyName))
                                            )
                                        )
                                    )
                            )
                        })
                    );

                    return PropertyDeclaration(propertyType, Identifier(propertyName))
                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithExpressionBody(ArrowExpressionClause(IdentifierName(field.Name)))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithBody(Block(setterBody))
                        );
                })
                .Concat(target.Fields
                    .Where(field => field.Strategy == FieldChangeTriggerStrategy.InitialSet)
                    .Select<ObservableFieldSettings, MemberDeclarationSyntax>(field =>
                    {
                        return FieldDeclaration(
                            attributeLists: default,
                            modifiers: TokenList(Token(SyntaxKind.PrivateKeyword)),
                            declaration: VariableDeclaration(
                                type: ParseTypeName("int"),
                                variables: SeparatedList(new VariableDeclaratorSyntax[]
                                {
                                    VariableDeclarator($"_{field.Name}IsSet")
                                })
                            )
                        );
                    })
                )
                .ToArray());

            CompilationUnitSyntax unitSyntax = CompilationUnit()
                .AddMembers(
                    NamespaceDeclaration(IdentifierName(@namespace))
                        .WithLeadingTrivia(syntaxTriviaList)
                        .AddMembers(typeDeclarationSyntax)
                )
                .NormalizeWhitespace();
            ctx.AddSource($"{target.Host.Name}.g.cs", unitSyntax.GetText(Utf8));
        });
    }
}