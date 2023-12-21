using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public readonly partial struct RawDateTime : IConvertible
{
    private static string FormatConvertibleException(string from, string to)
        => $"Invalid cast from '{from}' to '{to}'.";

    TypeCode IConvertible.GetTypeCode()
        => TypeCode.Object;

    /// <internalonly/>
    bool IConvertible.ToBoolean(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Boolean"));
    }

    /// <internalonly/>
    char IConvertible.ToChar(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Char"));
    }

    /// <internalonly/>
    sbyte IConvertible.ToSByte(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "SByte"));
    }

    /// <internalonly/>
    byte IConvertible.ToByte(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Byte"));
    }

    /// <internalonly/>
    short IConvertible.ToInt16(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Int16"));
    }

    /// <internalonly/>
    ushort IConvertible.ToUInt16(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "UInt16"));
    }

    /// <internalonly/>
    int IConvertible.ToInt32(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Int32"));
    }

    /// <internalonly/>
    uint IConvertible.ToUInt32(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "UInt32"));
    }

    /// <internalonly/>
    long IConvertible.ToInt64(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Int64"));
    }

    /// <internalonly/>
    ulong IConvertible.ToUInt64(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "UInt64"));
    }

    /// <internalonly/>
    float IConvertible.ToSingle(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Single"));
    }

    /// <internalonly/>
    double IConvertible.ToDouble(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Double"));
    }

    /// <internalonly/>
    decimal IConvertible.ToDecimal(IFormatProvider? provider) {
        throw new InvalidCastException(FormatConvertibleException("DateTime", "Decimal"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "IDE0060", MessageId = nameof(provider))]
    private DateTime ToDateTime(IFormatProvider? provider)
        => new(Year, Month, Day, Hour, Minute, Second, Millisecond, DateTimeKind.Unspecified);

    /// <internalonly/>
    DateTime IConvertible.ToDateTime(IFormatProvider? provider)
        => ToDateTime(provider);

    /// <internalonly/>
    object IConvertible.ToType(Type type, IFormatProvider? provider)
    {
        if (type.Equals(typeof(DateTime)))
        {
            return ToDateTime(provider);
        }
        if (type.Equals(typeof(string)))
        {
            return ToString(provider);
        }
        throw new InvalidCastException(FormatConvertibleException("DateTime", type.Name));
    }
}