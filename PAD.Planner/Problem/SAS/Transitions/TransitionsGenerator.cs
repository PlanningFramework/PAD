using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Generator of successors and predecessors (forward and backward transitions) in the SAS+ planning problem.
    /// </summary>
    public class TransitionsGenerator
    {
        /// <summary>
        /// Generator of forward transitions in the SAS+ planning problem.
        /// </summary>
        private Lazy<SuccessorsGenerator> SuccessorsGenerator { get; }

        /// <summary>
        /// Generator of backward transitions in the SAS+ planning problem.
        /// </summary>
        private Lazy<PredecessorsGenerator> PredecessorsGenerator { get; }

        /// <summary>
        /// Enumerator for successive generation of forward transitions of the SAS+ planning problem.
        /// </summary>
        private Lazy<TransitionsEnumerator<IState, ISuccessor>> SuccessorsEnumerator { get; } = new Lazy<TransitionsEnumerator<IState, ISuccessor>>();

        /// <summary>
        /// Enumerator for successive generation of backward transitions of the SAS+ planning problem.
        /// </summary>
        private Lazy<TransitionsEnumerator<IConditions, IPredecessor>> PredecessorsEnumerator { get; } = new Lazy<TransitionsEnumerator<IConditions, IPredecessor>>();

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Lazy<Random> RandomNumberGenerator { get; } = new Lazy<Random>();

        /// <summary>
        /// Reference to the parent planning problem.
        /// </summary>
        private Problem Problem { get; }

        /// <summary>
        /// Constructs the transitions generator.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        public TransitionsGenerator(Problem problem)
        {
            Problem = problem;
            SuccessorsGenerator = new Lazy<SuccessorsGenerator>(() => new SuccessorsGenerator(Problem.Operators, Problem.Variables, Problem.MutexGroups));
            PredecessorsGenerator = new Lazy<PredecessorsGenerator>(() => new PredecessorsGenerator(Problem.Operators, Problem.Variables));
        }

        /// <summary>
        /// Gets a collection of all possible successors (forward transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetSuccessors(IState state)
        {
            return SuccessorsGenerator.Value.GetSuccessors(state);
        }

        /// <summary>
        /// Enumeration method getting a list with a limited number of possible successors (forward transitions) from the specified state. The
        /// next call of this method returns new successors, until all of them are returned - then an empty collection is returned to signalize
        /// the end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <param name="numberOfSuccessors">Number of successors to be returned.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetNextSuccessors(IState state, int numberOfSuccessors)
        {
            Func<IState, IEnumerable<ISuccessor>> generator = (sourceState) => SuccessorsGenerator.Value.GetSuccessors(sourceState);
            return SuccessorsEnumerator.Value.GetNextTransitions(state, numberOfSuccessors, generator);
        }

        /// <summary>
        /// Gets a random successor (forward transition) from the specified state.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Random successor from the given state. Null if no valid successor found.</returns>
        public ISuccessor GetRandomSuccessor(IState state)
        {
            return GetSuccessors(state).RandomElement(RandomNumberGenerator.Value);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(IConditions conditions)
        {
            return PredecessorsGenerator.Value.GetPredecessors(conditions);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(IState state)
        {
            return GetPredecessors((IConditions)state.GetDescribingConditions(Problem));
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified relative state. Lazy generated via yield return.
        /// </summary>
        /// <param name="relativeState">Original relative state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(IRelativeState relativeState)
        {
            // This part may be later optimized to use operator decision tree, just like IConditions

            foreach (var op in Problem.Operators)
            {
                if (op.IsRelevant(relativeState))
                {
                    yield return new Predecessor(relativeState, op);
                }
            }
        }

        /// <summary>
        /// Enumeration method getting a list with a limited number of relevant predecessors (backward transitions) from the specified conditions. The
        /// next call of this method returns new predecessors, until all of them are returned - then an empty collection is returned to signalize the
        /// end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <param name="numberOfPredecessors">Number of predecessors to be returned.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetNextPredecessors(IConditions conditions, int numberOfPredecessors)
        {
            Func<IConditions, IEnumerable<IPredecessor>> generator = (sourceConditions) => PredecessorsGenerator.Value.GetPredecessors(sourceConditions);
            return PredecessorsEnumerator.Value.GetNextTransitions(conditions, numberOfPredecessors, generator);
        }

        /// <summary>
        /// Gets a random relevant predecessor (backwards transition) from the specified conditions.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Random relevant predecessor from the specified conditions. Null if no valid predecessor found.</returns>
        public IPredecessor GetRandomPredecessor(IConditions conditions)
        {
            return GetPredecessors(conditions).RandomElement(RandomNumberGenerator.Value);
        }

        /// <summary>
        /// Gets a collection of all explicitly enumerated successor states (created by forward applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all successor states.</returns>
        public IEnumerable<Planner.IState> GetSuccessorStates(IState state)
        {
            foreach (var successor in GetSuccessors(state))
            {
                yield return successor.GetSuccessorState();
            }
        }

        /// <summary>
        /// Gets a collection of all explicitly enumerated predecessor states (created by backwards applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all predecessor states.</returns>
        public IEnumerable<Planner.IState> GetPredecessorStates(IState state)
        {
            foreach (var predecessor in GetPredecessors(state))
            {
                foreach (var predecessorState in predecessor.GetPredecessorStates(Problem))
                {
                    yield return predecessorState;
                }
            }
        }
    }
}
