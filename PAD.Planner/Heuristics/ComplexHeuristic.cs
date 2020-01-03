
namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Base class for complex PDDL or SAS+ heuristics using the preceding entities to determine the heuristic value.
    /// </summary>
    public abstract class ComplexHeuristic : Heuristic
    {
        /// <summary>
        /// Gets the heuristic value for the given node.
        /// </summary>
        /// <param name="node">Node to be evaluated.</param>
        /// <param name="predecessor">Preceding node.</param>
        /// <param name="appliedOperator">Applied operator from the preceding node.</param>
        /// <returns>Heuristic value for the specified node.</returns>
        public double GetValue(ISearchNode node, ISearchNode predecessor, IOperator appliedOperator)
        {
            // at the moment, specific for states only!
            System.Diagnostics.Debug.Assert(node is IState && predecessor is IState);
            return GetValue((IState)node, (IState)predecessor, appliedOperator);
        }

        /// <summary>
        /// Gets the heuristic value for the given state.
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <param name="predecessor">Preceding state.</param>
        /// <param name="appliedOperator">Applied operator from the preceding state.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        public double GetValue(IState state, IState predecessor, IOperator appliedOperator)
        {
            if (Statistics.DoMeasure)
            {
                double value = GetValueImpl(state, predecessor, appliedOperator);
                Statistics.UpdateStatistics(value);
                return value;
            }
            return GetValueImpl(state, predecessor, appliedOperator);
        }

        /// <summary>
        /// Gets the heuristic value for the given state.
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <param name="predecessor">Preceding state.</param>
        /// <param name="appliedOperator">Applied operator from the preceding state.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected virtual double GetValueImpl(IState state, IState predecessor, IOperator appliedOperator)
        {
            return GetValueImpl(state);
        }
    }
}
