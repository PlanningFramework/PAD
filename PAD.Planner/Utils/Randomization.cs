using System.Collections.Generic;
using System;

namespace PAD.Planner
{
    /// <summary>
    /// Auxiliary class for advanced randomization support.
    /// </summary>
    public static class Randomization
    {
        /// <summary>
        /// Extension method for getting a random element of the enumerable collection. This function should have uniform
        /// probability distribution, while it needs only a single pass on the data collection.
        /// Author: Jon Skeet (at http://stackoverflow.com/).
        /// </summary>
        /// <typeparam name="T">Item type of the enumerable.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="random">Random number generator.</param>
        /// <returns>Random element of the collection.</returns>
        public static T RandomElement<T>(this IEnumerable<T> source, Random random)
        {
            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                ++count;
                if (random.Next(count) == 0)
                {
                    current = element;
                }
            }

            return current;
        }
    }
}
