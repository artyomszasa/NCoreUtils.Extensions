namespace NCoreUtils.Google.Gmail;

public class ListMessagesResponse(
    IReadOnlyList<Message> messages,
    string? nextPageToken,
    uint? resultSizeEstimate)
{
    public IReadOnlyList<Message> Messages { get; } = messages;

    public string? NextPageToken { get; } = nextPageToken;

    public uint? ResultSizeEstimate { get; } = resultSizeEstimate;
}