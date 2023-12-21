using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NCoreUtils;

public static class Utf8JsonWriterExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStringWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        string? value)
    {
        if (null != value)
        {
            writer.WriteString(propertyName, value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        short? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        int? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        long? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        float? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        double? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        decimal? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBooleanWhenNotNull(
        this Utf8JsonWriter writer,
        JsonEncodedText propertyName,
        bool? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolean(propertyName, value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStringWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        string? value)
        => writer.WriteStringWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        short? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        int? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        long? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        float? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        double? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNumberWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        decimal? value)
        => writer.WriteNumberWhenNotNull(JsonEncodedText.Encode(propertyName), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBooleanWhenNotNull(
        this Utf8JsonWriter writer,
        string propertyName,
        bool? value)
        => writer.WriteBooleanWhenNotNull(JsonEncodedText.Encode(propertyName), value);
}