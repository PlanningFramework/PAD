using System.Collections.Generic;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of fibonacci heap, used in search algorithms.
    /// </summary>
    public class FibonacciHeap2 : FibonacciHeap2<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a fibonacci heap (alternative implementation).
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class FibonacciHeap2<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Minimal tree node.
        /// </summary>
        private TreeNode MinNode { set; get; }

        /// <summary>
        /// Number of elements in the collection.
        /// </summary>
        private int Count { set; get; }

        /// <summary>
        /// Constant value used for heap consolidation.
        /// </summary>
        private readonly double _oneOverLogPhi = 1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            var node = new TreeNode(value, key);

            // concatenate node into min list
            if (MinNode != null)
            {
                node.Left = MinNode;
                node.Right = MinNode.Right;
                MinNode.Right = node;
                node.Right.Left = node;

                if (key < MinNode.Key)
                {
                    MinNode = node;
                }
            }
            else
            {
                MinNode = node;
            }

            ++Count;
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            TreeNode minNode = MinNode;

            if (minNode != null)
            {
                int childrenCount = minNode.Degree;
                TreeNode oldMinChild = minNode.Child;

                while (childrenCount > 0)
                {
                    TreeNode tempRight = oldMinChild.Right;

                    oldMinChild.Left.Right = oldMinChild.Right;
                    oldMinChild.Right.Left = oldMinChild.Left;

                    oldMinChild.Left = MinNode;
                    oldMinChild.Right = MinNode.Right;
                    MinNode.Right = oldMinChild;
                    oldMinChild.Right.Left = oldMinChild;

                    oldMinChild = tempRight;
                    --childrenCount;
                }

                minNode.Left.Right = minNode.Right;
                minNode.Right.Left = minNode.Left;

                if (minNode == minNode.Right)
                {
                    MinNode = null;
                }
                else
                {
                    MinNode = minNode.Right;
                    Consolidate();
                }

                --Count;

                return minNode.Value;
            }

            throw new InvalidOperationException("Invalid operation on empty heap!");
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
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
            MinNode = null;
            Count = 0;
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Fibonacci Heap (2)";
        }

        /// <summary>
        /// Consolidates the heap structure after modifications.
        /// </summary>
        private void Consolidate()
        {
            int arraySize = ((int)Math.Floor(Math.Log(Count) * _oneOverLogPhi)) + 1;
            var degreeArray = new List<TreeNode>(arraySize);

            // initialize degree array
            for (var i = 0; i < arraySize; i++)
            {
                degreeArray.Add(null);
            }

            // find the number of root nodes
            var numRoots = 0;
            TreeNode nodeX = MinNode;

            if (nodeX != null)
            {
                numRoots++;
                nodeX = nodeX.Right;

                while (nodeX != MinNode)
                {
                    numRoots++;
                    nodeX = nodeX.Right;
                }

                // for each node in root list
                while (numRoots > 0)
                {
                    // access this node's degree
                    int degree = nodeX.Degree;
                    TreeNode next = nodeX.Right;

                    // and see if there's another of the same degree
                    while (true)
                    {
                        TreeNode nodeY = degreeArray[degree];
                        if (nodeY == null)
                        {
                            break;
                        }

                        // if there is, make one of the nodes a child of the other (do this based on the key value)
                        if (nodeX.Key > nodeY.Key)
                        {
                            TreeNode temp = nodeY;
                            nodeY = nodeX;
                            nodeX = temp;
                        }

                        // TreeNode<Value> newChild disappears from root list
                        Link(nodeY, nodeX);

                        // we've handled this degree, go to next one
                        degreeArray[degree] = null;
                        ++degree;
                    }

                    // save this node for later when we might encounter another of the same degree
                    degreeArray[degree] = nodeX;

                    // move forward through list
                    nodeX = next;
                    numRoots--;
                }
            }

            // set min to null (effectively losing the root list) and reconstruct the root list from the array entries
            MinNode = null;
            for (var i = 0; i < arraySize; i++)
            {
                TreeNode nodeY = degreeArray[i];
                if (nodeY == null)
                {
                    continue;
                }

                // we've got a live one, add it to root list
                if (MinNode != null)
                {
                    // first remove node from root list
                    nodeY.Left.Right = nodeY.Right;
                    nodeY.Right.Left = nodeY.Left;

                    // now add to root list, again
                    nodeY.Left = MinNode;
                    nodeY.Right = MinNode.Right;
                    MinNode.Right = nodeY;
                    nodeY.Right.Left = nodeY;

                    // check if this is a new min
                    if (nodeY.Key < MinNode.Key)
                    {
                        MinNode = nodeY;
                    }
                }
                else
                {
                    MinNode = nodeY;
                }
            }
        }

        /// <summary>
        /// Links two tree nodes (makes newChild a child of node newParent).
        /// </summary>
        private static void Link(TreeNode newChild, TreeNode newParent)
        {
            newChild.Left.Right = newChild.Right;
            newChild.Right.Left = newChild.Left;

            if (newParent.Child == null)
            {
                newParent.Child = newChild;
                newChild.Right = newChild;
                newChild.Left = newChild;
            }
            else
            {
                newChild.Left = newParent.Child;
                newChild.Right = newParent.Child.Right;
                newParent.Child.Right = newChild;
                newChild.Right.Left = newChild;
            }

            ++newParent.Degree;
        }

        /// <summary>
        /// Tree node used in the heap collection.
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
            /// Child node.
            /// </summary>
            public TreeNode Child { get; set; }

            /// <summary>
            /// Left node.
            /// </summary>
            public TreeNode Left { get; set; }

            /// <summary>
            /// Right node.
            /// </summary>
            public TreeNode Right { get; set; }

            /// <summary>
            /// Degree of the node.
            /// </summary>
            public int Degree { get; set; }

            /// <summary>
            /// Constructs the tree node.
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="key">Key.</param>
            public TreeNode(Value value, double key)
            {
                Left = this;
                Right = this;
                Value = value;
                Key = key;
            }
        }
    }
}
