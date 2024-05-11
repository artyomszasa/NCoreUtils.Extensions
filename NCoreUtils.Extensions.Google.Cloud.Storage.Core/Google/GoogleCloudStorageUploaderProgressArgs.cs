namespace NCoreUtils.Google;

public class GoogleCloudStorageUploaderProgressArgs(long sent) : EventArgs
{
    public long Sent { get; } = sent;
}