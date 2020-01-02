using System.Collections.Generic;
using PAD.Planner.Heuristics;

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for PDDL/SAS+ states and conditions. Containing methods are used by the PDDL/SAS+ specific problems.
    /// </summary>
    public interface IStateOrConditions : ISearchNode
    {
        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        bool DetermineGoalNode(IProblem problem);

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        double DetermineHeuristicValue(IHeuristic heuristic);

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        IEnumerable<ITransition> DetermineTransitions(IProblem problem);
    }
}
