using System;

namespace NCoreUtils.Memory;

public interface ISpanExactEmplaceable : ISpanEmplaceable
{
    int GetEmplaceBufferSize();

    bool ISpanEmplaceable.TryGetEmplaceBufferSize(out int minimumBufferSize)
    {
        minimumBufferSize = GetEmplaceBufferSize();
        return true;
    }
}