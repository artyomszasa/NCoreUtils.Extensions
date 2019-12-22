using System;
using System.Collections.Generic;
using NCoreUtils.JsonSerialization.Internal;

namespace NCoreUtils.JsonSerialization
{
    public interface IJsonImmutableSerializerCache : IDictionary<Type, IImmutableObjectDescriptor> { }
}