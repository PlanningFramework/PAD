
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of the Max (Relaxation) heuristic. The heuristic value is calculated by forward cost evaluation of a
    /// corresponding relaxed planning graph by applying "max-rule" for action nodes.
    /// </summary>
    public class MaxRelaxationHeuristic : Heuristic
    {
        /// <summary>
        /// Relaxed planning graph.
        /// </summary>
        private IRelaxedPlanningGraph RelaxedPlanningGraph { get; }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="problem">Original planning problem.</param>
        public MaxRelaxationHeuristic(IProblem problem) : base(problem)
        {
            RelaxedPlanningGraph = problem.GetRelaxedProblem().GetRelaxedPlanningGraph();
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return RelaxedPlanningGraph.ComputeMaxForwardCost(state);
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return RelaxedPlanningGraph.ComputeMaxForwardCost(conditions);
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return "Max Relaxation Heuristic";
        }
    }
}
