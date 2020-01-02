using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for all elements of conjunctive-normal-form expressions.
    /// </summary>
    public interface IElementCNF
    {
        /// <summary>
        /// Accepts a conjunctive-normal-form expression relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        Tuple<bool, bool> Accept(IElementCNFRelevanceEvaluationVisitor visitor);

        /// <summary>
        /// Accepts a conjunctive-normal-form expression backwards applier visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        IElementCNF Accept(IElementCNFBackwardsApplierVisitor visitor);

        /// <summary>
        /// Accepts a conjunctive-normal-form expression evaluator.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        bool Accept(IConditionsCNFEvalVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        Tuple<int, int> Accept(IConditionsCNFPropCountVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        Tuple<double, double> Accept(IConditionsCNFPropEvalVisitor visitor);

        /// <summary>
        /// Accepts a generic visitor od the conjunctive-normal-form expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Accept(IConditionsCNFVisitor visitor);
    }
}
