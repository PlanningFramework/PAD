using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a single bucket heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class SingleBucketHeap<Value> : IHeap<int, Value>
    {
        /// <summary>
        /// Buckets collections.
        /// </summary>
        private List<TreeNode<int, Value>>[] Buckets { set; get; } = null;

        /// <summary>
        /// Size of the hash array, i.e. range of the elements.
        /// Must be greater than the difference between the highest possible key and the lowest possible key!
        /// </summary>
        private int C { set; get; } = 0;

        /// <summary>
        /// Initial value of C.
        /// </summary>
        private int InitialC { set; get; } = 0;

        /// <summary>
        /// The minimum key stored in the structure.
        /// </summary>
        private int MinKey { set; get; } = int.MaxValue;

        /// <summary>
        /// Position of the minimum element.
        /// </summary>
        private int MinPos { set; get; } = -1;

        /// <summary>
        /// Number of elements in the structure.
        /// </summary>
        private int Count { set; get; } = 0;

        /// <summary>
        /// Constructs the single bucket heap.
        /// </summary>
        /// <param name="hashArraySize">Size of hash array.</param>
        public SingleBucketHeap(int hashArraySize)
        {
            C = hashArraySize;
            InitialC = C;

            Buckets = new List<TreeNode<int, Value>>[C + 1];
            for (int i = 0; i < C + 1; ++i)
            {
                Buckets[i] = new List<TreeNode<int, Value>>();
            }
        }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(int key, Value value)
        {
            while (Count > 0 && key > MinKey + C)
            {
                // insuficient heap limit - rebuilding the heap
                ReHashWithLargerSize();
            }

            ++Count;
            int pos = key % (C + 1);
            if (key < MinKey)
            {
                MinPos = pos;
                MinKey = key;
            }

            InsertTo(pos, key, value);
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            Value x = default(Value);
            for (int i = 0; i < Buckets[MinPos].Count; ++i)
            {
                if (Buckets[MinPos][i].Key == MinKey)
                {
                    x = Buckets[MinPos][i].Value;
                    Buckets[MinPos].RemoveAt(i);
                    break;
                }
            }

            Count = Count - 1;
            if (Count > 0)
            {
                while (Buckets[MinPos].Count == 0)
                {
                    MinPos = (MinPos + 1) % (C + 1);
                }
                MinKey = Buckets[MinPos].Min(a => a.Key);
            }
            else
            {
                MinKey = int.MaxValue;
            }

            return x;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public int GetMinKey()
        {
            return MinKey;
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
            C = InitialC;
            Count = 0;
            MinPos = -1;
            MinKey = int.MaxValue;

            Buckets = new List<TreeNode<int, Value>>[C + 1];
            for (int i = 0; i < C + 1; ++i)
            {
                Buckets[i] = new List<TreeNode<int, Value>>();
            }
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Single Bucket Heap";
        }

        /// <summary>
        /// Rehash current buckets.
        /// </summary>
        private void ReHashWithLargerSize()
        {
            var oldBuckets = Buckets;
            C = C * 2;
            if (C >= 32000000)
            {
                throw new OutOfMemoryException();
            }

            Count = 0;
            MinPos = -1;
            MinKey = int.MaxValue;
            Buckets = new List<TreeNode<int, Value>>[C + 1];

            for (int i = 0; i < C + 1; i++)
            {
                Buckets[i] = new List<TreeNode<int, Value>>();
            }

            foreach (var bucket in oldBuckets)
            {
                foreach (var item in bucket)
                {
                    Insert(item);
                }
            }
        }

        /// <summary>
        /// Auxiliary method for inserting an item into buckets.
        /// </summary>
        /// <param name="bucket">Bucket number.</param>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        private void InsertTo(int bucket, int key, Value value)
        {
            Buckets[bucket].Add(new TreeNode<int, Value>(value, key));
        }

        /// <summary>
        /// Auxiliary method for inserting an item into buckets.
        /// </summary>
        /// <param name="bucket">Bucket number.</param>
        /// <param name="node">Tree node.</param>
        private void InsertTo(int bucket, TreeNode<int, Value> node)
        {
            Buckets[bucket].Add(node);
        }

        /// <summary>
        /// Auxiliary method for inserting an item into buckets.
        /// </summary>
        /// <param name="node">Tree node.</param>
        private void Insert(TreeNode<int, Value> node)
        {
            ++Count;
            int pos = node.Key % (C + 1);
            if (node.Key < MinKey)
            {
                MinPos = pos;
                MinKey = node.Key;
            }
            InsertTo(pos, node);
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
