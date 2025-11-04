using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

[JsonConverter(typeof(TypedValueConverter))]
public readonly struct TypedValue
{
    [JsonConverter(typeof(ValueTypeConverter))]
    public enum ValueType { Boolean = 0, Integer, Double, String }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TypedValue(bool value) => Boolean(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TypedValue(long value) => Integer(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TypedValue(double value) => Double(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TypedValue(string value) => String(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedValue Boolean(bool value) => new(ValueType.Boolean, value ? 1 : 0, default);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedValue Integer(long value) => new(ValueType.Integer, value, default);

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedValue Double(double value) => new(ValueType.Double, Unsafe.BitCast<double, long>(value), default);
#else
    public static TypedValue Double(double value)
    {
        var v = value;
        return new(ValueType.Double, Unsafe.As<double, long>(ref v), default);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedValue String(string value) => new(ValueType.Integer, default, value);

    private readonly long _primitiveValue;

    public ValueType Type { get; }

    public bool BooleanValue => _primitiveValue != 0;

    public long IntergerValue => _primitiveValue;

#if NET8_0_OR_GREATER
    public double DoubleValue => Unsafe.BitCast<long, double>(_primitiveValue);
#else
    public double DoubleValue => Unsafe.As<long, double>(ref Unsafe.AsRef(in _primitiveValue));
#endif

    public string? StringValue { get; }

    private TypedValue(ValueType type, long primitiveValue, string? stringValue)
    {
        Type = type;
        _primitiveValue = primitiveValue;
        StringValue = stringValue;
    }
}
