namespace NCoreUtils.Google;

public class ResumableUploaderProgressArgs(long sent) : EventArgs
{
    public long Sent { get; } = sent;
}