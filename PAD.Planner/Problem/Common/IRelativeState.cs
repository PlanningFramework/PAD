using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ relative state in the planning problem. Relative state is an extension of a standard state,
    /// representing a whole class of states. It is an alternative way to express conditions in the backwards planning (an alternative
    /// to the more general IConditions).
    /// </summary>
    public interface IRelativeState : IState
    {
        /// <summary>
        /// Checks whether the specified state is meeting conditions given by the relative state (i.e. belong to the corresponding class of states).
        /// </summary>
        /// <param name="state">State to be checked.</param>
        /// <returns>True if the state is meeting conditions of the relative state, false otherwise.</returns>
        bool Evaluate(IState state);

        /// <summary>
        /// Enumerates all possible states meeting the conditions of the current relative state (i.e. are in the same class of states).
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions of the relative state.</returns>
        IEnumerable<IState> GetCorrespondingStates(IProblem problem);
    }
}
