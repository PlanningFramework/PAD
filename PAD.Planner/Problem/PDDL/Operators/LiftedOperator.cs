using System.Collections.Generic;
// ReSharper disable StringLiteralTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Lifted version of the PDDL operator. With a specific substitution we get a grounded version (PDDL.Operator).
    /// </summary>
    public class LiftedOperator
    {
        /// <summary>
        /// Name of the operator.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Input parameters of the operator.
        /// </summary>
        public Parameters Parameters { set; get; }

        /// <summary>
        /// Preconditions for the operator applicability.
        /// </summary>
        public Conditions Preconditions { set; get; }

        /// <summary>
        /// Effects of the operator application.
        /// </summary>
        public Effects Effects { set; get; }

        /// <summary>
        /// Cost of the operator.
        /// </summary>
        public int Cost { set; get; } = DefaultOperatorCost;

        /// <summary>
        /// Default operator cost (if not specified in operator effects).
        /// </summary>
        public const int DefaultOperatorCost = 1;

        /// <summary>
        /// ID manager.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Constructs the object from the input data.
        /// </summary>
        /// <param name="action">PDDL action definition.</param>
        /// <param name="idManager">ID manager.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public LiftedOperator(InputData.PDDL.Action action, IdManager idManager, EvaluationManager evaluationManager)
        {
            idManager.Variables.RegisterLocalParameters(action.Parameters);

            Name = action.Name;
            Parameters = new Parameters(action.Parameters, idManager);
            Preconditions = new Conditions(action.Preconditions, Parameters, idManager, evaluationManager);
            Effects = new Effects(action.Effects, Preconditions, idManager, evaluationManager);
            IdManager = idManager;

            ExtractCostFromEffects();

            idManager.Variables.UnregisterLocalParameters(action.Parameters);
        }

        /// <summary>
        /// Checks whether the operator is applicable to the given state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>True if the operator is applicable to the given state, false otherwise.</returns>
        public bool IsApplicable(IState state, ISubstitution substitution)
        {
            return Preconditions.Evaluate(state, substitution);
        }

        /// <summary>
        /// Applies the operator to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        public IState Apply(IState state, ISubstitution substitution, bool directlyModify = false)
        {
            return Effects.Apply(state, substitution, directlyModify);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="relevantConditionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True if the operator is relevant to the given conditions, false otherwise.</returns>
        public bool IsRelevant(IConditions conditions, ISubstitution substitution, IList<int> relevantConditionalEffects = null)
        {
            if (!Preconditions.EvaluateRigidRelationsCompliance(substitution))
            {
                return false;
            }
            return Effects.IsRelevant(conditions, substitution, relevantConditionalEffects);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="relevantConditionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True if the operator is relevant to the given relative state, false otherwise.</returns>
        public bool IsRelevant(IRelativeState relativeState, ISubstitution substitution, IList<int> relevantConditionalEffects = null)
        {
            if (!Preconditions.EvaluateRigidRelationsCompliance(substitution))
            {
                return false;
            }
            return Effects.IsRelevant(relativeState, substitution, relevantConditionalEffects);
        }

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Preceding conditions.</returns>
        public IConditions ApplyBackwards(IConditions conditions, ISubstitution substitution)
        {
            return Effects.ApplyBackwards(conditions, substitution);
        }

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<Planner.IRelativeState> ApplyBackwards(IRelativeState relativeState, ISubstitution substitution)
        {
            return Effects.ApplyBackwards(relativeState, substitution);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double ComputePlanningGraphLabel(ISubstitution substitution, StateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return Preconditions.EvaluateOperatorPlanningGraphLabel(substitution, stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Gets a list of atoms from the specified state that were necessary to make this operator applicable. We already assume that the operator is applicable to the given state.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="predecessorState">Preceding state.</param>
        /// <returns>List of effective precondition atoms.</returns>
        public List<IAtom> GetEffectivePreconditions(ISubstitution substitution, IState predecessorState)
        {
            return Preconditions.GetSatisfyingAtoms(substitution, predecessorState);
        }

        /// <summary>
        /// Gets a list of atoms that are made true by the application of this operator. Only simple positive effects are wanted.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <returns>List of effective effect atoms.</returns>
        public List<IAtom> GetEffectiveEffects(ISubstitution substitution)
        {
            return Effects.GetResultAtoms(substitution);
        }

        /// <summary>
        /// Gets the operator full name with the given parameters grounding.
        /// </summary>
        /// <returns>Full operator name.</returns>
        public string GetName(ISubstitution substitution)
        {
            List<string> parametersList = Parameters.ConvertAll(parameter =>
            {
                int value;
                if (substitution.TryGetValue(parameter.ParameterNameId, out value))
                {
                    // constant name
                    return IdManager.Constants.GetNameFromId(value);
                }
                else
                {
                    // generic variable name
                    return $"{IdManager.GenericVariablePrefix}{parameter.ParameterNameId.ToString()}";
                }
            });

            return $"{Name}({string.Join(", ", parametersList)})";
        }

        /// <summary>
        /// Checks if the operator effects contain the action cost fluent and extracts it as the operator cost. The default operator cost is 1.
        /// </summary>
        private void ExtractCostFromEffects()
        {
            if (!IdManager.Functions.IsRegistered("total-cost"))
            {
                return;
            }

            IAtom operatorCostAtom = new Atom(IdManager.Functions.GetId("total-cost"));
            foreach (var effect in Effects)
            {
                NumericIncreaseEffect increaseEffect = effect as NumericIncreaseEffect;
                if (increaseEffect != null)
                {
                    if (increaseEffect.FunctionAtom.Equals(operatorCostAtom))
                    {
                        Number costValue = increaseEffect.Value as Number;
                        if (costValue != null)
                        {
                            Cost = (int)costValue.Value;
                        }
                        else
                        {
                            throw new System.NotSupportedException("General numeric fluents are not supported as an operator cost, at the moment.");
                        }
                    }
                }
            }
        }
    }
}