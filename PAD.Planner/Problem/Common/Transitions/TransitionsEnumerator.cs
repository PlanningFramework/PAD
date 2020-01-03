using System.Collections.Generic;
using System;

namespace PAD.Planner
{
    /// <summary>
    /// Enumerator object for successive generation of forward/backward transitions of the planning problem. The main purpose is to allow
    /// generation of a limited number of transitions (specified by a user), while the next call continue from the last generated instance.
    /// To implement this we need to store (lazy evaluated) enumerators of forward/backward transitions for all the requested states/conditions.
    /// </summary>
    public class TransitionsEnumerator<Source, TTransition> where TTransition : ITransition
    {
        /// <summary>
        /// Collection of enumerators for the successive generation of forward/backward transitions. The dictionary key is a source state
        /// or conditions from which the transitions are generated.
        /// </summary>
        private Dictionary<Source, IEnumerator<TTransition>> EnumeratorsMap { get; } = new Dictionary<Source, IEnumerator<TTransition>>();

        /// <summary>
        /// Enumeration method getting a list with a limited number of possible transitions from the specified state/conditions. The next call
        /// of this method returns the new transitions, until all of them are returned - then an empty collection is returned to signalize the
        /// end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="source">Original state/conditions.</param>
        /// <param name="numberOfTransitions">Number of transitions to be returned.</param>
        /// <param name="generator">Generator function creating the actual collection of transitions.</param>
        /// <returns>Lazy generated collection of transitions.</returns>
        public IEnumerable<TTransition> GetNextTransitions(Source source, int numberOfTransitions, Func<Source, IEnumerable<TTransition>> generator)
        {
            IEnumerator<TTransition> enumerator;
            if (!EnumeratorsMap.TryGetValue(source, out enumerator))
            {
                enumerator = generator(source).GetEnumerator();
                EnumeratorsMap[source] = enumerator;
            }

            int transitionsGenerated = 0;
            while (transitionsGenerated < numberOfTransitions)
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                    ++transitionsGenerated;
                }
                else
                {
                    if (transitionsGenerated == 0)
                    {
                        // if the enumeration reached the end in the previous call of this method, we intentionally return an empty collection to
                        // signalize this fact before resetting the enumerator for the next call (alternatively, there are no transitions at all)
                        EnumeratorsMap.Remove(source);
                    }
                    yield break;
                }
            }
        }
    }
}
