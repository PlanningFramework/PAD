using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ model of a planning problem.
    /// </summary>
    public interface IProblem : ISearchableProblem
    {
        /// <summary>
        /// Gets the planning problem name.
        /// </summary>
        /// <returns>The name of the planning problem.</returns>
        string GetDomainName();

        /// <summary>
        /// Gets the input file path of the planning problem.
        /// </summary>
        /// <returns>Input file path.</returns>
        string GetInputFilePath();

        /// <summary>
        /// Gets the initial state of the planning problem.
        /// </summary>
        /// <returns>Initial state.</returns>
        IState GetInitialState();

        /// <summary>
        /// Sets the initial state of the planning problem.
        /// </summary>
        /// <param name="state">Initial state.</param>
        void SetInitialState(IState state);

        /// <summary>
        /// Gets the goal conditions of the planning problem.
        /// </summary>
        /// <returns>Goal conditions.</returns>
        IConditions GetGoalConditions();

        /// <summary>
        /// Sets the goal conditions of the planning problem.
        /// </summary>
        /// <param name="conditions">Goal conditions.</param>
        void SetGoalConditions(IConditions conditions);

        /// <summary>
        /// Checks whether the specified state is meeting goal conditions of the planning problem.
        /// </summary>
        /// <param name="state">A state to be checked.</param>
        /// <returns>True if the given state is a goal state of the problem, false otherwise.</returns>
        bool IsGoalState(IState state);

        /// <summary>
        /// Checks whether the initial state of the planning problem is meeting specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions to be checked.</param>
        /// <returns>True if the given conditions are satisfied for the initial state of the problem, false otherwise.</returns>
        bool IsStartConditions(IConditions conditions);

        /// <summary>
        /// Checks whether the initial state of the planning problem is meeting conditions specified by the given relative state.
        /// </summary>
        /// <param name="relativeState">Relative state to be checked.</param>
        /// <returns>True if the given relative state is satisfied for the initial state of the problem, false otherwise.</returns>
        bool IsStartRelativeState(IRelativeState relativeState);

        /// <summary>
        /// Gets the number of not accomplished goals for the specified state (forward search).
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished goals.</returns>
        int GetNotAccomplishedGoalsCount(IState state);

        /// <summary>
        /// Gets the number of not accomplished goals for the specified conditions (backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evalatuated.</param>
        /// <returns>Number of not accomplished goals.</returns>
        int GetNotAccomplishedGoalsCount(IConditions conditions);

        /// <summary>
        /// Gets a collection of all possible successors (forward transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        IEnumerable<ISuccessor> GetSuccessors(IState state);

        /// <summary>
        /// Enumeration method getting a list with a limited number of possible successors (forward transitions) from the specified state. The
        /// next call of this method returns new successors, until all of them are returned - then an empty collection is returned to signalize
        /// the end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <param name="numberOfSuccessors">Number of successors to be returned.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        IEnumerable<ISuccessor> GetNextSuccessors(IState state, int numberOfSuccessors);

        /// <summary>
        /// Gets a random successor (forward transition) from the specified state.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Random successor from the given state. Null if no valid successor found.</returns>
        ISuccessor GetRandomSuccessor(IState state);

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        IEnumerable<IPredecessor> GetPredecessors(IConditions conditions);

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        IEnumerable<IPredecessor> GetPredecessors(IState state);

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified relative state. Lazy generated via yield return.
        /// </summary>
        /// <param name="relativeState">Original state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        IEnumerable<IPredecessor> GetPredecessors(IRelativeState relativeState);

        /// <summary>
        /// Enumeration method getting a list with a limited number of relevant predecessors (backward transitions) from the specified conditions. The
        /// next call of this method returns new predecessors, until all of them are returned - then an empty collection is returned to signalize the
        /// end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <param name="numberOfPredecessors">Number of predecessors to be returned.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        IEnumerable<IPredecessor> GetNextPredecessors(IConditions conditions, int numberOfPredecessors);

        /// <summary>
        /// Gets a random relevant predecessor (backwards transition) from the specified conditions.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Random relevant predecessor from the specified conditions. Null if no valid predecessor found.</returns>
        IPredecessor GetRandomPredecessor(IConditions conditions);

        /// <summary>
        /// Gets a collection of all explicly enumerated successor states (created by forward applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all successor states.</returns>
        IEnumerable<IState> GetSuccessorStates(IState state);

        /// <summary>
        /// Gets a collection of all explicly enumerated predecessor states (created by relevant backwards applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all predecessor states.</returns>
        IEnumerable<IState> GetPredecessorStates(IState state);

        /// <summary>
        /// Creates the relaxed version of the current planning problem.
        /// </summary>
        /// <returns>Relaxed planning problem.</returns>
        IRelaxedProblem GetRelaxedProblem();

        /// <summary>
        /// Creates the pattern database of the abstracted planning problem.
        /// </summary>
        /// <param name="findAdditivePatterns">Should the additive patterns be automatically found?</param>
        /// <param name="patternHints">Explicitly requested patterns for the database (only if findAdditivePatterns is false).</param>
        /// <returns>Pattern database of the abstracted planning problem.</returns>
        IPatternDatabase GetPatternDatabase(bool findAdditivePatterns = true, List<HashSet<int>> patternHints = null);
    }
}
