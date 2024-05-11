namespace NCoreUtils.Google;

public readonly struct GoogleObjectsPage(IReadOnlyList<string>? prefixes, IReadOnlyList<GoogleObjectData>? items)
{
    private readonly IReadOnlyList<string>? _prefixes = prefixes;

    private readonly IReadOnlyList<GoogleObjectData>? _items = items;

    public IReadOnlyList<string> Prefixes => _prefixes ?? [];

    public IReadOnlyList<GoogleObjectData> Items => _items ?? [];
}