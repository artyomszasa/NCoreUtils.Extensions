using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCoreUtils.JsonSerialization.Internal
{
    public interface IImmutableObjectDescriptor
    {
        Type Type { get; }

        ConstructorInfo Ctor { get; }

        IReadOnlyDictionary<ParameterInfo, PropertyInfo> ParameterMapping { get; }

        IReadOnlyDictionary<string, PropertyInfo> Properties { get; }
    }
}