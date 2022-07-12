using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Internal
{
    public interface IImmutableJsonConverter
    {
        Type TargetType { get; }

        ImmutableJsonCoverterOptions Options { get; }
    }

    public interface IImmutableJsonConverter<[DynamicallyAccessedMembers(D.CtorAndProps)] T> : IImmutableJsonConverter
    {
#if !NETSTANDARD2_0
        Type IImmutableJsonConverter.TargetType => typeof(T);
#endif
    }
}