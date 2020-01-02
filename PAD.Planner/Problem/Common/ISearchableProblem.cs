using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Topmost interface for planning problems capable of being evaluated by search algorithms. Operates only with abstracted search nodes.
    /// </summary>
    public interface ISearchableProblem
    {
        /// <summary>
        /// Gets the initial node of the planning problem.
        /// </summary>
        /// <returns>Initial node.</returns>
        ISearchNode GetInitialNode();

        /// <summary>
        /// Checks whether the specified node is meeting goal conditions of the planning problem.
        /// </summary>
        /// <param name="node">Node to be checked.</param>
        /// <returns>True if the given node is a goal node of the problem, false otherwise.</returns>
        bool IsGoalNode(ISearchNode node);

        /// <summary>
        /// Gets an enumerator of all possible transitions from the specified node. Lazy generated via yield return.
        /// </summary>
        /// <param name="node">Original node.</param>
        /// <returns>Lazy generated collection of transitions.</returns>
        IEnumerable<ITransition> GetTransitions(ISearchNode node);

        /// <summary>
        /// Gets the planning problem name.
        /// </summary>
        /// <returns>The name of the planning problem.</returns>
        string GetProblemName();
    }
}
