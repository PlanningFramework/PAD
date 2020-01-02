using System.Collections.Generic;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Solution plan of the planning problem. Can be a sequence of operators or just states.
    /// </summary>
    public interface ISolutionPlan
    {
        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISearchNode> GetStatesSequence();

        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IOperator> GetOperatorsSequence();

        /// <summary>
        /// Gets the solution cost (i.e. solution length).
        /// </summary>
        /// <returns>Solution cost.</returns>
        double GetCost();

        /// <summary>
        /// Gets the full string description of the solution plan, i.e. the full path of operartor applications
        /// including all the intermediate states.
        /// </summary>
        /// <returns>Full description of the solution plan.</returns>
        string GetFullDescription();
    }
}
