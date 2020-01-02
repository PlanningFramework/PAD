using System.Collections.Generic;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of regular ternary heap, used in search algorithms.
    /// </summary>
    public class RegularTernaryHeap : RegularTernaryHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a regular ternary heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class RegularTernaryHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Tree structure stored in the linear collection.
        /// </summary>
        private IList<TreeNode> Tree { set; get; } = new List<TreeNode>();

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            TreeNode newNode = new TreeNode(value, key, Tree.Count);
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
            Tree = new List<TreeNode>();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Regular Ternary Heap";
        }

        /// <summary>
        /// Is the node a root?
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>True if the node a root, false otherwise.</returns>
        private bool IsRoot(TreeNode node)
        {
            return (node.Index == 0);
        }

        /// <summary>
        /// Is the node a leaf?
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>True if the node a leaf, false otherwise.</returns>
        private bool IsLeaf(TreeNode node)
        {
            return GetLeftSuccesor(node) == null;
        }

        /// <summary>
        /// Gets the node predecessor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Node predecessor, if exists.</returns>
        private TreeNode GetPredecessor(TreeNode node)
        {
            return (node.Index == 0) ? null : Tree[(node.Index - 1) / 3];
        }

        /// <summary>
        /// Gets the left successor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Left successor, if exists.</returns>
        private TreeNode GetLeftSuccesor(TreeNode node)
        {
            int index = node.Index * 3 + 1;
            return (Tree.Count > index) ? Tree[index] : null;
        }

        /// <summary>
        /// Gets the middle successor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Middle successor, if exists.</returns>
        private TreeNode GetMiddleSuccesor(TreeNode node)
        {
            int index = node.Index * 3 + 2;
            return (Tree.Count > index) ? Tree[index] : null;
        }

        /// <summary>
        /// Gets the right successor.
        /// </summary>
        /// <param name="node">Tree node.</param>
        /// <returns>Right successor, if exists.</returns>
        private TreeNode GetRightSuccesor(TreeNode node)
        {
            int index = node.Index * 3 + 3;
            return (Tree.Count > index) ? Tree[index] : null;
        }

        /// <summary>
        /// Is the collection empty?
        /// </summary>
        /// <returns>True, if the collection is empty.</returns>
        private bool IsEmpty()
        {
            return (Tree.Count == 0);
        }

        /// <summary>
        /// Auxiliary "up" operation.
        /// </summary>
        /// <param name="node">Tree node.</param>
        private void Up(TreeNode node)
        {
            TreeNode current = node;
            TreeNode predecessor = GetPredecessor(current);

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
        private void Down(TreeNode node)
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
        private void Swap(TreeNode current, TreeNode predecessor)
        {
            TreeNode stored = Tree[current.Index];

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
        private TreeNode GetSmallestSuccesor(TreeNode current)
        {
            TreeNode result = null;
            TreeNode left = GetLeftSuccesor(current);
            if (left == null)
            {
                return null;
            }

            TreeNode middle = GetMiddleSuccesor(current);
            if (middle == null)
            {
                return left;
            }

            result = (middle.Key < left.Key) ? middle : left;

            TreeNode right = GetRightSuccesor(current);
            if (right == null)
            {
                return result;
            }

            result = (result.Key < right.Key) ? result : right;
            return result;
        }

        /// <summary>
        /// Tree node used in the heap collection.
        /// </summary>
        private class TreeNode
        {
            /// <summary>
            /// Value item.
            /// </summary>
            public Value Value { get; set; } = default(Value);

            /// <summary>
            /// Key item.
            /// </summary>
            public double Key { get; set; } = 0.0;

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
            public TreeNode(Value value, double key, int index)
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
