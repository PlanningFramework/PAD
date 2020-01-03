using System;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Common base class for PDDL or SAS+ heuristics.
    /// </summary>
    public abstract class Heuristic : IHeuristic
    {
        /// <summary>
        /// Reference to the planning problem.
        /// </summary>
        public IProblem Problem { set; get; }

        /// <summary>
        /// Heuristic statistics.
        /// </summary>
        public HeuristicStatistics Statistics { set; get; } = new HeuristicStatistics();

        /// <summary>
        /// Constructs the heuristics.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        protected Heuristic(IProblem problem = null)
        {
            Problem = problem;
        }

        /// <summary>
        /// Gets the heuristic statistics.
        /// </summary>
        /// <returns>Heuristic statistics.</returns>
        public HeuristicStatistics GetStatistics()
        {
            return Statistics;
        }

        /// <summary>
        /// Gets the heuristic value for the given search node.
        /// </summary>
        /// <param name="node">Node to be evaluated.</param>
        /// <returns>Heuristic value for the specified node.</returns>
        public double GetValue(ISearchNode node)
        {
            return ((IStateOrConditions)node).DetermineHeuristicValue(this);
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        public double GetValue(IState state)
        {
            if (Statistics.DoMeasure)
            {
                double value = GetValueImpl(state);
                Statistics.UpdateStatistics(value);
                return value;
            }
            return GetValueImpl(state);
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        public double GetValue(IConditions conditions)
        {
            if (Statistics.DoMeasure)
            {
                double value = GetValueImpl(conditions);
                Statistics.UpdateStatistics(value);
                return value;
            }
            return GetValueImpl(conditions);
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        public double GetValue(IRelativeState relativeState)
        {
            if (Statistics.DoMeasure)
            {
                double value = GetValueImpl(relativeState);
                Statistics.UpdateStatistics(value);
                return value;
            }
            return GetValueImpl(relativeState);
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected virtual double GetValueImpl(IState state)
        {
            throw new NotImplementedException("The heuristic does not implement forward search evaluation!");
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected virtual double GetValueImpl(IConditions conditions)
        {
            throw new NotImplementedException("The heuristic does not implement backward search evaluation!");
        }

        /// <summary>
        /// Gets the heuristic value for the given relative state (in the context of backward search).
        /// </summary>
        /// <param name="relativeState">Relative state to be evaluated.</param>
        /// <returns>Heuristic value for the specified relative state.</returns>
        protected virtual double GetValueImpl(IRelativeState relativeState)
        {
            return GetValueImpl(relativeState.GetDescribingConditions(Problem));
        }

        /// <summary>
        /// Gets a number of heuristic calls.
        /// </summary>
        /// <returns>Number of heuristic calls.</returns>
        public long GetCallsCount()
        {
            return Statistics.HeuristicCallsCount;
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public abstract string GetName();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return GetName();
        }
    }
}
