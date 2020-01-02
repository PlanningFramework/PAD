using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a radix heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class RadixHeap<Value> : IHeap<int, Value>
    {
        /// <summary>
        /// Buckets collections.
        /// </summary>
        private List<TreeNode<int, Value>>[] Buckets { set; get; } = null;

        /// <summary>
        /// Bounds array.
        /// </summary>
        private int[] Bounds { set; get; } = null;

        /// <summary>
        /// Range of the elements.
        /// Must be greater than the difference between the highest possible key and the lowest possible key!
        /// </summary>
        private int C { set; get; } = 0;

        /// <summary>
        /// Size of the hash array. Depends on C.
        /// </summary>
        private int B { set; get; } = 0;

        /// <summary>
        /// Number of elements in the structure.
        /// </summary>
        private int Count { set; get; } = 0;

        /// <summary>
        /// Constructs the radix heap.
        /// </summary>
        /// <param name="range">Range of elements.</param>
        public RadixHeap(int range)
        {
            C = range;
            Clear();
        }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(int key, Value value)
        {
            int i = (B - 1);
            while (Bounds[i] > key)
            {
                --i;
            }
            Buckets[i].Add(new TreeNode<int, Value>(value, key));
            Count++;
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            int i = 0, j = 0;
            while (Buckets[i].Count == 0)
            {
                ++i;
            }

            int minkey = Buckets[i][0].Key;
            int minIndex = 0;

            for (j = 1; j < Buckets[i].Count; j++)
            {
                if (Buckets[i][j].Key < minkey)
                {
                    minkey = Buckets[i][j].Key;
                    minIndex = j;
                }
            }

            Value minValue = Buckets[i][minIndex].Value;
            Buckets[i].RemoveAt(minIndex);
            --Count;

            if (Count == 0)
            {
                return minValue;
            }

            while (Buckets[i].Count == 0)
            {
                ++i;
            }

            if (i > 0)
            {
                int k = Buckets[i].Min(a => a.Key);
                Bounds[0] = k;
                Bounds[1] = k + 1;

                for (j = 2; j < i + 1; j++)
                {
                    Bounds[j] = Min(Bounds[j - 1] + 1 << (j - 2), Bounds[i + 1]);
                }

                while (Buckets[i].Count > 0)
                {
                    j = 0;
                    var el = Buckets[i][Buckets[i].Count - 1];
                    Buckets[i].RemoveAt(Buckets[i].Count - 1);
                    while (el.Key > Bounds[j + 1])
                    {
                        ++j;
                    }
                    Buckets[j].Add(el);
                }
            }
            return minValue;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public int GetMinKey()
        {
            int i = 0;
            while (Buckets[i].Count == 0)
            {
                ++i;
            }

            return Buckets[i][0].Key;
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            B = (int)Math.Ceiling(Math.Log(C + 1, 2) + 2);
            Count = 0;

            Buckets = new List<TreeNode<int, Value>>[B];
            Bounds = new int[B];
            for (int i = 0; i < B; ++i)
            {
                Buckets[i] = new List<TreeNode<int, Value>>();
            }

            Bounds[0] = 0;
            Bounds[1] = 1;
            int exp = 1;
            for (int i = 2; i < B; ++i)
            {
                Bounds[i] = Bounds[i - 1] + exp;
                exp *= 2;
            }
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Radix Heap";
        }

        /// <summary>
        /// Gets the min value.
        /// </summary>
        /// <param name="a">First argument.</param>
        /// <param name="b">Second argument.</param>
        /// <returns>The lesser of two values.</returns>
        private int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// Tree node to be used in the buckets collection.
        /// </summary>
        /// <typeparam name="KeyType">Key type.</typeparam>
        /// <typeparam name="ValueType">Value type.</typeparam>
        private class TreeNode<KeyType, ValueType> where KeyType : IComparable
        {
            /// <summary>
            /// Value item.
            /// </summary>
            public ValueType Value { get; set; } = default(ValueType);

            /// <summary>
            /// Key item.
            /// </summary>
            public KeyType Key { get; set; } = default(KeyType);

            /// <summary>
            /// Constructs the tree node.
            /// </summary>
            /// <param name="value">Value item.</param>
            /// <param name="key">Key item.</param>
            public TreeNode(ValueType value, KeyType key)
            {
                Value = value;
                Key = key;
            }

            /// <summary>
            /// String representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return $"Key: {Key}, value: {Value}";
            }
        }
    }
}
