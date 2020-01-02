using System.Collections.Generic;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a non-generic version of simple queue.
    /// </summary>
    public class SimpleQueue : SimpleQueue<ISearchNode>, IHeap
    {
    }

    /// <summary>
    /// Implementation of a simple queue (not actually a heap, for special usage).
    /// </summary>
    /// <typeparam name="Value">Value type.</typeparam>
    public class SimpleQueue<Value> : IHeap<double, Value>
    {
        /// <summary>
        /// Internal collection for the values.
        /// </summary>
        private LinkedList<Value> Queue = new LinkedList<Value>();

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(double key, Value value)
        {
            Queue.AddLast(value);
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            Value item = Queue.First.Value;
            Queue.RemoveFirst();
            return item;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public double GetMinKey()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return Queue.Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            Queue.Clear();
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Simple Queue";
        }
    }
}
