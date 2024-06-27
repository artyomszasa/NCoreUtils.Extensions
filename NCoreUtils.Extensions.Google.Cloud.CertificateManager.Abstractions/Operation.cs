using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

internal static class OperationHelpers
{
    public static bool TryGetOperationId(string name, [MaybeNullWhen(false)] out string id)
    {
        // projects/{PROJECT}/locations/{LOCATION}/operations/operation-1718627461699-61b15235b179d-4dc97895-d4ffcf2a
        if (string.IsNullOrEmpty(name))
        {
            id = default;
            return false;
        }
        var index = name.LastIndexOf('/');
        if (-1 == index)
        {
            id = default;
            return false;
        }
        id = name[(index + 1) .. ];
        return true;
    }

    public static string GetOperationId(string name)
        => TryGetOperationId(name, out var id)
            ? id
            : throw new InvalidOperationException($"Unable to get operation id from name \"{name}\".");
}

public class Operation(
    string name,
    bool done,
    Status? error)
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Name { get; } = name;

    // FIXME: metadata

    [JsonPropertyName("done")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Done { get; } = done;

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Status? Error { get; } = error;

    public string GetOperationId() => OperationHelpers.GetOperationId(Name);
}

public class Operation<T>(
    string name,
    bool done,
    Status? error,
    T? response)
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Name { get; } = name;

    // FIXME: metadata

    [JsonPropertyName("done")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Done { get; } = done;

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Status? Error { get; } = error;

    [JsonPropertyName("response")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Response { get; } = response;

    public string GetOperationId() => OperationHelpers.GetOperationId(Name);
}