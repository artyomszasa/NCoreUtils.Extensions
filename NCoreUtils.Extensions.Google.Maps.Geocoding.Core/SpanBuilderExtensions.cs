namespace NCoreUtils.Google.Maps.Geocoding;

#if !NET6_0_OR_GREATER

internal static class Polyfill
{
    public static void AppendUriEscaped(this ref SpanBuilder builder, string value)
    {
        builder.Append(Uri.EscapeDataString(value));
    }
}

#endif


internal static class SpanBuilderExtensions
{
    public static void AppendQueryParameter(this ref SpanBuilder builder, string key, string? value, ref bool fst)
    {
        if (!string.IsNullOrEmpty(value))
        {
            char delimiter;
            if (fst)
            {
                fst = false;
                delimiter = '?';
            }
            else
            {
                delimiter = '&';
            }
            builder.Append(delimiter);
            builder.Append(key);
            builder.Append('=');
            builder.AppendUriEscaped(value);
        }
    }

    public static void AppendQueryParameter(this ref SpanBuilder builder, string key, IReadOnlyList<string>? value, ref bool fst)
    {
        if (value is { Count: > 0 } lines)
        {
            char delimiter;
            if (fst)
            {
                fst = false;
                delimiter = '?';
            }
            else
            {
                delimiter = ':';
            }
            builder.Append(delimiter);
            builder.Append(key);
            builder.Append('=');
            var fstLine = true;
            foreach (var line in lines)
            {
                if (fstLine)
                {
                    fstLine = false;
                }
                else
                {
                    builder.Append('+');
                }
                builder.AppendUriEscaped(line);
            }
        }
    }
}
