using System.Collections.Generic;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of sorted set heap.
    /// </summary>
    public class SortedSetHeap : SortedSetHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a sorted set heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class SortedSetHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// ID holder, for a generation of unique keys.
        /// </summary>
        private uint IdHolder { set; get; }

        /// <summary>
        /// Sorted set container.
        /// </summary>
        private SortedSet<KeyValuePair<KeyIdPair, Value>> SortedSet { get; } = new SortedSet<KeyValuePair<KeyIdPair, Value>>(new KeyValueComparer<Value>());

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            SortedSet.Add(new KeyValuePair<KeyIdPair, Value>(new KeyIdPair(key, IdHolder++), value));
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            var min = SortedSet.Min;
            SortedSet.Remove(min);
            return min.Value;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return SortedSet.Min.Key.Key;
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return SortedSet.Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            SortedSet.Clear();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Sorted Set Heap";
        }

        /// <summary>
        /// Key-ID pair used in the sorted set collection (to allow duplicated keys).
        /// </summary>
        private struct KeyIdPair : IComparable
        {
            /// <summary>
            /// Key.
            /// </summary>
            public readonly double Key;

            /// <summary>
            /// ID.
            /// </summary>
            public readonly uint Id;

            /// <summary>
            /// Constructs the Key-ID pair.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="id">ID.</param>
            public KeyIdPair(double key, uint id)
            {
                Key = key;
                Id = id;
            }

            /// <summary>
            /// Compares two Key-ID pairs.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                if (obj is KeyIdPair)
                {
                    KeyIdPair p = (KeyIdPair)obj;
                    return Key.Equals(p.Key) ? (int)(Id - p.Id) : Math.Sign(Key - p.Key);
                }
                return 0;
            }
        }

        /// <summary>
        /// Comparer for the sorted set collection.
        /// </summary>
        /// <typeparam name="V">Value type.</typeparam>
        private class KeyValueComparer<V> : IComparer<KeyValuePair<KeyIdPair, V>>
        {
            /// <summary>
            /// Compares two items of the collection.
            /// </summary>
            /// <param name="first">First item.</param>
            /// <param name="second">Second item.</param>
            /// <returns></returns>
            public int Compare(KeyValuePair<KeyIdPair, V> first, KeyValuePair<KeyIdPair, V> second)
            {
                return first.Key.Key.Equals(second.Key.Key) ? (int)(first.Key.Id - second.Key.Id) : Math.Sign(first.Key.Key - second.Key.Key);
            }
        }
    }
}
