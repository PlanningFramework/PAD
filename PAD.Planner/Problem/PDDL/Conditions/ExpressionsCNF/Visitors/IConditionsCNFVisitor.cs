
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for generic visitors of CNF expressions.
    /// </summary>
    public interface IConditionsCNFVisitor
    {
        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        void Visit(PredicateLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        void Visit(EqualsLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        void Visit(NumericCompareLiteralCNF expression);
    }
}
