
namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a state layer in the relaxed planning graph, i.e. a collection of propositions or variable values
    /// (in form of a relaxed state) and corresponding label values for these propositions/values, calculated during forward cost
    /// evaluation of the relaxed planning problem.
    /// </summary>
    public interface IStateLayer
    {
        /// <summary>
        /// Gets the corresponding state.
        /// </summary>
        /// <returns>Corresponding state.</returns>
        IState GetState();

        /// <summary>
        /// Gets the corresponding state labels.
        /// </summary>
        /// <returns>Corresponding state labels.</returns>
        IStateLabels GetStateLabels();

        /// <summary>
        /// Checks whether the state layer has the specified proposition.
        /// </summary>
        /// <param name="proposition">Proposition.</param>
        /// <returns>True if the state layer has the given proposition, false otherwise.</returns>
        bool HasProposition(IProposition proposition);

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        bool Equals(object obj);
    }
}
