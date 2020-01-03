using System.Collections.Generic;
using System.Diagnostics;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of standard conditions (conjunction of constrains) in the SAS+ problem. Constraints are represented
    /// as a collection of required assignments, i.e. concrete values for specific variables.
    /// </summary>
    public class Conditions : HashSet<IAssignment>, ISimpleConditions
    {
        /// <summary>
        /// Constructs SAS+ conditions from other conditions.
        /// </summary>
        /// <param name="conditions">List of conditions.</param>
        public Conditions(Conditions conditions) : base(conditions)
        {
        }

        /// <summary>
        /// Constructs SAS+ conditions from the input data conditions.
        /// </summary>
        /// <param name="inputData">Input data conditions.</param>
        public Conditions(List<InputData.SAS.Assignment> inputData)
        {
            inputData.ForEach(assignment => Add(new Assignment(assignment)));
        }

        /// <summary>
        /// Constructs SAS+ conditions from a list of constraints.
        /// </summary>
        /// <param name="assignments">List of conditions.</param>
        public Conditions(IEnumerable<IAssignment> assignments) : base(assignments)
        {
        }

        /// <summary>
        /// Constructs SAS+ conditions from a list of constraints.
        /// </summary>
        /// <param name="assignments">List of conditions.</param>
        public Conditions(params IAssignment[] assignments) : base(assignments)
        {
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(Planner.IState state)
        {
            return Evaluate((IState)state);
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(IState state)
        {
            foreach (var assignment in this)
            {
                if (!state.HasValue(assignment))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">State labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            StateLabels labels = (StateLabels)stateLabels;

            double result = 0;
            foreach (var assignment in this)
            {
                double variableLabel = labels[assignment];
                if (evaluationStrategy == ForwardCostEvaluationStrategy.ADDITIVE_VALUE)
                {
                    result += variableLabel;
                }
                else
                {
                    result = Math.Max(result, variableLabel);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(IState state)
        {
            int count = 0;
            foreach (var assignment in this)
            {
                if (!state.HasValue(assignment))
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// Creates a conjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Conjunction of the current conditions and the given other conditions.</returns>
        public IConditions ConjunctionWith(IConditions other)
        {
            if (other is ConditionsClause || other is ConditionsContradiction)
            {
                return other.ConjunctionWith(this);
            }

            Conditions otherConditions = other as Conditions;
            Debug.Assert(otherConditions != null);

            if (IsConflictedWith(otherConditions))
            {
                return new ConditionsContradiction();
            }

            Conditions conditions = (Conditions)Clone();
            conditions.AddConditions(otherConditions);
            return conditions;
        }

        /// <summary>
        /// Adds the constraints from the specified conditions to the current conditions. This method performs no
        /// constraints conflict checking and should be used with caution.
        /// </summary>
        /// <param name="other">Conditions to be added.</param>
        private void AddConditions(Conditions other)
        {
            foreach (var assignment in other)
            {
                Add(assignment);
            }
        }

        /// <summary>
        /// Creates a disjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Disjunction of the current conditions and the given other conditions.</returns>
        public IConditions DisjunctionWith(IConditions other)
        {
            if (Count == 0)
            {
                // empty conditions always evaluate as true, so any disjunction with this conditions
                // would also be a tautology -> we don't create a clause in this case and just return 
                return (IConditions)Clone();
            }

            if (other is ConditionsClause || other is ConditionsContradiction)
            {
                return other.DisjunctionWith(this);
            }

            Debug.Assert(other is Conditions);
            return new ConditionsClause((Conditions)Clone(), (Conditions)other.Clone());
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the other conditions (i.e. different constraints on the same variables).
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>True if the conditions are conflicted with the other conditions, false otherwise.</returns>
        public bool IsConflictedWith(IConditions other)
        {
            foreach (var assignment in this)
            {
                if (other.IsConflictedWith(assignment))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the specified assignment constraint from the current conditions.
        /// </summary>
        /// <param name="assignment">Constraint assignment.</param>
        /// <returns>True if the constraint has been successfully removed, false otherwise.</returns>
        public bool RemoveConstraint(IAssignment assignment)
        {
            return (RemoveWhere(item => item.GetVariable() == assignment.GetVariable()) != 0);
        }

        /// <summary>
        /// Checks whether the specified assignment is actually constrained by the conditions.
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the given assignment is constrained by the conditions, false otherwise.</returns>
        public bool IsConstrained(IAssignment assignment)
        {
            return Contains(assignment);
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the specified assignment (i.e. different constraints on the same variable).
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the conditions are conflicted with the given assignment, false otherwise.</returns>
        public bool IsConflictedWith(IAssignment assignment)
        {
            int value;
            if (IsVariableConstrained(assignment.GetVariable(), out value))
            {
                // different constraint on the same variable -> conflict
                return (assignment.GetValue() != value);
            }
            return false;
        }

        /// <summary>
        /// Evaluates the relevance of a single effect assignment.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        /// <returns>Effect relevance of the specified assignment.</returns>
        public EffectRelevance IsEffectAssignmentRelevant(IAssignment assignment)
        {
            int value;
            if (IsVariableConstrained(assignment.GetVariable(), out value))
            {
                return (assignment.GetValue() == value) ? EffectRelevance.RELEVANT : EffectRelevance.ANTI_RELEVANT;
            }
            return EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Checks whether the conditions are compatible with the given mutually exclusion constraints.
        /// </summary>
        /// <param name="mutexConstraints">Mutex constraints.</param>
        /// <returns>True if the conditions is compatible with the specified mutex constraints, false otherwise.</returns>
        public bool IsCompatibleWithMutexConstraints(IList<IAssignment> mutexConstraints)
        {
            bool anyLocked = false;
            foreach (var constraint in mutexConstraints)
            {
                if (IsConstrained(constraint))
                {
                    if (anyLocked)
                    {
                        // some other constraint from the mutex group was marked earlier -> fail
                        return false;
                    }
                    else
                    {
                        // first matching constraint in the mutex group found -> mark it and continue
                        anyLocked = true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks whether the specified variable is actually constrained.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <returns>True if the given variable is constrained in the conditions.</returns>
        public bool IsVariableConstrained(int variable)
        {
            foreach (var assignment in this)
            {
                if (assignment.GetVariable() == variable)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether the specified variable is actually constrained. If the check is positive, returns also a constraining value.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Constraining value.</param>
        /// <returns>True if the given variable is constrained in the conditions, false otherwise.</returns>
        public bool IsVariableConstrained(int variable, out int value)
        {
            foreach (var assignment in this)
            {
                if (assignment.GetVariable() == variable)
                {
                    value = assignment.GetValue();
                    return true;
                }
            }
            value = Assignment.InvalidValue;
            return false;
        }

        /// <summary>
        /// Gets the list of assigned values, where order is given by variables. E.g. {(3, 5), (1, 6)} will generate list [6, 5].
        /// </summary>
        /// <returns>List of assigned values.</returns>
        public int[] GetAssignedValues()
        {
            int[] values = new int[Count];
            int i = 0;

            foreach (var assignment in this)
            {
                values[i++] = assignment.GetValue();
            }

            return values;
        }

        /// <summary>
        /// Gets the collection of simple conditions, i.e. individual conjuncts in case of the clause conditions, or itself
        /// if the current conditions is already simple conditions variant.
        /// </summary>
        /// <returns>Collection of simple conditions.</returns>
        public IEnumerable<ISimpleConditions> GetSimpleConditions()
        {
            yield return this;
        }

        /// <summary>
        /// Makes a deep copy of the conditions.
        /// </summary>
        /// <returns>Deep copy of the conditions.</returns>
        public Planner.IConditions Clone()
        {
            return new Conditions(this);
        }

        /// <summary>
        /// Gets the conditions size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>Conditions size.</returns>
        public int GetSize()
        {
            return Count;
        }

        /// <summary>
        /// Enumerates all possible states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions.</returns>
        public IEnumerable<Planner.IState> GetCorrespondingStates(IProblem problem)
        {
            return StatesEnumerator.EnumerateStates(this, ((Problem)problem).Variables);
        }

        /// <summary>
        /// Enumerates all possible relative states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible realtive states meeting the conditions.</returns>
        public IEnumerable<Planner.IRelativeState> GetCorrespondingRelativeStates(IProblem problem)
        {
            return StatesEnumerator.EnumerateRelativeStates(this, ((Problem)problem).Variables);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({string.Join(", ", this)})";
        }

        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public bool DetermineGoalNode(IProblem problem)
        {
            return problem.IsStartConditions(this);
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return heuristic.GetValue(this);
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            return problem.GetPredecessors(this);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCodeForOrderNoMatterCollection(this).CombineHashCode("Conditions");
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

            Conditions other = obj as Conditions;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }
    }
}
