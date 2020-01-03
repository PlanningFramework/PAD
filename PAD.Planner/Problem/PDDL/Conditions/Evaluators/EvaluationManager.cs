using System.Collections.Generic;
using System;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluation manager, handling evaluation of relationships within a PDDL planning problem. One planning problem
    /// should have only a single instance of this manager to minimize initialization overheads of concrete evaluators.
    /// </summary>
    public class EvaluationManager
    {
        /// <summary>
        /// Evaluator of PDDL logical expressions.
        /// </summary>
        private Lazy<ExpressionEvaluator> ExpressionEvaluator { set; get; }

        /// <summary>
        /// Evaluator of condition expressions in conjunctive-normal-form.
        /// </summary>
        private Lazy<ConditionsCNFEvaluator> ConditionsCNFEvaluator { set; get; }

        /// <summary>
        /// Rigid relations compliance evaluator.
        /// </summary>
        private Lazy<RigidRelationsComplianceEvaluator> RigidRelationsComplianceEvaluator { set; get; }

        /// <summary>
        /// Counter of not accomplished condition constraints for given states.
        /// </summary>
        private Lazy<NotAccomplishedConstraintsCounter> NotAccomplishedConstraintsCounter { get; }

        /// <summary>
        /// Counter of not accomplished condition constraints (in CNF form) for given states.
        /// </summary>
        private Lazy<NotAccomplishedConstraintsCounterCNF> NotAccomplishedConstraintsCounterCNF { get; }

        /// <summary>
        /// Evaluator of operators labels in relaxed planning graphs.
        /// </summary>
        private Lazy<PlanningGraphOperatorLabelEvaluator> PlanningGraphOperatorLabelEvaluator { get; }

        /// <summary>
        /// Evaluator of operators labels in relaxed planning graphs.
        /// </summary>
        private Lazy<PlanningGraphOperatorLabelEvaluatorCNF> PlanningGraphOperatorLabelEvaluatorCNF { get; }

        /// <summary>
        /// Evaluator/collector of satisfying atoms for specified conditions.
        /// </summary>
        private Lazy<SatisfyingAtomsEvaluator> SatisfyingAtomsEvaluator { get; }

        /// <summary>
        /// Renamer of the conditions parameters (renames occupied parameters with new parameter IDs).
        /// </summary>
        private Lazy<ConditionsParametersRenamer> ConditionsParametersRenamer { get; }

        /// <summary>
        /// Collector of used predicates in the conditions.
        /// </summary>
        private Lazy<ConditionsUsedPredicatesCollector> ConditionsUsedPredicatesCollector { get; }

        /// <summary>
        /// Rigid relations of the planning problem.
        /// </summary>
        private RigidRelations RigidRelations { set; get; }

        /// <summary>
        /// Grounding manager.
        /// </summary>
        public GroundingManager GroundingManager { set; get; }

        /// <summary>
        /// Constructs the evaluation manager.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="rigidRelations">Rigid relations.</param>
        public EvaluationManager(GroundingManager groundingManager, RigidRelations rigidRelations = null)
        {
            ExpressionEvaluator = new Lazy<ExpressionEvaluator>(() => new ExpressionEvaluator(groundingManager, rigidRelations));
            ConditionsCNFEvaluator = new Lazy<ConditionsCNFEvaluator>(() => new ConditionsCNFEvaluator(groundingManager, rigidRelations));
            RigidRelationsComplianceEvaluator = new Lazy<RigidRelationsComplianceEvaluator>(() => new RigidRelationsComplianceEvaluator(groundingManager, rigidRelations));
            NotAccomplishedConstraintsCounter = new Lazy<NotAccomplishedConstraintsCounter>(() => new NotAccomplishedConstraintsCounter(groundingManager, ExpressionEvaluator));
            NotAccomplishedConstraintsCounterCNF = new Lazy<NotAccomplishedConstraintsCounterCNF>(() => new NotAccomplishedConstraintsCounterCNF(ConditionsCNFEvaluator));
            PlanningGraphOperatorLabelEvaluator = new Lazy<PlanningGraphOperatorLabelEvaluator>(() => new PlanningGraphOperatorLabelEvaluator(groundingManager));
            PlanningGraphOperatorLabelEvaluatorCNF = new Lazy<PlanningGraphOperatorLabelEvaluatorCNF>(() => new PlanningGraphOperatorLabelEvaluatorCNF(groundingManager));
            SatisfyingAtomsEvaluator = new Lazy<SatisfyingAtomsEvaluator>(() => new SatisfyingAtomsEvaluator(groundingManager, rigidRelations));
            ConditionsParametersRenamer = new Lazy<ConditionsParametersRenamer>(() => new ConditionsParametersRenamer());
            ConditionsUsedPredicatesCollector = new Lazy<ConditionsUsedPredicatesCollector>(() => new ConditionsUsedPredicatesCollector());
            RigidRelations = rigidRelations;
            GroundingManager = groundingManager;
        }

        /// <summary>
        /// Set the rigid properties of the corresponding planning problem.
        /// </summary>
        /// <param name="rigidRelations">Rigid relations of the planning problem.</param>
        public void SetRigidRelations(RigidRelations rigidRelations)
        {
            RigidRelations = rigidRelations;

            if (ExpressionEvaluator.IsValueCreated)
            {
                ExpressionEvaluator.Value.SetRigidRelations(RigidRelations);
            }
            else
            {
                ExpressionEvaluator = new Lazy<ExpressionEvaluator>(() => new ExpressionEvaluator(GroundingManager, RigidRelations));
            }

            if (ConditionsCNFEvaluator.IsValueCreated)
            {
                ConditionsCNFEvaluator.Value.SetRigidRelations(RigidRelations);
            }
            else
            {
                ConditionsCNFEvaluator = new Lazy<ConditionsCNFEvaluator>(() => new ConditionsCNFEvaluator(GroundingManager, RigidRelations));
            }

            if (RigidRelationsComplianceEvaluator.IsValueCreated)
            {
                RigidRelationsComplianceEvaluator.Value.SetRigidRelations(RigidRelations);
            }
            else
            {
                RigidRelationsComplianceEvaluator = new Lazy<RigidRelationsComplianceEvaluator>(() => new RigidRelationsComplianceEvaluator(GroundingManager, RigidRelations));
            }
        }

        /// <summary>
        /// Evaluates the logical expression with the given reference state and variable substitution.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="substitution">Used variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(IExpression expression, ISubstitution substitution, IState referenceState = null)
        {
            return ExpressionEvaluator.Value.Evaluate(expression, substitution, referenceState);
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state and variable substitution.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <param name="substitution">Used variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(Conditions conditions, ISubstitution substitution, IState referenceState = null)
        {
            // the conditions is lifted, but no direct variable substitution provided -> try whether there is a valid grounding to satisfy the conditions
            if (conditions.Parameters != null && substitution == null)
            {
                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(conditions.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    if (conditions.TrueForAll(expression => Evaluate(expression, localSubstitution, referenceState)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return conditions.TrueForAll(expression => Evaluate(expression, substitution, referenceState));
            }
        }

        /// <summary>
        /// Evaluates the conditions with the given reference state and variable substitution.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <param name="substitution">Used variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        public bool Evaluate(ConditionsCNF conditions, ISubstitution substitution, IState referenceState = null)
        {
            // the conditions is lifted, but no direct variable substitution provided -> try whether there is a valid grounding to satisfy the conditions
            if (conditions.Parameters != null && substitution == null)
            {
                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(conditions.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    if (ConditionsCNFEvaluator.Value.Evaluate(conditions, localSubstitution, referenceState))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return ConditionsCNFEvaluator.Value.Evaluate(conditions, substitution, referenceState);
            }
        }

        /// <summary>
        /// Checks whether the specified predicate is a rigid relation of the planning problem.
        /// </summary>
        /// <param name="predicateAtom">Predicate atom.</param>
        /// <returns>True if the predicate is of rigid relation, false otherwise.</returns>
        public bool IsPredicateRigidRelation(IAtom predicateAtom)
        {
            return RigidRelations.IsPredicateRigidRelation(predicateAtom);
        }

        /// <summary>
        /// Evaluates whether the conditions are in compliance with the rigid relations of the planning problem. Ignores non-rigid relations.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>True if the conditions are in compliance with the rigid relations, false otherwise.</returns>
        public bool EvaluateRigidRelationsCompliance(Conditions conditions, ISubstitution substitution)
        {
            return conditions.TrueForAll(expression => EvaluateRigidRelationsCompliance(expression, substitution));
        }

        /// <summary>
        /// Evaluates whether the expression conditions are in compliance with the rigid relations of the planning problem. Ignores non-rigid relations.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>True if the conditions are in compliance with the rigid relations, false otherwise.</returns>
        public bool EvaluateRigidRelationsCompliance(IExpression expression, ISubstitution substitution)
        {
            return RigidRelationsComplianceEvaluator.Value.Evaluate(expression, substitution);
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(Conditions conditions, IState state)
        {
            return NotAccomplishedConstraintsCounter.Value.Evaluate(conditions, state);
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int GetNotAccomplishedConstraintsCount(ConditionsCNF conditions, IState state)
        {
            return NotAccomplishedConstraintsCounterCNF.Value.Evaluate(conditions, state);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Operator preconditions.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(Conditions conditions, ISubstitution substitution, StateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return PlanningGraphOperatorLabelEvaluator.Value.Evaluate(conditions, substitution, stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Operator preconditions.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double EvaluateOperatorPlanningGraphLabel(ConditionsCNF conditions, ISubstitution substitution, StateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return PlanningGraphOperatorLabelEvaluatorCNF.Value.Evaluate(conditions, substitution, stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Gets a list of atoms from the specified state that are necessary to make these conditions true.
        /// </summary>
        /// <param name="conditions">Conditions to evaluate.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="predecessorState">Preceding state.</param>
        /// <returns>List of satisfying atoms.</returns>
        public List<IAtom> GetSatisfyingAtoms(IConditions conditions, ISubstitution substitution, IState predecessorState)
        {
            return SatisfyingAtomsEvaluator.Value.Evaluate(conditions.GetCNF(), substitution, predecessorState);
        }

        /// <summary>
        /// Renames the parameters (and the corresponding occurences in the conditions), starting from the given free parameter ID.
        /// </summary>
        /// <param name="conditions">Conditions to be edited.</param>
        /// <param name="firstFreeParameterId">First free parameter ID.</param>
        public void RenameConditionParameters(ConditionsCNF conditions, int firstFreeParameterId)
        {
            ConditionsParametersRenamer.Value.Rename(conditions, firstFreeParameterId);
        }

        /// <summary>
        /// Gets a collection of predicates used in the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Collection of predicates used in the conditions.</returns>
        public HashSet<IAtom> CollectUsedPredicates(IConditions conditions)
        {
            return ConditionsUsedPredicatesCollector.Value.Collect(conditions);
        }
    }
}
