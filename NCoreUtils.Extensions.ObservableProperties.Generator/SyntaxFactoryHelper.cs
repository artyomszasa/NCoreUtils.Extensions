using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NCoreUtils.ObservableProperties;

internal static class SyntaxFactoryHelper
{
    public static class Identifiers
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Property name must match the contained string.")]
        public static SyntaxToken value { get; } = Identifier(nameof(value));

        public static SyntaxToken OnPropertyChanged { get; } = Identifier(nameof(OnPropertyChanged));
    }

    public static class IdentifierNames
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Property name must match the contained string.")]
        public static IdentifierNameSyntax value { get; } = IdentifierName(Identifiers.value);

        public static IdentifierNameSyntax OnPropertyChanged { get; } = IdentifierName(Identifiers.OnPropertyChanged);
    }

    public static class Keywords
    {
        public static SyntaxToken Partial { get; } = Token(SyntaxKind.PartialKeyword);

        public static SyntaxToken Private { get; } = Token(SyntaxKind.PrivateKeyword);

        public static SyntaxToken Public { get; } = Token(SyntaxKind.PublicKeyword);

        public static SyntaxToken Ref { get; } = Token(SyntaxKind.RefKeyword);
    }

    public static class Tokens
    {
        public static SyntaxToken CloseParen { get; } = Token(SyntaxKind.CloseParenToken);

        public static SyntaxToken OpenParen { get; } = Token(SyntaxKind.OpenParenToken);

        public static SyntaxToken Semicolon { get; } = Token(SyntaxKind.SemicolonToken);
    }

    public static class Types
    {
        public static PredefinedTypeSyntax Int32 { get; } = PredefinedType(Token(SyntaxKind.IntKeyword));

        public static TypeSyntax Interlocked => field ??= ParseTypeName("System.Threading.Interlocked");

        public static NameSyntax GeneratedCodeAttribute => field ??= ParseName("System.CodeDom.Compiler.GeneratedCodeAttribute");
    }

    public static class Methods
    {
        public static class Interlocked
        {
            public static ExpressionSyntax CompareExchange => field ??= SimpleMemberAccessExpression(
                Types.Interlocked,
                IdentifierName("CompareExchange")
            );
        }
    }

    public static class Trivias
    {
        public static SyntaxTrivia Tab4 { get; } = Whitespace("    ");

        public static SyntaxTrivia EndOfLine { get; } = EndOfLine("\r\n");
    }

    public static LiteralExpressionSyntax NumericLiteralExpression(int value)
        => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    public static LiteralExpressionSyntax ZeroNumericLiteralExpression
        => field ??= NumericLiteralExpression(0);

    public static LiteralExpressionSyntax StringLiteralExpression(string value)
        => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));

    public static PrefixUnaryExpressionSyntax NotExpression(ExpressionSyntax operand)
        => PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operand);

    public static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.EqualsExpression, left, right);

    public static BinaryExpressionSyntax LogicalAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);

    public static BinaryExpressionSyntax LogicalOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);

    public static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);

    public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);

    public static ArgumentSyntax RefArg(ExpressionSyntax expression)
        => Argument(default, Keywords.Ref, expression);

    private static ArgumentListSyntax Args()
        => ArgumentList(Tokens.OpenParen, default, Tokens.CloseParen);

    private static ArgumentListSyntax Args(ExpressionSyntax singleArg)
        => ArgumentList(Tokens.OpenParen, SingletonSeparatedList(Argument(singleArg)), Tokens.CloseParen);

    private static ArgumentListSyntax Args(params ExpressionSyntax[] args)
        => ArgumentList(Tokens.OpenParen, SeparatedList(args.Select(Argument)), Tokens.CloseParen);

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type)
        => ObjectCreationExpression(type, Args(), null);

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type, ExpressionSyntax singleArg)
        => ObjectCreationExpression(type, Args(singleArg), null);

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type, params ExpressionSyntax[] args)
        => ObjectCreationExpression(type, Args(args), null);

    public static InvocationExpressionSyntax SimpleInvocationExpression(ExpressionSyntax expression)
        => InvocationExpression(expression, Args());

    public static InvocationExpressionSyntax SimpleInvocationExpression(
        ExpressionSyntax expression,
        ExpressionSyntax singleArg)
        => InvocationExpression(expression, Args(singleArg));

    public static InvocationExpressionSyntax SimpleInvocationExpression(
        ExpressionSyntax expression,
        params ExpressionSyntax[] args)
        => InvocationExpression(expression, Args(args));

    private static SyntaxToken NameOfIdentifier { get; } = Identifier(
        TriviaList(),
        SyntaxKind.NameOfKeyword,
        "nameof",
        "nameof",
        TriviaList()
    );

    public static InvocationExpressionSyntax NameOfExpression(ExpressionSyntax operand)
        => SimpleInvocationExpression(IdentifierName(NameOfIdentifier), operand);
}