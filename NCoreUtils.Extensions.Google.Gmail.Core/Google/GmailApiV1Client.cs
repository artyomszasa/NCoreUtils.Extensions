using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail;
using NCoreUtils.Google.Gmail.Proto;
using NCoreUtils.Memory;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootGmailApiV1Info))]
[JsonSerializable(typeof(ModifyMessageRequest))]
internal sealed partial class GmailApiV1SerializerContext : JsonSerializerContext { }

internal static class GmailApiV1ClientSpanBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, string value)
    {
        char separator;
        if (first)
        {
            separator = '?';
            first = false;
        }
        else
        {
            separator = '&';
        }
        builder.Append(separator);
        builder.Append(name);
        builder.Append('=');
        builder.AppendUriEscaped(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, bool value)
        => builder.AppendQueryParameter(ref first, name, value ? "true" : "false");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, uint value)
    {
        char separator;
        if (first)
        {
            separator = '?';
            first = false;
        }
        else
        {
            separator = '&';
        }
        builder.Append(separator);
        builder.Append(name);
        builder.Append('=');
        builder.Append(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter<T>(this ref SpanBuilder builder, ref bool first, string name, T value, IEmplacer<T> emplacer)
        where T : struct
    {
        char separator;
        if (first)
        {
            separator = '?';
            first = false;
        }
        else
        {
            separator = '&';
        }
        builder.Append(separator);
        builder.Append(name);
        builder.Append('=');
        builder.Append(value, emplacer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, string? value)
    {
        if (value is not null)
        {
            AppendQueryParameter(ref builder, ref first, name, value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, bool? value)
    {
        if (value is bool v)
        {
            AppendQueryParameter(ref builder, ref first, name, v);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, uint? value)
    {
        if (value is uint v)
        {
            AppendQueryParameter(ref builder, ref first, name, v);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter<T>(this ref SpanBuilder builder, ref bool first, string name, T? value, IEmplacer<T> emplacer)
        where T : struct
    {
        if (value is T v)
        {
            AppendQueryParameter(ref builder, ref first, name, v, emplacer);
        }
    }
}

[ProtoClient(typeof(GmailApiV1Info), typeof(GmailApiV1SerializerContext))]
[ProtoClientConstructorParameter(typeof(IGmailConfiguration), "GmailConfiguration")]
public partial class GmailApiV1Client
{
    private sealed class FormatEmplacer : IEmplacer<Format>
    {
        public static FormatEmplacer Singleton { get; } = new();

        private static ReadOnlySpan<char> GetSpan(Format value) => value switch
        {
            Format.Minimal => "minimal",
            Format.Metadata => "metadata",
            Format.Raw => "raw",
            Format.Full => "full",
            _ => throw new InvalidOperationException($"Invalid format value: {value}")
        };

        private FormatEmplacer() { }

        public int Emplace(Format value, Span<char> span)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                throw new InsufficientBufferSizeException(span.Length, v.Length);
            }
            v.CopyTo(span);
            return v.Length;
        }

        public bool TryEmplace(Format value, Span<char> span, out int used)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                used = 0;
                return false;
            }
            v.CopyTo(span);
            used = v.Length;
            return true;
        }
    }

    private sealed class InternalDateSourceEmplacer : IEmplacer<InternalDateSource>
    {
        public static InternalDateSourceEmplacer Singleton { get; } = new();

        private static ReadOnlySpan<char> GetSpan(InternalDateSource value) => value switch
        {
            InternalDateSource.ReceivedTime => "receivedTime",
            InternalDateSource.DateHeader => "dateHeader",
            _ => throw new InvalidOperationException($"Invalid InternalDateSource value: {value}")
        };

        private InternalDateSourceEmplacer() { }

        public int Emplace(InternalDateSource value, Span<char> span)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                throw new InsufficientBufferSizeException(span.Length, v.Length);
            }
            v.CopyTo(span);
            return v.Length;
        }

        public bool TryEmplace(InternalDateSource value, Span<char> span, out int used)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                used = 0;
                return false;
            }
            v.CopyTo(span);
            used = v.Length;
            return true;
        }
    }

    private sealed class HistoryTypeEmplacer : IEmplacer<HistoryType>
    {
        public static HistoryTypeEmplacer Singleton { get; } = new();

        private static ReadOnlySpan<char> GetSpan(HistoryType value) => value switch
        {
            HistoryType.MessageAdded => "messageAdded",
            HistoryType.MessageDeleted => "messageDeleted",
            HistoryType.LabelAdded => "labelAdded",
            HistoryType.LabelRemoved => "labelRemoved",
            _ => throw new InvalidOperationException($"Invalid HistoryType value: {value}")
        };

        private HistoryTypeEmplacer() { }

        public int Emplace(HistoryType value, Span<char> span)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                throw new InsufficientBufferSizeException(span.Length, v.Length);
            }
            v.CopyTo(span);
            return v.Length;
        }

        public bool TryEmplace(HistoryType value, Span<char> span, out int used)
        {
            var v = GetSpan(value);
            if (span.Length < v.Length)
            {
                used = 0;
                return false;
            }
            v.CopyTo(span);
            used = v.Length;
            return true;
        }
    }

    public const string HttpClientConfigurationName = nameof(GmailApiV1Client);

    public const string GmailScope = "https://mail.google.com/";

    public const string GmailReadOnlyScope = "https://www.googleapis.com/auth/gmail.readonly";

    public const string GmailMetadataScope = "https://www.googleapis.com/auth/gmail.metadata";

    public const string GmailInsertScope = "https://www.googleapis.com/auth/gmail.insert";

    public const string GmailSendScope = "https://www.googleapis.com/auth/gmail.send";

    public const string GmailModifyScope = "https://www.googleapis.com/auth/gmail.modify";

    private HttpRequestMessage CreateDeleteMessageRequest(string userId, string id)
    {
        var basePath = GetCachedMethodPath(Methods.DeleteMessage);
        var path = $"{basePath}/{userId}/messages/{id}";
        var req = new HttpRequestMessage(HttpMethod.Delete, path);
        req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.DeleteMessage, GmailScope));
        return req;
    }

    private HttpRequestMessage CreateGetMessageRequest(string userId, string id, Format format)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.GetMessage);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/");
                builder.Append(id);
                builder.Append("?format=");
                builder.Append(format, FormatEmplacer.Singleton);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.GetMessage, format is Format.Metadata or Format.Minimal
                ? GmailMetadataScope
                : GmailReadOnlyScope
            ));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateImportMessageRequest(
        string userId,
        Message message,
        InternalDateSource? internalDateSource,
        bool? neverMarkSpam,
        bool? processForCalendar,
        bool? deleted)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ImportMessage);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/import");
                builder.AppendOptionalQueryParameter(ref firstArg, "internalDateSource", internalDateSource, InternalDateSourceEmplacer.Singleton);
                builder.AppendOptionalQueryParameter(ref firstArg, "neverMarkSpam", neverMarkSpam);
                builder.AppendOptionalQueryParameter(ref firstArg, "processForCalendar", processForCalendar);
                builder.AppendOptionalQueryParameter(ref firstArg, "deleted", deleted);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(message, GmailApiV1SerializerContext.Default.Message)
            };
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ImportMessage, GmailInsertScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateInsertMessageRequest(
        string userId,
        Message message,
        InternalDateSource? internalDateSource,
        bool? deleted)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ImportMessage);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages");
                builder.AppendOptionalQueryParameter(ref firstArg, "internalDateSource", internalDateSource, InternalDateSourceEmplacer.Singleton);
                builder.AppendOptionalQueryParameter(ref firstArg, "deleted", deleted);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(message, GmailApiV1SerializerContext.Default.Message)
            };
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.InsertMessage, GmailInsertScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateListMessagesRequest(
        string userId,
        string? q,
        string? pageToken,
        uint? maxResults,
        IReadOnlyList<string>? labelIds,
        bool? includeSpamTrash)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ListMessages);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages");
                builder.AppendOptionalQueryParameter(ref firstArg, "q", q);
                builder.AppendOptionalQueryParameter(ref firstArg, "pageToken", pageToken);
                builder.AppendOptionalQueryParameter(ref firstArg, "maxResults", maxResults);
                if (labelIds is not null)
                {
                    foreach (var labelId in labelIds)
                    {
                        builder.AppendOptionalQueryParameter(ref firstArg, "labelIds", labelId);
                    }
                }
                builder.AppendOptionalQueryParameter(ref firstArg, "includeSpamTrash", includeSpamTrash);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ListMessages, GmailMetadataScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateModifyMessageRequest(
        string userId,
        string id,
        ModifyMessageRequest request)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ModifyMessage);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/");
                builder.Append(id);
                builder.Append("/modify");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(request, GmailApiV1SerializerContext.Default.ModifyMessageRequest)
            };
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ModifyMessage, GmailModifyScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateSendMessageRequest(string userId, Message message)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.SendMessage);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/send");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(message, GmailApiV1SerializerContext.Default.Message)
            };
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.SendMessage, GmailSendScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateTrashMessageRequest(string userId, string id)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.TrashMessage);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/");
                builder.Append(id);
                builder.Append("/trash");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.TrashMessage, GmailModifyScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateUntrashMessageRequest(string userId, string id)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.UntrashMessage);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/");
                builder.Append(id);
                builder.Append("/untrash");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.UntrashMessage, GmailModifyScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateGetMessageAttachmentRequest(string userId, string messageId, string id)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.GetMessageAttachment);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/messages/");
                builder.Append(messageId);
                builder.Append("/attachments/");
                builder.Append(id);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.GetMessageAttachment, GmailReadOnlyScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateListHistoryRequest(
        string userId,
        string startHistoryId,
        string? pageToken,
        uint? maxResults,
        string? labelId,
        IReadOnlyList<HistoryType>? historyTypes)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ListHistory);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/history");
                builder.AppendOptionalQueryParameter(ref firstArg, "startHistoryId", startHistoryId);
                builder.AppendOptionalQueryParameter(ref firstArg, "pageToken", pageToken);
                builder.AppendOptionalQueryParameter(ref firstArg, "maxResults", maxResults);
                builder.AppendOptionalQueryParameter(ref firstArg, "labelId", labelId);
                if (historyTypes is not null)
                {
                    foreach (var historyType in historyTypes)
                    {
                        builder.AppendOptionalQueryParameter(ref firstArg, "historyTypes", historyType, HistoryTypeEmplacer.Singleton);
                    }
                }
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ListHistory, GmailMetadataScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateCreateLabelRequest(string userId, Label label)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.CreateLabel);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/labels");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(label, GmailApiV1SerializerContext.Default.Label)
            };
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ListLabels, GmailModifyScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateListLabelsRequest(string userId)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ListLabels);
            string path;
            {
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.Append('/');
                builder.Append(userId);
                builder.Append("/labels");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(GmailConfiguration.AdjustScope(Methods.ListLabels, GmailMetadataScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}