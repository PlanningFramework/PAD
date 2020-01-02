using System.Collections.Generic;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of regular binary heap, used in search algorithms.
    /// </summary>
    public class RegularBinaryHeap : RegularBinaryHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a regular binary heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class RegularBinaryHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Tree structure stored in the linear collection.
        /// </summary>
        private IList<TreeNode<double, Value>> Tree { set; get; } = new List<TreeNode<double, Value>>();

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            TreeNode<double, Value> newNode = new TreeNode<double, Value>(value, key, Tree.Count);
            Tree.Add(newNode);
            Up(newNode);
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            Value result = Tree[0].Value;
            Swap(Tree[0], Tree[Tree.Count - 1]);
            Tree.RemoveAt(Tree.Count - 1);

            if (!IsEmpty())
            {
                Down(Tree[0]);
            }
            return result;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return Tree[0].Key;
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return Tree.Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            Tree = new List<TreeNode<double, Value>>();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Regular Binary Heap";
        }

        /// <summary>
        /// Is the node a root?
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>True if the node a root, false otherwise.</returns>
        private bool IsRoot(TreeNode<double, Value> node)
        {
            return (node.Index == 0);
        }

        /// <summary>
        /// Is the node a leaf?
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>True if the node a leaf, false otherwise.</returns>
        private bool IsLeaf(TreeNode<double, Value> node)
        {
            return GetLeftSuccesor(node) == null;
        }

        /// <summary>
        /// Gets the node predecessor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Node predecessor, if exists.</returns>
        private TreeNode<double, Value> GetPredecessor(TreeNode<double, Value> node)
        {
            return (node.Index == 0) ? null : Tree[(node.Index - 1) / 2];
        }

        /// <summary>
        /// Gets the left successor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Left successor, if exists.</returns>
        private TreeNode<double, Value> GetLeftSuccesor(TreeNode<double, Value> node)
        {
            int index = node.Index * 2 + 1;
            return (Tree.Count > index) ? Tree[index] : null;
        }

        /// <summary>
        /// Gets the right successor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Right successor, if exists.</returns>
        private TreeNode<double, Value> GetRightSuccesor(TreeNode<double, Value> node)
        {
            int index = node.Index * 2 + 2;
            return (Tree.Count > index) ? Tree[index] : null;
        }

        /// <summary>
        /// Is the collection empty?
        /// </summary>
        /// <returns>True, if the collection is empty.</returns>
        public bool IsEmpty()
        {
            return (Tree.Count == 0);
        }

        /// <summary>
        /// Auxiliary "up" operation.
        /// </summary>
        /// <param name="node">Tree node.</param>
        private void Up(TreeNode<double, Value> node)
        {
            TreeNode<double, Value> current = node;
            TreeNode<double, Value> predecessor = GetPredecessor(current);

            while (!IsRoot(current) && current.Key < predecessor.Key)
            {
                Swap(current, predecessor);
                predecessor = GetPredecessor(current);
            }
        }

        /// <summary>
        /// Auxiliary "down" operation.
        /// </summary>
        /// <param name="node">Tree node.</param>
        private void Down(TreeNode<double, Value> node)
        {
            while (!IsLeaf(node))
            {
                var successor = GetSmallestSuccesor(node);
                if (successor.Key >= node.Key)
                {
                    break;
                }
                Swap(node, successor);
            }
        }

        /// <summary>
        /// Swaps the two tree nodes in the heap structure.
        /// </summary>
        /// <param name="current">Current node.</param>
        /// <param name="predecessor">Predecessor node.</param>
        private void Swap(TreeNode<double, Value> current, TreeNode<double, Value> predecessor)
        {
            TreeNode<double, Value> stored = Tree[current.Index];

            Tree[current.Index] = Tree[predecessor.Index];
            Tree[predecessor.Index] = stored;

            int storedIndex = current.Index;
            current.Index = predecessor.Index;
            predecessor.Index = storedIndex;
        }

        /// <summary>
        /// Gets the smallest successor from the available successors to the node.
        /// </summary>
        /// <param name="current">Current node.</param>
        /// <returns>Smallest successor to the specified node.</returns>
        private TreeNode<double, Value> GetSmallestSuccesor(TreeNode<double, Value> current)
        {
            TreeNode<double, Value> left = GetLeftSuccesor(current);
            if (left == null)
            {
                return null;
            }

            TreeNode<double, Value> right = GetRightSuccesor(current);
            if (right == null)
            {
                return left;
            }

            return (left.Key < right.Key) ? left : right;
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
            /// Node index.
            /// </summary>
            public int Index { get; set; } = 0;

            /// <summary>
            /// Constructs the tree node.
            /// </summary>
            /// <param name="value">Value item.</param>
            /// <param name="key">Key item.</param>
            /// <param name="index">Node index.</param>
            public TreeNode(ValueType value, KeyType key, int index)
            {
                Value = value;
                Key = key;
                Index = index;
            }

            /// <summary>
            /// String representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return $"Key: {Key}, index: {Index}";
            }
        }
    }
}
