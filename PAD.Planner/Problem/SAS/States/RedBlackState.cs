using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of the Red-Black state variant in the SAS+ planning problem. Some of the variables are considered as "red" (abstracted), while others
    /// are "black" (non-abstracted). Information about which variables are abstracted is provided by the parent planning problem.
    /// </summary>
    public class RedBlackState : RelaxedState
    {
        /// <summary>
        /// Parent planning problem.
        /// </summary>
        private Problem Problem { get; }

        /// <summary>
        /// Wild card value (we can specify that the value doesn't matter).
        /// </summary>
        public const int WildCardValue = -1;

        /// <summary>
        /// Constructs the Red-Black state from the given standard state.
        /// </summary>
        /// <param name="state">Source standard state.</param>
        /// <param name="problem">Parent planning problem.</param>
        public RedBlackState(State state, Problem problem) : base(state)
        {
            Problem = problem;
        }

        /// <summary>
        /// Constructs the Red-Black state from the given list of values.
        /// </summary>
        /// <param name="valuesList">List of values.</param>
        /// <param name="problem">Parent planning problem.</param>
        public RedBlackState(List<int>[] valuesList, Problem problem) : base(valuesList)
        {
            Problem = problem;
        }

        /// <summary>
        /// Sets the specified value to the given variable in the state.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <param name="value">Value to be assigned.</param>
        public override void SetValue(int variable, int value)
        {
            if (Problem.Variables[variable].IsAbstracted())
            {
                if (!Values[variable].Contains(value))
                {
                    Values[variable].Add(value);
                }
            }
            else
            {
                Values[variable][0] = value;
            }
        }

        /// <summary>
        /// Checks whether the state has given value for the specified variable.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Corresponding value to be checked.</param>
        /// <returns>True if the given variable has the given value, false otherwise.</returns>
        public override bool HasValue(int variable, int value)
        {
            return (value == WildCardValue || Values[variable].Contains(value));
        }

        /// <summary>
        /// Gets the relaxed variant of this state.
        /// </summary>
        /// <returns>Relaxed state.</returns>
        public override Planner.IState GetRelaxedState()
        {
            return new RelaxedState(Values);
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public override Planner.IState Clone()
        {
            return new RedBlackState(Values, Problem);
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

            RedBlackState other = obj as RedBlackState;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(Values, other.Values);
        }
    }
}
