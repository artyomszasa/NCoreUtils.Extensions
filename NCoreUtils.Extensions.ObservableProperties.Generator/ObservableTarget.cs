using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NCoreUtils.ObservableProperties;

public enum FieldChangeTriggerStrategy
{
    /// <summary>
    /// Change event is only triggered when current value is not equal to the new value according to the selected
    /// equality comparison.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Change event is triggered each time value is set (not depending in old/new value equality).
    /// </summary>
    Always = 1,
    /// <summary>
    /// Change event is triggered once value is set for the first time, then each time current value is not equal to the
    /// new value accoring to the selected equality comparison.
    /// </summary>
    InitialSet = 2
}

internal class ObservableFieldTarget(
    SemanticModel semanticModel,
    INamedTypeSymbol host,
    IFieldSymbol field,
    INamedTypeSymbol? equalityComparer,
    FieldChangeTriggerStrategy strategy)
{
    public SemanticModel SemanticModel { get; } = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));

    public INamedTypeSymbol Host { get; } = host ?? throw new ArgumentNullException(nameof(host));

    public IFieldSymbol Field { get; } = field ?? throw new ArgumentNullException(nameof(field));

    public INamedTypeSymbol? EqualityComparer { get; } = equalityComparer;

    public FieldChangeTriggerStrategy Strategy { get; } = strategy;
}

internal class ObservableFieldSettings(IFieldSymbol field, INamedTypeSymbol? equalityComparer, FieldChangeTriggerStrategy strategy)
{
    private ExpressionSyntax? _equalityComparerInstanceSyntax;

    public IFieldSymbol Field { get; } = field;

    public INamedTypeSymbol? EqualityComparer { get; } = equalityComparer;

    public FieldChangeTriggerStrategy Strategy { get; } = strategy;

    public string Name => Field.Name;

    public ITypeSymbol Type => Field.Type;

    public ExpressionSyntax EqualityComparerInstanceSyntax
    {
        get
        {
            if (EqualityComparer is null)
            {
                throw new InvalidOperationException("EqualityComparer is not defined.");
            }
            return _equalityComparerInstanceSyntax ??= EqualityComparer.GetMembers().Any(e => e.IsStatic && e is IPropertySymbol && e.Name == "Singleton")
                ? SyntaxFactory.MemberAccessExpression(
                    kind: SyntaxKind.SimpleMemberAccessExpression,
                    expression: SyntaxFactory.ParseTypeName(EqualityComparer.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                    name: SyntaxFactory.IdentifierName("Singleton")
                )
                : SyntaxFactory.ObjectCreationExpression(
                    type: SyntaxFactory.ParseTypeName(EqualityComparer.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.Token(SyntaxKind.OpenParenToken), SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>()), SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                    initializer: default
                );
        }
    }
}

internal class ObservableClassTarget(INamedTypeSymbol host, IReadOnlyList<ObservableFieldSettings> fields)
{
    public INamedTypeSymbol Host { get; } = host ?? throw new ArgumentNullException(nameof(host));

    public IReadOnlyList<ObservableFieldSettings> Fields { get; } = fields ?? throw new ArgumentNullException(nameof(fields));
}