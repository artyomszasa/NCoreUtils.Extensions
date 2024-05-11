using System.Runtime.CompilerServices;

namespace NCoreUtils.Google;

public readonly partial struct ScopeCollection
{
    [MethodImpl(AggressiveInliningAndAggressiveOptimization)]
    private static ArgsTuple CheckScopesArray(string[] scopes)
    {
        if (scopes.Length == 0)
        {
            return default;
        }
        var firstScope = scopes[0];
        if (scopes.Length < 2)
        {
            return new(firstScope, default);
        }
        var furtherScopes = GetSubArrayFrom(scopes, 1);
        foreach (var scope in furtherScopes)
        {
            if (scope is null)
            {
                throw new ArgumentException("Scope collection must not contain null values.", nameof(scopes));
            }
        }
        return new(firstScope, furtherScopes);
    }

    [MethodImpl(AggressiveInliningAndAggressiveOptimization)]
    private static ArgsTuple CheckScopesList(List<string> scopes)
    {
        var enumerator = scopes.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var firstScope = enumerator.Current;
            var count = scopes.Count - 1;
            if (count > 0)
            {
                var buffer = new string[count] ?? throw new ArgumentException("Scope collection must not contain null values.", nameof(scopes));
                for (var i = 0; enumerator.MoveNext(); ++i)
                {
                    buffer[i] = enumerator.Current ?? throw new ArgumentException("Scope collection must not contain null values.", nameof(scopes));
                }
                return new(firstScope, buffer);
            }
            return new(firstScope, default);
        }
        return default;
    }

    [MethodImpl(AggressiveInliningAndAggressiveOptimization)]
    private static ArgsTuple CheckScopesHashSet(HashSet<string> scopes)
    {
        var enumerator = scopes.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var firstScope = enumerator.Current;
            var count = scopes.Count - 1;
            if (count > 0)
            {
                var buffer = new string[count] ?? throw new ArgumentException("Scope collection must not contain null values.", nameof(scopes));
                for (var i = 0; enumerator.MoveNext(); ++i)
                {
                    buffer[i] = enumerator.Current ?? throw new ArgumentException("Scope collection must not contain null values.", nameof(scopes));
                }
                return new(firstScope, buffer);
            }
            return new(firstScope, default);
        }
        return default;
    }

    [MethodImpl(AggressiveInliningAndAggressiveOptimization)]
    private static ArgsTuple CheckScopesGeneric(IEnumerable<string> scopes)
    {
        var enumerator = scopes.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var firstScope = enumerator.Current;
            if (enumerator.MoveNext())
            {
                var buffer = new List<string>(4);
                do
                {
                    buffer.Add(enumerator.Current);
                }
                while (enumerator.MoveNext());
                return new(firstScope, buffer);
            }
            return new(firstScope, default);
        }
        return default;
    }

    [MethodImpl(NoInliningAndAggressiveOptimization)]
    private static ArgsTuple CheckScopes(IEnumerable<string> scopes)
    {
        ThrowIfScopesParameterIsNull(scopes);
        return scopes switch
        {
            string[] scopeArray => CheckScopesArray(scopeArray),
            List<string> scopeList => CheckScopesList(scopeList),
            HashSet<string> scopeSet => CheckScopesHashSet(scopeSet),
            _ => CheckScopesGeneric(scopes),
        };
    }
}