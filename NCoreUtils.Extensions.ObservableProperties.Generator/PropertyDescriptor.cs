using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NCoreUtils.ObservableProperties.SyntaxFactoryHelper;

namespace NCoreUtils.ObservableProperties;

internal class PropertyDescriptor(
    TypeDescriptor ownerType,
    TypeDescriptor valueType,
    string fieldName,
    string propertyName,
    EqualityComparerDescriptor? equalityComparer,
    FieldChangeTriggerStrategy strategy)
    : IEquatable<PropertyDescriptor>
{
    private static ExpressionSyntax CreateFieldExpression(string name, IdentifierNameSyntax identifier)
        => StringComparer.Ordinal.Equals("value", name)
            ? SimpleMemberAccessExpression(ThisExpression(), identifier)
            : identifier;

    private static DocumentationCommentTriviaSyntax CreateDocumentationComment(IdentifierNameSyntax fieldIdentifier)
        => DocumentationCommentTrivia(
            SyntaxKind.SingleLineDocumentationCommentTrivia,
            List(new XmlNodeSyntax[]
            {
                XmlText(XmlTextLiteral(TriviaList(DocumentationCommentExterior("///")), " ", " ", TriviaList())),
                XmlEmptyElement(
                    XmlName("inheritdoc"),
                    List(new XmlAttributeSyntax[]
                    {
                        XmlCrefAttribute(
                            NameMemberCref(fieldIdentifier)
                        )
                    })
                ),
                XmlText(XmlTextNewLine(TriviaList(), "\r\n", "\r\n", TriviaList()))
            }),
            Token(SyntaxKind.EndOfDocumentationCommentToken)
        ).WithTrailingTrivia(Trivias.EndOfLine);

    public TypeDescriptor OwnerType { get; } = ownerType;

    public TypeDescriptor ValueType { get; } = valueType;

    public string FieldName { get; } = fieldName;

    public string PropertyName { get; } = propertyName;

    public EqualityComparerDescriptor? EqualityComparer { get; } = equalityComparer;

    public FieldChangeTriggerStrategy Strategy { get; } = strategy;

    public string SetFieldName => field ??= $"_{FieldName}HasBeenSet";

    public IdentifierNameSyntax FieldIdentifier => field ??= IdentifierName(FieldName);

    public IdentifierNameSyntax PropertyIdentifier => field ??= IdentifierName(PropertyName);

    public IdentifierNameSyntax SetFieldIdentifier => field ??= IdentifierName(SetFieldName);

    public ExpressionSyntax FieldExpression => field ??= CreateFieldExpression(FieldName, FieldIdentifier);

    public DocumentationCommentTriviaSyntax DocumentationComment => field ??= CreateDocumentationComment(FieldIdentifier);

    #region equality

    public static bool operator== (PropertyDescriptor? a, PropertyDescriptor? b)
        => a is null
            ? b is null
            : a.Equals(b);

    public static bool operator!= (PropertyDescriptor? a, PropertyDescriptor? b)
        => a is null
            ? b is not null
            : !a.Equals(b);

    public bool Equals([NotNullWhen(true)] PropertyDescriptor? other)
        => other is not null
            && OwnerType == other.OwnerType
            && ValueType == other.ValueType
            && StringComparer.Ordinal.Equals(FieldName, other.FieldName)
            && StringComparer.Ordinal.Equals(PropertyName, other.PropertyName)
            && EqualityComparer == other.EqualityComparer
            && Strategy == other.Strategy;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => Equals(obj as PropertyDescriptor);

    public override int GetHashCode()
        // FIXME: better hashing
        => StringComparer.Ordinal.GetHashCode(FieldName)
            ^ (EqualityComparer is null ? default : EqualityComparer.GetHashCode())
            ^ Strategy.GetHashCode();

    #endregion
}
