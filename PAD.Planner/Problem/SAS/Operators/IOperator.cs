
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface of the grounded SAS+ operator.
    /// </summary>
    public interface IOperator : Planner.IOperator
    {
        /// <summary>
        /// Gets preconditions of the SAS+ operator.
        /// </summary>
        Conditions GetPreconditions();

        /// <summary>
        /// Gets effects of the SAS+ operator.
        /// </summary>
        Effects GetEffects();
    }
}
