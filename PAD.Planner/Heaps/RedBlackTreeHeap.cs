using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of red-black heap, used in search algorithms.
    /// </summary>
    public class RedBlackTreeHeap : RedBlackTreeHeap<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a red-black heap.
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class RedBlackTreeHeap<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Internal Wintellect collection.
        /// </summary>
        private Wintellect.PowerCollections.OrderedBag<Wintellect.PowerCollections.Pair<double, Value>> Collection { set; get; } = null;

        /// <summary>
        /// Constructs the heap.
        /// </summary>
        public RedBlackTreeHeap()
        {
            Collection = new Wintellect.PowerCollections.OrderedBag<Wintellect.PowerCollections.Pair<double, Value>>((a, b) => Math.Sign(a.First - b.First));
        }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            Collection.Add(new Wintellect.PowerCollections.Pair<double, Value>(key, value));
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            return Collection.RemoveFirst().Second;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            return Collection.GetFirst().First;
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return Collection.Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            Collection.Clear();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Red-Black Tree Heap";
        }
    }
}
