using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of leftist heap, used in search algorithms.
    /// </summary>
    public class LeftistHeap : LeftistHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a leftist heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class LeftistHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Root tree node.
        /// </summary>
        private TreeNode<double, Value> Root { set; get; } = null;

        /// <summary>
        /// Number of elements.
        /// </summary>
        private int Count { set; get; } = 0;

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            TreeNode<double, Value> newNode = new TreeNode<double, Value>(value, key, 0);
            Root = Merge(Root, newNode);
            ++Count;
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            Value result = Root.Value;

            Root = Merge(Root.LeftSuccesor, Root.RightSuccesor);
            --Count;

            return result;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return Root.Key;
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
            Root = null;
            Count = 0;
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Leftist Heap";
        }

        /// <summary>
        /// Gets the NPL (null path length) for the given tree node.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>NPL of the specified tree node.</returns>
        private int GetNPL(TreeNode<double, Value> node)
        {
            return (node == null) ? -1 : node.NPL;
        }

        /// <summary>
        /// Merges two given trees.
        /// </summary>
        /// <param name="first">First tree.</param>
        /// <param name="second">Second tree.</param>
        /// <returns>Merged tree node.</returns>
        private TreeNode<double, Value> Merge(TreeNode<double, Value> first, TreeNode<double, Value> second)
        {
            if (first == null)
            {
                return second;
            }
            else if (second == null)
            {
                return first;
            }
            else if (first.Key > second.Key)
            {
                return Merge(second, first);
            }

            TreeNode<double, Value> newRight = Merge(first.RightSuccesor, second);
            first.RightSuccesor = newRight;
            newRight.Ancestor = first;

            if (GetNPL(first.RightSuccesor) > GetNPL(first.LeftSuccesor))
            {
                TreeNode<double, Value> stored = first.LeftSuccesor;
                first.LeftSuccesor = first.RightSuccesor;
                first.RightSuccesor = stored;
            }
            first.NPL = GetNPL(first.RightSuccesor) + 1;

            return first;
        }

        /// <summary>
        /// Tree node used in the heap collection.
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
            /// NPL (null path length).
            /// </summary>
            public int NPL { get; set; } = -1;
            
            /// <summary>
            /// Ancestor node.
            /// </summary>
            public TreeNode<KeyType, ValueType> Ancestor { set; get; } = null;

            /// <summary>
            /// Left successor node.
            /// </summary>
            public TreeNode<KeyType, ValueType> LeftSuccesor { set; get; } = null;

            /// <summary>
            /// Right successor node.
            /// </summary>
            public TreeNode<KeyType, ValueType> RightSuccesor { set; get; } = null;

            /// <summary>
            /// Constructs the tree node.
            /// </summary>
            /// <param name="value">Value item.</param>
            /// <param name="key">Key item.</param>
            /// <param name="index">Node index.</param>
            public TreeNode(ValueType value, KeyType key, int npl)
            {
                Value = value;
                Key = key;
                NPL = npl;
            }

            /// <summary>
            /// String representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return $"Key: {Key}, npl: {NPL}";
            }
        }
    }
}
