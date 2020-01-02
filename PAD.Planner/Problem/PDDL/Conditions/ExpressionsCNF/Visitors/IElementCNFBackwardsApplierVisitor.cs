
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for backwards applier visitors of conjunctive-normal-form expressions.
    /// </summary>
    public interface IElementCNFBackwardsApplierVisitor
    {
        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        IElementCNF Visit(PredicateLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        IElementCNF Visit(EqualsLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        IElementCNF Visit(NumericCompareLiteralCNF expression);
    }
}
