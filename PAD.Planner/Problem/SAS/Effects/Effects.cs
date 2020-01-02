using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of SAS+ operator effects, containing individual effects and encapsuling the required functionality.
    /// </summary>
    public class Effects : List<IEffect>
    {
        /// <summary>
        /// Axiom rules of the SAS+ planning problem (implicit effects for any operator application).
        /// </summary>
        private AxiomRules AxiomRules { set; get; } = null;

        /// <summary>
        /// Cached item for collecting of used variables in the effects (optimization for IsRelevant() method).
        /// </summary>
        private HashSet<int> CashedVariablesCollector = new HashSet<int>();

        /// <summary>
        /// Constructs effects from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="axiomRules">Axiom rules of the SAS+ planning problem.</param>
        public Effects(InputData.SAS.Effects inputData, AxiomRules axiomRules)
        {
            inputData.ForEach(effect => Add((effect.Conditions.Count != 0) ? new ConditionalEffect(effect) : new PrimitiveEffect(effect)));
            AxiomRules = axiomRules;
        }

        /// <summary>
        /// Applies the operator effects to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        public IState Apply(IState state, bool directlyModify = false)
        {
            IState newState = (directlyModify) ? state : (IState)state.Clone();

            foreach (var effect in this)
            {
                // check applicability (conditional-effects) to the original state, not the modified one!
                if (effect.IsApplicable(state))
                {
                    effect.Apply(newState);
                }
            }
            AxiomRules.Apply(newState);

            return newState;
        }

        /// <summary>
        /// Checks whether the operator effects are relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Conditions for the application.</param>
        /// <param name="operatorPreconditions">Operator preconditions.</param>
        /// <returns>True if the operator effects are relevant to the given condititons, false otherwise.</returns>
        public bool IsRelevant(IConditions conditions, ISimpleConditions operatorPreconditions)
        {
            // Besides the effects relevance, we need to check whether the operator preconditions are not in conflict with the resulting
            // conditions (after backward-apply). This can be done via affected variables: if some effect is relevant, then it modifies a
            // variable, and a constraint corresponding to this variable can be removed from the preconditions. At the end, we check the
            // modified precondition constraints with the target conditions and if there are conflicts, we consider the operator not
            // relevant. This approach works even for conditional-effects, because we take the worst case scenario.

            IConditions nonAffectedPreconditions = (IConditions)operatorPreconditions.Clone();

            bool anyRelevant = false;
            foreach (var effect in this)
            {
                var relevance = effect.IsRelevant(conditions);
                if (relevance == EffectRelevance.ANTI_RELEVANT)
                {
                    return false;
                }
                else if (relevance == EffectRelevance.RELEVANT)
                {
                    anyRelevant = true;
                    nonAffectedPreconditions.RemoveConstraint(effect.GetAssignment());
                }
            }

            return (anyRelevant) ? !nonAffectedPreconditions.IsConflictedWith(conditions) : false;
        }

        /// <summary>
        /// Checks whether the operator effects are relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Relative state for the application.</param>
        /// <param name="operatorPreconditions">Operator preconditions.</param>
        /// <returns>True if the operator effects are relevant to the given relative state, false otherwise.</returns>
        public bool IsRelevant(IRelativeState relativeState, ISimpleConditions operatorPreconditions)
        {
            CashedVariablesCollector.Clear();

            bool anyRelevant = false;
            foreach (var effect in this)
            {
                var relevance = effect.IsRelevant(relativeState);
                switch (relevance)
                {
                    case EffectRelevance.ANTI_RELEVANT:
                    {
                        return false;
                    }
                    case EffectRelevance.RELEVANT:
                    {
                        anyRelevant = true;
                        CashedVariablesCollector.Add(effect.GetAssignment().GetVariable());
                        break;
                    }
                    case EffectRelevance.IRRELEVANT:
                    {
                        if (effect.GetConditions() != null)
                        {
                            CashedVariablesCollector.Add(effect.GetAssignment().GetVariable());
                        }
                        break;
                    }
                }
            }

            if (!anyRelevant)
            {
                return false;
            }

            // additionally, the relative state has to be checked against operator preconditions ("hidden effects")
            foreach (var constraint in operatorPreconditions)
            {
                int variable = constraint.GetVariable();
                if (!CashedVariablesCollector.Contains(variable))
                {
                    int value = relativeState.GetValue(variable);
                    if (value == RelativeState.WILD_CARD_VALUE)
                    {
                        continue;
                    }

                    if (value != constraint.GetValue())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Conditions for the application.</param>
        /// <param name="operatorPreconditions">Operator preconditions.</param>
        /// <returns>Preceding conditions.</returns>
        public IConditions ApplyBackwards(IConditions conditions, ISimpleConditions operatorPreconditions)
        {
            IConditions newConditions = (IConditions)conditions.Clone();

            foreach (var effect in this)
            {
                newConditions = effect.ApplyBackwards(newConditions);
            }
            newConditions = newConditions.ConjunctionWith(operatorPreconditions);

            return newConditions;
        }

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Relative state for the application.</param>
        /// <param name="operatorPreconditions">Operator preconditions.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<IRelativeState> ApplyBackwards(IRelativeState relativeState, ISimpleConditions operatorPreconditions)
        {
            IRelativeState result = (IRelativeState)relativeState.Clone();

            bool anyConditionalEffect = false;
            foreach (var effect in this)
            {
                if (effect.GetConditions() == null)
                {
                    result = effect.ApplyBackwards(result);
                }
                else
                {
                    anyConditionalEffect = true;
                }
            }

            foreach (var constraint in operatorPreconditions)
            {
                result.SetValue(constraint.GetVariable(), constraint.GetValue());
            }

            if (!anyConditionalEffect)
            {
                yield return result;
            }
            else
            {
                List<IRelativeState> states = new List<IRelativeState>() { result };

                foreach (var effect in this)
                {
                    if (effect.GetConditions() != null)
                    {
                        int statesCount = states.Count;
                        for (int i = 0; i < statesCount; ++i)
                        {
                            var newEffect = effect.ApplyBackwards(states[i]);
                            if (newEffect != null)
                            {
                                states.Add(newEffect);
                            }
                        }
                    }
                }

                foreach (var state in states)
                {
                    yield return state;
                }
            }
        }

        /// <summary>
        /// Checks whether the specified variable is actually affected by the operator effects. If the check is positive, returns
        /// also the effect application value.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Applied value.</param>
        /// <returns>True if the given variable is affected by the effects, false otherwise.</returns>
        public bool IsVariableAffected(int variable, out int value)
        {
            foreach (var effect in this)
            {
                IAssignment assignment = effect.GetAssignment();
                if (assignment.GetVariable() == variable)
                {
                    value = assignment.GetValue();
                    return true;
                }
            }
            value = Assignment.INVALID_VALUE;
            return false;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({string.Join(", ", this)})";
        }
    }
}
