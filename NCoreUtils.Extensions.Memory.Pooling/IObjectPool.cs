using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils;

public interface IObjectPool<T>
    where T : class
{
    bool TryRent([MaybeNullWhen(false)] out T item);

    void Return(T item);
}