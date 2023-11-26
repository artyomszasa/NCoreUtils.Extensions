using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using NCoreUtils.Internal;

namespace NCoreUtils;

public static class ExpressionExtensions
{
    private delegate bool TryExtractDelegate(Expression expression, out object? value);

    private static TryExtractDelegate TryExtractConstantDelegate { get; }
        = (Expression expression, out object? value) => expression.TryExtractConstant(out value);

    private static TryExtractDelegate TryExtractInstanceDelegate { get; }
        = (Expression expression, out object? value) => expression.TryExtractInstance(out value);

    private static FixSizePool<ParameterSubstitution> SubstitutionPool { get; } = new(32);

    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Constructor is supplied through expression thus is a preserved method.")]
    private static object InvokeParameterlessConstructor(NewExpression newExpression)
    {
        if (newExpression.Constructor is null)
        {
            // this must be a ValueType without ctor
            // see: https://github.com/dotnet/runtime/issues/22296
            return Activator.CreateInstance(newExpression.Type)!;
        }
        return newExpression.Constructor.Invoke(
#if NETFRAMEWORK
            new object[0]
#else
            []
#endif
        );
    }

    private static Maybe<PropertyInfo> MaybeExtractBodyProperty(this Expression body)
    {
        if (body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo.Just();
        }
        if (body.NodeType == ExpressionType.Convert)
        {
            return ((UnaryExpression)body).Operand.MaybeExtractBodyProperty();
        }
        return default;
    }

