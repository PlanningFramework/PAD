
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for a transformation visitor of terms.
    /// </summary>
    public interface ITermTransformVisitor
    {
        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        ITerm Visit(ConstantTerm term);

        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        ITerm Visit(VariableTerm term);

        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        ITerm Visit(ObjectFunctionTerm term);
    }
}
