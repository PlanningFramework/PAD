
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Common interface for PDDL or SAS+ heuristics.
    /// </summary>
    public interface IHeuristic : ISearchableHeuristic
    {
        /// <summary>
        /// Gets the heuristic statistics.
        /// </summary>
        /// <returns>Heuristic statistics.</returns>
        HeuristicStatistics GetStatistics();

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        double GetValue(IState state);

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        double GetValue(IConditions conditions);

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        double GetValue(IRelativeState relativeState);

        /// <summary>
        /// Gets a number of heuristic calls.
        /// </summary>
        /// <returns>Number of heuristic calls.</returns>
        long GetCallsCount();
    }
}

