using System;
using System.Collections.Generic;

namespace NCoreUtils.Google;

public readonly struct GoogleObjectsPage
{
    private readonly IReadOnlyList<string>? _prefixes;

    private readonly IReadOnlyList<GoogleObjectData>? _items;

    public IReadOnlyList<string> Prefixes => _prefixes ?? Array.Empty<string>();

    public IReadOnlyList<GoogleObjectData> Items => _items ?? Array.Empty<GoogleObjectData>();

    public GoogleObjectsPage(IReadOnlyList<string>? prefixes, IReadOnlyList<GoogleObjectData>? items)
    {
        _prefixes = prefixes;
        _items = items;
    }
}