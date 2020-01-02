using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Predecessors generator is a component handling generation of relevant backward transitions in SAS+ planning problem.
    /// </summary>
    public class PredecessorsGenerator
    {
        /// <summary>
        /// Root node of the relevance operators decision tree.
        /// </summary>
        private IOperatorDecisionTreeNode TreeRoot { set; get; } = null;

        /// <summary>
        /// Collector traversing the relevance operators decision tree and returning the actual predecessors.
        /// </summary>
        private PredecessorsCollector PredecessorsCollector { set; get; } = new PredecessorsCollector();

        /// <summary>
        /// Builds the predecessors generator.
        /// </summary>
        /// <param name="operators">Operators of the SAS+ planning problem.</param>
        /// <param name="variables">Variables data of the SAS+ planning problem.</param>
        public PredecessorsGenerator(Operators operators, Variables variables)
        {
            TreeRoot = OperatorDecisionTreeBuilder.BuildRelevanceTree(operators, variables);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(IConditions conditions)
        {
            return PredecessorsCollector.GetPredecessors(TreeRoot, conditions);
        }
    }
}
