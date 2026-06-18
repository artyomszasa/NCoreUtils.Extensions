namespace NCoreUtils.Google.Drive;

public interface IMinimalFileInfo
{
    public string? Kind { get; }

    public string? Id { get; }

    public string? Name { get; }

    public string? MimeType { get; }

    public bool? Trashed { get; }

    public DateTimeOffset? CreatedTime { get; }

    public DateTimeOffset? ModifiedTime { get; }
}
