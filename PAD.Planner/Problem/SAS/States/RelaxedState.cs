using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Relaxed implementation of the state in the SAS+ planning problem. Each variable may have multiple values at once.
    /// </summary>
    public class RelaxedState : IState
    {
        /// <summary>
        /// Values of the state.
        /// </summary>
        protected List<int>[] Values { set; get; } = null;

        /// <summary>
        /// Constructs the relaxed state from the given standard state.
        /// </summary>
        /// <param name="state">Source standard state.</param>
        public RelaxedState(State state)
        {
            Values = new List<int>[state.GetSize()];

            for (int variable = 0; variable < state.GetSize(); ++variable)
            {
                Values[variable] = new List<int>() { state.GetValue(variable) };
            }
        }

        /// <summary>
        /// Constructs the relaxed state from the given list of values.
        /// </summary>
        /// <param name="valuesList">List of values.</param>
        public RelaxedState(List<int>[] valuesList)
        {
            Values = new List<int>[valuesList.Length];

            for (int variable = 0; variable < valuesList.Length; ++variable)
            {
                Values[variable] = new List<int>(valuesList[variable]);
            }
        }

        /// <summary>
        /// Gets the corresponding value for the specified variable.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Value for the specified variable.</returns>
        public int GetValue(int variable)
        {
            return Values[variable][0];
        }

        /// <summary>
        /// Gets corresponding values for the specified variable (identical to GetValue for standard
        /// states, but useful for relaxed states with multiple values).
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Values for the specified variable.</returns>
        public int[] GetAllValues(int variable)
        {
            return Values[variable].ToArray();
        }

        /// <summary>
        /// Gets corresponding values for all the variables in the state.
        /// </summary>
        /// <returns>Values for all the variables.</returns>
        public int[] GetAllValues()
        {
            int[] values = new int[Values.Length];
            for (int i = 0; i < Values.Length; ++i)
            {
                values[i] = GetValue(i);
            }
            return values;
        }

        /// <summary>
        /// Gets corresponding values for the specified variables.
        /// </summary>
        /// <param name="variables">Target variables.</param>
        /// <returns>Values for the specified variables.</returns>
        public int[] GetValues(int[] variables)
        {
            int[] values = new int[variables.Length];
            for (int i = 0; i < variables.Length; ++i)
            {
                values[i] = GetValue(variables[i]);
            }
            return values;
        }

        /// <summary>
        /// Sets the specified value to the given variable in the state.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <param name="value">Value to be assigned.</param>
        public virtual void SetValue(int variable, int value)
        {
            if (!Values[variable].Contains(value))
            {
                Values[variable].Add(value);
            }
        }

        /// <summary>
        /// Sets the specified assignment in the state.
        /// </summary>
        /// <param name="assignment">Assignment ot be checked.</param>
        public void SetValue(IAssignment assignment)
        {
            SetValue(assignment.GetVariable(), assignment.GetValue());
        }

        /// <summary>
        /// Checks whether the state has given value for the specified variable.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Corresponding value to be checked.</param>
        /// <returns>True if the given variable has the given value, false otherwise.</returns>
        public virtual bool HasValue(int variable, int value)
        {
            return Values[variable].Contains(value);
        }

        /// <summary>
        /// Checks whether the state has the specified assignment.
        /// </summary>
        /// <param name="assignment">Assignment ot be checked.</param>
        /// <returns>True if the state has the given assignment, false otherwise.</returns>
        public bool HasValue(IAssignment assignment)
        {
            return HasValue(assignment.GetVariable(), assignment.GetValue());
        }

        /// <summary>
        /// Gets the relaxed variant of this state.
        /// </summary>
        /// <returns>Relaxed state.</returns>
        public virtual Planner.IState GetRelaxedState()
        {
            return this;
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public virtual Planner.IState Clone()
        {
            return new RelaxedState(Values);
        }

        /// <summary>
        /// Gets the state size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>State size.</returns>
        public int GetSize()
        {
            int count = 0;
            foreach (var variableValues in Values)
            {
                count += variableValues.Count;
            }
            return count;
        }

        /// <summary>
        /// Gets the state length (i.e. number of variables).
        /// </summary>
        /// <returns>State length.</returns>
        public int GetLength()
        {
            return Values.Length;
        }

        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public bool DetermineGoalNode(IProblem problem)
        {
            return problem.IsGoalState(this);
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return heuristic.GetValue(this);
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            return problem.GetSuccessors(this);
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            List<Conditions> conditions = new List<Conditions>();

            for (int variable = 0; variable < Values.Length; ++variable)
            {
                List<Conditions> newConditions = new List<Conditions>();
                foreach (var value in Values[variable])
                {
                    if (variable == 0)
                    {
                        newConditions.Add(new Conditions(new Assignment(variable, value)));
                        continue;
                    }

                    foreach (var condition in conditions)
                    {
                        var newCondition = (Conditions)condition.Clone();
                        newCondition.Add(new Assignment(variable, value));
                        newConditions.Add(newCondition);
                    }
                }
                conditions = newConditions;
            }

            return new ConditionsClause(conditions);
        }

        /// <summary>
        /// Constructs a string representing the state in the concrete planning problem. Both the planning problem and the state can be later reconstructed from the string.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>String representation of the state.</returns>
        public string GetInfoString(IProblem problem)
        {
            var splitted = problem.GetInputFilePath().Split(System.IO.Path.DirectorySeparatorChar);
            string stateInfo = splitted[splitted.Length - 2] + "_" + splitted[splitted.Length - 1] + "_" + ToString();
            return stateInfo;
        }

        /// <summary>
        /// Constructs a string representing the state.
        /// </summary>
        /// <returns>String representation of the state.</returns>
        public override string ToString()
        {
            List<string> valueList = new List<string>();
            foreach (var item in Values)
            {
                valueList.Add(string.Join("|", item));
            }

            return $"[{string.Join(" ", valueList)}]";
        }

        /// <summary>
        /// Constructs a string representing the state, with the actual symbolic meanings of the values.
        /// </summary>
        /// <param name="problem">Parent planning problem</param>
        /// <returns>String representation of the state, with symbolic meanings.</returns>
        public string ToStringWithMeanings(Problem problem)
        {
            List<string> list = new List<string>();
            for (int variable = 0; variable < Values.Length; ++variable)
            {
                List<string> valueList = new List<string>();
                foreach (var value in Values[variable])
                {
                    valueList.Add(problem.Variables[variable].Values[value].Replace("Atom", ""));
                }
                list.Add(string.Join("|", valueList));
            }

            return string.Join(", ", list);
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

            RelaxedState other = obj as RelaxedState;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(Values, other.Values);
        }
    }
}
