
namespace PAD.Planner.Search
{
    /// <summary>
    /// Encapsulation of a transition candidate for Beam search.
    /// </summary>
    public class BeamSearchTransitionCandidate
    {
        /// <summary>
        /// Transition node.
        /// </summary>
        public ISearchNode Node { set; get; } = null;

        /// <summary>
        /// Heuristic distance to a goal from the candidate.
        /// </summary>
        public double HValue { set; get; } = 0;

        /// <summary>
        /// Corresponding applied operator.
        /// </summary>
        public IOperator AppliedOperator { set; get; } = null;

        /// <summary>
        /// Constrtucts the successor candidate.
        /// </summary>
        /// <param name="node">Search node.</param>
        /// <param name="hValue">Heuristic distance to a goal.</param>
        /// <param name="appliedOperator">Applied operator.</param>
        public BeamSearchTransitionCandidate(ISearchNode node, double hValue, IOperator appliedOperator)
        {
            Node = node;
            HValue = hValue;
            AppliedOperator = appliedOperator;
        }

        /// <summary>
        /// Gets the cost of the successor (i.e. heuristic distance to the goal + applied operator cost).
        /// </summary>
        /// <returns>Successor cost.</returns>
        public double GetCost()
        {
            return HValue + AppliedOperator.GetCost();
        }
    }
}
