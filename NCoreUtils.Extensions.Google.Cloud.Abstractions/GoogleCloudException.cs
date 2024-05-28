using System.Buffers;

namespace NCoreUtils.Google;

public class GoogleCloudException(string? message, GoogleErrorData? errorData = default, Exception? innerException = default)
    : Exception(FormatGoogleError(message, errorData), innerException)
{
    private static string FormatGoogleError(GoogleErrorData googleError, bool includeDot)
    {
        if (googleError is null)
        {
            return string.Empty;
        }
        var buffer = ArrayPool<char>.Shared.Rent(16 * 1024);
        try
        {
            var builder = new SpanBuilder(buffer);
            builder.Append(googleError.Code);
            builder.Append(": ");
            builder.Append(googleError.Message);
            if (googleError.Errors is { Count: >0 } errors)
            {
                builder.Append(" => ");
                var firstError = true;
                foreach (var error in errors)
                {
                    if (firstError)
                    {
                        firstError = false;
                    }
                    else
                    {
                        builder.Append("; ");
                    }
                    builder.Append(error, GoogleErrorDetails.Emplacer);
                }
            }
            if (includeDot)
            {
                builder.Append('.');
            }
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static string FormatGoogleError(string? message, GoogleErrorData? googleError)
    {
        if (message is null or { Length: 0 })
        {
            if (googleError is null)
            {
                return "Google Cloud service related error has occured.";
            }
            return FormatGoogleError(googleError, includeDot: true);
        }
        if (googleError is null)
        {
            return message;
        }
        return $"{message.TrimEnd('.')}:{FormatGoogleError(googleError, includeDot: true)}";
    }

    public GoogleErrorData? ErrorData { get; } = errorData;

    public GoogleCloudException(GoogleErrorData? errorData)
        : this(default, errorData)
    { }

    public GoogleCloudException(string? message, Exception? innerException)
        : this(message, default, innerException)
    { }

    public GoogleCloudException(GoogleErrorData? errorData, Exception? innerException = default)
        : this(default, errorData, innerException)
    { }

}