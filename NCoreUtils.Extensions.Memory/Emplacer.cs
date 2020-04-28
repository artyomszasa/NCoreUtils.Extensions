using System;
using System.Collections.Generic;
using NCoreUtils.Memory;

namespace NCoreUtils
{
    public static class Emplacer
    {
        static readonly Dictionary<Type, object> _emplacers = new Dictionary<Type, object>
        {
            { typeof(sbyte), Int8Emplacer.Instance },
            { typeof(short), Int16Emplacer.Instance },
            { typeof(int), Int32Emplacer.Instance },
            { typeof(long), Int64Emplacer.Instance },
            { typeof(byte), UInt8Emplacer.Instance },
            { typeof(ushort), UInt16Emplacer.Instance },
            { typeof(uint), UInt32Emplacer.Instance },
            { typeof(ulong), UInt64Emplacer.Instance },
            { typeof(float), SingleEmplacer.Default },
            { typeof(double), DoubleEmplacer.Default },
            { typeof(char), CharEmplacer.Instance },
            { typeof(string), StringEmplacer.Instance }
        };

        public static IEmplacer<T> GetDefault<T>()
        {
            if (_emplacers.TryGetValue(typeof(T), out var boxed))
            {
                return (IEmplacer<T>)boxed;
            }
            if (typeof(IEmplaceable<T>).IsAssignableFrom(typeof(T)))
            {
                return (IEmplacer<T>)Activator.CreateInstance(typeof(EmplaceableEmplacer<>).MakeGenericType(typeof(T)), true);
            }
            return new DefaultEmplacer<T>();
        }
    }
}