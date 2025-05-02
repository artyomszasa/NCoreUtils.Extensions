using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NCoreUtils.Google;

internal static class Utf8JsonReaderExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Expect(this in Utf8JsonReader reader, JsonTokenType expected)
    {
        var actual = reader.TokenType;
        if (expected != actual)
        {
            throw new JsonException($"Expected {expected} found {actual}.");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ReadOrThrow(this ref Utf8JsonReader reader)
    {
        if (!reader.Read())
        {
            throw new JsonException("Unexpected end of JSON stream.");
        }
    }
}