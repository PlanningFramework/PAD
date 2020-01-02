using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evalutor of the operator effects relevance. The operator is relevant for the specified conditions, if it contributes to satisfy the
    /// conditions. This operator can be then used in the backward search and reverse application on the given conditions.
    /// </summary>
    public class EffectsRelevanceConditionsEvaluator : IElementCNFRelevanceEvaluationVisitor
    {
        /// <summary>
        /// Structure for collecting and preprocessing of operator effects that are being evaluated.
        /// </summary>
        private EffectsPreprocessedCollection Effects { set; get; } = new EffectsPreprocessedCollection();

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Variables substitution of the effects' parent operator.
        /// </summary>
        private ISubstitution OperatorSubstitution { set; get; } = null;

        /// <summary>
        /// Variables substitution of the expression (expression can be lifted e.g. in forall subexpressions).
        /// </summary>
        private ISubstitution ExpressionSubstitution { set; get; } = null;

        /// <summary>
        /// Constructs the effects relevance evaluator.
        /// </summary>
        /// <param name="effects">Effects for the evaluation.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        public EffectsRelevanceConditionsEvaluator(List<IEffect> effects, GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
            Effects.CollectEffects(effects);
        }

        /// <summary>
        /// Constructs the effects relevance evaluator.
        /// </summary>
        /// <param name="effects">Preprocessed effects for the evaluation.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        public EffectsRelevanceConditionsEvaluator(EffectsPreprocessedCollection effects, GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
            Effects = effects;
        }

        /// <summary>
        /// Evaluates whether the operator effects are relevant for the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions expression.</param>
        /// <param name="operatorSubstitution">Variables substitution of the operator.</param>
        /// <param name="expressionSubstitution">Variables substitution of the expression.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True when the effects are relevant, false otherwise.</returns>
        public bool Evaluate(IConditions conditions, ISubstitution operatorSubstitution, ISubstitution expressionSubstitution = null, IList<int> relevantContionalEffects = null)
        {
            var result = EvaluateWithExtendedResult(conditions, operatorSubstitution, (expressionSubstitution == null) ? new Substitution() : expressionSubstitution, relevantContionalEffects);
            return (result.Item1 && result.Item2);
        }

        /// <summary>
        /// Evaluates whether the operator effects are relevant for the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions expression.</param>
        /// <param name="operatorSubstitution">Variables substitution of the operator.</param>
        /// <param name="expressionSubstitution">Variables substitution of the expression.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>Tuple of two values, where the first is true when the positive relevance condition (inclusion) is satisfied, while the second is
        /// true when the negative condition (exclusion) is not violated. False otherwise.</returns>
        public Tuple<bool, bool> EvaluateWithExtendedResult(IConditions conditions, ISubstitution operatorSubstitution, ISubstitution expressionSubstitution, IList<int> relevantContionalEffects = null)
        {
            ConditionsCNF expression = (conditions != null) ? (ConditionsCNF)conditions.GetCNF() : null;

            if (expression == null)
            {
                return Tuple.Create(false, false);
            }

            Effects.GroundEffectsByCurrentOperatorSubstitution(GroundingManager, operatorSubstitution);

            OperatorSubstitution = operatorSubstitution;
            ExpressionSubstitution = expressionSubstitution;

            var primitivesResult = ProcessPrimitiveEffects(expression);
            if (!primitivesResult.Item2)
            {
                return primitivesResult;
            }

            var forallResult = ProcessForallEffects(expression, relevantContionalEffects);
            if (!forallResult.Item2)
            {
                return forallResult;
            }

            var whenResult = ProcessWhenEffects(expression, relevantContionalEffects);
            if (!whenResult.Item2)
            {
                return whenResult;
            }

            return Tuple.Create((primitivesResult.Item1 || forallResult.Item1 || whenResult.Item1), true);
        }

        /// <summary>
        /// Processes primitive effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        /// <returns>Tuple of two values, where the first is true when the positive relevance condition (inclusion) is satisfied, while the second is
        /// true when the negative condition (exclusion) is not violated. False otherwise.</returns>
        private Tuple<bool, bool> ProcessPrimitiveEffects(ConditionsCNF expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Processes forall effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>Tuple of two values, where the first is true when the positive relevance condition (inclusion) is satisfied, while the second is
        /// true when the negative condition (exclusion) is not violated. False otherwise.</returns>
        private Tuple<bool, bool> ProcessForallEffects(ConditionsCNF expression, IList<int> relevantContionalEffects)
        {
            bool positiveCondition = false;
            bool negativeCondition = true;

            foreach (var forallEffect in Effects.ForallEffects)
            {
                EffectsRelevanceConditionsEvaluator evaluator = new EffectsRelevanceConditionsEvaluator(forallEffect.Effects, GroundingManager);

                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(forallEffect.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    OperatorSubstitution.AddLocalSubstitution(localSubstitution);

                    var result = evaluator.EvaluateWithExtendedResult(expression, OperatorSubstitution, null);
                    positiveCondition |= result.Item1;
                    negativeCondition &= result.Item2;

                    OperatorSubstitution.RemoveLocalSubstitution(localSubstitution);

                    if (!negativeCondition)
                    {
                        return result;
                    }
                }
            }
            return Tuple.Create(positiveCondition, negativeCondition);
        }

        /// <summary>
        /// Processes conditional (when) effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>Tuple of two values, where the first is true when the positive relevance condition (inclusion) is satisfied, while the second is
        /// true when the negative condition (exclusion) is not violated. False otherwise.</returns>
        private Tuple<bool, bool> ProcessWhenEffects(ConditionsCNF expression, IList<int> relevantContionalEffects)
        {
            bool positiveCondition = false;
            int whenEffectIndex = -1;

            foreach (var whenEffect in Effects.WhenEffects)
            {
                ++whenEffectIndex;

                List<IEffect> subEffects = new List<IEffect>();
                whenEffect.Effects.ForEach(subEffect => subEffects.Add(subEffect));
                EffectsRelevanceConditionsEvaluator evaluator = new EffectsRelevanceConditionsEvaluator(subEffects, GroundingManager);

                var result = evaluator.EvaluateWithExtendedResult(expression, OperatorSubstitution, ExpressionSubstitution, relevantContionalEffects);
                if (!result.Item2)
                {
                    continue;
                }

                if (result.Item1 && result.Item2)
                {
                    if (relevantContionalEffects != null)
                    {
                        relevantContionalEffects.Add(whenEffectIndex);
                    }
                }
                positiveCondition |= result.Item1;
            }
            return Tuple.Create(positiveCondition, true);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public Tuple<bool, bool> Visit(PredicateLiteralCNF expression)
        {
            bool hasPositive = Effects.GroundedPositivePredicateEffects.Contains(GroundingManager.GroundAtom(expression.PredicateAtom, ExpressionSubstitution));
            bool hasNegative = Effects.GroundedNegativePredicateEffects.Contains(GroundingManager.GroundAtom(expression.PredicateAtom, ExpressionSubstitution));

            bool positiveCondition = (expression.IsNegated) ? hasNegative : hasPositive;
            bool negativeCondition = !((expression.IsNegated) ? hasPositive : hasNegative);

            return Tuple.Create(positiveCondition, negativeCondition);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public Tuple<bool, bool> Visit(EqualsLiteralCNF expression)
        {
            bool positiveCondition = false;
            bool negativeCondition = true;

            Action<ITerm, ITerm> CheckObjectFunctionArgument = (ITerm argument, ITerm secondaryArgument) =>
            {
                ObjectFunctionTerm objectFunction = argument as ObjectFunctionTerm;
                if (objectFunction != null)
                {
                    ITerm assignValue = null;
                    if (Effects.GroundedObjectFunctionAssignmentEffects.TryGetValue(GroundingManager.GroundAtom(objectFunction.FunctionAtom, ExpressionSubstitution), out assignValue))
                    {
                        positiveCondition |= true;
                        if (!(secondaryArgument is ObjectFunctionTerm) && !(assignValue is ObjectFunctionTerm))
                        {
                            bool valueDiffersValueAssign = !GroundingManager.GroundTerm(secondaryArgument, ExpressionSubstitution).Equals(GroundingManager.GroundTerm(assignValue, OperatorSubstitution));
                            if (valueDiffersValueAssign && !expression.IsNegated || !valueDiffersValueAssign && expression.IsNegated)
                            {
                                // surely assigning different value (or assigning exact value with negated equals) -> not relevant
                                negativeCondition = false;
                            }
                        }
                    }
                }
            };

            CheckObjectFunctionArgument(expression.LeftArgument, expression.RightArgument);
            CheckObjectFunctionArgument(expression.RightArgument, expression.LeftArgument);

            return Tuple.Create(positiveCondition, negativeCondition);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public Tuple<bool, bool> Visit(NumericCompareLiteralCNF expression)
        {
            HashSet<NumericFunction> numericFunctions = new HashSet<NumericFunction>();

            NumericFunctionsCollector collector = new NumericFunctionsCollector();
            numericFunctions.UnionWith(collector.Collect(expression.LeftArgument));
            numericFunctions.UnionWith(collector.Collect(expression.RightArgument));

            bool positiveCondition = false;
            foreach (var numericFunction in numericFunctions)
            {
                positiveCondition |= Effects.GroundedNumericFunctionAssignmentEffects.ContainsKey(GroundingManager.GroundAtom(numericFunction.FunctionAtom, ExpressionSubstitution));
            }
            return Tuple.Create(positiveCondition, true);
        }
    }
}
