using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Implementation of an action node (of action layer) in the relaxed relaxed planning graph, i.e. an applied operator with its properties.
    /// </summary>
    public class ActionNode
    {
        /// <summary>
        /// Applied operator.
        /// </summary>
        public IOperator Operator { set; get; } = null;

        /// <summary>
        /// Previous atoms in the planning graph, i.e. effective preconditions of the operator.
        /// </summary>
        public List<IProposition> Predecessors { set; get; } = null;

        /// <summary>
        /// Successor atoms in the planning graph, i.e. actual effects of the operator.
        /// </summary>
        public List<IProposition> Successors { set; get; } = null;

        /// <summary>
        /// Label value of the action node (within a forward cost evaluation).
        /// </summary>
        public double Label { set; get; } = -1;

        /// <summary>
        /// Constructs the action node from the specified operator and its calculated label (for forward cost calculation).
        /// </summary>
        /// <param name="appliedOperator">Applied operator.</param>
        /// <param name="label">Calculated label.</param>
        public ActionNode(IOperator appliedOperator, double label)
        {
            Operator = appliedOperator;
            Label = label;
        }

        /// <summary>
        /// Constructs the action node of the relaxed planning graph (for FF calculation).
        /// </summary>
        /// <param name="appliedOperator">Applied operator.</param>
        /// <param name="predecessor">Predecessor state.</param>
        public ActionNode(IOperator appliedOperator, List<IProposition> predecessors, List<IProposition> successors)
        {
            Operator = appliedOperator;
            Predecessors = predecessors;
            Successors = successors;
        }

        /// <summary>
        /// Constructs the action node of the relaxed planning graph (for FF calculation).
        /// </summary>
        /// <param name="appliedOperator">Applied operator.</param>
        /// <param name="predecessor">Predecessor state.</param>
        public ActionNode(List<IProposition> predecessors)
        {
            Predecessors = predecessors;
        }
    }
}
