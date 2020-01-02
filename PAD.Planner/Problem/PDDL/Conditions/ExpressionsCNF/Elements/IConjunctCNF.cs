using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for conjunct elements of normal-form expressions.
    /// </summary>
    public interface IConjunctCNF : IElementCNF
    {
        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        IConjunctCNF Clone();

        /// <summary>
        /// Gets the concrete literals of the conjunct.
        /// </summary>
        /// <returns>Literals.</returns>
        IEnumerable<LiteralCNF> GetLiterals();
    }
}
