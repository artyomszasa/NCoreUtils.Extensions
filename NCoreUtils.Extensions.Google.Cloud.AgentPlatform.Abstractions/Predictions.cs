using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public abstract class Prediction(string prompt)
{
    public string Prompt { get; } = prompt;
}

public abstract class ImagePrediction(string prompt, string? mimeType)
    : Prediction(prompt)
{
    public string? MimeType { get; } = mimeType;
}

public class GscImagePrediction(string prompt, string? mimeType, string gcsUri)
    : ImagePrediction(prompt, mimeType)
{
    public string GcsUri { get; } = gcsUri;
}

public static class Predictions
{
    private static bool TryExtractGscImagePrediction(in JsonElement item, [MaybeNullWhen(false)] out GscImagePrediction prediction)
    {
        if (item.ValueKind == JsonValueKind.Object
            && item.TryGetProperty("prompt"u8, out var jPrompt) && jPrompt.ValueKind == JsonValueKind.String
            && item.TryGetProperty("gcsUri"u8, out var jGcsUri) && jGcsUri.ValueKind == JsonValueKind.String)
        {
            var mimeType = item.TryGetProperty("mimeType", out var jMimeType) && jMimeType.ValueKind == JsonValueKind.String
                ? jMimeType.GetString()
                : default;
            prediction = new(jPrompt.GetString()!, mimeType, jGcsUri.GetString()!);
            return true;
        }
        prediction = default;
        return false;

    }

    private static bool TryExtractPrediction(in JsonElement item, [MaybeNullWhen(false)] out Prediction prediction)
    {
        if (TryExtractGscImagePrediction(in item, out var gcsImagePrediction))
        {
            prediction = gcsImagePrediction;
            return true;
        }
        prediction = default;
        return false;
    }

    private readonly struct SpanSource
    {
        private readonly object? _source;

        public SpanSource(JsonElement[] array)
            => _source = array;

        public SpanSource(List<JsonElement> array)
            => _source = array;

        public ReadOnlySpan<JsonElement> Span => _source switch
        {
            null => [],
            JsonElement[] array => new ReadOnlySpan<JsonElement>(array),
            List<JsonElement> list => CollectionsMarshal.AsSpan(list),
            _ => throw new InvalidOperationException("Should never happen")
        };
    }

    private sealed class SpanPredictionEnumerator(SpanSource source)
        : IEnumerator<Prediction>
    {
        private int index = -1;

        public Prediction Current { get; private set; } = default!;

        object IEnumerator.Current => Current;

        public void Dispose() { /* noop */ }

        public bool MoveNext()
        {
            var span = source.Span;
            for (var i = index + 1; i < span.Length; ++i)
            {
                if (TryExtractPrediction(in span[i], out var prediction))
                {
                    index = i;
                    Current = prediction;
                    return true;
                }
            }
            Current = default!;
            return false;
        }

        public void Reset()
        {
            index = -1;
        }
    }

    private sealed class SpanPredictionEnumerable(SpanSource source)
        : IEnumerable<Prediction>
    {
        public IEnumerator<Prediction> GetEnumerator() => new SpanPredictionEnumerator(source);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static IEnumerable<Prediction> EnumeratePredictions_Generic(IEnumerable<JsonElement> source)
    {
        foreach (var item in source)
        {
            if (TryExtractPrediction(in item, out var prediction))
            {
                yield return prediction;
            }
        }
    }

    public static IEnumerable<Prediction> EnumeratePredictions(IEnumerable<JsonElement> source) => source switch
    {
        JsonElement[] array => new SpanPredictionEnumerable(new SpanSource(array)),
        List<JsonElement> list => new SpanPredictionEnumerable(new SpanSource(list)),
        _ => EnumeratePredictions_Generic(source)
    };

    public static IEnumerable<Prediction> EnumeratePredictions(this PredictResponse source) => source switch
    {
        { Predictions: { Count: > 0 } predictions } => EnumeratePredictions(predictions),
        _ => []
    };
}