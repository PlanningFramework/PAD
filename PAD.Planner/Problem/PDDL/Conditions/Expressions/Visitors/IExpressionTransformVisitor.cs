
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors transforming PDDL logical expressions.
    /// </summary>
    public interface IExpressionTransformVisitor
    {
        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(PreferenceExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(PredicateExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(EqualsExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(AndExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(OrExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(NotExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(ImplyExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(ExistsExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(ForallExpression expression);

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        IExpression Visit(NumericCompareExpression expression);
    }
}
