namespace NCoreUtils.Google;

[Obsolete("Use ResumableUploaderProgressArgs instead.")]
public class GoogleCloudStorageUploaderProgressArgs(long sent) : EventArgs
{
    public long Sent { get; } = sent;
}