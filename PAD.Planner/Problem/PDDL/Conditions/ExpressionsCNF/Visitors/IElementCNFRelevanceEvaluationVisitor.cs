using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors evaluating relevance of conjunctive-normal-form expressions.
    /// </summary>
    public interface IElementCNFRelevanceEvaluationVisitor
    {
        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        Tuple<bool, bool> Visit(PredicateLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        Tuple<bool, bool> Visit(EqualsLiteralCNF expression);

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        Tuple<bool, bool> Visit(NumericCompareLiteralCNF expression);
    }
}
