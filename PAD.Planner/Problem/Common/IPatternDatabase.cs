
namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ pattern database of the abstracted planning problem. Used in PDB heuristic.
    /// </summary>
    public interface IPatternDatabase
    {
        /// <summary>
        /// Gets the calculated heuristic value for the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>Heuristic value for the state.</returns>
        double GetValue(IState state);

        /// <summary>
        /// Gets the calculated heuristic value for the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Heuristic value for the conditions.</returns>
        double GetValue(IConditions conditions);
    }
}
