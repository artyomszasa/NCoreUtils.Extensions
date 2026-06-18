using NCoreUtils.Google;

namespace NCoreUtils;

public class ResumableUploadException(string? message, GoogleErrorData? errorData = default, Exception? innerException = default)
    : GoogleCloudException(message, errorData, innerException)
{
    public ResumableUploadException(GoogleErrorData errorData, Exception? innerException = default)
        : this(default, errorData, innerException)
    { }

    public ResumableUploadException(string? message, Exception? innerException)
        : this(message, default, innerException)
    { }
}