
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a PDDL relaxed planning graph, computing forward cost heuristics.
    /// </summary>
    public class RelaxedPlanningGraph : Planner.RelaxedPlanningGraph
    {
        /// <summary>
        /// Constructs the relaxed planning graph.
        /// </summary>
        /// <param name="relaxedPProblem">Relaxed planning problem.</param>
        public RelaxedPlanningGraph(IRelaxedProblem relaxedPProblem) : base(relaxedPProblem)
        {
        }

        /// <summary>
        /// Creates the labeled state layer for the forward cost evaluation, from the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>New state layer.</returns>
        protected override IStateLayer CreateLabeledStateLayer(Planner.IState state)
        {
            return new StateLayer((IState)state);
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
            return new StateLayer((IState)state, false);
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
            var preconditions = oper.GetEffectivePreconditions((IState)state);
            var effects = oper.GetEffectiveEffects();

            return new ActionNode(oper, preconditions.ConvertAll(x => (IProposition)new Proposition(x)), effects.ConvertAll(x => (IProposition)new Proposition(x)));
        }

        /// <summary>
        /// Creates the goal action node for the FF evaluation, from the specified goal conditions and the previous state layer.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="state">Previous state layer.</param>
        /// <returns>New action node.</returns>
        protected override ActionNode CreateFFGoalActionNode(Planner.IConditions conditions, Planner.IState state)
        {
            var precondAtoms = ((Conditions)conditions).GetSatisfyingAtoms(new Substitution(), (IState)state);

            return new ActionNode(precondAtoms.ConvertAll(x => (IProposition)new Proposition(x)));
        }
    }
}