    /// <summary>
    /// Replaces all <paramref name="parameter" /> occurrences within the source expression with the specified
    /// expression.
    /// </summary>
    /// <typeparam name="T">Static type of the source expression.</typeparam>
    /// <param name="expression">Source expression.</param>
    /// <param name="parameter">Parameter expression to replace.</param>
    /// <param name="replacement">Replacement expression.</param>
    /// <param name="keepExtensions">When <c>true</c> extension nodes will not be reduced.</param>
    /// <returns>Result expression.</returns>
    public static T SubstituteParameter<T>(this T expression, ParameterExpression parameter, Expression replacement, bool keepExtensions)
        where T : Expression
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }
        if (replacement == null)
        {
            throw new ArgumentNullException(nameof(replacement));
        }
        if (SubstitutionPool.TryRent(out var visitor))
        {
            visitor.Update(parameter, replacement, keepExtensions);
        }
        else
        {
            visitor = new(parameter, replacement, keepExtensions);
        }
        try
        {
            return (T)visitor.Visit(expression);
        }
        finally
        {
            SubstitutionPool.Return(visitor.Clear());
        }
    }

    /// <summary>
    /// Replaces all <paramref name="parameter" /> occurrences within the source expression with the specified
    /// expression.
    /// </summary>
    /// <typeparam name="T">Static type of the source expression.</typeparam>
    /// <param name="expression">Source expression.</param>
    /// <param name="parameter">Parameter expression to replace.</param>
    /// <param name="replacement">Replacement expression.</param>
    /// <returns>Result expression.</returns>
    public static T SubstituteParameter<T>(this T expression, ParameterExpression parameter, Expression replacement)
        where T : Expression
        => expression.SubstituteParameter(parameter, replacement, false);

    public static T InlineSplices<T>(this T expression, bool keepExtensions)
        where T : Expression
        => (T)(keepExtensions ? SpliceInliner.NoReduceInstance : SpliceInliner.ReduceInstance).Visit(expression);

    public static T InlineSplices<T>(this T expression)
        where T : Expression
        => InlineSplices(expression, false);

    static Maybe<object?> MaybeExtractConstantImpl(Expression source)
    {
        if (source is ConstantExpression constantExpression)
        {
            return constantExpression.Value.Just()!;
        }
        if (source is MemberExpression memberExpression)
        {
            switch (memberExpression.Member)
            {
                case FieldInfo field:
                    if (field.IsStatic)
                    {
                        return field.GetValue(null).Just()!;
                    }
                    // NOTE: expression MUST be not-null for instance fields
                    return MaybeExtractConstantImpl(memberExpression.Expression!).Map(field.GetValue)!;
                case PropertyInfo property when property.CanRead && null != property.GetMethod:
                    if (property.GetMethod.IsStatic)
                    {
                        return property.GetValue(null, null).Just()!;
                    }
                    // NOTE: expression MUST be not-null for instance properties
                    return MaybeExtractConstantImpl(memberExpression.Expression!).Map(instance => property.GetValue(instance, null))!;
            }
        }
        return default;
    }

    /// <summary>
    /// Exracts constant expression value as nullable.
    /// </summary>
    /// <param name="source">Source expression.</param>
    /// <returns>
    /// Either constant value of the epxression or empty value.
    /// </returns>
    public static Maybe<object?> MaybeExtractConstant(this Expression source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        try
        {
            return MaybeExtractConstantImpl(source);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Attempts to extract constant value from the expression.
    /// </summary>
    /// <param name="source">Source expression.</param>
    /// <param name="value">Variable to store the extracted constant value.</param>
    /// <returns>
    /// <c>true</c> if constant value has been successfully extracted from the expression and stored into
    /// <paramref name="value" />, <c>false</c> otherwise.
    /// </returns>
    public static bool TryExtractConstant(this Expression source, out object? value)
        => source.MaybeExtractConstant().TryGetValue(out value);

    /// <summary>
    /// Extracts property info from the expression.
    /// </summary>
    /// <param name="expression">Source expression.</param>
    /// <returns>Either property info or an empty value.</returns>
    public static Maybe<PropertyInfo> MaybeExtractProperty(this LambdaExpression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        return expression.Body.MaybeExtractBodyProperty();
    }

    /// <summary>
    /// Extracts property info from the expression.
    /// </summary>
    /// <param name="expression">Source expression.</param>
    /// <param name="propertyInfo">On success contains extracted property.</param>
    /// <returns><c>true</c> if property info could be extracted from expression, <c>false</c> otherwise.</returns>
    public static bool TryExtractProperty(this LambdaExpression expression, [MaybeNullWhen(false)] out PropertyInfo propertyInfo)
        => MaybeExtractProperty(expression).TryGetValue(out propertyInfo);

    /// <summary>
    /// Extracts property info from the expression.
    /// </summary>
    /// <param name="expression">Source expression.</param>
    /// <returns>Extracted property info.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if property info cannot be extracted from the specified expression.
    /// </exception>
    public static PropertyInfo ExtractProperty(this LambdaExpression expression)
    {
        if (TryExtractProperty(expression, out var propertyInfo))
        {
            return propertyInfo;
        }
        throw new InvalidOperationException($"Expected property access expression, got: {expression}.");
    }

    /// <summary>
    /// Extracts properties from expression.
    /// </summary>
    /// <param name="source">Source expression.</param>
    /// <param name="throw">Whether to throw exception if the expression in invalid or unsupported.</param>
    /// <returns>Collection of properties found in the expression.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the expression invalid or unsupported and <paramref name="throw" /> is <c>true</c>
    /// </exception>
    public static IEnumerable<PropertyInfo> ExtractProperties(this Expression source, bool @throw = true)
    {
        switch (source)
        {
            case null: throw new ArgumentNullException(nameof(source));
            case LambdaExpression lambda:
                foreach (var prop in lambda.Body.ExtractProperties(@throw))
                {
                    yield return prop;
                }
                break;
            case MemberExpression memberExpression when memberExpression.Member is PropertyInfo singleProp:
                yield return singleProp;
                break;
            case UnaryExpression unaryExpression when unaryExpression.Operand is not null:
                foreach (var prop in unaryExpression.Operand.ExtractProperties(@throw))
                {
                    yield return prop;
                }
                break;
            case NewExpression newExpression:
                if (null != newExpression.Members && newExpression.Members.Count == newExpression.Arguments.Count)
                {
                    foreach (var argumentExpression in newExpression.Arguments)
                    {
                        foreach (var prop in argumentExpression.ExtractProperties(@throw))
                        {
                            yield return prop;
                        }
                    }
                }
                else if (@throw)
                {
                    throw new InvalidOperationException("Invalid new expression.");
                }
                break;
            default:
                if (@throw)
                {
                    throw new InvalidOperationException($"Unable to extract properties from {source}.");
                }
                break;
        }
    }

    public static Maybe<object> MaybeExtractNewInstance(this Expression source, bool allowNestedNew = true)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (source is NewExpression newExpression)
        {
            if (newExpression.Arguments.Count == 0)
            {
                return InvokeParameterlessConstructor(newExpression).Just();
            }
            // TODO: avoid allocation
            var arguments = new object?[newExpression.Arguments.Count];
            var extract = allowNestedNew ? TryExtractInstanceDelegate : TryExtractConstantDelegate;
            for (var i = 0; i < newExpression.Arguments.Count; ++i)
            {
                if (extract(newExpression.Arguments[i], out var arg))
                {
                    arguments[i] = arg;
                }
                else
                {
                    return default;
                }
            }
            // NOTE: if there are arguments there must be a non-null ctor property.
            return newExpression.Constructor!.Invoke(arguments).Just();
        }
        return default;
    }

    public static bool TryExtractNewInstance(this Expression source, bool allowNestedNew, [MaybeNullWhen(false)] out object instance)
        => source.MaybeExtractNewInstance(allowNestedNew).TryGetValue(out instance);

    public static Maybe<object?> MaybeExtractInstance(this Expression source)
    {
        var maybeConstant = source.MaybeExtractConstant();
        if (maybeConstant.HasValue)
        {
            return maybeConstant;
        }
        return source.MaybeExtractNewInstance(true)!;
    }

    public static bool TryExtractInstance(this Expression source, out object? instance)
        => source.MaybeExtractInstance().TryGetValue(out instance);

    /// <summary>
    /// Extracts method call data as nullable tuple.
    /// </summary>
    /// <param name="source">Source expression</param>
    /// <returns>
    /// Either method call data as tuple or empty value.
    /// </returns>
    public static Maybe<(MethodInfo method, Expression? instance, ReadOnlyCollection<Expression> arguments)> MaybeExtractCall(this Expression source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (source is MethodCallExpression callExpression)
        {
            return (callExpression.Method, callExpression.Object, callExpression.Arguments).Just()!;
        }
        return default;
    }

    public static bool TryExtractCall(
        this Expression source,
        [MaybeNullWhen(false)] out MethodInfo method,
        out Expression? instance,
        [MaybeNullWhen(false)] out ReadOnlyCollection<Expression> arguments)
    {
        if (source.MaybeExtractCall().TryGetValue(out var data))
        {
            (method, instance, arguments) = data;
            return true;
        }
        (method, instance, arguments) = (default, default, default);
        return false;
    }

    public static Maybe<LambdaExpression> MaybeExtractLambda(this Expression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.Quote:
                var quoteExpression = (UnaryExpression)expression;
                return quoteExpression.Operand.MaybeExtractLambda();
            case ExpressionType.Lambda:
                return Maybe.Just((LambdaExpression)expression);
            default:
                return Maybe.Nothing;
        }
    }

    public static bool TryExtractLambda(this Expression expression, [MaybeNullWhen(false)] out LambdaExpression lambdaExpression)
        => expression.MaybeExtractLambda().TryGetValue(out lambdaExpression);
}