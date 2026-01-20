using System.Linq.Expressions;
using NCoreUtils.Linq;

namespace NCoreUtils.Internal;

internal sealed class ParameterSubstitution(ParameterExpression parameter, Expression replacement, bool keepExtensions)
    : ExtensionExpressionVisitor(keepExtensions)
{
    private ParameterExpression Parameter { get; set; } = parameter;

    private Expression Replacement { get; set; } = replacement;

    internal ParameterSubstitution Clear()
    {
        Parameter = default!;
        Replacement = default!;
        return this;
    }

    internal void Update(ParameterExpression parameter, Expression replacement, bool keepExtensions)
    {
        Update(keepExtensions);
        Parameter = parameter;
        Replacement = replacement;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node.Equals(Parameter))
        {
            return Visit(Replacement);
        }
        return base.VisitParameter(node);
    }
}