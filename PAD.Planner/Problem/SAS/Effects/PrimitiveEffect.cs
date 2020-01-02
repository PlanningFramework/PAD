
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a single SAS+ primitive effect (simple assignment, without additional conditions).
    /// </summary>
    public class PrimitiveEffect : IEffect
    {
        /// <summary>
        /// Actual assignment to be applied (variable-value pair).
        /// </summary>
        public IAssignment Assignment { set; get; } = null;

        /// <summary>
        /// Constructs the SAS+ operator effect from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public PrimitiveEffect(InputData.SAS.Effect inputData)
        {
            Assignment = new Assignment(inputData.PrimitiveEffect);
        }

        /// <summary>
        /// Constructs the SAS+ operator effect.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        public PrimitiveEffect(IAssignment assignment)
        {
            Assignment = assignment;
        }

        /// <summary>
        /// Checks whether the effect can actually be applied to the given state (e.g. in case of conditional effect).
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>True if the effect can be applied on the given state, false otherwise.</returns>
        public virtual bool IsApplicable(IState state)
        {
            return true;
        }

        /// <summary>
        /// Applies the effect to the given state by directly modifying it.
        /// </summary>
        /// <param name="state">State.</param>
        public void Apply(IState state)
        {
            state.SetValue(Assignment);
        }

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Effect relevance result.</returns>
        public virtual EffectRelevance IsRelevant(IConditions conditions)
        {
            return conditions.IsEffectAssignmentRelevant(Assignment);
        }

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Effect relevance result.</returns>
        public virtual EffectRelevance IsRelevant(IRelativeState relativeState)
        {
            int value = relativeState.GetValue(Assignment.GetVariable());
            if (value == RelativeState.WILD_CARD_VALUE)
            {
                // not a conflict, but not positively contributing either
                return EffectRelevance.IRRELEVANT;
            }

            return (Assignment.GetValue() == value) ? EffectRelevance.RELEVANT : EffectRelevance.ANTI_RELEVANT;
        }

        /// <summary>
        /// Applies the effect backwards to the given conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        public virtual IConditions ApplyBackwards(IConditions conditions)
        {
            if (!conditions.IsConflictedWith(Assignment))
            {
                conditions.RemoveConstraint(Assignment);
                return conditions;
            }
            return new ConditionsContradiction();
        }

        /// <summary>
        /// Applies the effect backwards to the given relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        public virtual IRelativeState ApplyBackwards(IRelativeState relativeState)
        {
            // we are fine here with the direct modification
            relativeState.SetValue(Assignment.GetVariable(), RelativeState.WILD_CARD_VALUE);
            return relativeState;
        }

        /// <summary>
        /// Gets the effect condition (e.g. in case of conditional effect).
        /// </summary>
        /// <returns>Effect conditions (null if the effect is unconditional).</returns>
        public virtual ISimpleConditions GetConditions()
        {
            return null;
        }

        /// <summary>
        /// Gets the actual effect assignment.
        /// </summary>
        /// <returns>Effect assignment.</returns>
        public IAssignment GetAssignment()
        {
            return Assignment;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{Assignment}";
        }
    }
}
