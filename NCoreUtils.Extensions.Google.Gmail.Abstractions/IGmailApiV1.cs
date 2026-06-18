using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public interface IGmailApiV1
{
    /// <summary>
    /// Immediately and permanently deletes the specified message. This operation cannot be undone. Prefer messages.trash instead.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="id">The ID of the message to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteMessageAsync(string userId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the specified message.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="id">
    /// The ID of the message to retrieve. This ID is usually retrieved using messages.list. The ID is also contained
    /// in the result when a message is inserted (messages.insert) or imported (messages.import).
    /// </param>
    /// <param name="format">The format to return the message in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Message> GetMessageAsync(
        string userId,
        string id,
        Format format = Format.Minimal,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Imports a message into only this user's mailbox, with standard email delivery scanning and classification
    /// similar to receiving via SMTP. This method doesn't perform SPF checks, so it might not work for some spam
    /// messages, such as those attempting to perform domain spoofing. This method does not send a message. Note that
    /// the maximum size of the message is 150 MB.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="message">Data to import.</param>
    /// <param name="internalDateSource">Source for Gmail's internal date of the message.</param>
    /// <param name="neverMarkSpam">Ignore the Gmail spam classifier decision and never mark this email as SPAM in the mailbox.</param>
    /// <param name="processForCalendar">Process calendar invites in the email and add any extracted meetings to the Google Calendar for this user.</param>
    /// <param name="deleted">Mark the email as permanently deleted (not TRASH) and only visible in Google Vault to a Vault administrator. Only used for Google Workspace accounts.</param>
    /// <param name="cancellationToken">Canellation token.</param>
    Task<Message> ImportMessageAsync(
        string userId,
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? neverMarkSpam = false,
        bool? processForCalendar = false,
        bool? deleted = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Directly inserts a message into only this user's mailbox similar to <c>IMAP APPEND</c>, bypassing most scanning
    /// and classification. Does not send a message.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="message">Data to insert.</param>
    /// <param name="internalDateSource">Source for Gmail's internal date of the message.</param>
    /// <param name="deleted">Mark the email as permanently deleted (not TRASH) and only visible in Google Vault to a Vault administrator. Only used for Google Workspace accounts.</param>
    /// <param name="cancellationToken">Canellation token.</param>
    Task<Message> InsertMessageAsync(
        string userId,
        Message message,
        InternalDateSource? internalDateSource = default,
        bool? deleted = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists the messages in the user's mailbox.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="q">Only return messages matching the specified query. Supports the same query format as the Gmail search box.</param>
    /// <param name="pageToken">Page token to retrieve a specific page of results in the list.</param>
    /// <param name="maxResults">Maximum number of messages to return. This field defaults to 100. The maximum allowed value for this field is 500.</param>
    /// <param name="labelIds">Only return messages with labels that match all of the specified label IDs. Messages in a thread might have labels that other messages in the same thread don't have.</param>
    /// <param name="includeSpamTrash">Include messages from SPAM and TRASH in the results.</param>
    /// <param name="cancellationToken">Canellation token.</param>
    Task<ListMessagesResponse> ListMessagesAsync(
        string userId,
        string? q = default,
        string? pageToken = default,
        uint? maxResults = default,
        IReadOnlyList<string>? labelIds = default,
        bool? includeSpamTrash = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Modifies the labels and the Classification Label values on the specified message.
    /// For administrators modifying message for users in their organization, requests require authorization with a
    /// service account that has domain-wide delegation authority to impersonate users with the
    /// <c>https://www.googleapis.com/auth/gmail.modify.restricted</c> scope.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="id">The ID of the message to modify.</param>
    /// <param name="request">Modification request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Message> ModifyMessageAsync(
        string userId,
        string id,
        ModifyMessageRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends the specified message to the recipients in the To, Cc, and Bcc headers.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="message">Messagge to send.</param>
    /// <param name="cancellationToken">Cancecllation token.</param>
    Task<Message> SendMessageAsync(
        string userId,
        Message message,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Moves the specified message to the trash.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="id">The ID of the message to Trash.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Message> TrashMessageAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes the specified message from the trash.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="id">The ID of the message to remove from Trash.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Message> UntrashMessageAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the specified message attachment.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="messageId">The ID of the message containing the attachment.</param>
    /// <param name="id">The ID of the attachment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MessagePartBody> GetMessageAttachmentAsync(
        string userId,
        string messageId,
        string id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists the history of all changes to the given mailbox. History results are returned in chronological order
    /// (increasing <c>historyId</c>)
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="startHistoryId"> Returns history records after the specified startHistoryId. The supplied startHistoryId should be obtained from the historyId of a message, thread, or previous list response.</param>
    /// <param name="pageToken">Page token to retrieve a specific page of results in the list.</param>
    /// <param name="maxResults">Maximum number of history records to return. This field defaults to 100. The maximum allowed value for this field is 500.</param>
    /// <param name="labelId">Only return messages with a label matching the ID.</param>
    /// <param name="historyTypes">History types to be returned by the function</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<ListHistoryResponse> ListHistoryAsync(
        string userId,
        string startHistoryId,
        string? pageToken = default,
        uint? maxResults = default,
        string? labelId = default,
        IReadOnlyList<HistoryType>? historyTypes = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a label.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="label">Label data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Label> CreateLabelAsync(
        string userId,
        Label label,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists all labels in the user's mailbox.
    /// </summary>
    /// <param name="userId">The user's email address. The special value <c>me</c> can be used to indicate the authenticated user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ListLabelsResponse> ListLabelsAsync(
        string userId,
        CancellationToken cancellationToken = default
    );
}