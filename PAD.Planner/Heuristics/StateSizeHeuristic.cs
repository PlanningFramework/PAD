
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of a (auxiliary) state size heuristic. The heuristic value is calculated from the size of the given
    /// state/conditions, i.e. the number of grounded fluents.
    /// </summary>
    public class StateSizeHeuristic : Heuristic
    {
        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        public StateSizeHeuristic() : base(null)
        {
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return 1.0 / state.GetSize();
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return 1.0 / conditions.GetSize();
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        protected override double GetValueImpl(IRelativeState relativeState)
        {
            return 1.0 / relativeState.GetSize();
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return "State Size Heuristic";
        }
    }
}
