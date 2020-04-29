using System.Linq.Expressions;

namespace NCoreUtils.Linq
{
    public interface IExtensionExpression
    {
        Expression AcceptNoReduce(ExpressionVisitor visitor);
    }
}