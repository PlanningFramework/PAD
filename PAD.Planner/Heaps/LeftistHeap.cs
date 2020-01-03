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
        private TreeNode<double> Root { set; get; }

        /// <summary>
        /// Number of elements.
        /// </summary>
        private int Count { set; get; }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            TreeNode<double> newNode = new TreeNode<double>(value, key, 0);
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

            Root = Merge(Root.LeftSuccessor, Root.RightSuccessor);
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
        private static int GetNPL(TreeNode<double> node)
        {
            return node?.NPL ?? -1;
        }

        /// <summary>
        /// Merges two given trees.
        /// </summary>
        /// <param name="first">First tree.</param>
        /// <param name="second">Second tree.</param>
        /// <returns>Merged tree node.</returns>
        private static TreeNode<double> Merge(TreeNode<double> first, TreeNode<double> second)
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

            TreeNode<double> newRight = Merge(first.RightSuccessor, second);
            first.RightSuccessor = newRight;

            if (GetNPL(first.RightSuccessor) > GetNPL(first.LeftSuccessor))
            {
                TreeNode<double> stored = first.LeftSuccessor;
                first.LeftSuccessor = first.RightSuccessor;
                first.RightSuccessor = stored;
            }
            first.NPL = GetNPL(first.RightSuccessor) + 1;

            return first;
        }

        /// <summary>
        /// Tree node used in the heap collection.
        /// </summary>
        /// <typeparam name="KeyType">Key type.</typeparam>
        private class TreeNode<KeyType> where KeyType : IComparable
        {
            /// <summary>
            /// Value item.
            /// </summary>
            public Value Value { get; }

            /// <summary>
            /// Key item.
            /// </summary>
            public KeyType Key { get; }

            /// <summary>
            /// NPL (null path length).
            /// </summary>
            public int NPL { get; set; }

            /// <summary>
            /// Left successor node.
            /// </summary>
            public TreeNode<KeyType> LeftSuccessor { set; get; }

            /// <summary>
            /// Right successor node.
            /// </summary>
            public TreeNode<KeyType> RightSuccessor { set; get; }

            /// <summary>
            /// Constructs the tree node.
            /// </summary>
            /// <param name="value">Value item.</param>
            /// <param name="key">Key item.</param>
            /// <param name="npl">NPL value.</param>
            public TreeNode(Value value, KeyType key, int npl)
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
