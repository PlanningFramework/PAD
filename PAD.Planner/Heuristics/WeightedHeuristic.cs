
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of a weighted heuristic. The heuristic value is calculated from the parent heuristic function multiplied
    /// by the specified weight.
    /// </summary>
    public class WeightedHeuristic : Heuristic
    {
        /// <summary>
        /// Actually used heuristic.
        /// </summary>
        private IHeuristic Heuristic { get; }

        /// <summary>
        /// Used weight.
        /// </summary>
        private int Weight { get; }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="heuristic">Used heuristic.</param>
        /// <param name="weight">Used weight.</param>
        public WeightedHeuristic(IHeuristic heuristic, int weight)
        {
            Heuristic = heuristic;
            Weight = weight;
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return Weight * Heuristic.GetValue(state);
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return Weight * Heuristic.GetValue(conditions);
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        protected override double GetValueImpl(IRelativeState relativeState)
        {
            return Weight * Heuristic.GetValue(relativeState);
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return $"Weighted {Heuristic.GetName()} (weight = {Weight})";
        }
    }
}
