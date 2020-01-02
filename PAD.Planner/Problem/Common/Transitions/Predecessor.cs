using System.Collections.Generic;
using System.Linq;

namespace PAD.Planner
{
    /// <summary>
    /// Implementation of a single predecessor, i.e. a backwards application of a grounded operator to conditions (generated on-demand).
    /// </summary>
    public class Predecessor : IPredecessor
    {
        /// <summary>
        /// Reference to the original conditions.
        /// </summary>
        private IConditions ReferenceConditions { set; get; } = null;

        /// <summary>
        /// Reference to the original relative state.
        /// </summary>
        private IRelativeState ReferenceRelativeState { set; get; } = null;

        /// <summary>
        /// Grounded applied operator.
        /// </summary>
        private IOperator AppliedOperator { set; get; } = null;

        /// <summary>
        /// Constructs the predecessor entity from conditions.
        /// </summary>
        /// <param name="referenceConditions">Reference to the original conditions.</param>
        /// <param name="appliedOperator">Grounded applied operator.</param>
        public Predecessor(IConditions referenceConditions, IOperator appliedOperator)
        {
            ReferenceConditions = referenceConditions;
            AppliedOperator = appliedOperator;
        }

        /// <summary>
        /// Constructs the predecessor entity from relative state.
        /// </summary>
        /// <param name="referenceRelativeState">Reference to the original relative state.</param>
        /// <param name="appliedOperator">Grounded applied operator.</param>
        public Predecessor(IRelativeState referenceRelativeState, IOperator appliedOperator)
        {
            ReferenceRelativeState = referenceRelativeState;
            AppliedOperator = appliedOperator;
        }

        /// <summary>
        /// Gets the transition operator backwards applied to the original conditions.
        /// </summary>
        /// <returns>Applied operator.</returns>
        public IOperator GetAppliedOperator()
        {
            return AppliedOperator;
        }

        /// <summary>
        /// Gets the predecessor conditions.
        /// </summary>
        /// <returns>Predecessor conditions.</returns>
        public IConditions GetPredecessorConditions()
        {
            if (ReferenceConditions != null)
            {
                return AppliedOperator.ApplyBackwards(ReferenceConditions);
            }
            throw new System.NotSupportedException("Predecessor has been created from IRelativeState, not IConditions. Use GetPredecessorRelativeState() method.");
        }

        /// <summary>
        /// Gets the predecessor relative states (can be more than one, if conditional effects are present).
        /// </summary>
        /// <returns>Predecessor relative states.</returns>
        public IEnumerable<IRelativeState> GetPredecessorRelativeStates()
        {
            if (ReferenceRelativeState != null)
            {
                return AppliedOperator.ApplyBackwards(ReferenceRelativeState);
            }
            throw new System.NotSupportedException("Predecessor has been created from IConditions, not IRelativeState. Use GetPredecessorConditions() method.");
        }

        /// <summary>
        /// Gets the collection of all possible predecessing states for the current backwards application.
        /// </summary>
        /// <param name="problem">Reference planning problem.</param>
        /// <returns>All possible state predecessors.</returns>
        public IEnumerable<IState> GetPredecessorStates(IProblem problem)
        {
            if (ReferenceConditions != null)
            {
                foreach (var state in GetPredecessorConditions().GetCorrespondingStates(problem))
                {
                    yield return state;
                }
            }
            else
            {
                foreach (var relativeState in GetPredecessorRelativeStates())
                {
                    foreach (var state in relativeState.GetCorrespondingStates(problem))
                    {
                        yield return state;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the result of actually preforming the transition, i.e. a new searchable node.
        /// </summary>
        /// <returns>Transition result.</returns>
        public ISearchNode GetTransitionResult()
        {
            if (ReferenceConditions != null)
            {
                return GetPredecessorConditions();
            }
            else
            {
                // only the first relative state is returned!
                return GetPredecessorRelativeStates().First();
            }
        }

        /// <summary>
        /// Checks if the transition is complex, i.e. the result of application is more than one ISearchable node.
        /// </summary>
        /// <returns>True if the transition is complex, false otherwise.</returns>
        public bool IsComplexTransition()
        {
            if (ReferenceConditions != null)
            {
                return false;
            }

            // Possible optimization for relative states: find out beforehand whether the applied operator can create more than one
            // predecessing relative states by its backwards application (= contains conditional effects, OR in preconditions etc.)
            return true;
        }

        /// <summary>
        /// Gets the result of actually preforming the transition in case of complex transitions, i.e. new searchable nodes.
        /// </summary>
        /// <returns>Transition results.</returns>
        public IEnumerable<ISearchNode> GetComplexTransitionResults()
        {
            if (ReferenceConditions != null)
            {
                yield return GetPredecessorConditions();
            }
            else
            {
                foreach (var result in GetPredecessorRelativeStates())
                {
                    yield return result;
                }
            }
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            if (ReferenceConditions != null)
            {
                return HashHelper.GetHashCode(ReferenceConditions, AppliedOperator);
            }
            else
            {
                return HashHelper.GetHashCode(ReferenceRelativeState, AppliedOperator);
            }
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

            Predecessor other = obj as Predecessor;
            if (other == null)
            {
                return false;
            }

            if (!AppliedOperator.Equals(other.AppliedOperator))
            {
                return false;
            }

            if (ReferenceConditions != null)
            {
                if (other.ReferenceConditions == null || !ReferenceConditions.Equals(other.ReferenceConditions))
                {
                    return false;
                }
            }
            else if (ReferenceRelativeState != null)
            {
                if (other.ReferenceRelativeState == null || !ReferenceRelativeState.Equals(other.ReferenceRelativeState))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
