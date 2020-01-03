using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Implementation of a direct transition, explicitly containing the transition result (while the applied operator
    /// does not have to be even present or is simply not needed).
    /// </summary>
    public class Transition : ITransition
    {
        /// <summary>
        /// Transition result.
        /// </summary>
        private ISearchNode TransitionResult { get; }

        /// <summary>
        /// Grounded applied operator.
        /// </summary>
        private IOperator AppliedOperator { get; }

        /// <summary>
        /// Constructs the transition entity directly with the transition result.
        /// </summary>
        /// <param name="transitionResult">Transition result node.</param>
        public Transition(ISearchNode transitionResult)
        {
            TransitionResult = transitionResult;
        }

        /// <summary>
        /// Constructs the transition entity with the transition result and the applied operator.
        /// </summary>
        /// <param name="transitionResult">Transition result node.</param>
        /// <param name="appliedOperator">Applied operator.</param>
        public Transition(ISearchNode transitionResult, IOperator appliedOperator)
        {
            TransitionResult = transitionResult;
            AppliedOperator = appliedOperator;
        }

        /// <summary>
        /// Gets the transition operator applied to the original state/conditions.
        /// </summary>
        /// <returns>Applied operator.</returns>
        public IOperator GetAppliedOperator()
        {
            return AppliedOperator;
        }

        /// <summary>
        /// Gets the result of actually preforming the transition, i.e. a new searchable node.
        /// </summary>
        /// <returns>Transition result.</returns>
        public ISearchNode GetTransitionResult()
        {
            return TransitionResult;
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
            yield break;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(TransitionResult, AppliedOperator);
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

            Transition other = obj as Transition;
            if (other == null)
            {
                return false;
            }

            if ((AppliedOperator != null && !AppliedOperator.Equals(other.AppliedOperator))
              || (AppliedOperator == null && other.AppliedOperator != null))
            {
                return false;
            }

            return TransitionResult.Equals(other.TransitionResult);
        }
    }
}
