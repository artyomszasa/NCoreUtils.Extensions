using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils;

public interface IObjectPool<T>
{
    bool TryRent([MaybeNullWhen(false)] out T item);

    void Return(T item);
}