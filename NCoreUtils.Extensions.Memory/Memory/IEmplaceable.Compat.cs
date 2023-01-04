using System;

namespace NCoreUtils.Memory
{
    [Obsolete("Backward compatibility only, ISpanEmpaceable ot ISpanExactEmplaceable should be used insted.")]
    public interface IEmplaceable<T>
    {
        int Emplace(Span<char> span);

        bool TryEmplace(Span<char> span, out int used);
    }
}