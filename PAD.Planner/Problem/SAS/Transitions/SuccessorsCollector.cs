using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Auxiliary object for traversing an operator decision tree and generating the collection of successors.
    /// </summary>
    public class SuccessorsCollector : IOperatorDecisionTreeApplicabilityVisitor
    {
        /// <summary>
        /// Checker of mutex constraints of the corresponding SAS+ planning problem.
        /// </summary>
        private Lazy<MutexChecker> MutexChecker { get; }

        /// <summary>
        /// Constructs the successors collector.
        /// </summary>
        /// <param name="mutexGroups">Mutex groups of the SAS+ planning problem.</param>
        public SuccessorsCollector(MutexGroups mutexGroups)
        {
            MutexChecker = new Lazy<MutexChecker>(() => new MutexChecker(mutexGroups));
        }

        /// <summary>
        /// Gets a collection of all possible successors (forward transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="treeRoot">Root of the operator decision tree to be traversed.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetSuccessors(IOperatorDecisionTreeNode treeRoot, IState state)
        {
            return treeRoot.Accept(this, state);
        }

        /// <summary>
        /// Visits the inner node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Inner node of the tree.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        public IEnumerable<ISuccessor> Visit(OperatorDecisionTreeInnerNode treeNode, IState state)
        {
            foreach (var value in state.GetAllValues(treeNode.DecisionVariable))
            {
                foreach (var successor in treeNode.OperatorsByDecisionVariableValue[value].Accept(this, state))
                {
                    yield return successor;
                }
            }

            foreach (var successor in treeNode.OperatorsIndependentOnDecisionVariable.Accept(this, state))
            {
                yield return successor;
            }
        }

        /// <summary>
        /// Visits the leaf node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Leaf node of the tree.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        public IEnumerable<ISuccessor> Visit(OperatorDecisionTreeLeafNode treeNode, IState state)
        {
            foreach (var oper in treeNode.Operators)
            {
                if (MutexChecker.Value.CheckSuccessorCompatibility(state, oper))
                {
                    yield return new Successor(state, oper);
                }
            }
        }
    }
}
