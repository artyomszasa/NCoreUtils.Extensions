using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Google;

public readonly partial struct ScopeCollection : IEquatable<ScopeCollection>
{
    private readonly struct ArgsTuple(string? firstScope, IReadOnlyCollection<string>? furtherScopes)
    {
        public string? FirstScope { get; } = firstScope;

        public IReadOnlyCollection<string>? FurtherScopes { get; } = furtherScopes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ScopeCollection(string scope) => new(scope);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ScopeCollection(string[] scopes) => new(scopes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ScopeCollection(List<string> scopes) => new(scopes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ScopeCollection(HashSet<string> scopes) => new(scopes);

    public string? FirstScope { get; }

    public IReadOnlyCollection<string>? FurtherScopes { get; }

    [MemberNotNullWhen(false, nameof(FirstScope))]
    public bool IsEmpty => FirstScope is null;

    [MemberNotNullWhen(true, nameof(FirstScope))]
    [MemberNotNullWhen(true, nameof(FurtherScopes))]
    public bool HasMultipleScopes => FurtherScopes is not null;

    private ScopeCollection(string? firstScope, IReadOnlyCollection<string>? furtherScopes)
    {
        FirstScope = firstScope;
        FurtherScopes = furtherScopes;
    }

    private ScopeCollection(ArgsTuple tup)
        : this(tup.FirstScope, tup.FurtherScopes)
    { }

    public ScopeCollection(string scope)
        : this(scope ?? throw new ArgumentNullException(nameof(scope)), null)
    { }

    public ScopeCollection(IEnumerable<string> scopes)
        : this(CheckScopes(scopes))
    { }

    public ScopeCollection(string[] scopes)
        : this(CheckScopesArray(scopes))
    { }

    public ScopeCollection(List<string> scopes)
        : this(CheckScopesList(scopes))
    { }

    public ScopeCollection(HashSet<string> scopes)
        : this(CheckScopesHashSet(scopes))
    { }

    public string Join(string separator)
    {
        if (IsEmpty)
        {
            return string.Empty;
        }
        if (FurtherScopes is null)
        {
            return FirstScope;
        }
        var size = FirstScope.Length;
        var separatorSize = separator.Length;
        foreach (var item in FurtherScopes)
        {
            size += separatorSize + item.Length;
        }
        var buffer = ArrayPool<char>.Shared.Rent(size);
        try
        {
            var builder = new SpanBuilder(buffer);
            builder.Append(FirstScope);
            foreach (var item in FurtherScopes)
            {
                builder.Append(separator);
                builder.Append(item);
            }
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    public bool Equals(ScopeCollection other)
    {
        if (IsEmpty)
        {
            return other.IsEmpty;
        }
        if (other.IsEmpty)
        {
            return false;
        }
        if (FurtherScopes is null)
        {
            return FirstScope == other.FirstScope;
        }
        if (other.FurtherScopes is null)
        {
            return false;
        }
        return FirstScope == other.FirstScope && FurtherScopes.SequenceEqual(other.FurtherScopes);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ScopeCollection other && Equals(other);

    public override int GetHashCode()
    {
        if (IsEmpty)
        {
            return default;
        }
        var builder = new HashCode();
        if (FurtherScopes is not null)
        {
            builder.Add(1 + FurtherScopes.Count);
            builder.Add(StringComparer.InvariantCulture.GetHashCode(FirstScope));
            foreach (var item in FurtherScopes)
            {
                builder.Add(StringComparer.InvariantCulture.GetHashCode(item));
            }
        }
        else
        {
            builder.Add(1);
            builder.Add(StringComparer.InvariantCulture.GetHashCode(FirstScope));
        }
        return builder.ToHashCode();
    }

    public string[] ToArray()
    {
        if (IsEmpty)
        {
            return [];
        }
        if (FurtherScopes is not null)
        {
            return [ FirstScope, .. FurtherScopes ];
        }
        return [ FirstScope ];
    }
}