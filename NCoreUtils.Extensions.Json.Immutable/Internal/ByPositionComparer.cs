using System.Collections.Generic;

namespace NCoreUtils.Internal
{
    internal sealed class ByPositionComparer : IComparer<ILinkedProperty>
    {
        private static readonly Comparer<int> IntComparer = Comparer<int>.Default;

        public static ByPositionComparer Instance { get; } = new ByPositionComparer();

        public int Compare(ILinkedProperty? x, ILinkedProperty? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }
            if (y is null)
            {
                return 1;
            }
            return IntComparer.Compare(x.Parameter.Position, y.Parameter.Position);
        }
    }
}