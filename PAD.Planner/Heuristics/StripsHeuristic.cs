
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of a STRIPS heuristic. The heuristic value is calculated as a number of not accomplished goals count of the
    /// given state/conditions in the planning problem.
    /// </summary>
    public class StripsHeuristic : Heuristic
    {
        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        public StripsHeuristic(IProblem problem) : base(problem)
        {
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return Problem.GetNotAccomplishedGoalsCount(state);
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return Problem.GetNotAccomplishedGoalsCount(conditions);
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return "STRIPS Heuristic";
        }
    }
}
