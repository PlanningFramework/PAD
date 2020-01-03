
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expression evaluator.
    /// </summary>
    public class NumericExpressionEvaluator : INumericExpressionEvaluationVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Currently used variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; }

        /// <summary>
        /// Reference state for the evaluation.
        /// </summary>
        private IState ReferenceState { set; get; }

        /// <summary>
        /// Constructs numeric evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        public NumericExpressionEvaluator(GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
        }

        /// <summary>
        /// Evaluates numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Evaluated numeric value.</returns>
        public double Evaluate(INumericExpression expression, ISubstitution substitution, IState referenceState)
        {
            Substitution = substitution;
            ReferenceState = referenceState;
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits and evaluates numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Evaluated numeric value.</returns>
        public double Visit(NumericFunction expression)
        {
            IAtom groundedFunctionAtom = GroundingManager.GroundAtomDeep(expression.FunctionAtom, Substitution, ReferenceState);
            return ReferenceState.GetNumericFunctionValue(groundedFunctionAtom);
        }

        /// <summary>
        /// Visits and evaluates numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Evaluated numeric value.</returns>
        public double Visit(DurationVariable expression)
        {
            return 0.0;
        }
    }
}
