using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of a weighted sum heuristic.
    /// </summary>
    public class WeightedSumHeuristic : Heuristic
    {
        /// <summary>
        /// List of internal heuristics for evaluation with their corresponding weights.
        /// </summary>
        private List<Tuple<IHeuristic, double>> Heuristics { get; }

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random { get; } = new Random(123);

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="heuristics">List of heuristics with their corresponding weights.</param>
        /// <param name="normalize">Normalize the weights?</param>
        public WeightedSumHeuristic(List<Tuple<IHeuristic, double>> heuristics, bool normalize = true)
        {
            Heuristics = heuristics;

            if (normalize)
            {
                double sumOfWeights = Heuristics.Sum(heuristic => heuristic.Item2);
                Heuristics = Heuristics.Select(heuristic => Tuple.Create(heuristic.Item1, heuristic.Item2 / sumOfWeights)).ToList();
            }
        }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="normalize">Normalize the weights?</param>
        /// <param name="heuristics">List of heuristics with their corresponding weights.</param>
        public WeightedSumHeuristic(bool normalize, params Tuple<IHeuristic, double>[] heuristics) : this(heuristics.ToList(), normalize)
        {
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            if (Random.NextDouble() < Heuristics.First().Item2)
            {
                return Heuristics.First().Item1.GetValue(state);
            }
            return Heuristics.Sum(heuristic => heuristic.Item2 * heuristic.Item1.GetValue(state));
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            if (Random.NextDouble() < Heuristics.First().Item2)
            {
                return Heuristics.First().Item1.GetValue(conditions);
            }
            return Heuristics.Sum(heuristic => heuristic.Item2 * heuristic.Item1.GetValue(conditions));
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        protected override double GetValueImpl(IRelativeState relativeState)
        {
            if (Random.NextDouble() < Heuristics.First().Item2)
            {
                return Heuristics.First().Item1.GetValue(relativeState);
            }
            return Heuristics.Sum(heuristic => heuristic.Item2 * heuristic.Item1.GetValue(relativeState));
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            var heuristicsDescription = Heuristics.Select(heuristic => $"({heuristic.Item2} * {heuristic.Item1.GetName()})");
            return $"Weighted Sum Heuristic of ({string.Join(", ", heuristicsDescription)})";
        }
    }
}
