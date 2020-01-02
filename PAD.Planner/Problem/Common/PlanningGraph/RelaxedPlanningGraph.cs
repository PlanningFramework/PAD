using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Abstract implementation of a relaxed planning graph, computing forward cost heuristics and FF heuristic.
    /// </summary>
    public abstract class RelaxedPlanningGraph : IRelaxedPlanningGraph
    {
        /// <summary>
        /// Corresponding relaxed planning problem.
        /// </summary>
        protected IRelaxedProblem RelaxedProblem { set; get; } = null;

        /// <summary>
        /// Cached state layers collection (FF computation).
        /// </summary>
        private List<IStateLayer> StateLayers = new List<IStateLayer>();

        /// <summary>
        /// Cached action layers collection (FF computation).
        /// </summary>
        private List<ActionLayer> ActionLayers = new List<ActionLayer>();

        /// <summary>
        /// Cached marked action nodes collection (FF computation).
        /// </summary>
        private HashSet<IOperator> MarkedActionNodes { set; get; } = new HashSet<IOperator>();

        /// <summary>
        /// Cached unsatisfied propositions on the current layer collection (FF computation).
        /// </summary>
        private Stack<IProposition> UnsatisfiedOnCurrentLayer { set; get; } = new Stack<IProposition>();

        /// <summary>
        /// Cached unsatisfied propositions on the next layer collection (FF computation).
        /// </summary>
        private Stack<IProposition> UnsatisfiedOnNextLayer { set; get; } = new Stack<IProposition>();

        /// <summary>
        /// Cached unsatisfied propositions swapper (FF computation).
        /// </summary>
        private Stack<IProposition> UnsatisfiedSwapper { set; get; } = null;

        /// <summary>
        /// Constructs the relaxed planning graph.
        /// </summary>
        /// <param name="relaxedProblem">Relaxed planning problem.</param>
        public RelaxedPlanningGraph(IRelaxedProblem relaxedProblem)
        {
            RelaxedProblem = relaxedProblem;
        }

        /// <summary>
        /// Computes the max forward cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>Max forward cost in the relaxed planning graph.</returns>
        public double ComputeMaxForwardCost(IState state)
        {
            return ComputeForwardCost(state, RelaxedProblem.GetGoalConditions(), ForwardCostEvaluationStrategy.MAX_VALUE);
        }

        /// <summary>
        /// Computes the max forward cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Max forward cost in the relaxed planning graph.</returns>
        public double ComputeMaxForwardCost(IConditions conditions)
        {
            return ComputeForwardCost(RelaxedProblem.GetInitialState(), conditions, ForwardCostEvaluationStrategy.MAX_VALUE);
        }

        /// <summary>
        /// Computes the additive forward cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>Addititive forward cost in the relaxed planning graph.</returns>
        public double ComputeAdditiveForwardCost(IState state)
        {
            return ComputeForwardCost(state, RelaxedProblem.GetGoalConditions(), ForwardCostEvaluationStrategy.ADDITIVE_VALUE);
        }

        /// <summary>
        /// Computes the adititive forward cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Additive forward cost in the relaxed planning graph.</returns>
        public double ComputeAdditiveForwardCost(IConditions conditions)
        {
            return ComputeForwardCost(RelaxedProblem.GetInitialState(), conditions, ForwardCostEvaluationStrategy.ADDITIVE_VALUE);
        }

        /// <summary>
        /// Computes the FF cost from the specified state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>FF cost in the relaxed planning graph.</returns>
        public double ComputeFFCost(IState state)
        {
            return ComputeFFCost(state, RelaxedProblem.GetGoalConditions());
        }

        /// <summary>
        /// Computes the FF cost from the specified conditions in the relaxed planning graph.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>FF cost in the relaxed planning graph.</returns>
        public double ComputeFFCost(IConditions conditions)
        {
            return ComputeFFCost(RelaxedProblem.GetInitialState(), conditions);
        }

        /// <summary>
        /// Computes the forward cost heuristics for the given state in the relaxed planning graph.
        /// </summary>
        /// <param name="state">Starting state.</param>
        /// <param name="conditions">Goal conditions.</param>
        /// <param name="evaluationStrategy">Evalaution strategy.</param>
        /// <returns>Forward cost heuristic value from the specified state.</returns>
        private double ComputeForwardCost(IState state, IConditions goalConditions, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            IStateLayer previousStateLayer = null;
            IStateLayer stateLayer = CreateLabeledStateLayer(state.GetRelaxedState());
            ActionLayer actionLayer = new ActionLayer();

            while (!stateLayer.Equals(previousStateLayer))
            {
                // check goal conditions

                if (goalConditions.Evaluate(stateLayer.GetState()))
                {
                    return goalConditions.EvaluateOperatorPlanningGraphLabel(stateLayer.GetStateLabels(), evaluationStrategy);
                }

                // build new action layer

                actionLayer.Clear();
                foreach (var successor in RelaxedProblem.GetSuccessors(stateLayer.GetState()))
                {
                    IOperator appliedOperator = successor.GetAppliedOperator();
                    double label = appliedOperator.ComputePlanningGraphLabel(stateLayer.GetStateLabels(), evaluationStrategy);
                    actionLayer.Add(new ActionNode(appliedOperator, label + appliedOperator.GetCost()));
                }

                // build new state layer

                previousStateLayer = stateLayer;
                stateLayer = CreateLabeledStateLayer(stateLayer, actionLayer);
            }

            // failure, solution cannot be found from the specified state
            return int.MaxValue;
        }

        /// <summary>
        /// Builds the relaxed planning graph and computes the FF heuristic value.
        /// </summary>
        /// <param name="state">Starting state.</param>
        /// <param name="goalConditions">Goal conditions.</param>
        /// <returns>FF cost heuristic value from the specified state.</returns>
        private double ComputeFFCost(IState state, IConditions goalConditions)
        {
            // build an explicit relaxed planning graph

            StateLayers.Clear();
            ActionLayers.Clear();
            StateLayers.Add(CreateFFStateLayer(state.GetRelaxedState()));

            while (true)
            {
                // check goal conditions

                IStateLayer stateLayer = StateLayers[StateLayers.Count - 1];

                if (goalConditions.Evaluate(stateLayer.GetState()))
                {
                    ActionLayers.Add(new ActionLayer() { CreateFFGoalActionNode(goalConditions, stateLayer.GetState()) });
                    break;
                }

                // build new action layer and the next state layer

                ActionLayer actionLayer = new ActionLayer();
                IState newState = stateLayer.GetState().Clone();

                foreach (var successor in RelaxedProblem.GetSuccessors(stateLayer.GetState()))
                {
                    IOperator appliedOperator = successor.GetAppliedOperator();

                    actionLayer.Add(CreateFFActionNode(appliedOperator, stateLayer.GetState()));

                    newState = appliedOperator.Apply(newState, true);
                }

                ActionLayers.Add(actionLayer);
                StateLayers.Add(CreateFFStateLayer(newState));
            }

            // compute FF value

            UnsatisfiedOnCurrentLayer.Clear();
            UnsatisfiedOnNextLayer.Clear();
            MarkedActionNodes.Clear();

            var goalNode = ActionLayers[ActionLayers.Count - 1][0];
            foreach (var proposition in goalNode.Predecessors)
            {
                UnsatisfiedOnCurrentLayer.Push(proposition);
            }

            for (int i = StateLayers.Count - 1; i > 0; --i)
            {
                IStateLayer nextStateLayer = StateLayers[i - 1];
                ActionLayer currentActionLayer = ActionLayers[i - 1];

                while (UnsatisfiedOnCurrentLayer.Count != 0)
                {
                    IProposition proposition = UnsatisfiedOnCurrentLayer.Pop();

                    // 1.) try to satisfy the proposition by an idle arc to the next state layer

                    if (nextStateLayer.HasProposition(proposition))
                    {
                        UnsatisfiedOnNextLayer.Push(proposition);
                        continue;
                    }

                    // 2.) try to satisfy the proposition by a support action node

                    ActionNode relevantActionNode = GetBestRelevantActionNodeFF(proposition, currentActionLayer);
                    if (relevantActionNode != null)
                    {
                            MarkedActionNodes.Add(relevantActionNode.Operator);
                            foreach (var prevProposition in relevantActionNode.Predecessors)
                            {
                                UnsatisfiedOnNextLayer.Push(prevProposition);
                            }
                    }
                }

                UnsatisfiedSwapper = UnsatisfiedOnNextLayer;
                UnsatisfiedOnNextLayer = UnsatisfiedOnCurrentLayer;
                UnsatisfiedOnCurrentLayer = UnsatisfiedSwapper;
                UnsatisfiedOnNextLayer.Clear();
            }

            // the result value is a sum of costs of marked action nodes

            double result = 0;
            foreach (var markedActionNode in MarkedActionNodes)
            {
                result += markedActionNode.GetCost();
            }
            return result;
        }

        /// <summary>
        /// Creates the labeled state layer for the forward cost evaluation, from the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>New state layer.</returns>
        protected abstract IStateLayer CreateLabeledStateLayer(IState state);

        /// <summary>
        /// Creates the labeled state layer for the forward cost evaluation, from the previous state layer and the action layer.
        /// </summary>
        /// <param name="sLayer">Previous state layer.</param>
        /// <param name="aLayer">Action layer.</param>
        /// <returns>New state layer.</returns>
        protected abstract IStateLayer CreateLabeledStateLayer(IStateLayer sLayer, ActionLayer aLayer);

        /// <summary>
        /// Creates the state layer for the FF evaluation from the specified state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <returns>New state layer.</returns>
        protected abstract IStateLayer CreateFFStateLayer(IState state);

        /// <summary>
        /// Creates the action node for the FF evaluation, from the specified operator and the previous state layer.
        /// </summary>
        /// <param name="appliedOperator">Applied operator.</param>
        /// <param name="state">Previous state layer.</param>
        /// <returns>New action node.</returns>
        protected abstract ActionNode CreateFFActionNode(IOperator appliedOperator, IState state);

        /// <summary>
        /// Creates the goal action node for the FF evaluation, from the specified goal conditions and the previous state layer.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="state">Previous state layer.</param>
        /// <returns>New action node.</returns>
        protected abstract ActionNode CreateFFGoalActionNode(IConditions conditions, IState state);

        /// <summary>
        /// Gets the best (lowest cost) relevant action node satisfying the given proposition during the FF evaluation.
        /// </summary>
        /// <param name="proposition">Proposition to satisfy.</param>
        /// <param name="actionLayer">Action layer.</param>
        /// <returns>Best relevant action node.</returns>
        private ActionNode GetBestRelevantActionNodeFF(IProposition proposition, ActionLayer actionLayer)
        {
            ActionNode bestActionNode = null;

            foreach (var actionNode in actionLayer)
            {
                if (actionNode.Successors.Contains(proposition))
                {
                    if (bestActionNode == null || actionNode.Operator.GetCost() < bestActionNode.Operator.GetCost())
                    {
                        bestActionNode = actionNode;
                    }
                }
            }

            return bestActionNode;
        }
    }
}
