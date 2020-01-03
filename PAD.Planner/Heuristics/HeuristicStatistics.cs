using System;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Auxiliary object encapsulating heuristic statistics.
    /// </summary>
    public class HeuristicStatistics
    {
        /// <summary>
        /// Best heuristic value.
        /// </summary>
        public double BestHeuristicValue { set; get; } = double.MaxValue;

        /// <summary>
        /// Average heuristic value.
        /// </summary>
        public double AverageHeuristicValue => SumOfHeuristicValues / HeuristicCallsCount;

        /// <summary>
        /// Sum of heuristic values.
        /// </summary>
        public double SumOfHeuristicValues { set; get; }

        /// <summary>
        /// Number of heuristic calls.
        /// </summary>
        public long HeuristicCallsCount { set; get; }

        /// <summary>
        /// Should the statistics of the heuristic usage be measured?
        /// </summary>
        public bool DoMeasure { set; get; } = true;

        /// <summary>
        /// Updates the statistics based on the new heuristic value.
        /// </summary>
        /// <param name="heuristicValue">Heuristic value.</param>
        public void UpdateStatistics(double heuristicValue)
        {
            // infinity heuristic indicates dead-end (we don't want these included in the computation of the average heuristic value)
            if (!double.IsInfinity(heuristicValue))
            {
                HeuristicCallsCount++;
                SumOfHeuristicValues += heuristicValue;
                BestHeuristicValue = Math.Min(BestHeuristicValue, heuristicValue);
            }
        }
    }
}
