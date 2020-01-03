using System.Collections.Generic;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluator of the operator effects relevance. The operator is relevant for the specified relative state, if it contributes to satisfy the
    /// relative state. This operator can be then used in the backward search and reverse application on the given relative state.
    /// </summary>
    public class EffectsRelevanceRelativeStateEvaluator
    {
        /// <summary>
        /// Structure for collecting and preprocessing of operator effects that are being evaluated.
        /// </summary>
        private EffectsPreprocessedCollection Effects { get; } = new EffectsPreprocessedCollection();

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Variables substitution of the effects' parent operator.
        /// </summary>
        private ISubstitution OperatorSubstitution { set; get; }

        /// <summary>
        /// Constructs the effects relevance evaluator.
        /// </summary>
        /// <param name="effects">Effects for the evaluation.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        public EffectsRelevanceRelativeStateEvaluator(List<IEffect> effects, GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
            Effects.CollectEffects(effects);
        }

        /// <summary>
        /// Constructs the effects relevance evaluator.
        /// </summary>
        /// <param name="effects">Preprocessed effects for the evaluation.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        public EffectsRelevanceRelativeStateEvaluator(EffectsPreprocessedCollection effects, GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
            Effects = effects;
        }

        /// <summary>
        /// Evaluates whether the operator effects are relevant for the specified relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <param name="operatorSubstitution">Variables substitution of the operator.</param>
        /// <param name="relevantConditionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True when the effects are relevant, false otherwise.</returns>
        public bool Evaluate(IRelativeState relativeState, ISubstitution operatorSubstitution, IList<int> relevantConditionalEffects = null)
        {
            return (EvaluateInternal(relativeState, operatorSubstitution, relevantConditionalEffects) == EffectRelevance.RELEVANT);
        }

        /// <summary>
        /// Evaluates whether the operator effects are relevant for the specified relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <param name="operatorSubstitution">Variables substitution of the operator.</param>
        /// <param name="relevantConditionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>Effect relevance (relevant, irrelevant, or anti-relevant).</returns>
        private EffectRelevance EvaluateInternal(IRelativeState relativeState, ISubstitution operatorSubstitution, IList<int> relevantConditionalEffects = null)
        {
            Effects.GroundEffectsByCurrentOperatorSubstitution(GroundingManager, operatorSubstitution);

            OperatorSubstitution = operatorSubstitution;

            var primitivesResult = ProcessPrimitiveEffects(relativeState);
            if (primitivesResult == EffectRelevance.ANTI_RELEVANT)
            {
                return EffectRelevance.ANTI_RELEVANT;
            }

            var forallResult = ProcessForallEffects(relativeState);
            if (forallResult == EffectRelevance.ANTI_RELEVANT)
            {
                return EffectRelevance.ANTI_RELEVANT;
            }

            var whenResult = ProcessWhenEffects(relativeState, relevantConditionalEffects);
            if (whenResult == EffectRelevance.ANTI_RELEVANT)
            {
                return EffectRelevance.ANTI_RELEVANT;
            }

            if (primitivesResult == EffectRelevance.RELEVANT || forallResult == EffectRelevance.RELEVANT || whenResult == EffectRelevance.RELEVANT)
            {
                return EffectRelevance.RELEVANT;
            }

            return EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Processes primitive effects.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Effect relevance (relevant, irrelevant, or anti-relevant).</returns>
        private EffectRelevance ProcessPrimitiveEffects(IRelativeState relativeState)
        {
            bool anyRelevant = false;

            foreach (var predicateAtom in relativeState.GetPredicates())
            {
                anyRelevant |= Effects.GroundedPositivePredicateEffects.Contains(predicateAtom);
                if (Effects.GroundedNegativePredicateEffects.Contains(predicateAtom))
                {
                    return EffectRelevance.ANTI_RELEVANT;
                }
            }

            foreach (var predicateAtom in relativeState.GetNegatedPredicates())
            {
                anyRelevant |= Effects.GroundedNegativePredicateEffects.Contains(predicateAtom);
                if (Effects.GroundedPositivePredicateEffects.Contains(predicateAtom))
                {
                    return EffectRelevance.ANTI_RELEVANT;
                }
            }

            foreach (var function in relativeState.GetObjectFunctions())
            {
                ITerm assignValue;
                if (Effects.GroundedObjectFunctionAssignmentEffects.TryGetValue(function.Key, out assignValue))
                {
                    ConstantTerm constantValue = assignValue as ConstantTerm;
                    if (constantValue == null || constantValue.NameId != function.Value)
                    {
                        // surely assigning different value -> anti-relevant
                        return EffectRelevance.ANTI_RELEVANT;
                    }
                    anyRelevant = true;
                }
            }

            foreach (var function in relativeState.GetNumericFunctions())
            {
                INumericExpression assignExpression;
                if (Effects.GroundedNumericFunctionAssignmentEffects.TryGetValue(function.Key, out assignExpression))
                {
                    Number numberValue = assignExpression as Number;
                    if (numberValue == null || !numberValue.Value.Equals(function.Value))
                    {
                        // surely assigning different value -> anti-relevant
                        return EffectRelevance.ANTI_RELEVANT;
                    }
                    anyRelevant = true;
                }
            }

            return (anyRelevant) ? EffectRelevance.RELEVANT : EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Processes forall effects.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Effect relevance (relevant, irrelevant, or anti-relevant).</returns>
        private EffectRelevance ProcessForallEffects(IRelativeState relativeState)
        {
            bool anyRelevant = false;

            foreach (var forallEffect in Effects.ForallEffects)
            {
                EffectsRelevanceRelativeStateEvaluator evaluator = new EffectsRelevanceRelativeStateEvaluator(forallEffect.Effects, GroundingManager);

                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(forallEffect.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    OperatorSubstitution.AddLocalSubstitution(localSubstitution);

                    var result = evaluator.EvaluateInternal(relativeState, OperatorSubstitution);
                    anyRelevant |= (result == EffectRelevance.RELEVANT);

                    OperatorSubstitution.RemoveLocalSubstitution(localSubstitution);

                    if (result == EffectRelevance.ANTI_RELEVANT)
                    {
                        return EffectRelevance.ANTI_RELEVANT;
                    }
                }
            }

            return (anyRelevant) ? EffectRelevance.RELEVANT : EffectRelevance.IRRELEVANT;
        }

        /// <summary>
        /// Processes conditional (when) effects.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <param name="relevantConditionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>Effect relevance (relevant, irrelevant, or anti-relevant).</returns>
        private EffectRelevance ProcessWhenEffects(IRelativeState relativeState, IList<int> relevantConditionalEffects = null)
        {
            bool anyRelevant = false;
            int whenEffectIndex = -1;

            foreach (var whenEffect in Effects.WhenEffects)
            {
                ++whenEffectIndex;

                List<IEffect> subEffects = new List<IEffect>();
                whenEffect.Effects.ForEach(subEffect => subEffects.Add(subEffect));
                EffectsRelevanceRelativeStateEvaluator evaluator = new EffectsRelevanceRelativeStateEvaluator(subEffects, GroundingManager);

                var result = evaluator.EvaluateInternal(relativeState, OperatorSubstitution, relevantConditionalEffects);
                if (result == EffectRelevance.ANTI_RELEVANT)
                {
                    // anti-relevant conditional effect -> can't be used (but it doesn't have to be, we just ignore it)
                    continue;
                }

                if (result == EffectRelevance.RELEVANT)
                {
                    relevantConditionalEffects?.Add(whenEffectIndex);
                    anyRelevant = true;
                }
            }

            return (anyRelevant) ? EffectRelevance.RELEVANT : EffectRelevance.IRRELEVANT;
        }
    }
}
