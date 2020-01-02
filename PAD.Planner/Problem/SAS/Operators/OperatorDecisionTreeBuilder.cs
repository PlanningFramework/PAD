using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// The builder of operator decision trees used for the generation of successors, or predecessors. Can build an operator applicability tree
    /// or an operator relevance tree.
    /// </summary>
    public static class OperatorDecisionTreeBuilder
    {
        /// <summary>
        /// Delegate used for specifying of how operators will be handled during building of the operator decision tree. In case of the applicability
        /// tree we want to look at the operators preconditions, while for the relevance tree we are interested in the operators effects.
        /// </summary>
        /// <param name="oper">Operator.</param>
        /// <param name="variable">Variable.</param>
        /// <param name="value">Corresponding value for the variable (if the function returns true).</param>
        /// <returns>True if the operator is somehow affected by the specified variable, false otherwise.</returns>
        private delegate bool OperatorEvaluator(IOperator oper, int variable, out int value);

        /// <summary>
        /// Builds the operator decision tree for the evaluation of operators applicability in the SAS+ planning problem.
        /// </summary>
        /// <param name="operators">Operators of the SAS+ planning problem.</param>
        /// <param name="variables">Variables data of the SAS+ planning problem.</param>
        /// <returns>Operator decision tree of operators applicability.</returns>
        public static IOperatorDecisionTreeNode BuildApplicabilityTree(Operators operators, Variables variables)
        {
            OperatorEvaluator evaluator = (IOperator oper, int variable, out int value) =>
            {
                return oper.GetPreconditions().IsVariableConstrained(variable, out value);
            };
            return BuildTree(new List<IOperator>(operators), GetAllDecisionVariables(variables), evaluator);
        }

        /// <summary>
        /// Builds the operator decision tree for the evaluation of operators relevance in the SAS+ planning problem.
        /// </summary>
        /// <param name="operators">Operators of the SAS+ planning problem.</param>
        /// <param name="variables">Variables data of the SAS+ planning problem.</param>
        /// <returns>Operator decision tree of operators relevance.</returns>
        public static IOperatorDecisionTreeNode BuildRelevanceTree(Operators operators, Variables variables)
        {
            OperatorEvaluator evaluator = (IOperator oper, int variable, out int value) =>
            {
                return oper.GetEffects().IsVariableAffected(variable, out value);
            };
            return BuildTree(new List<IOperator>(operators), GetAllDecisionVariables(variables), evaluator);
        }

        /// <summary>
        /// Prepares the decision variables for the operator decision tree building process.
        /// </summary>
        /// <param name="variables">Variables data of the SAS+ planning problem.</param>
        /// <returns>List of variables to be processed during the tree building (the second item in tuple is the variable domain range).</returns>
        private static List<Tuple<int, int>> GetAllDecisionVariables(Variables variables)
        {
            List<Tuple<int, int>> variablesToBeProcessed = new List<Tuple<int, int>>();
            for (int i = 0; i < variables.Count; ++i)
            {
                if (!variables[i].IsAxiomatic())
                {
                    variablesToBeProcessed.Add(Tuple.Create(i, variables[i].GetDomainRange()));
                }
            }
            return variablesToBeProcessed;
        }

        /// <summary>
        /// Builds the operator decision tree from the given operators and decision variables.
        /// </summary>
        /// <param name="availableOperators">List of operators to be processed.</param>
        /// <param name="decisionVariables">List of decision variables to be processed with their corresponding domain range.</param>
        /// <param name="evaluator">Operator evaluator specifying how to handle operators during the tree building.</param>
        /// <returns>Operator decision tree node corresponding to the input operators and decision variables.</returns>
        private static IOperatorDecisionTreeNode BuildTree(List<IOperator> availableOperators, List<Tuple<int, int>> decisionVariables, OperatorEvaluator evaluator)
        {
            // if we have no remaining operators, then create an empty leaf node
            if (availableOperators.Count == 0)
            {
                return new OperatorDecisionTreeEmptyLeafNode();
            }

            // if we have no remaining decision variables, then create a leaf node containing the current operators list
            if (decisionVariables.Count == 0)
            {
                return new OperatorDecisionTreeLeafNode(availableOperators);
            }

            // find the current decision variable, i.e. available variable with the maximal domain range
            int decisionVariableDomainRange = decisionVariables.Max(variable => variable.Item2);
            int decisionVariable = decisionVariables.First(variable => variable.Item2 == decisionVariableDomainRange).Item1;

            // prepare decision variables modifiers
            Action RemoveCurrentDecisionVariable = () => { decisionVariables.RemoveAll(variable => variable.Item1 == decisionVariable); };
            Action RestoreCurrentDecisionVariable = () => { decisionVariables.Add(Tuple.Create(decisionVariable, decisionVariableDomainRange)); };

            // prepare collections for sorting out available operators
            List<IOperator>[] operatorsByValue = new List<IOperator>[decisionVariableDomainRange];
            List<IOperator> nonAffectedOperators = new List<IOperator>();
            for (int i = 0; i < operatorsByValue.Length; ++i)
            {
                operatorsByValue[i] = new List<IOperator>();
            }

            // sort out the available operators by the current decision variable: if the preconditions of the operator are constrained by the
            // decision variable (applicability tree), or the decision variable is affected by the operator effects (relevance tree), then put
            // it into the operatorsByValue collection, otherwise into the nonAffectedOperators collection
            foreach (var oper in availableOperators)
            {
                int value = Assignment.INVALID_VALUE;
                if (evaluator(oper, decisionVariable, out value))
                {
                    operatorsByValue[value].Add(oper);
                }
                else
                {
                    nonAffectedOperators.Add(oper);
                }
            }

            // the decision variable has zero impact -> continue without it
            if (nonAffectedOperators.Count == availableOperators.Count)
            {
                RemoveCurrentDecisionVariable();
                var result = BuildTree(nonAffectedOperators, decisionVariables, evaluator);
                RestoreCurrentDecisionVariable();
                return result;
            }

            // prepare a new decision node - create decision subtrees for all the possible values of the current decision variable
            RemoveCurrentDecisionVariable();
            IOperatorDecisionTreeNode[] childrenByValues = new IOperatorDecisionTreeNode[decisionVariableDomainRange];
            for (int i = 0; i < decisionVariableDomainRange; ++i)
            {
                childrenByValues[i] = BuildTree(operatorsByValue[i], decisionVariables, evaluator);
            }
            IOperatorDecisionTreeNode childIndependentOnDecisionVariable = BuildTree(nonAffectedOperators, decisionVariables, evaluator);
            RestoreCurrentDecisionVariable();

            // return the root of the current subtree
            return new OperatorDecisionTreeInnerNode(decisionVariable, childrenByValues, childIndependentOnDecisionVariable);
        }
    }
}
