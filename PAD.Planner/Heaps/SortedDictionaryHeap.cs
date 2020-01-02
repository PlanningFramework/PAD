using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of sorted dictionary heap.
    /// </summary>
    public class SortedDictionaryHeap : SortedDictionaryHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a sorted dictionary heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class SortedDictionaryHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// ID holder, for a generation of unique keys.
        /// </summary>
        private static uint IDHolder { set; get; } = 0;

        /// <summary>
        /// Sorted dictionary container.
        /// </summary>
        private SortedDictionary<KeyIDPair, Value> Dictionary { set; get; } = new SortedDictionary<KeyIDPair, Value>(new KeyIdPairComparer());

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            Dictionary.Add(new KeyIDPair(key, IDHolder++), value);
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            var result = Dictionary.First();
            Dictionary.Remove(result.Key);
            return result.Value;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return Dictionary.First().Key.key;
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return Dictionary.Count;
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Sorted Dictionary Heap";
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            Dictionary.Clear();
        }

        /// <summary>
        /// Key-ID pair used in the sorted dictionary collection (to allow duplicated keys).
        /// </summary>
        private struct KeyIDPair
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
        }

        /// <summary>
        /// Comparer for the sorted dictionary collection.
        /// </summary>
        /// <typeparam name="V">Value type.</typeparam>
        private class KeyIdPairComparer : IComparer<KeyIDPair>
        {
            public int Compare(KeyIDPair first, KeyIDPair second)
            {
                return (first.key == second.key) ? (int)(first.ID - second.ID) : Math.Sign(first.key - second.key);
            }
        }
    }
}
