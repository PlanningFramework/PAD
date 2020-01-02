using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// The standard implemetation of PDDL conditions, using simple logical expressions (single conditions is a conjunction of expressions).
    /// </summary>
    public class Conditions : List<IExpression>, IConditions
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
        /// Conditions CNF expressions builder.
        /// </summary>
        private Lazy<ConditionsCNFBuilder> ConditionsCNFBuilder { set; get; } = null;

        /// <summary>
        /// Collector of actually used conditions parameters.
        /// </summary>
        private Lazy<ConditionsParametersCollector> ConditionsParametersCollector { set; get; } = null;

        /// <summary>
        /// Constructs an empty conditions object.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        /// <param name="parameters">Parameters of the lifted arguments (null if fully grounded).</param>
        public Conditions(EvaluationManager evaluationManager, Parameters parameters = null)
        {
            EvaluationManager = evaluationManager;
            ConditionsCNFBuilder = new Lazy<ConditionsCNFBuilder>(() => new ConditionsCNFBuilder(evaluationManager));
            ConditionsParametersCollector = new Lazy<ConditionsParametersCollector>(() => new ConditionsParametersCollector());
            Parameters = parameters;
        }

        /// <summary>
        /// Constructs an empty conditions object.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        /// <param name="conditionsCNFBuilder">CNF builder.</param>
        /// <param name="conditionsParametersCollector">Conditions parameters collector.</param>
        /// <param name="parameters">Parameters of the lifted arguments (null if fully grounded).</param>
        protected Conditions(EvaluationManager evaluationManager, Lazy<ConditionsCNFBuilder> conditionsCNFBuilder, Lazy<ConditionsParametersCollector> conditionsParametersCollector, Parameters parameters = null)
        {
            EvaluationManager = evaluationManager;
            ConditionsCNFBuilder = conditionsCNFBuilder;
            ConditionsParametersCollector = conditionsParametersCollector;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructs the object from a single grounded expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public Conditions(IExpression expression, EvaluationManager evaluationManager) : this(evaluationManager, null)
        {
            Add(expression);
        }

        /// <summary>
        /// Constructs the object from the input data.
        /// </summary>
        /// <param name="inputData">Input data of the planning problem.</param>
        /// <param name="parameters">Parameters of the lifted arguments (null if fully grounded).</param>
        /// <param name="idManager">ID manager.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public Conditions(List<InputData.PDDL.Expression> inputData, Parameters parameters, IDManager idManager, EvaluationManager evaluationManager) : this(evaluationManager, null)
        {
            ExpressionsBuilder expressionsBuilder = new ExpressionsBuilder(idManager);
            inputData.ForEach(inputExpression => Add(expressionsBuilder.Build(inputExpression)));

            if (parameters != null)
            {
                Parameters = DetermineUsedParameters(parameters);
            }
        }

        /// <summary>
        /// Determine which parameters are actually used in the conditions. We can have e.g. parameters of operator which
        /// apply on several conditions and effects.
        /// </summary>
        /// <returns>Paramaters.</returns>
        private Parameters DetermineUsedParameters(Parameters sourceParameters)
        {
            if (sourceParameters == null)
            {
                return null;
            }

            Parameters newParameters = new Parameters();

            HashSet<int> usedParameters = ConditionsParametersCollector.Value.Collect(this);

            foreach (var parameter in sourceParameters)
            {
                if (usedParameters.Contains(parameter.ParameterNameID))
                {
                    newParameters.Add(parameter.Clone());
                }
            }

            return (newParameters.Count == 0) ? null : newParameters;
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
        /// Evaluates whether the conditions are in compliance with the rigid relations of the planning problem. Ignores non-rigid relations.
        /// </summary>
        /// <param name="substitution">Substitution.</param>
        /// <returns>True if the conditions are in compliance with the rigid relations, false otherwise.</returns>
        public bool EvaluateRigidRelationsCompliance(ISubstitution substitution)
        {
            return EvaluationManager.EvaluateRigidRelationsCompliance(this, substitution);
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
        /// Gets a list of atoms from the specified state that are necessary to make these conditions true.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="predecessorState">Predecessing state.</param>
        /// <returns>List of satisfying atoms.</returns>
        public List<IAtom> GetSatisfyingAtoms(ISubstitution substitution, IState predecessorState)
        {
            return EvaluationManager.GetSatisfyingAtoms(this, substitution, predecessorState);
        }

        /// <summary>
        /// Returns the CNF (conjunctive-normal-form) of the condition expressions.
        /// </summary>
        /// <returns>CNF of the conditions.</returns>
        public IConditions GetCNF()
        {
            return ConditionsCNFBuilder.Value.Build(this);
        }

        /// <summary>
        /// Wraps the conditions and returns them as a single AND expression.
        /// </summary>
        /// <returns>Conditions as a single expression.</returns>
        public IExpression GetWrappedConditions()
        {
            if (Count == 0)
            {
                return null;
            }

            IExpression conditionsExpression = null;
            if (Count > 1)
            {
                conditionsExpression = new AndExpression(this);
            }
            else
            {
                conditionsExpression = this[0];
            }

            return conditionsExpression;
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
        /// Clones an empty conditions (only with inited inner util objects and parameters).
        /// </summary>
        /// <returns>Creates an empty conditions.</returns>
        public Conditions CloneEmpty()
        {
            return new Conditions(EvaluationManager, ConditionsCNFBuilder, ConditionsParametersCollector,(Parameters != null) ? Parameters.Clone() : null);
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
            return HashHelper.GetHashCode(this).CombineHashCode("Conditions");
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
