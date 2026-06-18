using NCoreUtils.Google.Gmail;

namespace NCoreUtils;

public interface IGmailClient
{
    /// <inheritdoc cref="IGmailApiV1.DeleteMessageAsync(string, string, CancellationToken)"/>
    Task DeleteMessageAsync(string userId, string id, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IGmailApiV1.GetMessageAsync(string, string, Format, CancellationToken)"/>
    Task<Message?> GetMessageAsync(
        string userId,
        string id,
        Format format = Format.Minimal,
        CancellationToken cancellationToken = default
    );

    Task<Message?> GetMessageAsync(
        string id,
        Format format = Format.Minimal,
        CancellationToken cancellationToken = default)
        => GetMessageAsync("me", id, format, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.ImportMessageAsync(string, Message, InternalDateSource?, bool?, bool?, bool?, CancellationToken)"/>
    Task<Message> ImportMessageAsync(
        string userId,
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? neverMarkSpam = false,
        bool? processForCalendar = false,
        bool? deleted = false,
        CancellationToken cancellationToken = default
    );

    Task<Message> ImportMessageAsync(
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? neverMarkSpam = false,
        bool? processForCalendar = false,
        bool? deleted = false,
        CancellationToken cancellationToken = default)
        => ImportMessageAsync("me", message, internalDateSource, neverMarkSpam, processForCalendar, deleted, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.InsertMessageAsync(string, Message, InternalDateSource?, bool?, CancellationToken)"/>
    Task<Message> InsertMessageAsync(
        string userId,
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? deleted = false,
        CancellationToken cancellationToken = default
    );

    Task<Message> InsertMessageAsync(
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? deleted = false,
        CancellationToken cancellationToken = default)
        => InsertMessageAsync("me", message, internalDateSource, deleted, cancellationToken);

    /// <summary>
    /// Lists the messages in the user's mailbox.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="q">Only return messages matching the specified query. Supports the same query format as the Gmail search box.</param>
    /// <param name="maxResults">Maximum number of messages to return. This field defaults to 100. The maximum allowed value for this field is 500.</param>
    /// <param name="labelIds">Only return messages with labels that match all of the specified label IDs. Messages in a thread might have labels that other messages in the same thread don't have.</param>
    /// <param name="includeSpamTrash">Include messages from SPAM and TRASH in the results.</param>
    /// <param name="cancellationToken">Canellation token.</param>
    IAsyncEnumerable<Message> ListMessagesAsync(
        string userId,
        string? q = default,
        uint? maxResults = default,
        IReadOnlyList<string>? labelIds = default,
        bool? includeSpamTrash = false,
        CancellationToken cancellationToken = default
    );

    IAsyncEnumerable<Message> ListMessagesAsync(
        string? q = default,
        uint? maxResults = default,
        IReadOnlyList<string>? labelIds = default,
        bool? includeSpamTrash = false,
        CancellationToken cancellationToken = default)
        => ListMessagesAsync("me", q, maxResults, labelIds, includeSpamTrash, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.ModifyMessageAsync(string, string, ModifyMessageRequest, CancellationToken)"/>
    Task<Message> ModifyMessageAsync(
        string userId,
        string id,
        ModifyMessageRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Message> ModifyMessageAsync(
        string id,
        ModifyMessageRequest request,
        CancellationToken cancellationToken = default)
        => ModifyMessageAsync("me", id, request, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.SendMessageAsync(string, Message, CancellationToken)"/>
    Task<Message> SendMessageAsync(
        string userId,
        Message message,
        CancellationToken cancellationToken = default
    );

    Task<Message> SendMessageAsync(
        Message message,
        CancellationToken cancellationToken = default)
        => SendMessageAsync("me", message, cancellationToken);


    /// <inheritdoc cref="IGmailApiV1.TrashMessageAsync(string, string, CancellationToken)"/>
    Task<Message> TrashMessageAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Message> TrashMessageAsync(
        string id,
        CancellationToken cancellationToken = default)
        => TrashMessageAsync("me", id, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.UntrashMessageAsync(string, string, CancellationToken)"/>
    Task<Message> UntrashMessageAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Message> UntrashMessageAsync(string id, CancellationToken cancellationToken = default)
        => UntrashMessageAsync("me", id, cancellationToken);

    /// <inheritdoc cref="IGmailApiV1.GetMessageAttachmentAsync(string, string, string, CancellationToken)"/>
    Task<MessagePartBody> GetMessageAttachmentAsync(
        string userId,
        string messageId,
        string id,
        CancellationToken cancellationToken = default
    );

    Task<MessagePartBody> GetMessageAttachmentAsync(
        string messageId,
        string id,
        CancellationToken cancellationToken = default)
        => GetMessageAttachmentAsync("me", messageId, id, cancellationToken);

    /// <summary>
    /// Lists the history of all changes to the given mailbox. History results are returned in chronological order
    /// (increasing <c>historyId</c>)
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="startHistoryId"> Returns history records after the specified startHistoryId. The supplied startHistoryId should be obtained from the historyId of a message, thread, or previous list response.</param>
    /// <param name="maxResults">Maximum number of history records to return. This field defaults to 100. The maximum allowed value for this field is 500.</param>
    /// <param name="labelId">Only return messages with a label matching the ID.</param>
    /// <param name="historyTypes">History types to be returned by the function</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    IAsyncEnumerable<Message> ListHistoryAsync(
        string userId,
        string startHistoryId,
        string? labelId = default,
        IReadOnlyList<HistoryType>? historyTypes = default,
        CancellationToken cancellationToken = default
    );

    IAsyncEnumerable<Message> ListHistoryAsync(
        string startHistoryId,
        string? labelId = default,
        IReadOnlyList<HistoryType>? historyTypes = default,
        CancellationToken cancellationToken = default)
        => ListHistoryAsync("me", startHistoryId, labelId, historyTypes, cancellationToken);

    IAsyncEnumerable<Message> ListAddedMessagesViaHistoryAsync(
        string userId,
        string startHistoryId,
        string? labelId = null,
        CancellationToken cancellationToken = default
    );

    IAsyncEnumerable<Message> ListAddedMessagesViaHistoryAsync(
        string startHistoryId,
        string? labelId = null,
        CancellationToken cancellationToken = default)
        => ListAddedMessagesViaHistoryAsync("me", startHistoryId, labelId, cancellationToken);

    Task<Label> CreateLabelAsync(
        string userId,
        Label label,
        CancellationToken cancellationToken = default
    );

    Task<Label> CreateLabelAsync(
        Label label,
        CancellationToken cancellationToken = default)
        => CreateLabelAsync("me", label, cancellationToken);

    Task<IReadOnlyList<Label>> ListLabelsAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<Label>> ListLabelsAsync(
        CancellationToken cancellationToken = default)
        => ListLabelsAsync("me", cancellationToken);
}