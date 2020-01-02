
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for generic PDDL numeric expressions visitor.
    /// </summary>
    public interface INumericExpressionVisitor
    {
        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void PostVisit(Plus expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void PostVisit(Minus expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void PostVisit(UnaryMinus expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void PostVisit(Multiply expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void PostVisit(Divide expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void Visit(Number expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void Visit(DurationVariable expression);

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        void Visit(NumericFunction expression);
    }
}
