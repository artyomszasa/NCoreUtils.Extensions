using System;
using System.Collections.Generic;

namespace NCoreUtils.Google
{
    public class GoogleObjectsPage
    {
        public IReadOnlyList<string> Prefixes { get; }

        public IReadOnlyList<GoogleObjectData> Items { get; }

        public GoogleObjectsPage(IReadOnlyList<string>? prefixes, IReadOnlyList<GoogleObjectData>? items)
        {
            Prefixes = prefixes ?? Array.Empty<string>();
            Items = items ?? Array.Empty<GoogleObjectData>();
        }
    }
}