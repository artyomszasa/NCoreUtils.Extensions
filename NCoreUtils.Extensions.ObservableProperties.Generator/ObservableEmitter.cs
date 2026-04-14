using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NCoreUtils.ObservableProperties.SyntaxFactoryHelper;

namespace NCoreUtils.ObservableProperties;

internal static class ObservableEmitter
{
    private static PropertyDeclarationSyntax EmitProperty(PropertyDescriptor property)
    {
        ExpressionSyntax neq = property.EqualityComparer switch
        {
            null => NotEqualsExpression(property.FieldExpression, IdentifierNames.value),
            var equalityComparer => NotExpression(
                SimpleInvocationExpression(
                    equalityComparer.AccessExpression,
                    property.FieldExpression,
                    IdentifierNames.value
                )
            )
        };
        ExpressionSyntax condition = property.Strategy switch
        {
            FieldChangeTriggerStrategy.Default => neq,
            FieldChangeTriggerStrategy.Always => LiteralExpression(SyntaxKind.TrueLiteralExpression),
            // if ((0 == _isSet && 0 == Interlocked.CompareExchange(ref _isSet, 1, 0)) || neq([oldvalue], value))
            FieldChangeTriggerStrategy.InitialSet => LogicalOrExpression(
                ParenthesizedExpression(LogicalAndExpression(
                    EqualsExpression(ZeroNumericLiteralExpression, property.SetFieldIdentifier),
                    EqualsExpression(
                        ZeroNumericLiteralExpression,
                        InvocationExpression(
                            Methods.Interlocked.CompareExchange,
                            ArgumentList(SeparatedList(
                            [
                                RefArg(property.SetFieldIdentifier),
                                Argument(NumericLiteralExpression(1)),
                                Argument(ZeroNumericLiteralExpression),
                            ])
                        )
                    )
                ))),
                neq
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
                        property.FieldExpression,
                        IdentifierNames.value
                    )
                ),
                ExpressionStatement(
                    SimpleInvocationExpression(
                        IdentifierNames.OnPropertyChanged,
                        NameOfExpression(property.PropertyIdentifier)
                    )
                )
            })
        );

        return PropertyDeclaration(
            attributeLists: SingletonList(AttributeList(SingletonSeparatedList(Attribute(
                Types.GeneratedCodeAttribute,
                AttributeArgumentList(SeparatedList([ AttributeArgument(PackageName), AttributeArgument(SelfVersion) ]))
            )))),
            modifiers: TokenList(Keywords.Public),
            type: property.ValueType.Syntax,
            explicitInterfaceSpecifier: default,
            identifier: Identifier(property.PropertyName),
            accessorList: AccessorList(List(
            [
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(property.FieldIdentifier))
                    .WithSemicolonToken(Tokens.Semicolon),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithBody(Block(setterBody))
            ]))
        ).WithLeadingTrivia(TriviaList(
            Trivias.Tab4,
            Trivia(property.DocumentationComment),
            Trivias.EndOfLine,
            Trivias.Tab4
        ));
    }

    private static LiteralExpressionSyntax PackageName { get; }
        = StringLiteralExpression("NCoreUtils.Extensions.ObservableProperties");

    private static LiteralExpressionSyntax SelfVersion { get; }
        = StringLiteralExpression(typeof(ObservableEmitter).Assembly.GetName()?.Version.ToString() ?? string.Empty);

    private static FieldDeclarationSyntax EmitHasSetField(PropertyDescriptor property)
    {
        return FieldDeclaration(
            attributeLists: default,
            modifiers: TokenList(Keywords.Private),
            declaration: VariableDeclaration(
                type: Types.Int32,
                variables: SingletonSeparatedList(VariableDeclarator(property.SetFieldName))
            )
        );
    }

    private static ClassDeclarationSyntax EmitClass(TypeDescriptor type, ImmutableArray<PropertyDescriptor> properties)
    {
        var members = new List<MemberDeclarationSyntax>(properties.Length * 2);
        foreach (var property in properties)
        {
            if (property.Strategy == FieldChangeTriggerStrategy.InitialSet)
            {
                members.Add(EmitHasSetField(property));
            }
        }
        foreach (var property in properties)
        {
            members.Add(EmitProperty(property));
        }
        return ClassDeclaration(
            attributeLists: default,
            modifiers: TokenList(Keywords.Partial),
            identifier: Identifier(type.Name),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: List(members)
        );
    }

    private static SyntaxTriviaList SyntaxTriviaList { get; } = TriviaList(
        Comment("// <auto-generated/>"),
        Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))
    );

    public static CompilationUnitSyntax EmitUnit(TypeDescriptor type, ImmutableArray<PropertyDescriptor> properties)
    {
        CompilationUnitSyntax unitSyntax = CompilationUnit()
            .AddMembers(
                NamespaceDeclaration(IdentifierName(type.Namespace))
                    .WithLeadingTrivia(SyntaxTriviaList)
                    .AddMembers(EmitClass(type, properties))
            )
            .NormalizeWhitespace();
        return unitSyntax;
    }

    public static CompilationUnitSyntax EmitUnit(TargetDescriptor target)
        => EmitUnit(target.Type, target.Properties);
}