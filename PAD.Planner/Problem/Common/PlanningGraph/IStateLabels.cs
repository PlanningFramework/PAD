
namespace PAD.Planner
{
    /// <summary>
    /// Common interface for state labels in the relaxed planning graph, i.e. label values of state propositions or variable values
    /// (depending on the used representation) calculated in the forward cost heuristic evaluation of the relaxed planning graph.
    /// </summary>
    public interface IStateLabels
    {
        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        bool Equals(object obj);
    }
}
