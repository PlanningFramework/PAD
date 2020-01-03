using PAD.Planner.Heuristics;
using System.Diagnostics;
using System.Linq;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Base class of the heuristic search procedure.
    /// </summary>
    public abstract class HeuristicSearch : IHeuristicSearch
    {
        /// <summary>
        /// Instance of the planning problem.
        /// </summary>
        protected ISearchableProblem Problem { set; get; }

        /// <summary>
        /// Heuristic function.
        /// </summary>
        protected ISearchableHeuristic Heuristic { set; get; }

        /// <summary>
        /// Search type used (forward/backward search).
        /// </summary>
        protected SearchType SearchType { set; get; } = SearchType.Forward;

        /// <summary>
        /// Are logging messages enabled?
        /// </summary>
        public bool LoggingEnabled { set; get; }

        /// <summary>
        /// Time limit of the search.
        /// </summary>
        public TimeSpan TimeLimitOfSearch { set; get; } = TimeSpan.FromMinutes(DefaultTimeLimitMinutes);

        /// <summary>
        /// Memory limit of searched nodes.
        /// </summary>
        public long MemoryLimitOfStates { set; get; } = DefaultMemoryLimitNodes;

        /// <summary>
        /// Result status.
        /// </summary>
        protected ResultStatus ResultStatus { set; get; } = ResultStatus.Idle;

        /// <summary>
        /// Number of currently closed nodes.
        /// </summary>
        protected long ClosedNodesCount { set; get; }

        /// <summary>
        /// Maximal g-value encountered during the search.
        /// </summary>
        protected int MaxGValue { set; get; } = -1;

        /// <summary>
        /// Found goal node (used for extraction of solution plan and solution cost).
        /// </summary>
        protected ISearchNode GoalNode { set; get; }

        /// <summary>
        /// Timer for calculation measurements.
        /// </summary>
        protected Stopwatch Timer { set; get; }

        /// <summary>
        /// Is a complex heuristic used? (i.e. uses predecessors and operators to determine the heuristic value)
        /// </summary>
        protected bool IsComplexHeuristic { set; get; }

        /// <summary>
        /// Default time limit for the search procedure, in minutes.
        /// </summary>
        public const int DefaultTimeLimitMinutes = 30;

        /// <summary>
        /// Default memory limit of searched nodes.
        /// </summary>
        public const long DefaultMemoryLimitNodes = 1000000;

        /// <summary>
        /// Constructs the heuristic search procedure for the given planning problem.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic (if not specified, blind heuristic will be used).</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?.</param>
        protected HeuristicSearch(ISearchableProblem problem, ISearchableHeuristic heuristic = null, bool loggingEnabled = false)
        {
            Problem = problem;
            Heuristic = heuristic ?? new BlindHeuristic();
            LoggingEnabled = loggingEnabled;
            IsComplexHeuristic = Heuristic is ComplexHeuristic;
        }

        /// <summary>
        /// Constructs the heuristic search procedure for the given planning problem.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        /// <param name="loggingEnabled">Is logging of the search enabled?.</param>
        /// <param name="timeLimitOfSearch">Time limit of the search.</param>
        /// <param name="memoryLimitOfStates">Memory limit of searched nodes.</param>
        protected HeuristicSearch(ISearchableProblem problem, ISearchableHeuristic heuristic, bool loggingEnabled, TimeSpan timeLimitOfSearch, long memoryLimitOfStates)
            : this(problem, heuristic, loggingEnabled)
        {
            TimeLimitOfSearch = timeLimitOfSearch;
            MemoryLimitOfStates = memoryLimitOfStates;
        }

        /// <summary>
        /// Starts the search procedure.
        /// </summary>
        /// <param name="type">Type of search.</param>
        /// <returns>Result of the search.</returns>
        public abstract ResultStatus Start(SearchType type = SearchType.Forward);

        /// <summary>
        /// Initialization of the search procedure. Should be done before every search!
        /// </summary>
        /// <param name="type">Search type.</param>
        protected virtual void InitSearch(SearchType type)
        {
            SearchType = type;
            ResultStatus = ResultStatus.Idle;
            MaxGValue = -1;
            ClosedNodesCount = 0;
            GoalNode = null;

            LogSearchStart();
            StartTimer();
        }

        /// <summary>
        /// Finalization of the search procedure. Should be done after every search!
        /// </summary>
        /// <param name="resultStatus">Result of the search.</param>
        /// <param name="goalNode">Goal node (if solution found).</param>
        /// <returns>Result of the search.</returns>
        public virtual ResultStatus FinishSearch(ResultStatus resultStatus, ISearchNode goalNode = null)
        {
            ResultStatus = resultStatus;
            GoalNode = goalNode;

            StopTimer();
            LogSearchEnd();

            return resultStatus;
        }

        /// <summary>
        /// Gets the full search results of the planning search.
        /// </summary>
        /// <param name="includingSolutionPlan">Include the solution plan?</param>
        /// <returns>Search results.</returns>
        public SearchResults GetSearchResults(bool includingSolutionPlan = true)
        {
            IProblem problem = Problem as IProblem;
            IHeuristic heuristic = Heuristic as IHeuristic;

            SearchResults results = new SearchResults
            {
                ResultStatus = ResultStatus,
                DomainName = (problem != null) ? problem.GetDomainName() : "",
                ProblemName = Problem.GetProblemName(),
                Algorithm = GetName(),
                Heuristic = Heuristic.GetName(),
                SearchTime = GetSearchTime(),
                ClosedNodes = GetClosedNodesCount(),
                OpenNodes = GetOpenNodesCount(),
                MaxGValue = MaxGValue,
                BestHeuristicValue = heuristic?.GetStatistics().BestHeuristicValue ?? double.MaxValue,
                AverageHeuristicValue = heuristic?.GetStatistics().AverageHeuristicValue ?? double.MaxValue,
                SolutionPlan = null,
                SolutionCost = GetSolutionCost()
            };

            if (includingSolutionPlan)
            {
                results.SolutionPlan = GetSolutionPlan();
            }

            return results;
        }

        /// <summary>
        /// Gets the initial node of the search.
        /// </summary>
        /// <returns>Initial node of the search.</returns>
        protected ISearchNode GetInitialNode()
        {
            IProblem problem = Problem as IProblem;
            if (problem != null)
            {
                switch (SearchType)
                {
                    case SearchType.Forward:
                        return problem.GetInitialState();
                    case SearchType.BackwardWithConditions:
                        return problem.GetGoalConditions();
                    case SearchType.BackwardWithStates:
                        return problem.GetGoalConditions().GetCorrespondingRelativeStates(problem).First();
                    default:
                        throw new NotSupportedException("Unknown search type!");
                }
            }
            else
            {
                return Problem.GetInitialNode();
            }
        }

        /// <summary>
        /// Starts the timer of the calculation.
        /// </summary>
        protected void StartTimer()
        {
            Timer = Stopwatch.StartNew();
        }

        /// <summary>
        /// Stops the timer of the calculation.
        /// </summary>
        protected void StopTimer()
        {
            Timer.Stop();
        }

        /// <summary>
        /// Gets the elapsed search time.
        /// </summary>
        /// <returns>Elapsed search time.</returns>
        public TimeSpan GetSearchTime()
        {
            return Timer.Elapsed;
        }

        /// <summary>
        /// Checks whether the time limit of the search was exceeded.
        /// </summary>
        /// <returns>True if the time limit of the search was exceeded, false otherwise.</returns>
        protected bool IsTimeLimitExceeded()
        {
            return (Timer.Elapsed >= TimeLimitOfSearch);
        }

        /// <summary>
        /// Checks whether the memory limit of the search was exceeded.
        /// </summary>
        /// <returns>True if the memory limit of the search was exceeded, false otherwise.</returns>
        protected bool IsMemoryLimitExceeded()
        {
            return (GetProcessedNodesCount() >= MemoryLimitOfStates);
        }

        /// <summary>
        /// Logs the start of the search.
        /// </summary>
        protected void LogSearchStart()
        {
            if (LoggingEnabled)
            {
                LogMessage($"Search started. Algorithm: {GetName()}, Problem: {Problem.GetProblemName()}, Heuristics: {Heuristic.GetName()}");
            }
        }

        /// <summary>
        /// Logs the search statistics.
        /// </summary>
        protected virtual void LogSearchStatistics()
        {
        }

        /// <summary>
        /// Logs the given message to the console output.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        protected void LogMessage(string message)
        {
            if (LoggingEnabled)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Logs the end of the search.
        /// </summary>
        protected void LogSearchEnd()
        {
            if (LoggingEnabled)
            {
                LogMessage($"Search finished. Result: {SearchResults.ResultStatusToString(ResultStatus)}, Elapsed Time: {GetSearchTime().TotalMilliseconds} ms, Solution cost: {GetSolutionCost()}");
                LogSearchStatistics();
            }
        }

        /// <summary>
        /// Gets the number of closed nodes.
        /// </summary>
        /// <returns>Number of closed nodes.</returns>
        protected virtual long GetClosedNodesCount()
        {
            return ClosedNodesCount;
        }

        /// <summary>
        /// Gets the number of open nodes.
        /// </summary>
        /// <returns>Number of open nodes.</returns>
        protected virtual long GetOpenNodesCount()
        {
            return 0;
        }

        /// <summary>
        /// Gets the number of processed nodes.
        /// </summary>
        /// <returns>Number of processed nodes.</returns>
        protected virtual long GetProcessedNodesCount()
        {
            return GetClosedNodesCount();
        }

        /// <summary>
        /// Gets the solution plan (i.e. a concrete sequence of applied operators).
        /// </summary>
        /// <returns>Solution plan.</returns>
        public virtual ISolutionPlan GetSolutionPlan()
        {
            return null;
        }

        /// <summary>
        /// Gets the cost of the found solution plan.
        /// </summary>
        /// <returns>Solution plan cost.</returns>
        public virtual double GetSolutionCost()
        {
            return double.NaN;
        }

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        public abstract string GetName();
    }
}
