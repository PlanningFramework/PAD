using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Abstract literal for conjunctive-normal-form expressions.
    /// </summary>
    public abstract class LiteralCNF : IConjunctCNF
    {
        /// <summary>
        /// Is the literal negated?
        /// </summary>
        public bool IsNegated { set; get; } = false;

        /// <summary>
        /// Accepts a conjunctive-normal-form expression relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public abstract Tuple<bool, bool> Accept(IElementCNFRelevanceEvaluationVisitor visitor);

        /// <summary>
        /// Accepts a conjunctive-normal-form expression backwards applier visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public abstract IElementCNF Accept(IElementCNFBackwardsApplierVisitor visitor);

        /// <summary>
        /// Accepts a conjunctive-normal-form expression evaluator.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public abstract bool Accept(IConditionsCNFEvalVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public abstract Tuple<int, int> Accept(IConditionsCNFPropCountVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public abstract Tuple<double, double> Accept(IConditionsCNFPropEvalVisitor visitor);

        /// <summary>
        /// Accepts a generic visitor od the conjunctive-normal-form expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IConditionsCNFVisitor visitor);

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public abstract IConjunctCNF Clone();

        /// <summary>
        /// Gets the concrete literals of the conjunct.
        /// </summary>
        /// <returns>Literals.</returns>
        public abstract IEnumerable<LiteralCNF> GetLiterals();
    }
}
