
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors of PDDL logical expressions.
    /// </summary>
    public interface IExpressionVisitor
    {
        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(PreferenceExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(PredicateExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(EqualsExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(AndExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(OrExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(NotExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(ImplyExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(ExistsExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(ForallExpression expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        void Visit(NumericCompareExpression expression);
    }
}
