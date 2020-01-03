using System.Collections.Generic;
// ReSharper disable IdentifierTypo

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a SAS+ relaxed planning graph, computing forward cost heuristics.
    /// </summary>
    public class RelaxedPlanningGraph : Planner.RelaxedPlanningGraph
    {
        /// <summary>
        /// Constructs the relaxed planning graph.
        /// </summary>
        /// <param name="relaxedProblem">Relaxed planning problem.</param>
        public RelaxedPlanningGraph(IRelaxedProblem relaxedProblem) : base(relaxedProblem)
        {
        }

        /// <summary>
        /// Creates the labeled state layer for the forward cost evaluation, from the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>New state layer.</returns>
        protected override IStateLayer CreateLabeledStateLayer(Planner.IState state)
        {
            return new StateLayer((RelaxedState)state);
        }

        /// <summary>
        /// Creates the labeled state layer for the forward cost evaluation, from the previous state layer and the action layer.
        /// </summary>
        /// <param name="sLayer">Previous state layer.</param>
        /// <param name="aLayer">Action layer.</param>
        /// <returns>New state layer.</returns>
        protected override IStateLayer CreateLabeledStateLayer(IStateLayer sLayer, ActionLayer aLayer)
        {
            return new StateLayer((StateLayer)sLayer, aLayer);
        }

        /// <summary>
        /// Creates the state layer for the FF evaluation from the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>New state layer.</returns>
        protected override IStateLayer CreateFFStateLayer(Planner.IState state)
        {
            return new StateLayer((RelaxedState)state, false);
        }

        /// <summary>
        /// Creates the action node for the FF evaluation, from the specified operator and the previous state layer.
        /// </summary>
        /// <param name="appliedOperator">Applied operator.</param>
        /// <param name="state">Previous state layer.</param>
        /// <returns>New action node.</returns>
        protected override ActionNode CreateFFActionNode(Planner.IOperator appliedOperator, Planner.IState state)
        {
            Operator oper = (Operator)appliedOperator;

            List<IProposition> predecessors = new List<IProposition>();
            foreach (var assignment in oper.Preconditions)
            {
                predecessors.Add(new Proposition(assignment));
            }

            List<IProposition> successors = new List<IProposition>();
            foreach (var effect in oper.Effects)
            {
                if (effect.IsApplicable((IState)state))
                {
                    successors.Add(new Proposition(effect.GetAssignment()));
                }
            }

            return new ActionNode(oper, predecessors, successors);
        }

        /// <summary>
        /// Creates the goal action node for the FF evaluation, from the specified goal conditions and the previous state layer.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="state">Previous state layer.</param>
        /// <returns>New action node.</returns>
        protected override ActionNode CreateFFGoalActionNode(Planner.IConditions conditions, Planner.IState state)
        {
            Conditions cond = (Conditions)conditions;

            List<IProposition> predecessors = new List<IProposition>();
            foreach (var assignment in cond)
            {
                predecessors.Add(new Proposition(assignment));
            }

            return new ActionNode(predecessors);
        }
    }
}
