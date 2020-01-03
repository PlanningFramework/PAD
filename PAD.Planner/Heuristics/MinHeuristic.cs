using System.Collections.Generic;
using System.Linq;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of a min heuristic. A given list of heuristics is evaluated, and the minimum from the heuristic values is returned.
    /// </summary>
    public class MinHeuristic : Heuristic
    {
        /// <summary>
        /// List of internal heuristics for evaluation.
        /// </summary>
        private IEnumerable<IHeuristic> Heuristics { get; }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="heuristics">List of heuristics.</param>
        public MinHeuristic(IEnumerable<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="heuristics">List of heuristics.</param>
        public MinHeuristic(params IHeuristic[] heuristics)
        {
            Heuristics = heuristics;
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return Heuristics.Min(heuristic => heuristic.GetValue(state));
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return Heuristics.Min(heuristic => heuristic.GetValue(conditions));
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        protected override double GetValueImpl(IRelativeState relativeState)
        {
            return Heuristics.Min(heuristic => heuristic.GetValue(relativeState));
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return $"Min Heuristic of ({string.Join(", ", Heuristics.Select(heuristic => heuristic.GetName()))})";
        }
    }
}
