
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a pattern in the pattern database. Consists of the pattern variables (chosen non-abstracted variables) and
    /// all the precomputed distance values for the corresponding pattern values (concrete state values for the pattern variables).
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// Variables of the pattern (i.e. chosen non-abstracted variables in the corresponding abstracted planning problem).
        /// </summary>
        public int[] PatternVariables { set; get; } = null;

        /// <summary>
        /// Mapping of pattern values to their computed distances in the abstracted planning problem.
        /// </summary>
        public PatternValuesDistances PatternValuesDistances { set; get; } = null;

        /// <summary>
        /// Constructs the pattern.
        /// </summary>
        /// <param name="patternVariables">Pattern variables.</param>
        /// <param name="patternValuesDistances">Pattern values with computed corresponding distances.</param>
        public Pattern(int[] patternVariables, PatternValuesDistances patternValuesDistances)
        {
            PatternVariables = patternVariables;
            PatternValuesDistances = patternValuesDistances;
        }

        /// <summary>
        /// Gets the distance for the specified state.
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Distance to goals from the given state.</returns>
        public double GetDistance(IState state)
        {
            return PatternValuesDistances.GetDistance(state.GetValues(PatternVariables));
        }
    }
}
