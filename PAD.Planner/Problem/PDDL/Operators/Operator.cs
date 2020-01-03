using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of the grounded PDDL operator. Consists of a reference to the lifted operator and a substitution for the operator.
    /// </summary>
    public class Operator : IOperator
    {
        /// <summary>
        /// Reference to the lifted version of the operator.
        /// </summary>
        private LiftedOperator LiftedOperator { get; }

        /// <summary>
        /// Variable substitution for the lifted operator.
        /// </summary>
        private ISubstitution Substitution { get; }

        /// <summary>
        /// Constructs an instance of the grounded PDDL operator.
        /// </summary>
        /// <param name="liftedOperator">Reference to the lifted PDDL operator.</param>
        /// <param name="substitution">Concrete PDDL operator substitution.</param>
        public Operator(LiftedOperator liftedOperator, ISubstitution substitution)
        {
            LiftedOperator = liftedOperator;
            Substitution = substitution;
        }

        /// <summary>
        /// Gets the corresponding lifted operator.
        /// </summary>
        /// <returns>Corresponding lifted operator.</returns>
        public LiftedOperator GetLiftedOperator()
        {
            return LiftedOperator;
        }

        /// <summary>
        /// Gets the grounding substitution for the parameter of the lifted operator.
        /// </summary>
        /// <returns>Variables substitution.</returns>
        public ISubstitution GetSubstitution()
        {
            return Substitution;
        }

        /// <summary>
        /// Gets the operator name.
        /// </summary>
        /// <returns>Full operator name.</returns>
        public string GetName()
        {
            return LiftedOperator.GetName(Substitution);
        }

        /// <summary>
        /// Checks whether the operator is applicable to the given state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if the operator is applicable to the given state, false otherwise.</returns>
        public bool IsApplicable(Planner.IState state)
        {
            return LiftedOperator.IsApplicable((IState)state, Substitution);
        }

        /// <summary>
        /// Applies the operator to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        public Planner.IState Apply(Planner.IState state, bool directlyModify = false)
        {
            return LiftedOperator.Apply((IState)state, Substitution, directlyModify);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>True if the operator is relevant to the given conditions, false otherwise.</returns>
        public bool IsRelevant(Planner.IConditions conditions)
        {
            return LiftedOperator.IsRelevant((IConditions)conditions, Substitution);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>True if the operator is relevant to the given relative state, false otherwise.</returns>
        public bool IsRelevant(Planner.IRelativeState relativeState)
        {
            return LiftedOperator.IsRelevant((IRelativeState)relativeState, Substitution);
        }

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>Preceding conditions.</returns>
        public Planner.IConditions ApplyBackwards(Planner.IConditions conditions)
        {
            return LiftedOperator.ApplyBackwards((IConditions)conditions, Substitution);
        }

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<Planner.IRelativeState> ApplyBackwards(Planner.IRelativeState relativeState)
        {
            return LiftedOperator.ApplyBackwards((IRelativeState)relativeState, Substitution);
        }

        /// <summary>
        /// Gets the cost of the operator.
        /// </summary>
        /// <returns>Operator cost.</returns>
        public int GetCost()
        {
            return LiftedOperator.Cost;
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double ComputePlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return LiftedOperator.ComputePlanningGraphLabel(Substitution, (StateLabels)stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Gets a list of atoms from the specified state that were necessary to make this operator applicable. We already assume that the operator is applicable to the given state.
        /// </summary>
        /// <param name="predecessorState">Preceding state.</param>
        /// <returns>List of effective precondition atoms.</returns>
        public List<IAtom> GetEffectivePreconditions(IState predecessorState)
        {
            return LiftedOperator.GetEffectivePreconditions(Substitution, predecessorState);
        }

        /// <summary>
        /// Gets a list of atoms that are made true by the application of this operator. Only simple positive effects are wanted.
        /// </summary>
        /// <returns>List of effective effect atoms.</returns>
        public List<IAtom> GetEffectiveEffects()
        {
            return LiftedOperator.GetEffectiveEffects(Substitution);
        }

        /// <summary>
        /// Creates a deep copy of the operator.
        /// </summary>
        /// <returns>A copy of the operator.</returns>
        public Planner.IOperator Clone()
        {
            return new Operator(LiftedOperator, Substitution.Clone());
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return GetName();
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(LiftedOperator, Substitution);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            Operator other = obj as Operator;
            if (other == null)
            {
                return false;
            }

            return LiftedOperator == other.LiftedOperator
                && Substitution.Equals(other.Substitution);
        }
    }
}