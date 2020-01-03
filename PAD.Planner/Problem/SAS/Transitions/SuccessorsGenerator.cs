using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Successors generator is a component handling preprocessing and storage of applicable operators and subsequent generation of forward
    /// transitions in SAS+ planning problem. The internal cache is implemented in a form of a decision tree, where every inner node has a decision
    /// variable - each subtree of this node represents a class of states where the decision variable has a specific value. The actual applicable
    /// operators are in the leaf nodes of the decision tree. This preprocessing can build quite a large tree, but the subsequent traversing of
    /// this tree can significantly decrease the time we need to find applicable operators for a specific state. The actual generation of successors
    /// is lazy evaluated in a form of IEnumerable collection of successors.
    /// </summary>
    public class SuccessorsGenerator
    {
        /// <summary>
        /// Root node of the applicable operators decision tree.
        /// </summary>
        private IOperatorDecisionTreeNode TreeRoot { get; }

        /// <summary>
        /// Collector traversing the applicable operators decision tree and returning the actual successors.
        /// </summary>
        private SuccessorsCollector SuccessorsCollector { get; }

        /// <summary>
        /// Builds the successors generator.
        /// </summary>
        /// <param name="operators">Operators of the SAS+ planning problem.</param>
        /// <param name="variables">Variables data of the SAS+ planning problem.</param>
        /// <param name="mutexGroups">Mutex groups of the SAS+ planning problem.</param>
        public SuccessorsGenerator(Operators operators, Variables variables, MutexGroups mutexGroups)
        {
            TreeRoot = OperatorDecisionTreeBuilder.BuildApplicabilityTree(operators, variables);
            SuccessorsCollector = new SuccessorsCollector(mutexGroups);
        }

        /// <summary>
        /// Gets a collection of all possible successors (forward transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetSuccessors(IState state)
        {
            return SuccessorsCollector.GetSuccessors(TreeRoot, state);
        }
    }
}
