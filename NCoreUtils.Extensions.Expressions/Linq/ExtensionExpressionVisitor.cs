using System.Linq.Expressions;

namespace NCoreUtils.Linq;

public abstract class ExtensionExpressionVisitor(bool keepExtensions = false) : ExpressionVisitor
{
    protected bool KeepExtensions { get; private set; } = keepExtensions;

    internal void Update(bool keepExtensions)
    {
        KeepExtensions = keepExtensions;
    }

    protected override Expression VisitExtension(Expression node)
    {
        if (KeepExtensions && node is IExtensionExpression enode)
        {
            return enode.AcceptNoReduce(this);
        }
        return base.VisitExtension(node);
    }
}