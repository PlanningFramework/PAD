
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for numeric expressions.
    /// </summary>
    public interface INumericExpression
    {
        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        double Accept(INumericExpressionEvaluationVisitor visitor);

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Accept(INumericExpressionVisitor visitor);

        /// <summary>
        /// Accepts a visitor transforming numeric expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        INumericExpression Accept(INumericExpressionTransformVisitor visitor);

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        INumericExpression Clone();
    }
}
