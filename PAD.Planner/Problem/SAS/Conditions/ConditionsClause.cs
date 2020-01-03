using System.Collections.Generic;
using System.Diagnostics;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Special implementation of conditions in a form of a clause (disjunction) of conditions.
    /// </summary>
    public class ConditionsClause : HashSet<Conditions>, IConditions
    {
        /// <summary>
        /// Constructs conditions clause from conditions list.
        /// </summary>
        /// <param name="conditionsList">List of conditions.</param>
        public ConditionsClause(params Conditions[] conditionsList) : base(conditionsList)
        {
        }

        /// <summary>
        /// Constructs conditions clause from conditions list.
        /// </summary>
        /// <param name="conditionsList">List of conditions.</param>
        public ConditionsClause(IEnumerable<Conditions> conditionsList) : base(conditionsList)
        {
        }

        /// <summary>
        /// Adds the given conditions list to the clause.
        /// </summary>
        /// <param name="conditionsList">List of conditions.</param>
        private void Add(IEnumerable<Conditions> conditionsList)
        {
            foreach (var conditions in conditionsList)
            {
                Add(conditions);
            }
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(IState state)
        {
            foreach (var conditions in this)
            {
                if (conditions.Evaluate(state))
                {
                    return true;
                }
            }
            return false;
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
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">State labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            double result = 0;
            foreach (var conditions in this)
            {
                result = Math.Max(result, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, evaluationStrategy));
            }
            return result;
        }

        /// <summary>
        /// Creates a conjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Conjunction of the current conditions and the given other conditions.</returns>
        public IConditions ConjunctionWith(IConditions other)
        {
            List<Conditions> newConditions = new List<Conditions>();
            foreach (var conditions in this)
            {
                if (!conditions.IsConflictedWith(other))
                {
                    IConditions conjunction = conditions.ConjunctionWith(other);

                    ConditionsClause clause = conjunction as ConditionsClause;
                    if (clause != null)
                    {
                        foreach (var innerConditions in clause)
                        {
                            newConditions.Add(innerConditions);
                        }
                    }
                    else
                    {
                        Conditions innerConditions = conjunction as Conditions;
                        if (innerConditions != null)
                        {
                            newConditions.Add(innerConditions);
                        }
                    }
                }
            }

            switch (newConditions.Count)
            {
                case 0:
                    return new ConditionsContradiction();
                case 1:
                    return newConditions[0];
                default:
                    return new ConditionsClause(newConditions.ToArray());
            }
        }

        /// <summary>
        /// Creates a disjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Disjunction of the current conditions and the given other conditions.</returns>
        public IConditions DisjunctionWith(IConditions other)
        {
            if (other is ConditionsContradiction)
            {
                return (IConditions)Clone();
            }

            ConditionsClause newClause = (ConditionsClause)Clone();

            ConditionsClause otherClause = other as ConditionsClause;
            if (otherClause != null)
            {
                foreach (var conditions in otherClause)
                {
                    newClause.Add((Conditions)conditions.Clone());
                }
            }
            else
            {
                Debug.Assert(other is Conditions);
                newClause.Add((Conditions)other.Clone());
            }

            return newClause;
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the other conditions (i.e. different constraints on the same variables).
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>True if the conditions are conflicted with the other conditions, false otherwise.</returns>
        public bool IsConflictedWith(IConditions other)
        {
            // in the context of clause condition, at least one of the disjuncts needs to be non-conflicting
            foreach (var conditions in this)
            {
                if (!conditions.IsConflictedWith(other))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes the specified assignment constraint from the current conditions.
        /// </summary>
        /// <param name="assignment">Constraint assignment.</param>
        /// <returns>True if the constraint has been successfully removed, false otherwise.</returns>
        public bool RemoveConstraint(IAssignment assignment)
        {
            bool anyRemoved = false;
            bool anyFulfilled = false;

            List<Conditions> newConditions = new List<Conditions>();
            foreach (var conditions in this)
            {
                if (!conditions.IsConflictedWith(assignment))
                {
                    anyRemoved |= conditions.RemoveConstraint(assignment);
                    if (conditions.Count == 0)
                    {
                        anyFulfilled = true;
                        break;
                    }
                    newConditions.Add(conditions);
                }
            }

            if (anyFulfilled)
            {
                // if any of the disjuncts is fulfilled, then the whole clause is fulfilled
                // -> clear and add single empty conditions (always true on evaluation)
                Clear();
                Add(new Conditions());
            }
            else if (anyRemoved)
            {
                // the whole clause needs to be reinitialized, if any changes to the inner conditions occur
                Clear();
                Add(newConditions);
            }

            return anyRemoved;
        }

        /// <summary>
        /// Checks whether the specified assignment is actually constrained by the conditions.
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the given assignment is constrained by the conditions, false otherwise.</returns>
        public bool IsConstrained(IAssignment assignment)
        {
            foreach (var conditions in this)
            {
                if (conditions.IsConstrained(assignment))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether the conditions are in conflict with the specified assignment (i.e. different constraints on the same variable).
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the conditions are conflicted with the given assignment, false otherwise.</returns>
        public bool IsConflictedWith(IAssignment assignment)
        {
            // in the context of clause condition, at least one of the disjuncts needs to be non-conflicting
            foreach (var conditions in this)
            {
                if (!conditions.IsConflictedWith(assignment))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Evaluates the relevance of a single effect assignment.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        /// <returns>Effect relevance of the specified assignment.</returns>
        public EffectRelevance IsEffectAssignmentRelevant(IAssignment assignment)
        {
            bool anyAntiRelevant = false;
            foreach (var conditions in this)
            {
                EffectRelevance result = conditions.IsEffectAssignmentRelevant(assignment);
                if (result == EffectRelevance.RELEVANT)
                {
                    return EffectRelevance.RELEVANT;
                }
                anyAntiRelevant |= (result == EffectRelevance.ANTI_RELEVANT);
            }

            return (anyAntiRelevant) ? EffectRelevance.ANTI_RELEVANT : EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Checks whether the conditions are compatible with the given mutually exclusion constraints.
        /// </summary>
        /// <param name="mutexConstraints">Mutex constraints.</param>
        /// <returns>True if the conditions is compatible with the specified mutex constraints, false otherwise.</returns>
        public bool IsCompatibleWithMutexConstraints(IList<IAssignment> mutexConstraints)
        {
            // if at least one of the disjuncts is compatible, then the whole clause is compatible
            foreach (var conditions in this)
            {
                if (conditions.IsCompatibleWithMutexConstraints(mutexConstraints))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the collection of simple conditions, i.e. individual conjuncts in case of the clause conditions, or itself
        /// if the current conditions is already simple conditions variant.
        /// </summary>
        /// <returns>Collection of simple conditions.</returns>
        public IEnumerable<ISimpleConditions> GetSimpleConditions()
        {
            foreach (var conditions in this)
            {
                yield return conditions;
            }
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(IState state)
        {
            int minNotAccomplished = int.MaxValue;

            foreach (var conditions in this)
            {
                minNotAccomplished = Math.Min(minNotAccomplished, conditions.GetNotAccomplishedConstraintsCount(state));
            }

            return minNotAccomplished;
        }

        /// <summary>
        /// Makes a deep copy of the conditions.
        /// </summary>
        /// <returns>Deep copy of the conditions.</returns>
        public Planner.IConditions Clone()
        {
            ConditionsClause clause = new ConditionsClause();
            foreach (var conditions in this)
            {
                clause.Add((Conditions)conditions.Clone());
            }
            return clause;
        }

        /// <summary>
        /// Gets the conditions size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>Conditions size.</returns>
        public int GetSize()
        {
            int minCount = int.MaxValue;
            foreach (var conditions in this)
            {
                minCount = Math.Min(minCount, conditions.GetSize());
            }
            return minCount;
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
        /// <returns>All possible relative states meeting the conditions.</returns>
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
            return $"({string.Join(" or ", this)})";
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
            return HashHelper.GetHashCodeForOrderNoMatterCollection(this).CombineHashCode("ConditionsClause");
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

            ConditionsClause other = obj as ConditionsClause;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }
    }
}
