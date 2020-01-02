
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Topmost interface for heuristics capable of being evaluated by search algorithms. Evaluates only abstracted search nodes.
    /// </summary>
    public interface ISearchableHeuristic
    {
        /// <summary>
        /// Gets the heuristic value for the given search node.
        /// </summary>
        /// <param name="node">Node to be evaluated.</param>
        /// <returns>Heuristic value for the specified node.</returns>
        double GetValue(ISearchNode node);

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        string GetName();
    }
}
