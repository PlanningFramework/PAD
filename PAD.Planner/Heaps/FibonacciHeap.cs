using System.Collections.Generic;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of fibonacci heap, used in search algorithms.
    /// </summary>
    public class FibonacciHeap : FibonacciHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a fibonacci heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class FibonacciHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Root nodes of the collection.
        /// </summary>
        private List<TreeNode> Root = new List<TreeNode>();

        /// <summary>
        /// Number of elements in the collection.
        /// </summary>
        private int Count { set; get; } = 0;

        /// <summary>
        /// Node with the minimal value.
        /// </summary>
        private TreeNode MinNode { set; get; } = null;

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            TreeNode node = new TreeNode { Key = key, Value = value };

            ++Count;
            Root.Add(node);

            if (MinNode == null)
            {
                MinNode = node;
            }
            else if (node.Key < MinNode.Key)
            {
                MinNode = node;
            }
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            if (MinNode == null)
            {
                throw new InvalidOperationException();
            }

            var minValueNode = MinNode;

            foreach (var child in MinNode.Children)
            {
                child.Parent = null;
                Root.Add(child);
            }
            Root.Remove(MinNode);

            if (Root.Count == 0)
            {
                MinNode = null;
            }
            else
            {
                MinNode = Root[0];
                Consolidate();
            }
            --Count;

            return minValueNode.Value;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            if (MinNode == null)
            {
                throw new InvalidOperationException();
            }
            return MinNode.Key;
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
            Root = new List<TreeNode>();
            Count = 0;
            MinNode = null;
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Fibonacci Heap (1)";
        }

        /// <summary>
        /// Consolidates the collection after manipulation.
        /// </summary>
        private void Consolidate()
        {
            var controlNode = new TreeNode[UpperBound()];
            for (int i = 0; i < Root.Count; ++i)
            {
                var rootNode = Root[i];
                var rootNodeChildCount = rootNode.Children.Count;
                while (true)
                {
                    var childNode = controlNode[rootNodeChildCount];
                    if (childNode == null)
                    {
                        break;
                    }

                    if (rootNode.Key > childNode.Key)
                    {
                        var tempNode = rootNode;
                        rootNode = childNode;
                        childNode = tempNode;
                    }
                    Root.Remove(childNode);
                    --i;

                    rootNode.AddChild(childNode);
                    childNode.Mark = false;
                    controlNode[rootNodeChildCount] = null;
                    ++rootNodeChildCount;
                }
                controlNode[rootNodeChildCount] = rootNode;
            }

            MinNode = null;
            for (int i = 0; i < controlNode.Length; ++i)
            {
                var controlNodeChild = controlNode[i];
                if (controlNodeChild == null)
                {
                    continue;
                }

                if (MinNode == null)
                {
                    Root.Clear();
                    MinNode = controlNodeChild;
                }
                else
                {
                    if (controlNodeChild.Key < MinNode.Key)
                    {
                        MinNode = controlNodeChild;
                    }
                }
                Root.Add(controlNodeChild);
            }
        }

        /// <summary>
        /// Upper bound for collection consolidations.
        /// </summary>
        /// <returns></returns>
        private int UpperBound()
        {
            return (int)Math.Floor(Math.Log(Count, (1.0 + Math.Sqrt(5)) / 2.0)) + 1;
        }

        /// <summary>
        /// Tree node used in the collection.
        /// </summary>
        private class TreeNode
        {
            /// <summary>
            /// Value of the node.
            /// </summary>
            public Value Value { get; set; } = default(Value);

            /// <summary>
            /// Key of the node.
            /// </summary>
            public double Key { get; set; } = 0.0;

            /// <summary>
            /// Parent node.
            /// </summary>
            public TreeNode Parent { get; set; } = null;

            /// <summary>
            /// Children nodes.
            /// </summary>
            public List<TreeNode> Children { get; set; } = new List<TreeNode>();

            /// <summary>
            /// Is node marked?
            /// </summary>
            public bool Mark { get; set; } = false;

            /// <summary>
            /// Adds the child node.
            /// </summary>
            /// <param name="child">Child node.</param>
            public void AddChild(TreeNode child)
            {
                child.Parent = this;
                Children.Add(child);
            }

            /// <summary>
            /// String representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return $"({Key},{Value})";
            }
        }
    }
}
