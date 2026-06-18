using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NCoreUtils.Json;

internal static class SyntaxFactoryHelper
{
    public static class Keywords
    {
        public static SyntaxToken Override { get; } = Token(SyntaxKind.OverrideKeyword);

        public static SyntaxToken Partial { get; } = Token(SyntaxKind.PartialKeyword);

        public static SyntaxToken Public { get; } = Token(SyntaxKind.PublicKeyword);

        public static SyntaxToken Ref { get; } = Token(SyntaxKind.RefKeyword);

        public static SyntaxToken Static { get; } = Token(SyntaxKind.StaticKeyword);
    }

    public static class Tokens
    {
        public static SyntaxToken CloseBrace { get; } = Token(SyntaxKind.CloseBraceToken);

        public static SyntaxToken CloseParen { get; } = Token(SyntaxKind.CloseParenToken);

        public static SyntaxToken InterpolatedStringEnd { get; } = Token(SyntaxKind.InterpolatedStringEndToken);

        public static SyntaxToken InterpolatedStringStart { get; } = Token(SyntaxKind.InterpolatedStringStartToken);

        public static SyntaxToken OpenBrace { get; } = Token(SyntaxKind.OpenBraceToken);

        public static SyntaxToken OpenParen { get; } = Token(SyntaxKind.OpenParenToken);

        public static SyntaxToken Semicolon { get; } = Token(SyntaxKind.SemicolonToken);
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Name must match the string.")]
    public static class Identifiers
    {
        public static SyntaxToken options { get; } = Identifier(nameof(options));

        public static SyntaxToken reader { get; } = Identifier(nameof(reader));

        public static SyntaxToken value { get; } = Identifier(nameof(value));

        public static SyntaxToken writer { get; } = Identifier(nameof(writer));

        public static SyntaxToken GetString { get; } = Identifier(nameof(GetString));

        public static SyntaxToken Read { get; } = Identifier(nameof(Read));

        public static SyntaxToken TokenType { get; } = Identifier(nameof(TokenType));

        public static SyntaxToken ValueTextEquals { get; } = Identifier(nameof(ValueTextEquals));

        public static SyntaxToken Write { get; } = Identifier(nameof(Write));

        public static SyntaxToken WriteStringValue { get; } = Identifier(nameof(WriteStringValue));
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Name must match the string.")]
    public static class IdentifierNames
    {
        public static IdentifierNameSyntax reader { get; } = IdentifierName(Identifiers.reader);

        public static IdentifierNameSyntax value { get; } = IdentifierName(Identifiers.value);

        public static IdentifierNameSyntax writer { get; } = IdentifierName(Identifiers.writer);

        public static IdentifierNameSyntax GetString { get; } = IdentifierName(Identifiers.GetString);

        public static IdentifierNameSyntax Read { get; } = IdentifierName(Identifiers.Read);

        public static IdentifierNameSyntax TokenType { get; } = IdentifierName(Identifiers.TokenType);

        public static IdentifierNameSyntax ValueTextEquals { get; } = IdentifierName(Identifiers.ValueTextEquals);

        public static IdentifierNameSyntax Write { get; } = IdentifierName(Identifiers.Write);

        public static IdentifierNameSyntax WriteStringValue { get; } = IdentifierName(Identifiers.WriteStringValue);
    }

    public static class Types
    {
        public static TypeSyntax JsonException { get; } = ParseTypeName("System.Text.Json.JsonException");

        public static TypeSyntax JsonSerializerOptions { get; } = ParseTypeName("System.Text.Json.JsonSerializerOptions");

        public static TypeSyntax Type { get; } = ParseTypeName("System.Type");

        public static TypeSyntax Utf8JsonReader { get; } = ParseTypeName("System.Text.Json.Utf8JsonReader");

        public static TypeSyntax Utf8JsonWriter { get; } = ParseTypeName("System.Text.Json.Utf8JsonWriter");

        public static PredefinedTypeSyntax Void { get; } = PredefinedType(Token(SyntaxKind.VoidKeyword));
    }

    private static ArgumentListSyntax Args()
        => ArgumentList(Tokens.OpenParen, default, Tokens.CloseParen);

    private static ArgumentListSyntax Args(ExpressionSyntax singleArg)
        => ArgumentList(Tokens.OpenParen, SingletonSeparatedList(Argument(singleArg)), Tokens.CloseParen);

    private static ArgumentListSyntax Args(params ExpressionSyntax[] args)
        => ArgumentList(Tokens.OpenParen, SeparatedList(args.Select(Argument)), Tokens.CloseParen);

    public static InterpolatedStringTextSyntax InterpolatedStringText(string raw)
        => SyntaxFactory.InterpolatedStringText(Token(
            leading: default,
            kind: SyntaxKind.InterpolatedStringTextToken,
            text: raw,
            valueText: raw,
            trailing: default
        ));

    public static LiteralExpressionSyntax StringLiteralExpression(string value)
        => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));

    public static LiteralExpressionSyntax Utf8StringLiteralExpression(string value)
        => LiteralExpression(SyntaxKind.Utf8StringLiteralExpression, Literal(default, $"\"{value}\"u8", value, default));

    public static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.EqualsExpression, left, right);

    public static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, ExpressionSyntax right)
        => BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);

    public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);

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

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type)
        => ObjectCreationExpression(type, Args(), null);

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type, ExpressionSyntax singleArg)
        => ObjectCreationExpression(type, Args(singleArg), null);

    public static ObjectCreationExpressionSyntax NewExpression(TypeSyntax type, params ExpressionSyntax[] args)
        => ObjectCreationExpression(type, Args(args), null);

    public static BlockSyntax BracedBlock(StatementSyntax statement)
        => Block(
            openBraceToken: Tokens.OpenBrace,
            statements: SingletonList(statement),
            closeBraceToken: Tokens.CloseBrace
        );

    public static BlockSyntax BracedBlock(params StatementSyntax[] statements)
        => Block(
            openBraceToken: Tokens.OpenBrace,
            statements: List(statements),
            closeBraceToken: Tokens.CloseBrace
        );
}