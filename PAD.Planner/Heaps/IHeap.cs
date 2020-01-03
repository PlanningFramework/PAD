using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Common interface for non-generic search heaps, used in search algorithms.
    /// </summary>
    public interface IHeap : IHeap<double, ISearchNode>
    {
    }

    /// <summary>
    /// Common interface for generic (minimal) heaps, i.e. partially ordered collections with operations optimized for getting a minimum value.
    /// </summary>
    /// <typeparam name="Key">Key type.</typeparam>
    /// <typeparam name="Value">Value type.</typeparam>
    public interface IHeap<Key, Value> where Key : IComparable
    {
        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        void Add(Key key, Value value);

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        Value RemoveMin();

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        Key GetMinKey();

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        int GetSize();

        /// <summary>
        /// Clears the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        string GetName();
    }
}