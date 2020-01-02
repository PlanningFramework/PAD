
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface of the grounded PDDL operator.
    /// </summary>
    public interface IOperator : Planner.IOperator
    {
        /// <summary>
        /// Gets the corresponding lifted operator.
        /// </summary>
        /// <returns>Corresponding lifted operator.</returns>
        LiftedOperator GetLiftedOperator();

        /// <summary>
        /// Gets the grounding substitution for the parameter of the lifted operator.
        /// </summary>
        /// <returns>Variables substitution.</returns>
        ISubstitution GetSubstitution();
    }
}