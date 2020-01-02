using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Condition expression in CNF (conjunctive-normal-form), i.e. in form of a conjunction of disjunctions. The literals in
    /// this context are predicate expressions, equals expressions and numeric compare expressions, and their negations.
    /// Expressions in this form are used e.g. when evaluating conditions against operator effects to determine relevant
    /// operators.
    /// </summary>
    public class ConditionsCNF : HashSet<IConjunctCNF>, IElementCNF, IConditions
    {
        /// <summary>
        /// Parameters of the lifted arguments (for the partially lifted conditions). Variables in condition atoms refer to these typed
        /// parameters. If the parameters are null or empty, the conditions are fully grounded.
        /// </summary>
        public Parameters Parameters { set; get; } = null;

        /// <summary>
        /// Evaluation manager.
        /// </summary>
        private EvaluationManager EvaluationManager { set; get; } = null;

        /// <summary>
        /// Constructs an empty CNF expression.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        /// <param name="parameters">Condition parameters.</param>
        public ConditionsCNF(EvaluationManager evaluationManager, Parameters parameters)
        {
            EvaluationManager = evaluationManager;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructs the CNF expression from the given conjuncts.
        /// </summary>
        /// <param name="conjuncts">Conjuncts.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        // <param name="parameters">Condition parameters.</param>
        public ConditionsCNF(HashSet<IConjunctCNF> conjuncts, EvaluationManager evaluationManager, Parameters parameters) : base(conjuncts)
        {
            EvaluationManager = evaluationManager;
        }

        /// <summary>
        /// Gets the parameters of the conditions specifying of the lifted arguments.
        /// </summary>
        /// <returns>Parameters of the conditions. Can be null, if conditions fully grounded.</returns>
        public Parameters GetParameters()
        {
            return Parameters;
        }

        /// <summary>
        /// Merges the specified CNF conditions into the current CNF conditions. The other CNF shouldn't be used anymore (its items are reused in the current CNF).
        /// </summary>
        /// <param name="other">Other CNF conditions.</param>
        public void Merge(ConditionsCNF other)
        {
            if (Parameters == null)
            {
                Parameters = new Parameters();
            }

            if (Parameters.AreConflictedWith(other.Parameters))
            {
                EvaluationManager.RenameConditionParameters(other, Parameters.GetMaxUsedParameterID() + 1);
            }

            if (other.Parameters != null)
            {
                Parameters.AddRange(other.Parameters);
            }

            if (Parameters.Count == 0)
            {
                Parameters = null;
            }

            foreach (var items in other)
            {
                Add(items);
            }
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(Planner.IState state)
        {
            return Evaluate((IState)state, null);
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state and variable substitution.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(IState state, ISubstitution substitution = null)
        {
            return EvaluationManager.Evaluate(this, substitution, state);
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(IState state)
        {
            return EvaluationManager.GetNotAccomplishedConstraintsCount(this, state);
        }

        /// <summary>
        /// Returns a collection of all used predicates within this conditions. Can be used for some preprocessing.
        /// </summary>
        /// <returns>Collection of used predicates.</returns>
        public HashSet<IAtom> GetUsedPredicates()
        {
            return EvaluationManager.CollectUsedPredicates(this);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">State labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return EvaluateOperatorPlanningGraphLabel(null, stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(ISubstitution substitution, IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return EvaluationManager.EvaluateOperatorPlanningGraphLabel(this, substitution, (StateLabels)stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Returns the CNF (conjunctive-normal-form) of the condition expressions.
        /// </summary>
        /// <returns>CNF of the conditions.</returns>
        public IConditions GetCNF()
        {
            return (IConditions)Clone();
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(and {string.Join(" ", this)})";
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
            return StatesEnumerator.EnumerateStates(this, (Problem)problem);
        }

        /// <summary>
        /// Enumerates all possible relative states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible realtive states meeting the conditions.</returns>
        public IEnumerable<Planner.IRelativeState> GetCorrespondingRelativeStates(IProblem problem)
        {
            return StatesEnumerator.EnumerateRelativeStates(this, (Problem)problem);
        }

        /// <summary>
        /// Creates a deep copy of the conditions.
        /// </summary>
        /// <returns>Conditions clone.</returns>
        public Planner.IConditions Clone()
        {
            var newConditions = CloneEmpty();
            foreach (var expression in this)
            {
                newConditions.Add(expression.Clone());
            }
            return newConditions;
        }

        /// <summary>
        /// Clones an empty conditions (only with inited inner util objects and parameters).
        /// </summary>
        /// <returns>Creates an empty conditions.</returns>
        public ConditionsCNF CloneEmpty()
        {
            return new ConditionsCNF(EvaluationManager, (Parameters != null) ? Parameters.Clone() : null);
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
            return HashHelper.GetHashCodeForOrderNoMatterCollection(this).CombineHashCode("ConditionsCNF");
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

            ConditionsCNF other = obj as ConditionsCNF;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public Tuple<bool, bool> Accept(IElementCNFRelevanceEvaluationVisitor visitor)
        {
            bool positiveCondition = false;
            bool negativeCondition = true;

            foreach (var conjunct in this)
            {
                var result = conjunct.Accept(visitor);
                positiveCondition |= result.Item1;
                negativeCondition &= result.Item2;

                if (!negativeCondition)
                {
                    break;
                }

            }
            return Tuple.Create(positiveCondition, negativeCondition);
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression backwards applier visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public IElementCNF Accept(IElementCNFBackwardsApplierVisitor visitor)
        {
            ConditionsCNF newExpression = CloneEmpty();
            foreach (var conjunct in this)
            {
                var resultExpression = conjunct.Accept(visitor);
                if (resultExpression != null)
                {
                    newExpression.Add((IConjunctCNF)resultExpression);
                }
            }
            return newExpression;
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression evaluator.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public bool Accept(IConditionsCNFEvalVisitor visitor)
        {
            foreach (var conjunct in this)
            {
                if (!conjunct.Accept(visitor))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<int, int> Accept(IConditionsCNFPropCountVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<double, double> Accept(IConditionsCNFPropEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic visitor od the conjunctive-normal-form expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IConditionsCNFVisitor visitor)
        {
            foreach (var conjunct in this)
            {
                conjunct.Accept(visitor);
            }
        }
    }
}
