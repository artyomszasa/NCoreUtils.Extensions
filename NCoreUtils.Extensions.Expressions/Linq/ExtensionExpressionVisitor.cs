using System.Linq.Expressions;

namespace NCoreUtils.Linq
{
    public abstract class ExtensionExpressionVisitor : ExpressionVisitor
    {
        protected bool KeepExtensions { get; }

        protected ExtensionExpressionVisitor(bool keepExtensions = false)
            => KeepExtensions = keepExtensions;

        protected override Expression VisitExtension(Expression node)
        {
            if (KeepExtensions && node is IExtensionExpression enode)
            {
                return enode.AcceptNoReduce(this);
            }
            return base.VisitExtension(node);
        }
    }
}