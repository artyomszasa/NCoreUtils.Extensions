using System.Collections.Generic;

namespace NCoreUtils.Collections
{
    public ref struct ImmutableHeadArrayBuilder<T>
        where T : unmanaged
    {
        public T? Head { get; private set; }

        public List<T>? Tail { get; private set; }

        public int Count
        {
            get => Head.HasValue
                ? Tail is null ? 1 : Tail.Count + 1
                : 0;
        }

        public ImmutableHeadArrayBuilder(int capacity)
        {
            Head = default;
            Tail = capacity < 2 ? default : new List<T>(capacity - 1);
        }

        public void Add(T value)
        {
            if (Head.HasValue)
            {
                Tail ??= new List<T>();
                Tail.Add(value);
            }
            else
            {
                Head = value;
            }
        }

        public ImmutableHeadArray<T> Build()
            => Head.HasValue
                ? new ImmutableHeadArray<T>(Head.Value, (Tail is null || Tail.Count == 0) ? default : Tail.ToArray())
                : default;
    }
}