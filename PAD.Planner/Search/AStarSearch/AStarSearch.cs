using PAD.Planner.Heuristics;
using PAD.Planner.Heaps;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Implementation of A* search procedure (for both forward and backward planning search).
    /// </summary>
    public class AStarSearch : HeuristicSearch
    {
        /// <summary>
        /// Open nodes of the A* search.
        /// </summary>
        public IHeap OpenNodes { set; get; }

        /// <summary>
        /// Information about already processed nodes (gValue, predecessor, open/closed).
        /// </summary>
        protected Dictionary<ISearchNode, NodeInfo> NodesInfo { set; get; } = new Dictionary<ISearchNode, NodeInfo>();

        /// <summary>
        /// Constructs the A* search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic (if not specified, blind heuristic will be used).</param>
        /// <param name="heap">Heap collection (if not specified, red-black heap will be used).</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        public AStarSearch(ISearchableProblem problem, ISearchableHeuristic heuristic = null, IHeap heap = null, bool loggingEnabled = false)
            : base(problem, heuristic, loggingEnabled)
        {
            OpenNodes = heap ?? GetDefaultHeap();
        }

        /// <summary>
        /// Constructs the A* search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        /// <param name="heap">Heap collection.</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        /// <param name="timeLimitOfSearch">Time limit of the search.</param>
        /// <param name="memoryLimitOfStates">Memory limit of searched nodes.</param>
        public AStarSearch(ISearchableProblem problem, ISearchableHeuristic heuristic, IHeap heap, bool loggingEnabled, TimeSpan timeLimitOfSearch, long memoryLimitOfStates)
            : this(problem, heuristic, heap, loggingEnabled)
        {
            TimeLimitOfSearch = timeLimitOfSearch;
            MemoryLimitOfStates = memoryLimitOfStates;
        }

        /// <summary>
        /// Specifies the default heap used, if not provided by the user.
        /// </summary>
        /// <returns>Default heap.</returns>
        protected static IHeap GetDefaultHeap()
        {
            return new RedBlackTreeHeap();
        }

        /// <summary>
        /// Starts the search procedure.
        /// </summary>
        /// <param name="type">Type of search.</param>
        /// <returns>Result of the search.</returns>
        public override ResultStatus Start(SearchType type = SearchType.Forward)
        {
            InitSearch(type);
            SetInitialNode(GetInitialNode());

            while (HasAnyOpenNode())
            {
                if (IsTimeLimitExceeded())
                {
                    return FinishSearch(ResultStatus.TimeLimitExceeded);
                }

                if (IsMemoryLimitExceeded())
                {
                    return FinishSearch(ResultStatus.MemoryLimitExceeded);
                }

                LogSearchStatistics();

                ISearchNode node = GetMinOpenNode();

                var nodeInfo = NodesInfo[node];
                if (!nodeInfo.IsClosed)
                {
                    if (Problem.IsGoalNode(node))
                    {
                        return FinishSearch(ResultStatus.SolutionFound, node);
                    }

                    ProcessTransitionsFromNode(node, nodeInfo.GValue);

                    nodeInfo.IsClosed = true;
                    ++ClosedNodesCount;
                }
            }

            return FinishSearch(ResultStatus.NoSolutionFound);
        }

        /// <summary>
        /// Initialization of the search procedure. Should be done before every search!
        /// </summary>
        /// <param name="type">Search type.</param>
        protected override void InitSearch(SearchType type)
        {
            base.InitSearch(type);

            OpenNodes.Clear();
            NodesInfo.Clear();
        }

        /// <summary>
        /// Sets the initial search node of the search procedure.
        /// </summary>
        /// <param name="initialNode">Initial node of the search.</param>
        protected virtual void SetInitialNode(ISearchNode initialNode)
        {
            OpenNodes.Add(0, initialNode);
            NodesInfo.Add(initialNode, new NodeInfo(0, null));
        }

        /// <summary>
        /// Checks whether there is any open node for the search available.
        /// </summary>
        /// <returns>True if there is some open node available. False otherwise.</returns>
        protected virtual bool HasAnyOpenNode()
        {
            return (OpenNodes.GetSize() > 0);
        }

        /// <summary>
        /// Gets the new open node from the list of open nodes (with minimal value).
        /// </summary>
        /// <returns>New open node.</returns>
        protected virtual ISearchNode GetMinOpenNode()
        {
            return OpenNodes.RemoveMin();
        }

        /// <summary>
        /// Processes the transitions from the specified search node.
        /// </summary>
        /// <param name="node">Search node to be processed.</param>
        /// <param name="gValue">Distance of the node from the initial node.</param>
        protected virtual void ProcessTransitionsFromNode(ISearchNode node, int gValue)
        {
            MaxGValue = Math.Max(MaxGValue, gValue);

            foreach (var transition in Problem.GetTransitions(node))
            {
                IOperator appliedOperator = transition.GetAppliedOperator();
                int gValueNew = gValue + (appliedOperator?.GetCost() ?? 0);

                if (!transition.IsComplexTransition())
                {
                    ISearchNode newNode = transition.GetTransitionResult();
                    AddToOpenNodes(newNode, gValueNew, node, appliedOperator);
                }
                else
                {
                    foreach (var newNode in transition.GetComplexTransitionResults())
                    {
                        AddToOpenNodes(newNode, gValueNew, node, appliedOperator);
                    }
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
        protected virtual void AddToOpenNodes(ISearchNode node, int gValue, ISearchNode predecessor, IOperator appliedOperator)
        {
            NodeInfo nodeInfo;
            if (NodesInfo.TryGetValue(node, out nodeInfo))
            {
                if (nodeInfo.GValue > gValue)
                {
                    nodeInfo.GValue = gValue;
                    nodeInfo.Predecessor = Tuple.Create(predecessor, appliedOperator);
                    NodesInfo[node] = nodeInfo;

                    if (!nodeInfo.IsClosed)
                    {
                        ComputeHeuristicAndInsertNewOpenNode(node, gValue, predecessor, appliedOperator);
                    }
                }
            }
            else
            {
                NodesInfo.Add(node, new NodeInfo(gValue, Tuple.Create(predecessor, appliedOperator)));
                ComputeHeuristicAndInsertNewOpenNode(node, gValue, predecessor, appliedOperator);
            }
        }

        /// <summary>
        /// Computes the heuristic value for the specified node and inserts both into open nodes. Should not be called separately, but only
        /// within AddToOpenNodes() method containing necessary checks and additional operations.
        /// </summary>
        /// <param name="node">Node to be added.</param>
        /// <param name="gValue">Calculated distance from the start.</param>
        /// <param name="predecessor">Predecessor node.</param>
        /// <param name="appliedOperator">Predecessor operator.</param>
        protected virtual void ComputeHeuristicAndInsertNewOpenNode(ISearchNode node, int gValue, ISearchNode predecessor, IOperator appliedOperator)
        {
            double hValue = (IsComplexHeuristic) ? ((ComplexHeuristic)Heuristic).GetValue(node, predecessor, appliedOperator) : Heuristic.GetValue(node);

            if (!double.IsInfinity(hValue)) // Infinity heuristic indicates dead-end
            {
                OpenNodes.Add(ComputeNewOpenNodeValueWithTieBreakingRule(gValue, hValue), node);
            }
        }

        /// <summary>
        /// Computes the key value for new open nodes with respect to tie breaking rules.
        /// </summary>
        /// <param name="gValue">Calculated distance from the start.</param>
        /// <param name="hValue">Heuristic estimate.</param>
        /// <returns>Calculated value for the new open node.</returns>
        protected double ComputeNewOpenNodeValueWithTieBreakingRule(double gValue, double hValue)
        {
            // Breaking ties in favor of nodes that have lesser heuristic estimates. Heuristic value needs to be lesser than the tie breaking factor!
            const double tieBreakingScaleFactor = 10000.0;
            return (gValue + hValue + hValue / tieBreakingScaleFactor);
        }

        /// <summary>
        /// Gets the solution plan (i.e. a concrete sequence of applied operators).
        /// </summary>
        /// <returns>Solution plan.</returns>
        public override ISolutionPlan GetSolutionPlan()
        {
            if (ResultStatus != ResultStatus.SolutionFound)
            {
                return null;
            }

            var predecessor = GetPredecessor(GoalNode);
            bool isStatesSpecific = (predecessor != null && predecessor.Item2 == null);

            if (isStatesSpecific) // no operators involved
            {
                SolutionPlanViaStates solution = new SolutionPlanViaStates {GoalNode};

                while (predecessor != null)
                {
                    solution.Add(predecessor.Item1);
                    predecessor = GetPredecessor(predecessor.Item1);
                }

                if (SearchType == SearchType.Forward)
                {
                    solution.Reverse();
                }

                return solution;
            }
            else
            {
                SolutionPlan solution = new SolutionPlan(Problem.GetInitialNode());
                while (predecessor != null)
                {
                    solution.Add(predecessor.Item2);
                    predecessor = GetPredecessor(predecessor.Item1);
                }

                if (SearchType == SearchType.Forward)
                {
                    solution.Reverse();
                }

                return solution;
            }
        }

        /// <summary>
        /// Gets the cost of the found solution plan.
        /// </summary>
        /// <returns>Solution plan cost.</returns>
        public override double GetSolutionCost()
        {
            if (GoalNode == null)
            {
                return double.NaN;
            }
            return NodesInfo[GoalNode].GValue;
        }

        /// <summary>
        /// Gets the predecessor for the given node, after the search successfully ended.
        /// </summary>
        /// <param name="node">Search node.</param>
        /// <returns>Predecessor node and the corresponding applied operator.</returns>
        protected Tuple<ISearchNode, IOperator> GetPredecessor(ISearchNode node)
        {
            return NodesInfo[node].Predecessor;
        }

        /// <summary>
        /// Gets the number of open nodes.
        /// </summary>
        /// <returns>Number of open nodes.</returns>
        protected override long GetOpenNodesCount()
        {
            return OpenNodes.GetSize();
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
                                 $"\tMax g-Value: {MaxGValue}";

                IHeuristic heuristic = Heuristic as IHeuristic;
                if (heuristic != null)
                {
                    message += $"\tHeuristic calls: {heuristic.GetCallsCount()}" +
                               $"\tMin heuristic: {heuristic.GetStatistics().BestHeuristicValue}" +
                               $"\tAvg heuristic: {heuristic.GetStatistics().AverageHeuristicValue:0.###}";
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
            return (SearchType == SearchType.Forward) ? "A* Search" : "A* Search (Backward)";
        }

        /// <summary>
        /// Gets the set of currently closed search nodes.
        /// </summary>
        /// <returns>Set of closed search nodes.</returns>
        public HashSet<ISearchNode> GetClosedNodes()
        {
            return new HashSet<ISearchNode>(NodesInfo.Keys.Where(node => NodesInfo[node].IsClosed));
        }

        /// <summary>
        /// Gets the set of currently open search nodes.
        /// </summary>
        /// <returns>Set of open search nodes.</returns>
        public HashSet<ISearchNode> GetOpenNodes()
        {
            return new HashSet<ISearchNode>(NodesInfo.Keys.Where(node => !NodesInfo[node].IsClosed));
        }
    }
}
