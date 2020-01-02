
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for conjunctive-normal-form condition expression evaluator.
    /// </summary>
    public interface IConditionsCNFEvalVisitor
    {
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        bool Visit(PredicateLiteralCNF expression);

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        bool Visit(EqualsLiteralCNF expression);

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        bool Visit(NumericCompareLiteralCNF expression);
    }
}
