using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors performing some property count on PDDL logical expression.
    /// </summary>
    public interface IExpressionPropCountVisitor
    {
        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(PredicateExpression expression);

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(EqualsExpression expression);

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(AndExpression expression);

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(OrExpression expression);

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(ImplyExpression expression);

        /// <summary>
        /// Visits and performs a property count on exists expression.
        /// </summary>
        /// <param name="expression">Exists expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(ExistsExpression expression);

        /// <summary>
        /// Visits and performs a property count on forall expression.
        /// </summary>
        /// <param name="expression">Forall expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(ForallExpression expression);

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(NumericCompareExpression expression);
    }
}
