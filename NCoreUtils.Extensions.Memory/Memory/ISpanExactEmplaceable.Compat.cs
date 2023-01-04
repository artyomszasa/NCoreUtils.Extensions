using System;

namespace NCoreUtils.Memory;

public interface ISpanExactEmplaceable : ISpanEmplaceable
{
    int GetEmplaceBufferSize();
}