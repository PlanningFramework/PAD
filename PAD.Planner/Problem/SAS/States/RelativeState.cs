using System.Collections.Generic;
using System.Linq;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a relative state in the SAS+ planning problem. Relative state is an extension of a standard state, representing
    /// a whole class of states. It is an alternative way to express conditions in the backwards planning (an alternative to the more general
    /// IConditions). Relative states in SAS+ allow variables to have another value: a wild card (-1), which indicates that the value might be
    /// arbitrary.
    /// </summary>
    public class RelativeState : State, IRelativeState
    {
        /// <summary>
        /// Wild card value in the state (indicating that the value may be arbitrary).
        /// </summary>
        public const int WILD_CARD_VALUE = -1;

        /// <summary>
        /// Creates a relative state from the given list of values.
        /// </summary>
        /// <param name="valuesList">Values of the state.</param>
        public RelativeState(params int[] valuesList) : base(valuesList)
        {
        }

        /// <summary>
        /// Creates a relative state.
        /// </summary>
        /// <param name="valuesList">Values of the state.</param>
        public RelativeState(IEnumerable<int> valuesList) : base(valuesList)
        {
        }

        /// <summary>
        /// Checks whether the specified state is meeting conditions given by the relative state (i.e. belong to the corresponding class of states).
        /// </summary>
        /// <param name="state">State to be checked.</param>
        /// <returns>True if the state is meeting conditions of the relative state, false otherwise.</returns>
        public bool Evaluate(Planner.IState state)
        {
            IState evaluatedState = (IState)state;

            for (int variable = 0; variable < Values.Length; ++variable)
            {
                int value = Values[variable];
                if (value != WILD_CARD_VALUE)
                {
                    if (!evaluatedState.HasValue(variable, value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public override Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            Conditions conditions = new Conditions();

            for (int variable = 0; variable < Values.Length; ++variable)
            {
                int value = Values[variable];
                if (value != WILD_CARD_VALUE)
                {
                    conditions.Add(new Assignment(variable, value));
                }
            }

            return conditions;
        }

        /// <summary>
        /// Enumerates all possible states meeting the conditions of the current relative state (i.e. are in the same class of states).
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions of the relative state.</returns>
        public IEnumerable<Planner.IState> GetCorrespondingStates(IProblem problem)
        {
            return StatesEnumerator.EnumerateStates(this, ((Problem)problem).Variables);
        }

        /// <summary>
        /// Constructs a string representing the state, with the actual symbolic meanings of the values.
        /// </summary>
        /// <param name="problem">Parent planning problem</param>
        /// <returns>String representation of the state, with symbolic meanings.</returns>
        public override string ToStringWithMeanings(Problem problem)
        {
            return string.Join(", ", Enumerable.Range(0, Values.Length).Select(i =>
            {
                var value = Values[i];
                return (value == WILD_CARD_VALUE) ? "*" : problem.Variables[i].Values[value].Replace("Atom", "");
            }));
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public override Planner.IState Clone()
        {
            return new RelativeState((int[])Values.Clone());
        }

        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public new bool DetermineGoalNode(IProblem problem)
        {
            return problem.IsStartRelativeState(this);
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public new double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return heuristic.GetValue(this);
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public new IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            return problem.GetPredecessors(this);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Values);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            RelativeState other = obj as RelativeState;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(Values, other.Values);
        }
    }
}
