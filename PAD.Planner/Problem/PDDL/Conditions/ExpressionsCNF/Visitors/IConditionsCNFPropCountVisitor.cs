using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors performing some property count on PDDL conjuctive-normal-form condition expression.
    /// </summary>
    public interface IConditionsCNFPropCountVisitor
    {
        /// <summary>
        /// Visits and performs a property count on CNF conjunction.
        /// </summary>
        /// <param name="expression">CNF conjunction.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(ConditionsCNF expression);

        /// <summary>
        /// Visits and performs a property count on CNF clause.
        /// </summary>
        /// <param name="expression">CNF clause.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(ClauseCNF expression);

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(PredicateLiteralCNF expression);

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(EqualsLiteralCNF expression);

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        Tuple<int, int> Visit(NumericCompareLiteralCNF expression);
    }
}
