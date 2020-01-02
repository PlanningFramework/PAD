using PAD.Planner.Heuristics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Implementation of an abstracted Hill-Climbing search procedure (for both forward and backward planning search).
    /// </summary>
    public class HillClimbingSearch : HeuristicSearch
    {
        /// <summary>
        /// Number of processed states.
        /// </summary>
        private long ProcessedStatesCount { set; get; } = 0;

        /// <summary>
        /// Evaluated solution cost.
        /// </summary>
        protected double SolutionCost { set; get; } = -1.0;

        /// <summary>
        /// Evaluated solution plan.
        /// </summary>
        protected SolutionPlan SolutionPlan { set; get; } = null;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random RandomNumberGenerator { set; get; } = new Random();

        /// <summary>
        /// Constructs the Hill-Climbing search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic (if not specified, blind heuristic will be used).</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        public HillClimbingSearch(ISearchableProblem problem, ISearchableHeuristic heuristic = null, bool loggingEnabled = false)
            : base(problem, heuristic, loggingEnabled)
        {
        }

        /// <summary>
        /// Constructs the Hill-Climbing search procedure.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?</param>
        /// <param name="timeLimitOfSearch">Time limit of the search.</param>
        /// <param name="memoryLimitOfStates">Memory limit of searched nodes.</param>
        public HillClimbingSearch(ISearchableProblem problem, ISearchableHeuristic heuristic, bool loggingEnabled, TimeSpan timeLimitOfSearch, long memoryLimitOfStates)
            : base(problem, heuristic, loggingEnabled, timeLimitOfSearch, memoryLimitOfStates)
        {
        }

        /// <summary>
        /// Starts the search procedure.
        /// </summary>
        /// <param name="type">Type of search.</param>
        /// <returns>Result of the search.</returns>
        public override ResultStatus Start(SearchType type = SearchType.Forward)
        {
            InitSearch(type);

            List<IOperator> transitionCandidates = new List<IOperator>();
            
            ISearchNode currentNode = GetInitialNode();
            while (!Problem.IsGoalNode(currentNode))
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

                transitionCandidates.Clear();
                double bestCost = double.MaxValue;
                IOperator bestCostOperator = null;

                foreach (var transition in Problem.GetTransitions(currentNode))
                {
                    IOperator appliedOperator = transition.GetAppliedOperator();
                    ISearchNode node = transition.GetTransitionResult();

                    double transitionCost = appliedOperator.GetCost() + Heuristic.GetValue(node);
                    if (transitionCost < bestCost)
                    {
                        bestCost = transitionCost;
                        transitionCandidates.Clear();
                        transitionCandidates.Add(appliedOperator);
                    }
                    else if (transitionCost == bestCost)
                    {
                        transitionCandidates.Add(appliedOperator);
                    }
                }

                if (transitionCandidates.Count == 0)
                {
                    return FinishSearch(ResultStatus.NoSolutionFound); // dead-end reached
                }
                else if (transitionCandidates.Count == 1)
                {
                    bestCostOperator = transitionCandidates[0];
                }
                else
                {
                    bestCostOperator = transitionCandidates[RandomNumberGenerator.Next(transitionCandidates.Count)];
                }

                SolutionPlan.Add(bestCostOperator);
                SolutionCost += bestCostOperator.GetCost();

                currentNode = Apply(currentNode, bestCostOperator);
                ++ProcessedStatesCount;
            }

            if (SearchType != SearchType.Forward)
            {
                SolutionPlan.Reverse();
            }

            return FinishSearch(ResultStatus.SolutionFound, currentNode);
        }

        /// <summary>
        /// Initialization of the search procedure. Should be done before every search!
        /// </summary>
        /// <param name="type">Search type.</param>
        protected override void InitSearch(SearchType type)
        {
            base.InitSearch(type);

            ProcessedStatesCount = 0;
            SolutionCost = 0.0;
            SolutionPlan = new SolutionPlan(Problem.GetInitialNode());
        }

        /// <summary>
        /// Applies the specified operator to the specified search node.
        /// </summary>
        /// <param name="node">Search node.</param>
        /// <param name="oper">Operator to be applied.</param>
        /// <returns>New search node.</returns>
        protected ISearchNode Apply(ISearchNode node, IOperator oper)
        {
            switch (SearchType)
            {
                case SearchType.Forward:
                    return oper.Apply((IState)node);
                case SearchType.BackwardWithConditions:
                    return oper.ApplyBackwards((IConditions)node);
                case SearchType.BackwardWithStates:
                    return oper.ApplyBackwards((IRelativeState)node).First();
                default:
                    throw new NotSupportedException("Unknown search type!");
            }
        }

        /// <summary>
        /// Gets the number of processed nodes.
        /// </summary>
        /// <returns>Number of processed nodes.</returns>
        protected override long GetProcessedNodesCount()
        {
            return ProcessedStatesCount;
        }

        /// <summary>
        /// Gets the solution plan (i.e. a concrete sequence of applied operators).
        /// </summary>
        /// <returns>Solution plan.</returns>
        public override ISolutionPlan GetSolutionPlan()
        {
            return SolutionPlan;
        }

        /// <summary>
        /// Gets the cost of the found solution plan.
        /// </summary>
        /// <returns>Solution plan cost.</returns>
        public override double GetSolutionCost()
        {
            return SolutionCost;
        }

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        public override string GetName()
        {
            return (SearchType == SearchType.Forward) ? "Hill-Climbing Search" : "Hill-Climbing Search (Backward)";
        }
    }
}
