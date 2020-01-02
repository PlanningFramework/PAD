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
        private static uint IDHolder { set; get; } = 0;

        /// <summary>
        /// Sorted set container.
        /// </summary>
        private SortedSet<KeyValuePair<KeyIDPair, Value>> SortedSet { set; get; } = new SortedSet<KeyValuePair<KeyIDPair, Value>>(new KeyValueComparer<Value>());

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            SortedSet.Add(new KeyValuePair<KeyIDPair, Value>(new KeyIDPair(key, IDHolder++), value));
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
            return SortedSet.Min.Key.key;
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
        private struct KeyIDPair : IComparable
        {
            /// <summary>
            /// Key.
            /// </summary>
            public double key;

            /// <summary>
            /// ID.
            /// </summary>
            public uint ID;

            /// <summary>
            /// Constructs the Key-ID pair.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="ID">ID.</param>
            public KeyIDPair(double key, uint ID)
            {
                this.key = key;
                this.ID = ID;
            }

            /// <summary>
            /// Compares two Key-ID pairs.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                if (obj is KeyIDPair)
                {
                    KeyIDPair p = (KeyIDPair)obj;
                    return (key == p.key) ? (int)(ID - p.ID) : Math.Sign(key - p.key);
                }
                return 0;
            }
        }

        /// <summary>
        /// Comparer for the sorted set collection.
        /// </summary>
        /// <typeparam name="V">Value type.</typeparam>
        private class KeyValueComparer<V> : IComparer<KeyValuePair<KeyIDPair, V>>
        {
            /// <summary>
            /// Compares two items of the collection.
            /// </summary>
            /// <param name="first">First item.</param>
            /// <param name="second">Second item.</param>
            /// <returns></returns>
            public int Compare(KeyValuePair<KeyIDPair, V> first, KeyValuePair<KeyIDPair, V> second)
            {
                return (first.Key.key == second.Key.key) ? (int)(first.Key.ID - second.Key.ID) : Math.Sign(first.Key.key - second.Key.key);
            }
        }
    }
}
