using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of binomial heap, used in search algorithms.
    /// </summary>
    public class BinomialHeap : BinomialHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a binomial heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class BinomialHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Collection of binomial trees.
        /// </summary>
        private LinkedList<TreeNode> Trees { set; get; } = new LinkedList<TreeNode>();

        /// <summary>
        /// Number of items in the heap.
        /// </summary>
        private int Count { set; get; }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            LinkedListNode<TreeNode> newNode = new LinkedListNode<TreeNode>(new TreeNode(value, key, 0));
            Trees.AddFirst(newNode);
            ++Count;
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            if (Trees.First == null)
            {
                return default(Value);
            }

            TreeNode min = Trees.First.Value;
            List<TreeNode>[] byRank = new List<TreeNode>[(int)Math.Log(Count, 2) + 1];
            for (int i = 0; i < byRank.Length; i++)
            {
                byRank[i] = new List<TreeNode>();
            }

            foreach (TreeNode item in Trees)
            {
                if (item.Key < min.Key)
                {
                    min = item;
                }
                byRank[item.Rank].Add(item);
            }

            byRank[min.Rank].Remove(min);

            foreach (TreeNode item in min.Successors)
            {
                byRank[item.Rank].Add(item);
            }

            Trees = Repair(byRank);
            --Count;

            return min.Value;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return Trees.Min(a => a.Key);
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
            Count = 0;
            Trees = new LinkedList<TreeNode>();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Binomial Heap";
        }

        /// <summary>
        /// Auxiliary method for joining two tree nodes.
        /// </summary>
        /// <param name="first">First tree node.</param>
        /// <param name="second">Second tree node.</param>
        /// <returns>Joined tree root.</returns>
        private static TreeNode Join(TreeNode first, TreeNode second)
        {
            if (first.Key > second.Key)
            {
                return Join(second, first);
            }

            first.Successors.Add(second);
            first.Rank += 1;
            return first;
        }

        /// <summary>
        /// Repairs the list of binomial trees.
        /// </summary>
        /// <param name="list">List of binomial trees.</param>
        /// <returns>Repaired list of binomial trees.</returns>
        private static LinkedList<TreeNode> Repair(List<TreeNode>[] list)
        {
            LinkedList<TreeNode> result = new LinkedList<TreeNode>();
            for (int i = 0; i < list.Length; i++)
            {
                while (list[i].Count > 1)
                {
                    TreeNode first = list[i][0];
                    TreeNode second = list[i][1];
                    list[i].RemoveAt(1);
                    list[i].RemoveAt(0);
                    list[i + 1].Add(Join(first, second));
                }

                if (list[i].Count > 0)
                {
                    result.AddLast(new LinkedListNode<TreeNode>(list[i][0]));
                }
            }
            return result;
        }

        /// <summary>
        /// Tree node used in the binomial heap implementation.
        /// </summary>
        private class TreeNode
        {
            /// <summary>
            /// Value of the node.
            /// </summary>
            public Value Value { get; }

            /// <summary>
            /// Key of the node.
            /// </summary>
            public double Key { get; }

            /// <summary>
            /// Rank of the node.
            /// </summary>
            public int Rank { set; get; }

            /// <summary>
            /// Successor tree nodes.
            /// </summary>
            public List<TreeNode> Successors { get; } = new List<TreeNode>();

            /// <summary>
            /// Constructs the tree node of the binomial heap.
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="key">Key.</param>
            /// <param name="rank">Node rank.</param>
            public TreeNode(Value value, double key, int rank)
            {
                Value = value;
                Key = key;
                Rank = rank;
            }

            /// <summary>
            /// String representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return $"Key: {Key}";
            }
        }
    }
}
