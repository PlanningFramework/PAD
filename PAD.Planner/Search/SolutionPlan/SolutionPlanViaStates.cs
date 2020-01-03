using System.Collections.Generic;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Solution plan explicitly enumerating the states from the initial state to a goal state, without operators.
    /// </summary>
    public class SolutionPlanViaStates : List<ISearchNode>, ISolutionPlan
    {
        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISearchNode> GetStatesSequence()
        {
            return this;
        }

        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IOperator> GetOperatorsSequence()
        {
            yield break;
        }

        /// <summary>
        /// Gets the solution cost (i.e. solution length).
        /// </summary>
        /// <returns>Solution cost.</returns>
        public double GetCost()
        {
            return Count;
        }

        /// <summary>
        /// Gets the full string description of the solution plan, i.e. the full path of operator applications
        /// including all the intermediate states.
        /// </summary>
        /// <returns>Full description of the solution plan.</returns>
        public string GetFullDescription()
        {
            return string.Join(", ", this);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return GetFullDescription();
        }
    }
}
