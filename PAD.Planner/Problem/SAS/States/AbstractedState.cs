using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of the abstracted state variant in the SAS+ planning problem. Specified variables are considered as fully abstracted and their
    /// value is considered as arbitrary, while the other variables have standard behaviour with a single unique value. The set of abstracted variables
    /// is parametrized globally, so only a single abstraction is allowed to be worked with at any time (i.e. there cannot be two instances of this state
    /// with different sets of abstracted variables).
    /// </summary>
    public sealed class AbstractedState : State
    {
        /// <summary>
        /// Set of not abstracted variables, mapping original variable indices into reduced underlying state variable indices.
        /// </summary>
        private static Dictionary<int, int> NotAbstractedVariables { get; } = new Dictionary<int, int>();

        /// <summary>
        /// Wild card value (i.e. the value is considered as arbitrary).
        /// </summary>
        public const int WildCardValue = -1;

        /// <summary>
        /// Constructs an empty abstracted state.
        /// </summary>
        public AbstractedState() : base(new int[NotAbstractedVariables.Keys.Count])
        {
        }

        /// <summary>
        /// Constructs the abstracted state from the given standard state.
        /// </summary>
        /// <param name="state">Standard reference state.</param>
        public AbstractedState(State state) : base(new int[NotAbstractedVariables.Keys.Count])
        {
            for (int variable = 0; variable < state.GetSize(); ++variable)
            {
                // the abstracted variables are not set, non-abstracted are remapped
                SetValue(variable, state.GetValue(variable));
            }
        }

        /// <summary>
        /// Constructs the abstracted state from the given list of values.
        /// </summary>
        /// <param name="valuesList">Values of the state.</param>
        private AbstractedState(int[] valuesList) : base(valuesList)
        {
        }

        /// <summary>
        /// Sets the not abstracted set of variables for this kind of SAS+ states.
        /// </summary>
        /// <param name="variables">Set of variables.</param>
        public static void SetNotAbstractedVariables(HashSet<int> variables)
        {
            NotAbstractedVariables.Clear();

            int mappedVariableIndex = 0;
            foreach (var variable in variables)
            {
                NotAbstractedVariables.Add(variable, mappedVariableIndex++);
            }
        }

        /// <summary>
        /// Checks whether the specified variable is abstracted.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <returns>True if the given variable is abstracted, false otherwise.</returns>
        public static bool IsVariableAbstracted(int variable)
        {
            return !NotAbstractedVariables.ContainsKey(variable);
        }

        /// <summary>
        /// Gets the corresponding value for the specified variable.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Value for the specified variable.</returns>
        public override int GetValue(int variable)
        {
            if (IsVariableAbstracted(variable))
            {
                return WildCardValue;
            }
            return base.GetValue(NotAbstractedVariables[variable]);
        }

        /// <summary>
        /// Sets the specified value to the given variable in the state.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <param name="value">Value to be assigned.</param>
        public override void SetValue(int variable, int value)
        {
            if (IsVariableAbstracted(variable))
            {
                return;
            }
            base.SetValue(NotAbstractedVariables[variable], value);
        }

        /// <summary>
        /// Checks whether the state has given value for the specified variable.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Corresponding value to be checked.</param>
        /// <returns>True if the given variable has the given value, false otherwise.</returns>
        public override bool HasValue(int variable, int value)
        {
            if (IsVariableAbstracted(variable))
            {
                return true;
            }
            return base.HasValue(NotAbstractedVariables[variable], value);
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public override Planner.IState Clone()
        {
            return new AbstractedState(Values);
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public override Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            Conditions conditions = new Conditions();

            for (int variable = 0; variable < ((Problem)problem).Variables.Count; ++variable)
            {
                if (IsVariableAbstracted(variable))
                {
                    continue;
                }
                conditions.Add(new Assignment(variable, GetValue(variable)));
            }

            return conditions;
        }

        /// <summary>
        /// Constructs a string representing the state, with the actual symbolic meanings of the values.
        /// </summary>
        /// <param name="problem">Parent planning problem</param>
        /// <returns>String representation of the state, with symbolic meanings.</returns>
        public override string ToStringWithMeanings(Problem problem)
        {
            List<string> descriptionParts = new List<string>();
            for (int variable = 0; variable < problem.Variables.Count; ++variable)
            {
                if (!IsVariableAbstracted(variable))
                {
                    descriptionParts.Add(problem.Variables[variable].Values[GetValue(variable)].Replace("Atom", ""));
                }
            }
            return string.Join(", ", descriptionParts);
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

            AbstractedState other = obj as AbstractedState;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(Values, other.Values);
        }
    }
}
