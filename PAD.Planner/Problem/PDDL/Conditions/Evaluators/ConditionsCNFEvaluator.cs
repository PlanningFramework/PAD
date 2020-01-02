using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluator of conditions in CNF expression form.
    /// </summary>
    public class ConditionsCNFEvaluator : IConditionsCNFEvalVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        protected GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Numeric evaluator.
        /// </summary>
        protected Lazy<NumericExpressionEvaluator> NumericEvaluator { set; get; } = null;

        /// <summary>
        /// Rigid relations of the planning problem.
        /// </summary>
        protected RigidRelations RigidRelations { set; get; } = null;

        /// <summary>
        /// Currently used variables substitution.
        /// </summary>
        protected ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Reference state for the evaluation.
        /// </summary>
        protected IState ReferenceState { set; get; } = null;

        /// <summary>
        /// Constructs the CNF expression evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="rigidRelations">Rigid relations.</param>
        public ConditionsCNFEvaluator(GroundingManager groundingManager, RigidRelations rigidRelations = null)
        {
            GroundingManager = groundingManager;
            RigidRelations = (rigidRelations != null) ? rigidRelations : new RigidRelations();
            NumericEvaluator = new Lazy<NumericExpressionEvaluator>(() => new NumericExpressionEvaluator(groundingManager));
        }

        /// <summary>
        /// Set the rigid properties of the corresponding planning problem.
        /// </summary>
        /// <param name="rigidRelations">Rigid relations of the planning problem.</param>
        public void SetRigidRelations(RigidRelations rigidRelations)
        {
            RigidRelations = rigidRelations;
        }

        /// <summary>
        /// Evaluates the given CNF expression.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="substitution">Used variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>True if the expression evaluates as true, false otherwise.</returns>
        public bool Evaluate(IElementCNF expression, ISubstitution substitution, IState referenceState)
        {
            Substitution = substitution;
            ReferenceState = referenceState;
            return expression.Accept(this);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public virtual bool Visit(PredicateLiteralCNF expression)
        {
            IAtom groundedPredicateAtom = GroundingManager.GroundAtomDeep(expression.PredicateAtom, Substitution, ReferenceState);
            bool evaluationResult = (RigidRelations.Contains(groundedPredicateAtom) || ReferenceState.HasPredicate(groundedPredicateAtom));
            return (!expression.IsNegated == evaluationResult);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public bool Visit(EqualsLiteralCNF expression)
        {
            bool evaluationResult = GroundingManager.GroundTermDeep(expression.LeftArgument, Substitution, ReferenceState).Equals(
                                    GroundingManager.GroundTermDeep(expression.RightArgument, Substitution, ReferenceState));
            return (!expression.IsNegated == evaluationResult);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public bool Visit(NumericCompareLiteralCNF expression)
        {
            double leftValue = NumericEvaluator.Value.Evaluate(expression.LeftArgument, Substitution, ReferenceState);
            double rightValue = NumericEvaluator.Value.Evaluate(expression.RightArgument, Substitution, ReferenceState);
            bool evaluationResult = NumericCompareExpression.ApplyCompare(expression.Operator, leftValue, rightValue);
            return (!expression.IsNegated == evaluationResult);
        }
    }
}
