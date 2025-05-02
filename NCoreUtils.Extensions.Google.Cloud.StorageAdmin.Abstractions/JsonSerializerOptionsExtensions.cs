using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NCoreUtils.Google;

internal static class JsonSerializerOptionsExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static JsonTypeInfo<T> GetTypeInfo<T>(this JsonSerializerOptions options)
        => options.GetTypeInfo(typeof(T)) switch
        {
            null => throw new InvalidOperationException($"No JSON type info found for {typeof(T)}."),
            JsonTypeInfo<T> typeInfo => typeInfo,
            _ => throw new InvalidOperationException($"Invalid JSON type info found for {typeof(T)}.")
        };
}