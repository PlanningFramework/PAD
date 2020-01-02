using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface for the nodes of SAS+ operator decision tree.
    /// </summary>
    public interface IOperatorDecisionTreeNode
    {
        /// <summary>
        /// Accepts applicability evaluation visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        IEnumerable<ISuccessor> Accept(IOperatorDecisionTreeApplicabilityVisitor visitor, IState state);

        /// <summary>
        /// Accepts relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        IEnumerable<IPredecessor> Accept(IOperatorDecisionTreeRelevanceVisitor visitor, IConditions sourceConditions, ISimpleConditions currentSubConditions);
    }

    /// <summary>
    /// Inner tree node of the SAS+ operator decision tree. Each inner node has a decision variable, references for the subtrees
    /// representing each of the possible values of the decision variable and also a subtree where the decision variable actually
    /// doesn't matter. The leaf nodes contain actual applicable operators.
    /// </summary>
    public class OperatorDecisionTreeInnerNode : IOperatorDecisionTreeNode
    {
        /// <summary>
        /// The decision variable of for this tree node.
        /// </summary>
        public int DecisionVariable { set; get; } = -1;

        /// <summary>
        /// The subtrees representing each possible value of the decision variable.
        /// </summary>
        public IOperatorDecisionTreeNode[] OperatorsByDecisionVariableValue { set; get; } = null;

        /// <summary>
        /// The subtree for the operators where the decision varaible doesn't matter.
        /// </summary>
        public IOperatorDecisionTreeNode OperatorsIndependentOnDecisionVariable { set; get; } = null;

        /// <summary>
        /// Constructs a new SAS+ operator decision tree inner node.
        /// </summary>
        /// <param name="decisionVariable">Decision variable of the current subtree.</param>
        /// <param name="operatorsByDecisionVariableValue">Subtrees for all the possible values of the decision variable.</param>
        /// <param name="operatorsIndependentOnDecisionVariable">Subtree where the decision variable doesn't matter.</param>
        public OperatorDecisionTreeInnerNode(int decisionVariable, IOperatorDecisionTreeNode[] operatorsByDecisionVariableValue, IOperatorDecisionTreeNode operatorsIndependentOnDecisionVariable)
        {
            DecisionVariable = decisionVariable;
            OperatorsByDecisionVariableValue = operatorsByDecisionVariableValue;
            OperatorsIndependentOnDecisionVariable = operatorsIndependentOnDecisionVariable;
        }

        /// <summary>
        /// Accepts evaluating visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        public IEnumerable<ISuccessor> Accept(IOperatorDecisionTreeApplicabilityVisitor visitor, IState state)
        {
            return visitor.Visit(this, state);
        }

        /// <summary>
        /// Accepts relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        public IEnumerable<IPredecessor> Accept(IOperatorDecisionTreeRelevanceVisitor visitor, IConditions sourceConditions, ISimpleConditions currentSubConditions)
        {
            return visitor.Visit(this, sourceConditions, currentSubConditions);
        }
    }

    /// <summary>
    /// Leaf tree node of the SAS+ operator decision tree. It holds the actual applicable operators to be returned.
    /// </summary>
    public class OperatorDecisionTreeLeafNode : IOperatorDecisionTreeNode
    {
        /// <summary>
        /// The list of applicable operators.
        /// </summary>
        public List<IOperator> Operators { set; get; } = null;

        /// <summary>
        /// Constructs a SAS+ operator decision tree leaf node.
        /// </summary>
        /// <param name="operators">List of operators for the node.</param>
        public OperatorDecisionTreeLeafNode(List<IOperator> operators)
        {
            Operators = operators;
        }

        /// <summary>
        /// Accepts evaluating visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        public IEnumerable<ISuccessor> Accept(IOperatorDecisionTreeApplicabilityVisitor visitor, IState state)
        {
            return visitor.Visit(this, state);
        }

        /// <summary>
        /// Accepts relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        public IEnumerable<IPredecessor> Accept(IOperatorDecisionTreeRelevanceVisitor visitor, IConditions sourceConditions, ISimpleConditions currentSubConditions)
        {
            return visitor.Visit(this, sourceConditions, currentSubConditions);
        }
    }

    /// <summary>
    /// Empty leaf tree node of the SAS+ operator decision tree, with no useful containment.
    /// </summary>
    public class OperatorDecisionTreeEmptyLeafNode : IOperatorDecisionTreeNode
    {
        /// <summary>
        /// Constructs a SAS+ operator decision tree leaf node.
        /// </summary>
        /// <param name="operators">List of operators for the node.</param>
        public OperatorDecisionTreeEmptyLeafNode()
        {
        }

        /// <summary>
        /// Accepts evaluating visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        public IEnumerable<ISuccessor> Accept(IOperatorDecisionTreeApplicabilityVisitor visitor, IState state)
        {
            yield break;
        }

        /// <summary>
        /// Accepts relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">Evaluating visitor.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        public IEnumerable<IPredecessor> Accept(IOperatorDecisionTreeRelevanceVisitor visitor, IConditions sourceConditions, ISimpleConditions currentSubConditions)
        {
            yield break;
        }
    }

    /// <summary>
    /// Common interface for traversing of SAS+ applicability operator decision tree.
    /// </summary>
    public interface IOperatorDecisionTreeApplicabilityVisitor
    {
        /// <summary>
        /// Visits the inner node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Inner node of the tree.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        IEnumerable<ISuccessor> Visit(OperatorDecisionTreeInnerNode treeNode, IState state);

        /// <summary>
        /// Visits the leaf node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Leaf node of the tree.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>List of successors.</returns>
        IEnumerable<ISuccessor> Visit(OperatorDecisionTreeLeafNode treeNode, IState state);
    }

    /// <summary>
    /// Common interface for traversing of SAS+ relevance operator decision tree.
    /// </summary>
    public interface IOperatorDecisionTreeRelevanceVisitor
    {
        /// <summary>
        /// Visits the inner node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Inner node of the tree.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        IEnumerable<IPredecessor> Visit(OperatorDecisionTreeInnerNode treeNode, IConditions sourceConditions, ISimpleConditions currentSubConditions);

        /// <summary>
        /// Visits the leaf node of the SAS+ operator decision tree.
        /// </summary>
        /// <param name="treeNode">Leaf node of the tree.</param>
        /// <param name="sourceConditions">Source conditions being evaluated.</param>
        /// <param name="currentSubConditions">Currently evaluated sub-conditions.</param>
        /// <returns>List of predecessors.</returns>
        IEnumerable<IPredecessor> Visit(OperatorDecisionTreeLeafNode treeNode, IConditions sourceConditions, ISimpleConditions currentSubConditions);
    }
}
