
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors evaluating PDDL logical expressions.
    /// </summary>
    public interface IExpressionEvalVisitor
    {
        /// <summary>
        /// Visits and evaluates predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        bool Visit(PredicateExpression expression);

        /// <summary>
        /// Visits and evaluates equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        bool Visit(EqualsExpression expression);

        /// <summary>
        /// Visits and evaluates exists expression.
        /// </summary>
        /// <param name="expression">Exists expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        bool Visit(ExistsExpression expression);

        /// <summary>
        /// Visits and evaluates forall expression.
        /// </summary>
        /// <param name="expression">Forall expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        bool Visit(ForallExpression expression);

        /// <summary>
        /// Visits and evaluates relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        bool Visit(NumericCompareExpression expression);
    }
}
