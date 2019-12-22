using System;
using System.Collections.Concurrent;
using NCoreUtils.JsonSerialization.Internal;

namespace NCoreUtils.JsonSerialization
{
    public class DefaultJsonImmutableSerializerCache
        : ConcurrentDictionary<Type, IImmutableObjectDescriptor>
        , IJsonImmutableSerializerCache
    {
        public static DefaultJsonImmutableSerializerCache SharedInstance { get; } = new DefaultJsonImmutableSerializerCache();
    }
}