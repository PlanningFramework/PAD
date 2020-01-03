
namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ relaxed planning graph.
    /// </summary>
    public interface IRelaxedPlanningGraph
    {
        /// <summary>
        /// Computes the max forward cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>Max forward cost in the relaxed planning graph.</returns>
        double ComputeMaxForwardCost(IState state);

        /// <summary>
        /// Computes the max forward cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Max forward cost in the relaxed planning graph.</returns>
        double ComputeMaxForwardCost(IConditions conditions);

        /// <summary>
        /// Computes the additive forward cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>Additive forward cost in the relaxed planning graph.</returns>
        double ComputeAdditiveForwardCost(IState state);

        /// <summary>
        /// Computes the additive forward cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Additive forward cost in the relaxed planning graph.</returns>
        double ComputeAdditiveForwardCost(IConditions conditions);

        /// <summary>
        /// Computes the FF cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>FF cost in the relaxed planning graph.</returns>
        double ComputeFFCost(IState state);

        /// <summary>
        /// Computes the FF cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>FF cost in the relaxed planning graph.</returns>
        double ComputeFFCost(IConditions conditions);
    }
}
