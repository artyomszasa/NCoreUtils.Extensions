using System;
using System.Runtime.Serialization;

namespace NCoreUtils.Google;

#if !NET8_0_OR_GREATER
[Serializable]
#endif
public class GoogleCloudStorageUploadException : Exception
{
#if !NET8_0_OR_GREATER
    protected GoogleCloudStorageUploadException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
#endif

    public GoogleCloudStorageUploadException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public GoogleCloudStorageUploadException(string message)
        : base(message)
    { }
}