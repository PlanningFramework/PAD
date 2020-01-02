
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Mode specification for effects applier.
    /// </summary>
    public enum EffectsApplierMode
    {
        /// <summary>
        /// Standard mode - no special treatment of operator effects.
        /// </summary>
        STANDARD,

        /// <summary>
        /// Delete relaxation mode - negative operator effects are omitted.
        /// </summary>
        DELETE_RELAXATION
    };
}
