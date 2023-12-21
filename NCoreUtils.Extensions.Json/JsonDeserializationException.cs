using System;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif
using System.Text.Json;

namespace NCoreUtils;

/// <summary>
/// Extends <see cref="JsonException" /> by including actual path information after the custom message. If default
/// message is required use <see cref="JsonException" /> instead.
/// </summary>
#if !NET8_0_OR_GREATER
[Serializable]
#endif
public class JsonDeserializationException : JsonException
{
    public override string Message
    {
        get
        {
            if (string.IsNullOrEmpty(Path))
            {
                return base.Message;
            }
            return base.Message
                + Environment.NewLine
                + $"Path: {Path} | LineNumber: {LineNumber} | BytePositionInLine: {BytePositionInLine}.";
        }
    }

#if !NET8_0_OR_GREATER
    protected JsonDeserializationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
#endif

    public JsonDeserializationException(string message)
        : base(message)
    { }

    public JsonDeserializationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}