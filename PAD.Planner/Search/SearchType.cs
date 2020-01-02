
namespace PAD.Planner.Search
{
    /// <summary>
    /// Type of search in the heuristic search procedure.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// Classic forward search from an initial state of the planning problem, trying to fulfill goal conditions of the problem.
        /// States are used as search nodes.
        /// </summary>
        Forward,

        /// <summary>
        /// Backward search from goal conditions of the planning problem, trying to fulfill the initial state of the problem.
        /// Conditions are used as search nodes.
        /// </summary>
        BackwardWithConditions,

        /// <summary>
        /// Backward search from goal conditions of the planning problem, trying to fulfill the initial state of the problem.
        /// Relative states are used as search nodes.
        /// </summary>
        BackwardWithStates
    }
}
