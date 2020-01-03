using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// General interface for a predecessor (backwards transition).
    /// </summary>
    public interface IPredecessor : ITransition
    {
        /// <summary>
        /// Gets the predecessor conditions.
        /// </summary>
        /// <returns>Predecessor conditions.</returns>
        IConditions GetPredecessorConditions();

        /// <summary>
        /// Gets the predecessor relative states (can be more than one, if conditional effects are present).
        /// </summary>
        /// <returns>Predecessor relative states.</returns>
        IEnumerable<IRelativeState> GetPredecessorRelativeStates();

        /// <summary>
        /// Gets the collection of all possible preceding states for the current backwards application.
        /// </summary>
        /// <param name="problem">Reference planning problem.</param>
        /// <returns>All possible state predecessors.</returns>
        IEnumerable<IState> GetPredecessorStates(IProblem problem);
    }
}
