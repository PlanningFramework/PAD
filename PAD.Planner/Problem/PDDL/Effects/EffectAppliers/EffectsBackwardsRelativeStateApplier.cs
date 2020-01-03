using System.Collections.Generic;
using System.Linq;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Effects backwards applier, applying the relevant operators backwards to the given conditions.
    /// </summary>
    public class EffectsBackwardsRelativeStateApplier
    {
        /// <summary>
        /// Structure for collecting and preprocessing of operator effects that are being evaluated.
        /// </summary>
        private EffectsPreprocessedCollection Effects { get; } = new EffectsPreprocessedCollection();

        /// <summary>
        /// Operator preconditions.
        /// </summary>
        private Conditions OperatorPreconditions { get; }

        /// <summary>
        /// Evaluation manager.
        /// </summary>
        private EvaluationManager EvaluationManager { get; }

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Variables substitution of the effects' parent operator.
        /// </summary>
        private ISubstitution OperatorSubstitution { set; get; }

        /// <summary>
        /// Constructs the effects backwards applier.
        /// </summary>
        /// <param name="operatorPreconditions">Corresponding operator preconditions.</param>
        /// <param name="operatorEffects">Corresponding operator effects.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public EffectsBackwardsRelativeStateApplier(Conditions operatorPreconditions, List<IEffect> operatorEffects, EvaluationManager evaluationManager)
        {
            OperatorPreconditions = (operatorPreconditions != null && operatorPreconditions.Count > 0) ? operatorPreconditions : null;
            EvaluationManager = evaluationManager;
            GroundingManager = evaluationManager.GroundingManager;
            Effects.CollectEffects(operatorEffects);
        }

        /// <summary>
        /// Applies backwards the relevant operator effects and operator preconditions to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state to be modified.</param>
        /// <param name="operatorSubstitution">Variables substitution.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<Planner.IRelativeState> ApplyBackwards(IRelativeState relativeState, ISubstitution operatorSubstitution)
        {
            OperatorSubstitution = operatorSubstitution;
            Effects.GroundEffectsByCurrentOperatorSubstitution(GroundingManager, operatorSubstitution);

            relativeState = (IRelativeState)relativeState.Clone();

            // prepare operator preconditions
            var operatorPreconditions = (OperatorPreconditions != null) ? GroundingManager.GroundConditions(ClearRigidRelations(OperatorPreconditions), OperatorSubstitution) : null;

            // remove positively contributing effects from the relative state and insert the operator preconditions
            ProcessPrimitiveEffects(relativeState);
            relativeState = ProcessForallEffects(relativeState);
            foreach (var resultState in ProcessWhenEffects(relativeState))
            {
                if (operatorPreconditions != null)
                {
                    foreach (var modifiedResultState in ProcessOperatorPreconditions(operatorPreconditions, resultState))
                    {
                        yield return modifiedResultState;
                    }
                }
                else
                {
                    yield return resultState;
                }
            }
        }

        /// <summary>
        /// Processes primitive effects.
        /// </summary>
        /// <param name="relativeState">Relative state to be modified.</param>
        private void ProcessPrimitiveEffects(IRelativeState relativeState)
        {
            foreach (var positivePredicate in Effects.GroundedPositivePredicateEffects)
            {
                if (relativeState.HasPredicate(positivePredicate))
                {
                    relativeState.RemovePredicate(positivePredicate);
                }
            }

            foreach (var negatedPredicate in Effects.GroundedNegativePredicateEffects)
            {
                if (relativeState.HasNegatedPredicate(negatedPredicate))
                {
                    relativeState.RemoveNegatedPredicate(negatedPredicate);
                }
            }

            foreach (var objectFunction in Effects.GroundedObjectFunctionAssignmentEffects)
            {
                var groundedValue = GroundingManager.GroundTermDeep(objectFunction.Value, OperatorSubstitution, relativeState);

                ConstantTerm constantValue = groundedValue as ConstantTerm;
                if (constantValue != null)
                {
                    if (relativeState.GetObjectFunctionValue(objectFunction.Key) == constantValue.NameId)
                    {
                        relativeState.AssignObjectFunction(objectFunction.Key, ObjectFunctionTerm.UndefinedValue);
                    }
                }
            }

            foreach (var numericFunction in Effects.GroundedNumericFunctionAssignmentEffects)
            {
                NumericAssignmentsBackwardsReplacer replacer = new NumericAssignmentsBackwardsReplacer(Effects.GroundedNumericFunctionAssignmentEffects, GroundingManager, OperatorSubstitution, new Substitution());
                INumericExpression reducedAssignExpression = replacer.Replace(numericFunction.Value);

                Number assignNumber = reducedAssignExpression as Number;
                if (assignNumber != null)
                {
                    if (relativeState.GetNumericFunctionValue(numericFunction.Key).Equals(assignNumber.Value))
                    {
                        relativeState.AssignNumericFunction(numericFunction.Key, NumericFunction.UndefinedValue);
                    }
                }
            }
        }

        /// <summary>
        /// Processes forall effects.
        /// </summary>
        /// <param name="relativeState">Relative state to be modified.</param>
        /// <returns>Possibly modified relative state.</returns>
        private IRelativeState ProcessForallEffects(IRelativeState relativeState)
        {
            if (Effects.ForallEffects.Count == 0)
            {
                return relativeState;
            }

            foreach (var forallEffect in Effects.ForallEffects)
            {
                EffectsBackwardsRelativeStateApplier innerApplier = new EffectsBackwardsRelativeStateApplier(null, forallEffect.Effects, EvaluationManager);

                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(forallEffect.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    OperatorSubstitution.AddLocalSubstitution(localSubstitution);
                    relativeState = (IRelativeState)innerApplier.ApplyBackwards(relativeState, OperatorSubstitution).First();
                    OperatorSubstitution.RemoveLocalSubstitution(localSubstitution);
                }
            }

            return relativeState;
        }

        /// <summary>
        /// Processes conditional (when) effects.
        /// </summary>
        /// <param name="relativeState">>Relative state to be processed.</param>
        /// <returns>Modified relative states.</returns>
        private IEnumerable<IRelativeState> ProcessWhenEffects(IRelativeState relativeState)
        {
            if (Effects.WhenEffects.Count == 0)
            {
                yield return relativeState;
                yield break;
            }

            // Collect the relevant when effects, first
            List<WhenEffect> relevantWhenEffects = GetRelevantWhenEffectsForConditions(relativeState);

            // Each of the relevant when effect is either used or not (dynamic programming approach to get all combinations of when effects usage)
            List<IRelativeState> applicationResults = new List<IRelativeState> {relativeState};

            foreach (var whenEffect in relevantWhenEffects)
            {
                Conditions whenCondition = new Conditions(whenEffect.Expression, EvaluationManager);
                EffectsBackwardsRelativeStateApplier innerApplier = new EffectsBackwardsRelativeStateApplier(whenCondition, new List<IEffect>(whenEffect.Effects), EvaluationManager);

                List<IRelativeState> currentEffectApplications = new List<IRelativeState>();
                foreach (var currentRelativeState in applicationResults)
                {
                    foreach (var resultRelativeState in innerApplier.ApplyBackwards(currentRelativeState, new Substitution()))
                    {
                        currentEffectApplications.Add((IRelativeState)resultRelativeState);
                    }
                }

                applicationResults.AddRange(currentEffectApplications);
            }

            foreach (var resultRelativeState in applicationResults)
            {
                yield return resultRelativeState;
            }
        }

        /// <summary>
        /// Collects relevant when effects for the specified relative state.
        /// </summary>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>List of relevant when effects.</returns>
        private List<WhenEffect> GetRelevantWhenEffectsForConditions(IRelativeState relativeState)
        {
            List<int> relevantWhenEffectsIndices = new List<int>();
            EffectsRelevanceRelativeStateEvaluator relevanceEvaluator = new EffectsRelevanceRelativeStateEvaluator(Effects, GroundingManager);
            relevanceEvaluator.Evaluate(relativeState, new Substitution(), relevantWhenEffectsIndices); // empty substitution here because effects already grounded

            List<WhenEffect> relevantWhenEffects = new List<WhenEffect>();
            relevantWhenEffectsIndices.ForEach(index => relevantWhenEffects.Add(Effects.WhenEffects[index]));
            return relevantWhenEffects;
        }

        /// <summary>
        /// Applies the operator preconditions to the given relative state.
        /// </summary>
        /// <param name="operatorPreconditions">Grounded operator preconditions.</param>
        /// <param name="relativeState">Relative state.</param>
        /// <returns>Modified relative states.</returns>
        private static IEnumerable<IRelativeState> ProcessOperatorPreconditions(Conditions operatorPreconditions, IRelativeState relativeState)
        {
            var conditionsCNF = (ConditionsCNF)operatorPreconditions.GetCNF();

            HashSet<IRelativeState> states = new HashSet<IRelativeState> { relativeState };
            foreach (var conjunct in conditionsCNF)
            {
                // this block processes all possible combinations of applications in a single clause (even though it is a primitive clause of one literal)

                HashSet<IRelativeState> newStatesForConjunct = new HashSet<IRelativeState>();

                foreach (var literal in conjunct.GetLiterals())
                {
                    HashSet<IRelativeState> newStatesForLiteral = new HashSet<IRelativeState>();

                    foreach (var state in states)
                    {
                        IRelativeState newState = (IRelativeState)state.Clone();
                        ProcessPreconditionLiteral(literal, newState);
                        newStatesForLiteral.Add(newState);
                    }

                    foreach (var state in newStatesForConjunct)
                    {
                        IRelativeState newState = (IRelativeState)state.Clone();
                        ProcessPreconditionLiteral(literal, newState);
                        newStatesForLiteral.Add(newState);
                    }

                    newStatesForConjunct.UnionWith(newStatesForLiteral);
                }

                states = newStatesForConjunct;
            }

            foreach (var resultState in states)
            {
                yield return resultState;
            }
        }

        /// <summary>
        /// Processes a single CNF literal of operator preconditions.
        /// </summary>
        /// <param name="literal">CNF literal.</param>
        /// <param name="state">Relative state to be applied to.</param>
        private static void ProcessPreconditionLiteral(LiteralCNF literal, IRelativeState state)
        {
            PredicateLiteralCNF predicateLiteral = literal as PredicateLiteralCNF;
            if (predicateLiteral != null)
            {
                if (predicateLiteral.IsNegated)
                {
                    state.AddNegatedPredicate(predicateLiteral.PredicateAtom);
                }
                else
                {
                    state.AddPredicate(predicateLiteral.PredicateAtom);
                }
                return;
            }

            EqualsLiteralCNF equalsLiteral = literal as EqualsLiteralCNF;
            if (equalsLiteral != null)
            {
                ObjectFunctionTerm objFunc = equalsLiteral.LeftArgument as ObjectFunctionTerm;
                ConstantTerm constTerm = equalsLiteral.RightArgument as ConstantTerm;

                if (objFunc == null || constTerm == null)
                {
                    objFunc = equalsLiteral.RightArgument as ObjectFunctionTerm;
                    constTerm = equalsLiteral.LeftArgument as ConstantTerm;
                }

                if (objFunc != null && constTerm != null)
                {
                    if (equalsLiteral.IsNegated)
                    {
                        if (state.GetObjectFunctionValue(objFunc.FunctionAtom) == constTerm.NameId)
                        {
                            state.AssignObjectFunction(objFunc.FunctionAtom, IdManager.InvalidId);
                        }
                    }
                    else
                    {
                        state.AssignObjectFunction(objFunc.FunctionAtom, constTerm.NameId);
                    }
                }
                return;
            }

            NumericCompareLiteralCNF compareLiteral = literal as NumericCompareLiteralCNF;
            if (compareLiteral != null)
            {
                if (compareLiteral.Operator != NumericCompareExpression.RelationalOperator.EQ)
                {
                    return;
                }

                NumericFunction numFunc = compareLiteral.LeftArgument as NumericFunction;
                Number number = compareLiteral.RightArgument as Number;

                if (numFunc == null || number == null)
                {
                    numFunc = compareLiteral.RightArgument as NumericFunction;
                    number = compareLiteral.LeftArgument as Number;
                }

                if (numFunc != null && number != null)
                {
                    if (compareLiteral.IsNegated)
                    {
                        if (state.GetNumericFunctionValue(numFunc.FunctionAtom).Equals(number.Value))
                        {
                            state.AssignNumericFunction(numFunc.FunctionAtom, NumericFunction.DefaultValue);
                        }
                    }
                    else
                    {
                        state.AssignNumericFunction(numFunc.FunctionAtom, number.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Removes rigid relations from the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Conditions without rigid relations.</returns>
        private Conditions ClearRigidRelations(Conditions conditions)
        {
            Conditions newConditions = conditions.CloneEmpty();
            foreach (var expression in conditions)
            {
                PredicateExpression predicate = expression as PredicateExpression;
                if (predicate != null)
                {
                    if (EvaluationManager.IsPredicateRigidRelation(predicate.PredicateAtom))
                    {
                        continue;
                    }
                }
                newConditions.Add(expression);
            }

            return newConditions;
        }
    }
}
