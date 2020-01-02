using PAD.Planner.Heuristics;
using PAD.Planner.Heaps;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Implementation of Beam search procedure (for both forward and backward planning search).
    /// </summary>
    public class BeamSearch : AStarSearch
    {
        /// <summary>
        /// Collection of new transition candidates.
        /// </summary>
        protected BeamSearchTransitionCandidates Candidates { set; get; } = null;

        /// <summary>
        /// Constructs the Beam search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic (if not specified, blind heuristic will be used).</param>
        /// <param name="heap">Heap collection (if not specified, red-black heap will be used).</param>
        /// <param name="beamWidth">Beam width (default width is 2).</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        public BeamSearch(ISearchableProblem problem, ISearchableHeuristic heuristic = null, IHeap heap = null, int beamWidth = 2, bool loggingEnabled = false)
            : base(problem, heuristic, heap, loggingEnabled)
        {
            Candidates = new BeamSearchTransitionCandidates(problem, heuristic, beamWidth);
        }

        /// <summary>
        /// Constructs the Beam search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        /// <param name="heap">Heap collection.</param>
        /// <param name="beamWidth">Beam width.</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        /// <param name="timeLimitOfSearch">Time limit of the search.</param>
        /// <param name="memoryLimitOfStates">Memory limit of searched nodes.</param>
        public BeamSearch(ISearchableProblem problem, ISearchableHeuristic heuristic, IHeap heap, int beamWidth, bool loggingEnabled, TimeSpan timeLimitOfSearch, long memoryLimitOfStates)
            : base(problem, heuristic, heap, loggingEnabled, timeLimitOfSearch, memoryLimitOfStates)
        {
            Candidates = new BeamSearchTransitionCandidates(problem, heuristic, beamWidth);
        }

        /// <summary>
        /// Initialization of the search procedure. Should be done before every search!
        /// </summary>
        /// <param name="type">Search type.</param>
        protected override void InitSearch(SearchType type)
        {
            base.InitSearch(type);
            Candidates.Clear();
        }

        /// <summary>
        /// Processes the transitions from the specified search node.
        /// </summary>
        /// <param name="node">Search node to be processed.</param>
        /// <param name="gValue">Distance of the node from the initial node.</param>
        protected override void ProcessTransitionsFromNode(ISearchNode node, int gValue)
        {
            Candidates.SelectBestTransitionCandidates(node);

            foreach (var candidate in Candidates)
            {
                AddToOpenNodes(candidate.Node, candidate.AppliedOperator.GetCost() + gValue, candidate.HValue, node, candidate.AppliedOperator);
            }
        }

        /// <summary>
        /// Adds the specified node to the open nodes collection.
        /// </summary>
        /// <param name="node">Node to be added.</param>
        /// <param name="gValue">Calculated distance from the start.</param>
        /// <param name="hValue">Heuristic distance to the goal.</param>
        /// <param name="predecessor">Predecessor node.</param>
        /// <param name="appliedOperator">Predecessor operator.</param>
        protected void AddToOpenNodes(ISearchNode node, int gValue, double hValue, ISearchNode predecessor, IOperator appliedOperator)
        {
            NodeInfo nodeInfo;
            if (NodesInfo.TryGetValue(node, out nodeInfo))
            {
                if (nodeInfo.GValue > gValue)
                {
                    nodeInfo.GValue = gValue;
                    nodeInfo.Predecessor = Tuple.Create(predecessor, appliedOperator);
                    NodesInfo[node] = nodeInfo;

                    OpenNodes.Add(gValue + hValue, node);
                }
            }
            else
            {
                NodesInfo.Add(node, new NodeInfo(gValue, Tuple.Create(predecessor, appliedOperator)));
                OpenNodes.Add(gValue + hValue, node);
            }
        }

        /// <summary>
        /// Logs the search statistics.
        /// </summary>
        protected override void LogSearchStatistics()
        {
            if (LoggingEnabled)
            {
                string message = $"Expanded nodes: {NodesInfo.Count - OpenNodes.GetSize()}" +
                                 $"\tOpen nodes: {GetOpenNodesCount()}" +
                                 $"\tVisited nodes: {GetProcessedNodesCount()}";

                IHeuristic heuristic = Heuristic as IHeuristic;
                if (heuristic != null)
                {
                    message += $"\tHeuristic calls: {heuristic.GetCallsCount()}";
                }

                LogMessage(message);
            }
        }

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        public override string GetName()
        {
            return (SearchType == SearchType.Forward) ? "Beam Search" : "Beam Search (Backward)";
        }
    }
}
