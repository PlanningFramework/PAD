using System.Collections.Generic;
// ReSharper disable CommentTypo

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for PDDL or SAS+ conditions in the planning problem.
    /// </summary>
    public interface IConditions : IStateOrConditions
    {
        /// <summary>
        /// Gets the conditions size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>Conditions size.</returns>
        int GetSize();

        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        bool Evaluate(IState state);

        /// <summary>
        /// Computes the operator label in the relaxed planning graph.
        /// </summary>
        /// <param name="stateLabels">State labels from the predecessor layer in the graph.</param>
        /// <param name="evaluationStrategy">Evaluation strategy of the planning graph.</param>
        /// <returns>Computed operator label in the relaxed planning graph.</returns>
        double EvaluateOperatorPlanningGraphLabel(IStateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy);

        /// <summary>
        /// Enumerates all possible states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions.</returns>
        IEnumerable<IState> GetCorrespondingStates(IProblem problem);

        /// <summary>
        /// Enumerates all possible relative states meeting the current conditions.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible relative states meeting the conditions.</returns>
        IEnumerable<IRelativeState> GetCorrespondingRelativeStates(IProblem problem);

        /// <summary>
        /// Creates a deep copy of the conditions.
        /// </summary>
        /// <returns>Conditions clone.</returns>
        IConditions Clone();
    }
}
