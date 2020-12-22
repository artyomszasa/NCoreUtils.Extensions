using System.Collections.Generic;

namespace NCoreUtils.Google
{
    public class GoogleObjectsPage
    {
        private static readonly IReadOnlyList<string> _noPrefixes = new string[0];

        private static readonly IReadOnlyList<GoogleObjectData> _noItems = new GoogleObjectData[0];

        public IReadOnlyList<string> Prefixes { get; }

        public IReadOnlyList<GoogleObjectData> Items { get; }

        public GoogleObjectsPage(IReadOnlyList<string>? prefixes, IReadOnlyList<GoogleObjectData>? items)
        {
            Prefixes = prefixes ?? _noPrefixes;
            Items = items ?? _noItems;
        }
    }
}