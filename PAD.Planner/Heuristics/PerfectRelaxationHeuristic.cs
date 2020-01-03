using PAD.Planner.Search;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of the Perfect Delete Relaxation heuristic. The heuristic value is calculated as a cost of the optimal solution
    /// path from the given state/conditions in the corresponding relaxed planning problem.
    /// </summary>
    public class PerfectRelaxationHeuristic : Heuristic
    {
        /// <summary>
        /// Relaxed planning problem.
        /// </summary>
        private IRelaxedProblem RelaxedProblem { get; }

        /// <summary>
        /// Heuristic forward search procedure.
        /// </summary>
        private IHeuristicSearch HeuristicSearch { get; }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="problem">Original planning problem.</param>
        public PerfectRelaxationHeuristic(IProblem problem) : base(problem)
        {
            RelaxedProblem = problem.GetRelaxedProblem();
            HeuristicSearch = new AStarSearch(RelaxedProblem, new StateSizeHeuristic());
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            RelaxedProblem.SetInitialState(state);
            HeuristicSearch.Start();
            return HeuristicSearch.GetSolutionCost();
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            RelaxedProblem.SetGoalConditions(conditions);
            HeuristicSearch.Start();
            return HeuristicSearch.GetSolutionCost();
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return "Perfect Relaxation Heuristic";
        }
    }
}
