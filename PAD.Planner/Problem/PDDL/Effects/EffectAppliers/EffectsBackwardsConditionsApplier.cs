using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Effects backwards applier, applying the relevant operators backwards to the given conditions.
    /// </summary>
    public class EffectsBackwardsConditionsApplier : IElementCNFBackwardsApplierVisitor
    {
        /// <summary>
        /// Structure for collecting and preprocessing of operator effects that are being evaluated.
        /// </summary>
        private EffectsPreprocessedCollection Effects { set; get; } = new EffectsPreprocessedCollection();

        /// <summary>
        /// Actually applied grounded predicate atoms.
        /// </summary>
        private HashSet<IAtom> UsedGroundedPredicates { set; get; } = new HashSet<IAtom>();

        /// <summary>
        /// Actually applied grounded function atoms.
        /// </summary>
        private HashSet<IAtom> UsedGroundedFunctions { set; get; } = new HashSet<IAtom>();

        /// <summary>
        /// Operator preconditions.
        /// </summary>
        private Conditions OperatorPreconditions { set; get; } = null;

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Evaluation manager.
        /// </summary>
        private EvaluationManager EvaluationManager { set; get; } = null;

        /// <summary>
        /// Variables substitution of the effects' parent operator.
        /// </summary>
        private ISubstitution OperatorSubstitution { set; get; } = null;

        /// <summary>
        /// Variables substitution of the expression (expression can be lifted e.g. in forall subexpressions).
        /// </summary>
        private ISubstitution ExpressionSubstitution { set; get; } = null;

        /// <summary>
        /// Is the backwards application lifted (i.e. only the minimal required operator substitution is used).
        /// </summary>
        private bool IsApplicationLifted { set; get; } = false;

        /// <summary>
        /// Constructs the effects backwards applier.
        /// </summary>
        /// <param name="operatorPreconditions">Corresponding operator preconditions.</param>
        /// <param name="operatorEffects">Corresponding operator effects.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        /// <param name="liftedApplication">Is the backward application lifted?</param>
        public EffectsBackwardsConditionsApplier(Conditions operatorPreconditions, List<IEffect> operatorEffects, EvaluationManager evaluationManager, bool liftedApplication = false)
        {
            OperatorPreconditions = (operatorPreconditions != null && operatorPreconditions.Count > 0) ? operatorPreconditions : null;
            GroundingManager = evaluationManager.GroundingManager;
            EvaluationManager = evaluationManager;
            IsApplicationLifted = liftedApplication;
            Effects.CollectEffects(operatorEffects);
        }

        /// <summary>
        /// Applies backwards the relevant operator effects and operator preconditions to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions to be modified.</param>
        /// <param name="operatorSubstitution">Variables substitution.</param>
        /// <returns>Preceding conditions.</returns>
        public IConditions ApplyBackwards(IConditions conditions, ISubstitution operatorSubstitution)
        {
            ConditionsCNF expression = (conditions != null) ? (ConditionsCNF)conditions.GetCNF() : null;

            if (expression == null)
            {
                return null;
            }

            OperatorSubstitution = operatorSubstitution;
            Effects.GroundEffectsByCurrentOperatorSubstitution(GroundingManager, operatorSubstitution);

            if (expression.Parameters != null)
            {
                EffectsRelevanceConditionsEvaluator evaluator = new EffectsRelevanceConditionsEvaluator(Effects, GroundingManager);

                List<ConditionsCNF> subResults = new List<ConditionsCNF>();
                IEnumerable<ISubstitution> expressionSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
                expression.Parameters = null;

                foreach (var expressionSubstitution in expressionSubstitutions)
                {
                    // do backward apply only when the effects are relevant to the current grounding of the expression
                    if (evaluator.Evaluate(expression, operatorSubstitution, expressionSubstitution))
                    {
                        subResults.Add(ApplyBackwardsImpl(expression, expressionSubstitution));
                    }
                }

                return ConstructCNFFromDisjunction(subResults);
            }

            return ApplyBackwardsImpl(expression, new Substitution());
        }

        /// <summary>
        /// Applies backwards the relevant operator effects and operator preconditions to the given target conditions.
        /// </summary>
        /// <param name="expression">Target conditions in CNF.</param>
        /// <param name="expressionSubstitution">Variables substitution of the expression.</param>
        /// <returns>Preceding conditions.</returns>
        private ConditionsCNF ApplyBackwardsImpl(ConditionsCNF expression, ISubstitution expressionSubstitution)
        {
            UsedGroundedPredicates.Clear();
            UsedGroundedFunctions.Clear();
            ExpressionSubstitution = expressionSubstitution;

            // remove positively contributing effects from the target conditions
            ConditionsCNF resultExpression = ProcessPrimitiveEffects(expression);
            resultExpression = ProcessForallEffects(resultExpression);
            resultExpression = ProcessWhenEffects(resultExpression);

            // unite processed conditions with partially grounded preconditions of the operator
            if (OperatorPreconditions != null)
            {
                var preconditions = ClearRigidRelations(OperatorPreconditions);
                resultExpression.Merge((ConditionsCNF)GroundConditions(preconditions).GetCNF());
            }

            return resultExpression;
        }

        /// <summary>
        /// Processes primitive effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        private ConditionsCNF ProcessPrimitiveEffects(ConditionsCNF expression)
        {
            // standard processing of simple effects via visitor pattern
            return (ConditionsCNF)expression.Accept(this);
        }

        /// <summary>
        /// Processes forall effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        private ConditionsCNF ProcessForallEffects(ConditionsCNF expression)
        {
            if (Effects.ForallEffects.Count == 0)
            {
                return expression;
            }

            foreach (var forallEffect in Effects.ForallEffects)
            {
                EffectsBackwardsConditionsApplier innerApplier = new EffectsBackwardsConditionsApplier(null, forallEffect.Effects, EvaluationManager);

                IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(forallEffect.Parameters);
                foreach (var localSubstitution in localSubstitutions)
                {
                    OperatorSubstitution.AddLocalSubstitution(localSubstitution);
                    expression = (ConditionsCNF)innerApplier.ApplyBackwards(expression, OperatorSubstitution);
                    OperatorSubstitution.RemoveLocalSubstitution(localSubstitution);
                }
            }
            return expression;
        }

        /// <summary>
        /// Processes conditional (when) effects.
        /// </summary>
        /// <param name="expression">Conditions expression in CNF.</param>
        private ConditionsCNF ProcessWhenEffects(ConditionsCNF expression)
        {
            if (Effects.WhenEffects.Count == 0)
            {
                return expression;
            }

            // Collect the relevant when effects, first
            List<WhenEffect> relevantWhenEffects = GetRelevantWhenEffectsForConditions(expression);

            // Each of the relevant when effect is either used or not (dynamic programming approach to get all combinations of when effects usage)
            List<ConditionsCNF> applicationResults = new List<ConditionsCNF>();
            applicationResults.Add(expression);

            foreach (var whenEffect in relevantWhenEffects)
            {
                Conditions whenCondition = new Conditions(whenEffect.Expression, EvaluationManager);
                EffectsBackwardsConditionsApplier innerApplier = new EffectsBackwardsConditionsApplier(whenCondition, new List<IEffect>(whenEffect.Effects), EvaluationManager);

                List<ConditionsCNF> currentEffectApplications = new List<ConditionsCNF>();
                foreach (var currentCondition in applicationResults)
                {
                    ConditionsCNF modifiedCondition = (ConditionsCNF)innerApplier.ApplyBackwards(currentCondition, new Substitution());
                    currentEffectApplications.Add(modifiedCondition);
                }

                applicationResults.AddRange(currentEffectApplications);
            }

            // Now, all the possible results need to be merged into a single condition via OR and converted into a valid CNF
            return ConstructCNFFromDisjunction(applicationResults);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public IElementCNF Visit(PredicateLiteralCNF expression)
        {
            var positivePredicateEffects = Effects.GroundedPositivePredicateEffects;
            var negativePredicateEffects = Effects.GroundedNegativePredicateEffects;

            if ((!expression.IsNegated && positivePredicateEffects.Count == 0) || (expression.IsNegated && negativePredicateEffects.Count == 0))
            {
                return expression.Clone();
            }

            IAtom atom = GroundAtom(expression.PredicateAtom);

            bool positivelyContibuting = (expression.IsNegated) ? negativePredicateEffects.Contains(atom) : positivePredicateEffects.Contains(atom);
            if (positivelyContibuting)
            {
                UsedGroundedPredicates.Add(atom);
                return null;
            }

            return expression.Clone();
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public IElementCNF Visit(EqualsLiteralCNF expression)
        {
            var objectAssignEffects = Effects.GroundedObjectFunctionAssignmentEffects;
            if (objectAssignEffects.Count == 0)
            {
                return expression.Clone();
            }

            Func<ITerm, ITerm> TransformArgument = null;
            TransformArgument = (ITerm term) =>
            {
                ObjectFunctionTerm objectFunction = term as ObjectFunctionTerm;
                if (objectFunction != null)
                {
                    ITerm assignmentValue = null;
                    if (objectAssignEffects.TryGetValue(GroundAtom(objectFunction.FunctionAtom), out assignmentValue))
                    {
                        UsedGroundedFunctions.Add(objectFunction.FunctionAtom);
                        return TransformArgument(assignmentValue);
                    }
                }
                return term.Clone();
            };

            ITerm transformedLeftTerm = TransformArgument(expression.LeftArgument);
            ITerm transformedRightTerm = TransformArgument(expression.RightArgument);

            bool termsEqual = transformedLeftTerm.Equals(transformedRightTerm);
            if ((termsEqual && !expression.IsNegated) || (!termsEqual && expression.IsNegated))
            {
                // exact constant assignment (or unassignment with negated equals) -> positively contributing, i.e. remove
                return null;
            }

            return new EqualsLiteralCNF(transformedLeftTerm, transformedRightTerm, expression.IsNegated);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public IElementCNF Visit(NumericCompareLiteralCNF expression)
        {
            // 1.) Evaluates both of the arguments by NumericAssignmentsBackwardsReplacer - every numeric function instance is replaced by
            //     the available numeric assignments from effects and the whole numeric expression is reduced (partially or fully evaluated).
            // 2.) In case of the full reduction of both arguments, the numeric comparison is evaluated - if successfully, the whole condition
            //     is satisfied and we can remove it (i.e. return null).
            // 3.) Otherwise, the modified numeric compare expression is returned, replacing the original condition.

            var numericAssignEffects = Effects.GroundedNumericFunctionAssignmentEffects;
            if (numericAssignEffects.Count == 0)
            {
                // no assignment effects available, just return a copy of the original expression
                return expression.Clone();
            }

            NumericAssignmentsBackwardsReplacer replacer = new NumericAssignmentsBackwardsReplacer(numericAssignEffects, GroundingManager, OperatorSubstitution, ExpressionSubstitution);
            INumericExpression newLeftNumericExpression = replacer.Replace(expression.LeftArgument);
            INumericExpression newRightNumericExpression = replacer.Replace(expression.RightArgument);

            UsedGroundedFunctions.UnionWith(replacer.ReplacedFunctionAtomsInSubExpression);

            Number leftNumber = newLeftNumericExpression as Number;
            Number rightNumber = newRightNumericExpression as Number;
            if (leftNumber != null && rightNumber != null)
            {
                bool relationHolds = NumericCompareExpression.ApplyCompare(expression.Operator, leftNumber.Value, rightNumber.Value);
                if ((relationHolds && !expression.IsNegated) || (!relationHolds && expression.IsNegated))
                {
                    // the numeric comparison was successful -> condition is satisfied, i.e. remove
                    return null;
                }
            }

            // modified numeric compare expression, replacing the original one
            return new NumericCompareLiteralCNF(expression.Operator, newLeftNumericExpression, newRightNumericExpression, expression.IsNegated);
        }

        /// <summary>
        /// Grounds the specified atom.
        /// </summary>
        /// <param name="atom">Atom.</param>
        /// <returns>Grounded atom.</returns>
        private IAtom GroundAtom(IAtom atom)
        {
            return GroundingManager.GroundAtom(atom, ExpressionSubstitution);
        }

        /// <summary>
        /// Grounds the specified term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Grounded term.</returns>
        private ITerm GroundTerm(ITerm term)
        {
            return GroundingManager.GroundTerm(term, ExpressionSubstitution);
        }

        /// <summary>
        /// Grounds the specified conditions by a minimal needed operator substitution.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Grounded conditions.</returns>
        private Conditions GroundConditions(Conditions conditions)
        {
            return GroundingManager.GroundConditions(conditions, IsApplicationLifted ? ExtractMinimalOperatorSubstitution() : OperatorSubstitution);
        }

        /// <summary>
        /// Extracts a minimal necessary operator substitution needed for the backward application (other parameters remain lifted).
        /// </summary>
        /// <returns>Minimal substitution.</returns>
        public ISubstitution ExtractMinimalOperatorSubstitution()
        {
            // we need to determine, which effects were actually used for the backwards application (i.e. have to be grounded).

            ISubstitution substitution = new Substitution();

            foreach (var predicate in UsedGroundedPredicates)
            {
                IAtom liftedPredicate = null;
                if (Effects.OriginalLiftedPredicates.TryGetValue(predicate, out liftedPredicate))
                {
                    var unification = liftedPredicate.GetUnificationWith(predicate);
                    substitution.AddLocalSubstitution(unification);
                }
            }

            foreach (var function in UsedGroundedFunctions)
            {
                IAtom liftedFunction = null;
                if (Effects.OriginalLiftedFunctions.TryGetValue(function, out liftedFunction))
                {
                    var unification = liftedFunction.GetUnificationWith(function);
                    substitution.AddLocalSubstitution(unification);
                }
            }

            return substitution;
        }

        /// <summary>
        /// Collects relevant when effects for the specified conditions.
        /// </summary>
        /// <param name="expression">Conditions in CNF.</param>
        /// <returns>List of relevant when effects.</returns>
        private List<WhenEffect> GetRelevantWhenEffectsForConditions(ConditionsCNF expression)
        {
            List<int> relevantWhenEffectsIndices = new List<int>();
            EffectsRelevanceConditionsEvaluator relevanceEvaluator = new EffectsRelevanceConditionsEvaluator(Effects, GroundingManager);
            relevanceEvaluator.Evaluate(expression, new Substitution(), ExpressionSubstitution, relevantWhenEffectsIndices); // empty substitution here because effects already grounded

            List<WhenEffect> relevantWhenEffects = new List<WhenEffect>();
            relevantWhenEffectsIndices.ForEach(index => relevantWhenEffects.Add(Effects.WhenEffects[index]));
            return relevantWhenEffects;
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

        /// <summary>
        /// Constructs a new CNF from the given disjunction of CNFs.
        /// </summary>
        /// <param name="cnfList">List of possible CNF expressions.</param>
        /// <returns>CNF expression of the CNFs disjunction.</returns>
        private ConditionsCNF ConstructCNFFromDisjunction(List<ConditionsCNF> cnfList)
        {
            if (cnfList.Count == 1)
            {
                return cnfList[0];
            }

            // adjust the CNF list, so their parameters don't collide before they will be merged
            Parameters mergedParameters;
            MakeUniqueParametersForCNFList(cnfList, out mergedParameters);

            HashSet<IConjunctCNF> conjuncts = new HashSet<IConjunctCNF>();

            foreach (var andExpr in cnfList)
            {
                HashSet<IConjunctCNF> newConjuncts = new HashSet<IConjunctCNF>();
                foreach (var andElem in andExpr)
                {
                    HashSet<LiteralCNF> andElemItems = new HashSet<LiteralCNF>();
                    if (andElem is ClauseCNF)
                    {
                        ClauseCNF orExpr = andElem as ClauseCNF;
                        foreach (var child in orExpr)
                        {
                            andElemItems.Add(child);
                        }
                    }
                    else
                    {
                        andElemItems.Add((LiteralCNF)andElem);
                    }

                    if (conjuncts.Count == 0)
                    {
                        newConjuncts.Add(new ClauseCNF(andElemItems));
                    }
                    else
                    {
                        foreach (var conjunct in conjuncts)
                        {
                            if (conjunct is ClauseCNF)
                            {
                                ClauseCNF clause = conjunct as ClauseCNF;
                                HashSet<LiteralCNF> newOrChildren = new HashSet<LiteralCNF>();
                                foreach (var child in clause)
                                {
                                    newOrChildren.Add(child);
                                }
                                newOrChildren.UnionWith(andElemItems);
                                newConjuncts.Add(new ClauseCNF(newOrChildren));
                            }
                            else
                            {
                                andElemItems.Add((LiteralCNF)conjunct);
                                if (andElemItems.Count > 1)
                                {
                                    newConjuncts.Add(new ClauseCNF(andElemItems));
                                }
                                else
                                {
                                    newConjuncts.Add(conjunct);
                                }
                            }
                        }
                    }
                }
                conjuncts = newConjuncts;
            }

            return new ConditionsCNF(conjuncts, EvaluationManager, mergedParameters);
        }

        /// <summary>
        /// Adjusts the given CNF list so that their parameters don't collide (they are renamed) and returns the merged parameters.
        /// </summary>
        /// <param name="cnfList">List of CNFs.</param>
        /// <param name="mergedParameters">Output merged parameters.</param>
        private void MakeUniqueParametersForCNFList(List<ConditionsCNF> cnfList, out Parameters mergedParameters)
        {
            mergedParameters = new Parameters();

            ConditionsParametersRenamer renamer = new ConditionsParametersRenamer();
            int firstFreeParameterID = 0;
            bool first = true;

            foreach (var cnf in cnfList)
            {
                if (cnf.Parameters == null)
                {
                    continue;
                }

                if (first)
                {
                    firstFreeParameterID = cnf.Parameters.GetMaxUsedParameterID() + 1;
                    first = false;
                    continue;
                }

                renamer.Rename(cnf, firstFreeParameterID);
                firstFreeParameterID = cnf.Parameters.GetMaxUsedParameterID() + 1;

                mergedParameters.Add(cnf.Parameters);
            }
        }
    }
}
