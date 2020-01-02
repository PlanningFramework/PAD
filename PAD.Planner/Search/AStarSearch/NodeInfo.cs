using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Structure encapsuling the information about processed search nodes (i.e. states or conditions).
    /// </summary>
    public class NodeInfo
    {
        /// <summary>
        /// gValue of the node.
        /// </summary>
        public int GValue { set; get; } = 0;

        /// <summary>
        /// Is the node closed?
        /// </summary>
        public bool IsClosed { set; get; } = false;

        /// <summary>
        /// Predecessor to the node (predecessor node and the applied operator to the node).
        /// </summary>
        public Tuple<ISearchNode, IOperator> Predecessor { set; get; } = null;

        /// <summary>
        /// Constructs the node info.
        /// </summary>
        /// <param name="gValue">Calculated distance from the initial node to this node.</param>
        /// <param name="predecessor">Predecessor entity.</param>
        public NodeInfo(int gValue, Tuple<ISearchNode, IOperator> predecessor)
        {
            GValue = gValue;
            IsClosed = false;
            Predecessor = predecessor;
        }
    }
}
