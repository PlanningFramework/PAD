using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Auxiliary object for traversing an operator decision tree and generating the collection of predecessors.
    /// </summary>
    public class PredecessorsCollector : IOperatorDecisionTreeRelevanceVisitor
    {
        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="treeRoot">Root of the operator decision tree to be traversed.</param>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(IOperatorDecisionTreeNode treeRoot, IConditions conditions)
        {
            foreach (var simpleConditions in conditions.GetSimpleConditions())
            {
                foreach (var predecessor in treeRoot.Accept(this, conditions, simpleConditions))
                {
                    yield return predecessor;
                }
            }
        }

        /// <summary>
        /// Visits the inner node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Inner node of the tree.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        public IEnumerable<IPredecessor> Visit(OperatorDecisionTreeInnerNode treeNode, IConditions sourceConditions, ISimpleConditions currentSubConditions)
        {
            int value;
            if (currentSubConditions.IsVariableConstrained(treeNode.DecisionVariable, out value))
            {
                // if constrained, collect the operators only in the corresponding subtree
                foreach (var predecessor in treeNode.OperatorsByDecisionVariableValue[value].Accept(this, sourceConditions, currentSubConditions))
                {
                    yield return predecessor;
                }
            }
            else
            {
                // if not constrained, collect operators from all the subtrees
                foreach (var subTree in treeNode.OperatorsByDecisionVariableValue)
                {
                    foreach (var predecessor in subTree.Accept(this, sourceConditions, currentSubConditions))
                    {
                        yield return predecessor;
                    }
                }
            }

            // always search in the subtree with the value-independent operators
            foreach (var successor in treeNode.OperatorsIndependentOnDecisionVariable.Accept(this, sourceConditions, currentSubConditions))
            {
                yield return successor;
            }
        }

        /// <summary>
        /// Visits the leaf node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Leaf node of the tree.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        public IEnumerable<IPredecessor> Visit(OperatorDecisionTreeLeafNode treeNode, IConditions sourceConditions, ISimpleConditions currentSubConditions)
        {
            foreach (var oper in treeNode.Operators)
            {
                // relevant operator candidates need to be double-checked for the relevance with the original conditions, because there can be additional
                // conflicts with the operator preconditions, conditional effects constraints, and/or incompatibility with the mutex constraints
                if (oper.IsRelevant(sourceConditions))
                {
                    yield return new Predecessor(sourceConditions, oper);
                }
            }
        }
    }
}
