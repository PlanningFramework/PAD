
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for PDDL numeric expressions transforming visitor.
    /// </summary>
    public interface INumericExpressionTransformVisitor
    {
        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(Plus expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(Minus expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(UnaryMinus expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(Multiply expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(Divide expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(Number expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(DurationVariable expression);

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        INumericExpression Visit(NumericFunction expression);
    }
}
