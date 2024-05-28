namespace NCoreUtils.Google;

public class GoogleCloudStorageUploadException(string? message, GoogleErrorData? errorData = default, Exception? innerException = default)
    : GoogleCloudException(message, errorData, innerException)
{
    public GoogleCloudStorageUploadException(GoogleErrorData errorData, Exception? innerException = default)
        : this(default, errorData, innerException)
    { }

    public GoogleCloudStorageUploadException(string? message, Exception? innerException)
        : this(message, default, innerException)
    { }
}