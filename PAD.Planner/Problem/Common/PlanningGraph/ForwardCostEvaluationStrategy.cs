
namespace PAD.Planner
{
    /// <summary>
    /// Evaluation strategy for the forward cost evaluation in the relaxed planning graph.
    /// </summary>
    public enum ForwardCostEvaluationStrategy
    {
        /// <summary>
        /// Max value strategy - maximum values of conjunct atoms are taken into account.
        /// </summary>
        MAX_VALUE,

        /// <summary>
        /// Additive value strategy - the values of conjunct atoms are added together.
        /// </summary>
        ADDITIVE_VALUE
    }
}
