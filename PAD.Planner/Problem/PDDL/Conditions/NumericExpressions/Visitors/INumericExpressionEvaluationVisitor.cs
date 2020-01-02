
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for evaluating PDDL numeric expressions.
    /// </summary>
    public interface INumericExpressionEvaluationVisitor
    {
        /// <summary>
        /// Visits and evaluates numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Evaluated numeric value.</returns>
        double Visit(NumericFunction expression);

        /// <summary>
        /// Visits and evaluates numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Evaluated numeric value.</returns>
        double Visit(DurationVariable expression);
    }
}
