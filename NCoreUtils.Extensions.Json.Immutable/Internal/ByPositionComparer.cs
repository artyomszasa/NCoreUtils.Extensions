using System.Collections.Generic;

namespace NCoreUtils.Internal
{
    internal sealed class ByPositionComparer : IComparer<LinkedProperty>
    {
        private static readonly Comparer<int> IntComparer = Comparer<int>.Default;

        public static ByPositionComparer Instance { get; } = new ByPositionComparer();

        public int Compare(LinkedProperty x, LinkedProperty y)
        {
            return IntComparer.Compare(x.Parameter.Position, y.Parameter.Position);
        }
    }
}