using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Memory;

#pragma warning disable CS0618

public class EmplaceableEmplacer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : IEmplacer<T>
    where T : IEmplaceable<T>
{
    public int Emplace(T value, Span<char> span)
        => value.Emplace(span);

    public bool TryEmplace(T value, Span<char> span, out int used)
        => value.TryEmplace(span, out used);
}

#pragma warning restore CS0618