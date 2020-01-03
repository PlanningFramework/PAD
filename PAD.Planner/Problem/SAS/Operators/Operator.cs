using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a single grounded SAS+ operator.
    /// </summary>
    public class Operator : IOperator
    {
        /// <summary>
        /// Name of the SAS+ operator.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Operator unique identifier (index of the operator in the list of all operators).
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Preconditions of the SAS+ operator.
        /// </summary>
        public Conditions Preconditions { set; get; }

        /// <summary>
        /// Effects of the SAS+ operator.
        /// </summary>
        public Effects Effects { set; get; }

        /// <summary>
        /// Cost of the operator in the SAS+ planning problem.
        /// </summary>
        public int Cost { set; get; }

        /// <summary>
        /// Checker of the mutex groups of the SAS+ planning problem.
        /// </summary>
        private Lazy<MutexChecker> MutexChecker { get; }

        /// <summary>
        /// Constructs the SAS+ operator from the input data.
        /// </summary>
        /// <param name="inputData">Operator input data.</param>
        /// <param name="index">Operator index.</param>
        /// <param name="axiomRules">Axiom rules of the SAS+ planning problem.</param>
        /// <param name="mutexGroups">Mutex groups of the SAS+ planning problem.</param>
        public Operator(InputData.SAS.Operator inputData, int index, AxiomRules axiomRules, MutexGroups mutexGroups)
        {
            Name = inputData.Name;
            Id = index;
            Preconditions = new Conditions(inputData.Conditions);
            Effects = new Effects(inputData.Effects, axiomRules);
            Cost = inputData.Cost;

            MutexChecker = new Lazy<MutexChecker>(() => new MutexChecker(mutexGroups));
        }

        /// <summary>
        /// Constructs the SAS+ operator.
        /// </summary>
        /// <param name="name">Operator name.</param>
        /// <param name="id">Operator ID.</param>
        /// <param name="preconditions">Operator preconditions.</param>
        /// <param name="effects">Operator effects.</param>
        /// <param name="cost">Operator cost.</param>
        public Operator(string name, int id, Conditions preconditions, Effects effects, int cost)
        {
            Name = name;
            Id = id;
            Preconditions = preconditions;
            Effects = effects;
            Cost = cost;
        }

        /// <summary>
        /// Gets the operator name.
        /// </summary>
        /// <returns>Full operator name.</returns>
        public string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Gets preconditions of the SAS+ operator.
        /// </summary>
        public Conditions GetPreconditions()
        {
            return Preconditions;
        }

        /// <summary>
        /// Gets effects of the SAS+ operator.
        /// </summary>
        public Effects GetEffects()
        {
            return Effects;
        }

        /// <summary>
        /// Checks whether the operator is applicable to the given state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if the operator is applicable to the given state, false otherwise.</returns>
        public bool IsApplicable(Planner.IState state)
        {
            return Preconditions.Evaluate((IState)state) && MutexChecker.Value.CheckSuccessorCompatibility((IState)state, this);
        }

        /// <summary>
        /// Applies the operator to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        public Planner.IState Apply(Planner.IState state, bool directlyModify = false)
        {
            return Effects.Apply((IState)state, directlyModify);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>True if the operator is relevant to the given conditions, false otherwise.</returns>
        public bool IsRelevant(Planner.IConditions conditions)
        {
            return Effects.IsRelevant((IConditions)conditions, Preconditions) && MutexChecker.Value.CheckPredecessorCompatibility((IConditions)conditions, this);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>True if the operator is relevant to the given relative state, false otherwise.</returns>
        public bool IsRelevant(Planner.IRelativeState relativeState)
        {
            return Effects.IsRelevant((IRelativeState)relativeState, Preconditions);
        }

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>Preceding conditions.</returns>
        public Planner.IConditions ApplyBackwards(Planner.IConditions conditions)
        {
            return Effects.ApplyBackwards((IConditions)conditions, Preconditions);
        }

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<Planner.IRelativeState> ApplyBackwards(Planner.IRelativeState relativeState)
        {
            return Effects.ApplyBackwards((IRelativeState)relativeState, Preconditions);
        }

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        public double ComputePlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            return Preconditions.EvaluateOperatorPlanningGraphLabel(stateLabels, evaluationStrategy);
        }

        /// <summary>
        /// Gets a cost of the operator.
        /// </summary>
        /// <returns>Operator cost.</returns>
        public int GetCost()
        {
            return Cost;
        }

        /// <summary>
        /// Creates a deep copy of the operator.
        /// </summary>
        /// <returns>A copy of the operator.</returns>
        public Planner.IOperator Clone()
        {
            return new Operator(Name, Id, Preconditions, Effects, Cost);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{Name}: {Preconditions} -> {Effects}";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
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

            return (Name == other.Name && Id == other.Id && Cost == other.Cost &&
                    Preconditions.Equals(other.Preconditions) && Effects.Equals(other.Effects));
        }
    }
}
