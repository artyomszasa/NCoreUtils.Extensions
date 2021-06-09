using System;

namespace NCoreUtils.Internal
{
    public interface IImmutableJsonConverter
    {
        Type TargetType { get; }

        ImmutableJsonCoverterOptions Options { get; }
    }

    public interface IImmutableJsonConverter<T> : IImmutableJsonConverter
    {
#if !NETSTANDARD2_0
        Type IImmutableJsonConverter.TargetType => typeof(T);
#endif
    }
}