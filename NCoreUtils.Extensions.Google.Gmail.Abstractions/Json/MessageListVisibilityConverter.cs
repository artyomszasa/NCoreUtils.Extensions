using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(MessageListVisibility))]
public sealed partial class MessageListVisibilityConverter : JsonConverter<MessageListVisibility> { }