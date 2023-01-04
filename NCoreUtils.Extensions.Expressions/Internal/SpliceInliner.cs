using System;
using System.Linq.Expressions;
using NCoreUtils.Linq;

namespace NCoreUtils.Internal;

internal sealed class SpliceInliner : ExtensionExpressionVisitor
{
    public static SpliceInliner ReduceInstance { get; } = new SpliceInliner(false);

    public static SpliceInliner NoReduceInstance { get; } = new SpliceInliner(true);

    public SpliceInliner(bool keepExtensions) : base(keepExtensions) { }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Splice))
        {
            if (node.Method.Name == nameof(Splice.Value))
            {
                if (node.Arguments[0].TryExtractConstant(out var cexpr0) && cexpr0 is Expression cexpr)
                {
                    return Visit(cexpr);
                }
                else
                {
                    throw new InvalidOperationException("Splice value must be constant extractable expression.");
                }
            }
            if (node.Method.Name == nameof(Splice.Apply))
            {
                using var argumentEnumerator = node.Arguments.GetEnumerator();
                // first argument MUST be a lambda
                if (!argumentEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("No splice applicant argument found.");
                }
                if (!argumentEnumerator.Current.TryExtractConstant(out var boxedLambda) || boxedLambda is not LambdaExpression lambda)
                {
                    throw new InvalidOperationException("Splice applicant must be constant extractable lambda expression.");
                }
                // second Apply argument is a first lambda parameter
                if (!argumentEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("At least one argument is required when applying a slice.");
                }
                using var parameterEnumerator = lambda.Parameters.GetEnumerator();
                if (!parameterEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("Splice applicant parameter count must be he same as splice argument count.");
                }
                var inlined = lambda.SubstituteParameter(parameterEnumerator.Current, argumentEnumerator.Current);
                while (true)
                {
                    var hasNextArgument = argumentEnumerator.MoveNext();
                    var hasNextParameter = parameterEnumerator.MoveNext();
                    if (hasNextArgument)
                    {
                        if (hasNextParameter)
                        {
                            inlined = inlined.SubstituteParameter(parameterEnumerator.Current, argumentEnumerator.Current);
                        }
                        else
                        {
                            throw new InvalidOperationException("Splice applicant parameter count must be he same as splice argument count.");
                        }
                    }
                    else
                    {
                        if (hasNextParameter)
                        {
                            throw new InvalidOperationException("Splice applicant parameter count must be he same as splice argument count.");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return Visit(inlined);
            }
        }
        return base.VisitMethodCall(node);
    }
}