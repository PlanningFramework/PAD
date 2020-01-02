using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Results of the searching procedure.
    /// </summary>
    public class SearchResults
    {
        /// <summary>
        /// Result status. Has the search been successful, or has it failed?
        /// </summary>
        public ResultStatus ResultStatus { set; get; } = ResultStatus.NoSolutionFound;

        /// <summary>
        /// Domain name.
        /// </summary>
        public string DomainName { set; get; } = "";

        /// <summary>
        /// Problem name.
        /// </summary>
        public string ProblemName { set; get; } = "";

        /// <summary>
        /// Search algorithm used.
        /// </summary>
        public string Algorithm { set; get; } = "";

        /// <summary>
        /// Search heuristic used.
        /// </summary>
        public string Heuristic { set; get; } = "";

        /// <summary>
        /// Elapsed time of the search in miliseconds.
        /// </summary>
        public TimeSpan SearchTime { set; get; } = default(TimeSpan);

        /// <summary>
        /// Number of closed nodes.
        /// </summary>
        public long ClosedNodes { set; get; } = -1;

        /// <summary>
        /// Number of open nodes.
        /// </summary>
        public long OpenNodes { set; get; } = -1;

        /// <summary>
        /// Maximal g-value.
        /// </summary>
        public int MaxGValue { set; get; } = -1;

        /// <summary>
        /// Best heuristic value.
        /// </summary>
        public double BestHeuristicValue { set; get; } = double.MaxValue;

        /// <summary>
        /// Best heuristic value.
        /// </summary>
        public double AverageHeuristicValue { set; get; } = double.MaxValue;

        /// <summary>
        /// Solution to the planning problem, if any found.
        /// </summary>
        public ISolutionPlan SolutionPlan { set; get; } = null;

        /// <summary>
        /// Cost of the solution plan (i.e. plan length), if any found.
        /// </summary>
        public double SolutionCost { set; get; } = 0.0;

        /// <summary>
        /// Constructs empty the results of the searching procedure.
        /// </summary>
        public SearchResults()
        {
        }

        /// <summary>
        /// Constructs results of the searching procedure.
        /// </summary>
        /// <param name="resultStatus">Result status.</param>
        /// <param name="domainName">Domain name.</param>
        /// <param name="problemName">Problem name.</param>
        /// <param name="algorithm">Search algorithm name.</param>
        /// <param name="heuristic">Search heuristic name.</param>
        /// <param name="searchTime">Elapsed time of the search.</param>
        /// <param name="closedNodes">Number of closed nodes.</param>
        /// <param name="openNodes">Number of open nodes.</param>
        /// <param name="maxGValue">Max g-value.</param>
        /// <param name="bestHeuristicValue">Best heuristic value.</param>
        /// <param name="averageHeuristicValue">Average heuristic value.</param>
        /// <param name="solutionPlan">Solution plan (if found).</param>
        /// <param name="solutionCost">Solution cost (if solution found).</param>
        public SearchResults(ResultStatus resultStatus, string domainName, string problemName, string algorithm, string heuristic, TimeSpan searchTime, long closedNodes,
            long openNodes, int maxGValue, double bestHeuristicValue, double averageHeuristicValue, ISolutionPlan solutionPlan = null, double solutionCost = 0.0)
        {
            ResultStatus = resultStatus;
            DomainName = domainName;
            ProblemName = problemName;
            Algorithm = algorithm;
            Heuristic = heuristic;
            SearchTime = searchTime;
            ClosedNodes = closedNodes;
            OpenNodes = openNodes;
            MaxGValue = maxGValue;
            BestHeuristicValue = bestHeuristicValue;
            AverageHeuristicValue = averageHeuristicValue;
            SolutionPlan = solutionPlan;
            SolutionCost = solutionCost;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return GetFullDescription();
        }

        /// <summary>
        /// Gets a full description of the search result.
        /// </summary>
        /// <returns>Full description of the search result.</returns>
        public string GetFullDescription()
        {
            string domainName = string.IsNullOrEmpty(DomainName) ? "N/A" : DomainName;

            var descriptionParts = new List<string>()
            {
                $"Domain: {domainName}, Problem: {ProblemName}, Algorithm: {Algorithm}, Heuristic: {Heuristic}",
                $"Result: {ResultStatusToString(ResultStatus)}, Elapsed Time: {SearchTime.TotalMilliseconds} ms",
                $"Closed nodes: {ClosedNodes}, Open nodes: {OpenNodes}, Max g-value: {MaxGValue}",
                $"Best heuristic value: {BestHeuristicValue}, Average heuristic value: {AverageHeuristicValue}",
            };

            if (SolutionPlan != null)
            {
                descriptionParts.Add($"Solution cost: {SolutionCost}");
                descriptionParts.Add($"Solution plan: {SolutionPlan}");
            }

            return string.Join(Environment.NewLine, descriptionParts);
        }

        /// <summary>
        /// Gets a brief description of the search result.
        /// </summary>
        /// <returns>Bried description of the search result.</returns>
        public string GetBriefDescription()
        {
            var descriptionParts = new List<string>()
            {
                $"Result: {ResultStatusToString(ResultStatus)}, Elapsed Time: {SearchTime.TotalMilliseconds} ms"
            };

            if (SolutionPlan != null)
            {
                descriptionParts.Add($"Solution cost: {SolutionCost}");
            }

            return string.Join(Environment.NewLine, descriptionParts);
        }

        /// <summary>
        /// Gets the string description for the result status.
        /// </summary>
        /// <param name="resultStatus">Result status.</param>
        /// <returns>String description of the result status.</returns>
        public static string ResultStatusToString(ResultStatus resultStatus)
        {
            switch (resultStatus)
            {
                case ResultStatus.Idle:
                    return "Search not performed";
                case ResultStatus.SolutionFound:
                    return "Solution found";
                case ResultStatus.NoSolutionFound:
                    return "Solution not found";
                case ResultStatus.TimeLimitExceeded:
                    return "Search failed - time limit exceeded";
                case ResultStatus.MemoryLimitExceeded:
                    return "Search failed - memory limit exceeded";
                default:
                    Debug.Assert(false);
                    return "?";
            }
        }
    }
}
