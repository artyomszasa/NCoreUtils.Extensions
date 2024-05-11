using System.Buffers;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleErrorDetails : ISpanEmplaceable
{
    private static bool TryAppendProp(ref SpanBuilder builder, scoped ref bool fst, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (fst)
            {
                fst = false;
            }
            else
            {
                if (!builder.TryAppend(", ")) { return false; }
            }
            return builder.TryAppend(name)
                && builder.TryAppend('=')
                && builder.TryAppend(value);
        }
        return true;
    }

    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("locationType")]
    public string? LocationType { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    public override string ToString()
    {
        var buffer = ArrayPool<char>.Shared.Rent(16 * 1024);
        try
        {
            var size = ((ISpanEmplaceable)this).Emplace(buffer);
            return new string(buffer, 0, size);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
        => ToString();

    public bool TryEmplace(Span<char> span, out int used)
    {
        var builder = new SpanBuilder(span);
        if (!builder.TryAppend('[')) { used = 0; return false; }
        var fst = true;
        if (TryAppendProp(ref builder, ref fst, nameof(Domain), Domain)
            && TryAppendProp(ref builder, ref fst, nameof(Location), Location)
            && TryAppendProp(ref builder, ref fst, nameof(LocationType), LocationType)
            && TryAppendProp(ref builder, ref fst, nameof(Message), Message)
            && TryAppendProp(ref builder, ref fst, nameof(Reason), Reason))
        {
            used = builder.Length;
            return true;
        }
        used = 0;
        return false;
    }

#if !NET6_0_OR_GREATER
    int ISpanEmplaceable.Emplace(System.Span<char> span)
    {
        if (this.TryEmplace(span, out var used))
        {
            return used;
        }
        throw new InsufficientBufferSizeException(span);
    }

    bool ISpanEmplaceable.TryGetEmplaceBufferSize(out int minimumBufferSize)
    {
        minimumBufferSize = default;
        return false;
    }

    bool ISpanEmplaceable.TryFormat(System.Span<char> destination, out int charsWritten, System.ReadOnlySpan<char> format, System.IFormatProvider? provider)
        => TryEmplace(destination, out charsWritten);
#endif
}