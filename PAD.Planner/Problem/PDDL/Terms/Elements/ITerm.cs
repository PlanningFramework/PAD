
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for a term. These are generally used as arguments of predicates and functions, arguments of equals
    /// expression, and as return values of object functions.
    /// </summary>
    public interface ITerm
    {
        /// <summary>
        /// Creates a deep copy of the term.
        /// </summary>
        /// <returns>A copy of the term.</returns>
        ITerm Clone();

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">Term visitor.</param>
        ITerm Accept(ITermTransformVisitor visitor);
    }
}
