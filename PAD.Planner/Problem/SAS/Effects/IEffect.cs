
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface for a single SAS+ operator effect.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Gets the effect condition (e.g. in case of conditional effect).
        /// </summary>
        /// <returns>Effect conditions (null if the effect is unconditional).</returns>
        ISimpleConditions GetConditions();

        /// <summary>
        /// Gets the actual effect assignment.
        /// </summary>
        /// <returns>Effect assignment.</returns>
        IAssignment GetAssignment();

        /// <summary>
        /// Checks whether the effect can actually be applied to the given state (e.g. in case of conditional effect).
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>True if the effect can be applied on the given state, false otherwise.</returns>
        bool IsApplicable(IState state);

        /// <summary>
        /// Applies the effect to the given state by directly modifying it.
        /// </summary>
        /// <param name="state">State.</param>
        void Apply(IState state);

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Effect relevance result.</returns>
        EffectRelevance IsRelevant(IConditions conditions);

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Effect relevance result.</returns>
        EffectRelevance IsRelevant(IRelativeState relativeState);

        /// <summary>
        /// Applies the effect backwards to the given conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        IConditions ApplyBackwards(IConditions conditions);

        /// <summary>
        /// Applies the effect backwards to the given relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        IRelativeState ApplyBackwards(IRelativeState relativeState);
    }
}
