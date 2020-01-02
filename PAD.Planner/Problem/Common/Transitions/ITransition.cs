using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// General interface for a generic transition.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Gets the transition operator applied to the original state/conditions.
        /// </summary>
        /// <returns>Applied operator.</returns>
        IOperator GetAppliedOperator();

        /// <summary>
        /// Gets the result of actually preforming the transition, i.e. a new searchable node.
        /// </summary>
        /// <returns>Transition result.</returns>
        ISearchNode GetTransitionResult();

        /// <summary>
        /// Checks if the transition is complex, i.e. the result of application is more than one ISearchable node.
        /// </summary>
        /// <returns>True if the transition is complex, false otherwise.</returns>
        bool IsComplexTransition();

        /// <summary>
        /// Gets the result of actually preforming the transition in case of complex transitions, i.e. new searchable nodes.
        /// </summary>
        /// <returns>Transition results.</returns>
        IEnumerable<ISearchNode> GetComplexTransitionResults();
    }
}
