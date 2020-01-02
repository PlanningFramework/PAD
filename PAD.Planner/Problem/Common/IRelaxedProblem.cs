
namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ model of a relaxed planning problem.
    /// </summary>
    public interface IRelaxedProblem : IProblem
    {
        /// <summary>
        /// Gets the corresponding relaxed planning graph.
        /// </summary>
        /// <returns>Relaxed planning graph.</returns>
        IRelaxedPlanningGraph GetRelaxedPlanningGraph();
    }
}
