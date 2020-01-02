using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a special kind of conditions, which are contradicted, i.e. are always evaluated as false for any state.
    /// These conditions are basically dead-ends in solution search, but they can be produced under some circumstances.
    /// </summary>
    public class ConditionsContradiction : IConditions
    {
        /// <summary>
        /// Constructs SAS+ conditions.
        /// </summary>
        public ConditionsContradiction()
        {
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(IState state)
        {
            return false;
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(Planner.IState state)
        {
            return false;
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">State labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return int.MaxValue;
        }

        /// <summary>
        /// Creates a conjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Conjunction of the current conditions and the given other conditions.</returns>
        public IConditions ConjunctionWith(IConditions other)
        {
            return (IConditions)Clone();
        }

        /// <summary>
        /// Creates a disjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Disjunction of the current conditions and the given other conditions.</returns>
        public IConditions DisjunctionWith(IConditions other)
        {
            return (IConditions)other.Clone();
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the other conditions (i.e. different constraints on the same variables).
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>True if the conditions are conflicted with the other conditions, false otherwise.</returns>
        public bool IsConflictedWith(IConditions other)
        {
            return true;
        }

        /// <summary>
        /// Removes the specified assignment constraint from the current conditions.
        /// </summary>
        /// <param name="assignment">Constraint assignment.</param>
        /// <returns>True if the constraint has been successfully removed, false otherwise.</returns>
        public bool RemoveConstraint(IAssignment assignment)
        {
            return false;
        }

        /// <summary>
        /// Checks whether the specified assignment is actually constrained by the conditions.
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the given assignment is constrained by the conditions, false otherwise.</returns>
        public bool IsConstrained(IAssignment assignment)
        {
            return false;
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the specified assignment (i.e. different constraints on the same variable).
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the conditions are conflicted with the given assignment, false otherwise.</returns>
        public bool IsConflictedWith(IAssignment assignment)
        {
            return true;
        }

        /// <summary>
        /// Evaluates the relavance of a single effect assignment.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        /// <returns>Effect relevance of the specified assignment.</returns>
        public EffectRelevance IsEffectAssignmentRelevant(IAssignment assignment)
        {
            return EffectRelevance.ANTI_RELEVANT;
        }

        /// <summary>
        /// Checks whether the conditions are compatible with the given mutually exclusion constraints.
        /// </summary>
        /// <param name="mutexConstraints">Mutex constraints.</param>
        /// <returns>True if the conditions is compatible with the specified mutex constraints, false otherwise.</returns>
        public bool IsCompatibleWithMutexContraints(IList<IAssignment> mutexConstraints)
        {
            return false;
        }

        /// <summary>
        /// Gets the collection of simple conditions, i.e. individual conjuncts in case of the clause conditions, or itself
        /// if the current conditions is already simple conditions variant.
        /// </summary>
        /// <returns>Collection of simple conditions.</returns>
        public IEnumerable<ISimpleConditions> GetSimpleConditions()
        {
            yield break;
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(IState state)
        {
            return int.MaxValue;
        }

        /// <summary>
        /// Enumerates all possible states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions.</returns>
        public IEnumerable<Planner.IState> GetCorrespondingStates(IProblem problem)
        {
            yield break;
        }

        /// <summary>
        /// Enumerates all possible relative states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible realtive states meeting the conditions.</returns>
        public IEnumerable<Planner.IRelativeState> GetCorrespondingRelativeStates(IProblem problem)
        {
            yield break;
        }

        /// <summary>
        /// Makes a deep copy of the conditions.
        /// </summary>
        /// <returns>Deep copy of the conditions.</returns>
        public Planner.IConditions Clone()
        {
            return new ConditionsContradiction();
        }

        /// <summary>
        /// Gets the conditions size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>Conditions size.</returns>
        public int GetSize()
        {
            return int.MinValue;
        }

        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public virtual bool DetermineGoalNode(IProblem problem)
        {
            return false;
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public virtual double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return double.MaxValue;
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public virtual IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            yield break; 
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "(false)";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("ConditionsContradiction");
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (obj == this || obj is ConditionsContradiction);
        }
    }
}
