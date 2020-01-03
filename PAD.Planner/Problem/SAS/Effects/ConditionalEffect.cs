
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a single SAS+ conditional effect.
    /// </summary>
    public class ConditionalEffect : PrimitiveEffect
    {
        /// <summary>
        /// Conditions to be met for the effect (in case of conditional effect).
        /// </summary>
        public ISimpleConditions Conditions { set; get; }

        /// <summary>
        /// Constructs the SAS+ operator conditional effect from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public ConditionalEffect(InputData.SAS.Effect inputData) : base(inputData)
        {
            Conditions = new Conditions(inputData.Conditions);
        }

        /// <summary>
        /// Constructs the SAS+ operator conditional effect.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="assignment">Effect assignment.</param>
        public ConditionalEffect(ISimpleConditions conditions, IAssignment assignment) : base(assignment)
        {
            Conditions = conditions;
        }

        /// <summary>
        /// Checks whether the effect can actually be applied to the given state (e.g. in case of conditional effect).
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>True if the effect can be applied on the given state, false otherwise.</returns>
        public override bool IsApplicable(IState state)
        {
            return Conditions.Evaluate(state);
        }

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Effect relevance result.</returns>
        public override EffectRelevance IsRelevant(IConditions conditions)
        {
            var assignmentRelevance = base.IsRelevant(conditions);

            if (assignmentRelevance == EffectRelevance.RELEVANT)
            {
                IConditions nonAffectedConditions = (IConditions)Conditions.Clone();
                nonAffectedConditions.RemoveConstraint(Assignment);
                if (!nonAffectedConditions.IsConflictedWith(conditions))
                {
                    return EffectRelevance.RELEVANT;
                }
            }

            // conditional effect does not have to be used, so it should never be explicitly anti-relevant
            return EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Checks whether the operator effect is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Effect relevance result.</returns>
        public override EffectRelevance IsRelevant(IRelativeState relativeState)
        {
            var assignmentRelevance = base.IsRelevant(relativeState);

            if (assignmentRelevance == EffectRelevance.RELEVANT)
            {
                foreach (var constraint in Conditions)
                {
                    if (constraint.GetVariable() == Assignment.GetVariable())
                    {
                        continue;
                    }

                    int value = relativeState.GetValue(constraint.GetVariable());
                    if (value == RelativeState.WildCardValue)
                    {
                        continue;
                    }

                    if (constraint.GetValue() != value)
                    {
                        return EffectRelevance.IRRELEVANT;
                    }
                }
                return EffectRelevance.RELEVANT;
            }

            // conditional effect does not have to be used, so it should never be explicitly anti-relevant
            return EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Applies the effect backwards to the given conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        public override IConditions ApplyBackwards(IConditions conditions)
        {
            if (IsRelevant(conditions) == EffectRelevance.RELEVANT)
            {
                IConditions newConditions = (IConditions)conditions.Clone();
                newConditions.RemoveConstraint(Assignment);
                newConditions = newConditions.ConjunctionWith(Conditions);

                return conditions.DisjunctionWith(newConditions);
            }
            return conditions;
        }

        /// <summary>
        /// Applies the effect backwards to the given relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        public override IRelativeState ApplyBackwards(IRelativeState relativeState)
        {
            // we explicitly apply the effect for the relative state, if it's relevant
            if (IsRelevant(relativeState) == EffectRelevance.RELEVANT)
            {
                var newState = (IRelativeState)relativeState.Clone();

                newState.SetValue(Assignment.GetVariable(), RelativeState.WildCardValue);
                foreach (var constraint in Conditions)
                {
                    newState.SetValue(constraint.GetVariable(), constraint.GetValue());
                }

                return newState;
            }

            // the conditional effect was not applied, because it was not relevant
            return null;
        }

        /// <summary>
        /// Gets the effect condition (e.g. in case of conditional effect).
        /// </summary>
        /// <returns>Effect conditions (null if the effect is unconditional).</returns>
        public override ISimpleConditions GetConditions()
        {
            return Conditions;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({Conditions}) -> {Assignment}";
        }
    }
}
