using PAD.Planner.Heuristics;
using PAD.Planner.Heaps;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Implementation of multi-heuristic A* search (MHA*) procedure. This search variant uses multiple lists of open nodes each corresponding to a different heuristic.
    /// These heuristic with their open nodes are repeatedly evaluated a processed, while the list of closed nodes is shared within the search procedure.
    /// </summary>
    public class MultiHeuristicAStarSearch : AStarSearch
    {
        /// <summary>
        /// Index of currently used open list.
        /// </summary>
        private int CurrentOpenListIndex { set; get; }

        /// <summary>
        /// Number of lists of open nodes.
        /// </summary>
        private int NumberOfOpenLists => OpenLists.Count;

        /// <summary>
        /// List of used heuristics.
        /// </summary>
        private List<ISearchableHeuristic> Heuristics { get; }

        /// <summary>
        /// List of used heaps with open nodes.
        /// </summary>
        private List<IHeap> OpenLists { get; }

        /// <summary>
        /// Number of all open nodes.
        /// </summary>
        private long AllOpenNodesCount { set; get; }

        /// <summary>
        /// Constructs the multi-heuristic A* search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        public MultiHeuristicAStarSearch(ISearchableProblem problem, ISearchableHeuristic heuristic) : this(problem, new List<ISearchableHeuristic> { heuristic })
        {
        }

        /// <summary>
        /// Constructs the multi-heuristic A* search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristics">Heuristics.</param>
        public MultiHeuristicAStarSearch(ISearchableProblem problem, List<ISearchableHeuristic> heuristics) : base(problem, heuristics.First(), GetDefaultHeap())
        {
            Heuristics = heuristics;
            OpenLists = new List<IHeap>(Heuristics.Count);

            for (int i = 0; i < Heuristics.Count; ++i)
            {
                OpenLists.Add(GetDefaultHeap());
            }
        }

        /// <summary>
        /// Specifies the default heap used.
        /// </summary>
        /// <returns>Default heap.</returns>
        protected new static IHeap GetDefaultHeap()
        {
            return new FibonacciHeap();
        }

        /// <summary>
        /// Initialization of the search procedure. Should be done before every search!
        /// </summary>
        /// <param name="type">Search type.</param>
        protected override void InitSearch(SearchType type)
        {
            base.InitSearch(type);

            OpenLists.ForEach(item => item.Clear());
            CurrentOpenListIndex = 0;
            AllOpenNodesCount = 0;
        }

        /// <summary>
        /// Sets the initial search node of the search procedure.
        /// </summary>
        /// <param name="initialNode">Initial node of the search.</param>
        protected override void SetInitialNode(ISearchNode initialNode)
        {
            foreach (var item in OpenLists)
            {
                item.Add(0, initialNode);
                ++AllOpenNodesCount;
            }
            NodesInfo.Add(initialNode, new NodeInfo(0, null));
        }

        /// <summary>
        /// Checks whether there is any open node for the search available.
        /// </summary>
        /// <returns>True if there is some open node available. False otherwise.</returns>
        protected override bool HasAnyOpenNode()
        {
            return (AllOpenNodesCount > 0);
        }

        /// <summary>
        /// Gets the new open node from the list of open nodes (with minimal value).
        /// </summary>
        /// <returns>New open node.</returns>
        protected override ISearchNode GetMinOpenNode()
        {
            do // periodically iterate over the non-empty open lists
            {
                CurrentOpenListIndex = (CurrentOpenListIndex + 1) % NumberOfOpenLists;
                OpenNodes = OpenLists[CurrentOpenListIndex];
            }
            while (OpenNodes.GetSize() <= 0);

            --AllOpenNodesCount;
            return base.GetMinOpenNode();
        }

        /// <summary>
        /// Computes the heuristic value for the specified node and inserts both into open nodes. Should not be called separately, but only
        /// within AddToOpenNodes() method containing necessary checks and additional operations.
        /// </summary>
        /// <param name="node">Node to be added.</param>
        /// <param name="gValue">Calculated distance from the start.</param>
        /// <param name="predecessor">Predecessor node.</param>
        /// <param name="appliedOperator">Predecessor operator.</param>
        protected override void ComputeHeuristicAndInsertNewOpenNode(ISearchNode node, int gValue, ISearchNode predecessor, IOperator appliedOperator)
        {
            for (int i = 0; i < NumberOfOpenLists; ++i)
            {
                Heuristic = Heuristics[i];
                OpenNodes = OpenLists[i];

                double hValue = Heuristic.GetValue(node);
                if (!double.IsInfinity(hValue)) // Infinity heuristic indicates dead-end
                {
                    OpenNodes.Add(ComputeNewOpenNodeValueWithTieBreakingRule(gValue, hValue), node);
                    ++AllOpenNodesCount;
                }
            }
        }

        /// <summary>
        /// Gets the number of open nodes.
        /// </summary>
        /// <returns>Number of open nodes.</returns>
        protected override long GetOpenNodesCount()
        {
            return AllOpenNodesCount;
        }

        /// <summary>
        /// Gets the number of processed nodes.
        /// </summary>
        /// <returns>Number of processed nodes.</returns>
        protected override long GetProcessedNodesCount()
        {
            return NodesInfo.Count;
        }

        /// <summary>
        /// Logs the search statistics.
        /// </summary>
        protected override void LogSearchStatistics()
        {
            if (LoggingEnabled)
            {
                string message = $"Closed nodes: {GetClosedNodesCount()}" +
                                 $"\tOpen nodes: {GetOpenNodesCount()}" +
                                 $"\tMax g-value: {MaxGValue}";

                IHeuristic heuristic = Heuristic as IHeuristic;
                if (heuristic != null)
                {
                    message += $"\tHeuristic calls: {heuristic.GetCallsCount()}" +
                               $"\tMin heuristic: ({string.Join(", ", Heuristics.Select(h => ((IHeuristic)h).GetStatistics().BestHeuristicValue))})" +
                               $"\tAvg heuristic: ({string.Join(", ", Heuristics.Select(h => ((IHeuristic)h).GetStatistics().AverageHeuristicValue.ToString("0.###")))})";
                }
                message += $"\tCurrent time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}";

                LogMessage(message);
            }
        }

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        public override string GetName()
        {
            return (SearchType == SearchType.Forward) ? "Multi-Heuristic A* Search" : "Multi-Heuristic A* Search (Backward)";
        }
    }
}
