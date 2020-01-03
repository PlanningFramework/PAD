using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Implementation of a single successor, i.e. a forward application of a grounded operator to a state (generated on-demand).
    /// </summary>
    public class Successor : ISuccessor
    {
        /// <summary>
        /// Reference to the original state.
        /// </summary>
        private IState ReferenceState { get; }

        /// <summary>
        /// Grounded applied operator.
        /// </summary>
        private IOperator AppliedOperator { get; }

        /// <summary>
        /// Constructs the successor entity.
        /// </summary>
        /// <param name="referenceState">Reference to the original state.</param>
        /// <param name="appliedOperator">Grounded applied operator.</param>
        public Successor(IState referenceState, IOperator appliedOperator)
        {
            ReferenceState = referenceState;
            AppliedOperator = appliedOperator;
        }

        /// <summary>
        /// Gets the transition operator applied to the original state.
        /// </summary>
        /// <returns>Applied operator.</returns>
        public IOperator GetAppliedOperator()
        {
            return AppliedOperator;
        }

        /// <summary>
        /// Gets the successor state. Lazy evaluated.
        /// </summary>
        /// <returns>Successor state.</returns>
        public IState GetSuccessorState()
        {
            return AppliedOperator.Apply(ReferenceState);
        }

        /// <summary>
        /// Gets the result of actually preforming the transition, i.e. a new searchable node.
        /// </summary>
        /// <returns>Transition result.</returns>
        public ISearchNode GetTransitionResult()
        {
            return GetSuccessorState();
        }

        /// <summary>
        /// Checks if the transition is complex, i.e. the result of application is more than one ISearchable node.
        /// </summary>
        /// <returns>True if the transition is complex, false otherwise.</returns>
        public bool IsComplexTransition()
        {
            return false;
        }

        /// <summary>
        /// Gets the result of actually preforming the transition in case of complex transitions, i.e. new searchable nodes.
        /// </summary>
        /// <returns>Transition results.</returns>
        public IEnumerable<ISearchNode> GetComplexTransitionResults()
        {
            yield return GetTransitionResult();
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(ReferenceState, AppliedOperator);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            Successor other = obj as Successor;
            if (other == null)
            {
                return false;
            }

            return ReferenceState.Equals(other.ReferenceState) && AppliedOperator.Equals(other.AppliedOperator);
        }
    }
}
