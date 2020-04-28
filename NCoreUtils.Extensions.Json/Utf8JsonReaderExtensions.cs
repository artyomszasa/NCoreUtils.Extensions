using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NCoreUtils
{
    public static class Utf8JsonReaderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetStringOrNull(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.String => reader.GetString(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.String} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? GetBooleanOrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null}, {JsonTokenType.True} or {JsonTokenType.False} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? GetInt16OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetInt16(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? GetInt32OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetInt32(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? GetInt64OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetInt64(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ushort? GetUInt16OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetUInt16(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint? GetUInt32OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetUInt32(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong? GetUInt64OrDefault(this in Utf8JsonReader reader)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.Number => reader.GetUInt64(),
                var tokenType => throw new InvalidOperationException($"Expected {JsonTokenType.Null} or {JsonTokenType.Number} but found {tokenType}.")
            };
    }
}