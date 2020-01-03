using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ grounded operator in the planning problem.
    /// </summary>
    public interface IOperator
    {
        /// <summary>
        /// Gets the operator name.
        /// </summary>
        /// <returns>Full operator name.</returns>
        string GetName();

        /// <summary>
        /// Checks whether the operator is applicable to the given state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if the operator is applicable to the given state, false otherwise.</returns>
        bool IsApplicable(IState state);

        /// <summary>
        /// Applies the operator to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        IState Apply(IState state, bool directlyModify = false);

        /// <summary>
        /// Checks whether the operator is relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>True if the operator is relevant to the given conditions, false otherwise.</returns>
        bool IsRelevant(IConditions conditions);

        /// <summary>
        /// Checks whether the operator is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>True if the operator is relevant to the given relative state, false otherwise.</returns>
        bool IsRelevant(IRelativeState relativeState);

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <returns>Preceding conditions.</returns>
        IConditions ApplyBackwards(IConditions conditions);

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <returns>Preceding relative states.</returns>
        IEnumerable<IRelativeState> ApplyBackwards(IRelativeState relativeState);

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">Atom labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        double ComputePlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy);

        /// <summary>
        /// Gets the cost of the operator.
        /// </summary>
        /// <returns>Operator cost.</returns>
        int GetCost();

        /// <summary>
        /// Creates a deep copy of the operator.
        /// </summary>
        /// <returns>A copy of the operator.</returns>
        IOperator Clone();
    }
}
