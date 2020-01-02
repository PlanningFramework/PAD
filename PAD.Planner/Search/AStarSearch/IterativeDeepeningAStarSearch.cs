using PAD.Planner.Heuristics;
using PAD.Planner.Heaps;
using System.Diagnostics;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Implementation of Iterative Deepening A* search (IDA*) procedure. This search variant limited A* search for iterative progression.
    /// </summary>
    public class IterativeDeepeningAStarSearch : AStarSearch
    {
        /// <summary>
        /// Currently used limit value.
        /// </summary>
        private double LimitValue { set; get; } = 0.0;

        /// <summary>
        /// Lowest value f-value over limit, which will be used for the next iteration.
        /// </summary>
        private double LowestValueOverLimit { set; get; } = 0.0;

        /// <summary>
        /// Constructs the Iterative Deepening A* search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        public IterativeDeepeningAStarSearch(ISearchableProblem problem, ISearchableHeuristic heuristic) : base(problem, heuristic)
        {
            // simple stack is used as a collection for open nodes (=> implies deep-first-search).
            OpenNodes = new SimpleStack();
        }

        /// <summary>
        /// Starts the search procedure.
        /// </summary>
        /// <param name="type">Type of search.</param>
        /// <returns>Result of the search.</returns>
        public override ResultStatus Start(SearchType type = SearchType.Forward)
        {
            LimitValue = Heuristic.GetValue(GetInitialNode());
            LowestValueOverLimit = double.MaxValue;
            Stopwatch fullSearchTimer = Stopwatch.StartNew();

            while (true)
            {
                LogMessage($"Searching IDA* with f-limit = {LimitValue}");
                var result = base.Start(type);

                switch (result)
                {
                    case ResultStatus.SolutionFound:
                    case ResultStatus.TimeLimitExceeded:
                    case ResultStatus.MemoryLimitExceeded:
                        fullSearchTimer.Stop();
                        Timer = fullSearchTimer;
                        return result;

                    case ResultStatus.NoSolutionFound:
                        LogMessage("Current IDA* iteration failed. Increasing the limit.");
                        LimitValue = LowestValueOverLimit;
                        LowestValueOverLimit = double.MaxValue;
                        break;

                    default:
                        throw new Exception("Unsupported result type.");
                }
            }
        }

        /// <summary>
        /// Adds the specified node to the open nodes collection.
        /// </summary>
        /// <param name="node">Node to be added.</param>
        /// <param name="gValue">Calculated distance from the start.</param>
        /// <param name="predecessor">Predecessor node.</param>
        /// <param name="appliedOperator">Predecessor operator.</param>
        protected override void AddToOpenNodes(ISearchNode node, int gValue, ISearchNode predecessor, IOperator appliedOperator)
        {
            if (gValue > LimitValue)
            {
                return;
            }

            NodeInfo nodeInfo;
            if (NodesInfo.TryGetValue(node, out nodeInfo))
            {
                if (nodeInfo.GValue > gValue)
                {
                    nodeInfo.GValue = gValue;
                    nodeInfo.Predecessor = Tuple.Create(predecessor, appliedOperator);
                    NodesInfo[node] = nodeInfo;

                    double hValue = Heuristic.GetValue(node);
                    OpenNodes.Add(gValue + hValue, node);
                }
            }
            else
            {
                double hValue = Heuristic.GetValue(node);
                double fValue = gValue + hValue;
                if (fValue > LimitValue)
                {
                    LowestValueOverLimit = Math.Min(LowestValueOverLimit, fValue);
                    return;
                }
                NodesInfo.Add(node, new NodeInfo(gValue, Tuple.Create(predecessor, appliedOperator)));
                OpenNodes.Add(fValue, node);
            }
        }

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        public override string GetName()
        {
            return (SearchType == SearchType.Forward) ? "Iterative Deepening A* Search" : "Iterative Deepening A* Search (Backward)";
        }
    }
}
