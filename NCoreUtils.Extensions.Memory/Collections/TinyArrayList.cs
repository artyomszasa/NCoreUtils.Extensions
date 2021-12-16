using System.Collections.Generic;

namespace NCoreUtils.Collections
{
    public struct TinyArrayList<T>
    {
        private InlineArray3<T> _inlineData;

        private List<T>? _data;

        public int Count { get; private set; }

        public TinyArrayList()
        {
            _inlineData = default;
            _data = default;
            Count = 0;
        }

        public TinyArrayList(T item)
        {
            _inlineData = default;
            _inlineData.First = item;
            _data = default;
            Count = 1;
        }

        public TinyArrayList(T item1, T item2)
        {
            _inlineData = default;
            _inlineData.First = item1;
            _inlineData.Second = item2;
            _data = default;
            Count = 2;
        }

        public TinyArrayList(T item1, T item2, T item3)
        {
            _inlineData = default;
            _inlineData.First = item1;
            _inlineData.Second = item2;
            _inlineData.Second = item3;
            _data = default;
            Count = 3;
        }
    }
}