using System.Runtime.CompilerServices;
using NCoreUtils.Google.Gmail;

namespace NCoreUtils.Google;

public class GmailClient(IGmailApiV1 api) : IGmailClient
{
    public Task DeleteMessageAsync(string userId, string id, CancellationToken cancellationToken = default)
        => api.DeleteMessageAsync(userId, id, cancellationToken);

    public async Task<Message?> GetMessageAsync(string userId, string id, Format format = Format.Minimal, CancellationToken cancellationToken = default)
    {
        try
        {
            return await api.GetMessageAsync(userId, id, format, cancellationToken);
        }
        catch (GoogleCloudException exn) when (exn.ErrorData is { Code: 404 })
        {
            return default;
        }
    }

    public Task<MessagePartBody> GetMessageAttachmentAsync(string userId, string messageId, string id, CancellationToken cancellationToken = default)
        => api.GetMessageAttachmentAsync(userId, messageId, id, cancellationToken);

    public Task<Message> ImportMessageAsync(string userId, Message message, InternalDateSource? internalDateSource = null, bool? neverMarkSpam = false, bool? processForCalendar = false, bool? deleted = false, CancellationToken cancellationToken = default)
        => api.ImportMessageAsync(userId, message, internalDateSource, neverMarkSpam, processForCalendar, deleted, cancellationToken);

    public Task<Message> InsertMessageAsync(string userId, Message message, InternalDateSource? internalDateSource = null, bool? deleted = false, CancellationToken cancellationToken = default)
        => api.InsertMessageAsync(userId, message, internalDateSource, deleted, cancellationToken);


    public async IAsyncEnumerable<Message> ListHistoryAsync(
        string userId,
        string startHistoryId,
        string? labelId = null,
        IReadOnlyList<HistoryType>? historyTypes = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? pageToken = default;
        while (true)
        {
            var resp = await api.ListHistoryAsync(userId, startHistoryId, pageToken, 500u, labelId, historyTypes, cancellationToken);
            if (resp.History is not { Count: > 0 } history)
            {
                break;
            }
            foreach (var entry in history)
            {
                if (entry.Messages is IReadOnlyList<Message> messages)
                {
                    foreach (var message in messages)
                    {
                        yield return message;
                    }
                }
            }
            if (string.IsNullOrEmpty(resp.NextPageToken))
            {
                break;
            }
            pageToken = resp.NextPageToken;
        }
    }

    public async IAsyncEnumerable<Message> ListAddedMessagesViaHistoryAsync(
        string userId,
        string startHistoryId,
        string? labelId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? pageToken = default;
        while (true)
        {
            var resp = await api.ListHistoryAsync(userId, startHistoryId, pageToken, 500u, labelId, [HistoryType.MessageAdded], cancellationToken);
            if (resp.History is not { Count: > 0} history)
            {
                break;
            }
            foreach (var historyEntry in history)
            {
                if (historyEntry.MessagesAdded is IReadOnlyList<MessageAdded> entries)
                {
                    foreach (var entry in entries)
                    {
                        yield return entry.Message;
                    }
                }
            }
            if (string.IsNullOrEmpty(resp.NextPageToken))
            {
                break;
            }
            pageToken = resp.NextPageToken;
        }
    }

    public async IAsyncEnumerable<Message> ListMessagesAsync(
        string userId,
        string? q = null,
        uint? maxResults = null,
        IReadOnlyList<string>? labelIds = null,
        bool? includeSpamTrash = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var emitted = 0u;
        string? pageToken = default;
        while (emitted < maxResults)
        {
            var desiredCount = maxResults is uint max
                ? Math.Min(500u, max - emitted)
                : 500u;
            var resp = await api.ListMessagesAsync(userId, q, pageToken, desiredCount, labelIds, includeSpamTrash, cancellationToken).ConfigureAwait(false);
            foreach (var message in resp.Messages)
            {
                yield return message;
                if (++emitted >= maxResults)
                {
                    break;
                }
            }
            if (string.IsNullOrEmpty(resp.NextPageToken))
            {
                break;
            }
            pageToken = resp.NextPageToken;
        }
    }

    public Task<Message> ModifyMessageAsync(string userId, string id, ModifyMessageRequest request, CancellationToken cancellationToken = default)
        => api.ModifyMessageAsync(userId, id, request, cancellationToken);

    public Task<Message> SendMessageAsync(string userId, Message message, CancellationToken cancellationToken = default)
        => api.SendMessageAsync(userId, message, cancellationToken);

    public Task<Message> TrashMessageAsync(string userId, string id, CancellationToken cancellationToken = default)
        => api.TrashMessageAsync(userId, id, cancellationToken);

    public Task<Message> UntrashMessageAsync(string userId, string id, CancellationToken cancellationToken = default)
        => api.UntrashMessageAsync(userId, id, cancellationToken);

    public Task<Label> CreateLabelAsync(string userId, Label label, CancellationToken cancellationToken = default)
        => api.CreateLabelAsync(userId, label, cancellationToken);

    public async Task<IReadOnlyList<Label>> ListLabelsAsync(string userId, CancellationToken cancellationToken = default)
        => (await api.ListLabelsAsync(userId, cancellationToken))?.Labels ?? [];
}
