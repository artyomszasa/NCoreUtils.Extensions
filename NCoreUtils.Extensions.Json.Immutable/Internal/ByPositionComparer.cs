using System.Collections.Generic;

namespace NCoreUtils.Internal
{
    internal sealed class ByPositionComparer : IComparer<LinkedProperty>
    {
        private static readonly Comparer<int> IntComparer = Comparer<int>.Default;

        public static ByPositionComparer Instance { get; } = new ByPositionComparer();

        public int Compare(LinkedProperty? x, LinkedProperty? y)
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