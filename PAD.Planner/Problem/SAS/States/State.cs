using System.Collections.Generic;
using System.Linq;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Standard implementation of the state in the SAS+ planning problem. A state is defined by enumeration of all state values of the SAS+ planning problem.
    /// </summary>
    public class State : IState
    {
        /// <summary>
        /// Values of the state.
        /// </summary>
        protected int[] Values { set; get; }

        /// <summary>
        /// Constructs the state from the given list of values.
        /// </summary>
        /// <param name="valuesList">Values of the state.</param>
        public State(params int[] valuesList)
        {
            Values = (int[])valuesList.Clone();
        }

        /// <summary>
        /// Constructs the state from the given list of values.
        /// </summary>
        /// <param name="valuesList">Values of the state.</param>
        public State(IEnumerable<int> valuesList)
        {
            Values = valuesList.ToArray();
        }

        /// <summary>
        /// Constructs the state from the input data.
        /// </summary>
        /// <param name="initState">Init state data.</param>
        public State(InputData.SAS.InitialState initState) : this(initState.ToArray())
        {
        }

        /// <summary>
        /// Gets the corresponding value for the specified variable.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Value for the specified variable.</returns>
        public virtual int GetValue(int variable)
        {
            return Values[variable];
        }

        /// <summary>
        /// Gets corresponding values for the specified variable (identical to GetValue for standard
        /// states, but useful for relaxed states with multiple values).
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Values for the specified variable.</returns>
        public int[] GetAllValues(int variable)
        {
            return new[] { GetValue(variable) };
        }

        /// <summary>
        /// Gets corresponding values for all the variables in the state.
        /// </summary>
        /// <returns>Values for all the variables.</returns>
        public int[] GetAllValues()
        {
            return Values;
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
            Values[variable] = value;
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
            return (Values[variable] == value);
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
        public Planner.IState GetRelaxedState()
        {
            return new RelaxedState(this);
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public virtual Planner.IState Clone()
        {
            return new State(Values);
        }

        /// <summary>
        /// Gets the state size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>State size.</returns>
        public int GetSize()
        {
            return Values.Length;
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public virtual Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            Conditions conditions = new Conditions();

            for (int variable = 0; variable < Values.Length; ++variable)
            {
                conditions.Add(new Assignment(variable, Values[variable]));
            }

            return conditions;
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
        /// Auxiliary static method for parsing and creating a SAS+ state represented in the string. The format is as "[valuesList]"
        /// with a possibility to use compressions, e.g. "[1 2 1 3x2]" is a state of 6 variables, where "3x2" is equivalent to  "2 2 2".
        /// </summary>
        /// <param name="state">String coded SAS+ state.</param>
        /// <returns>New instance of SAS+ state corresponding to the given string code.</returns>
        public static State Parse(string state)
        {
            List<int> values = new List<int>();
            state = state.Replace('[', ' ').Replace(']', ' ');

            foreach (var item in state.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!item.Contains("x"))
                {
                    values.Add(int.Parse(item));
                }
                else
                {
                    var parts = item.Split('x');
                    int count = int.Parse(parts[0]);
                    int val = int.Parse(parts[1]);

                    for (int i = 0; i < count; ++i)
                    {
                        values.Add(val);
                    }
                }
            }

            return new State(values.ToArray());
        }

        /// <summary>
        /// Constructs a string representing the state in the concrete planning problem. Both the planning problem and the state can be later reconstructed from the string.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>String representation of the state.</returns>
        public string GetInfoString(IProblem problem)
        {
            var split = problem.GetInputFilePath().Split(System.IO.Path.DirectorySeparatorChar);
            string stateInfo = split[split.Length - 2] + "_" + split[split.Length - 1] + "_" + GetCompressedDescription();
            return stateInfo;
        }

        /// <summary>
        /// Constructs a compressed string representing the state, in the form like "[1 2 1 3x2]", where 3x2 meaning "3 3 3".
        /// </summary>
        /// <returns>Compressed string representation of the state.</returns>
        public string GetCompressedDescription()
        {
            List<string> values = new List<string>();

            int? previous = null;
            int previousCount = 0;

            Action<int?, int> addPrevious = (prev, prevCount) =>
            {
                if (prevCount > 1)
                {
                    values.Add($"{prevCount}x{prev}");
                }
                else if (prevCount == 1)
                {
                    values.Add(prev.ToString());
                }
            };

            foreach (var current in Values)
            {
                if (previous == null)
                {
                    previous = current;
                    previousCount = 1;
                    continue;
                }

                if (previous == current)
                {
                    ++previousCount;
                    continue;
                }

                addPrevious(previous, previousCount);
                previous = current;
                previousCount = 1;
            }
            addPrevious(previous, previousCount);

            return $"[{string.Join(" ", values)}]";
        }

        /// <summary>
        /// Constructs a string representing the state.
        /// </summary>
        /// <returns>String representation of the state.</returns>
        public override string ToString()
        {
            return $"[{string.Join(" ", Values)}]";
        }

        /// <summary>
        /// Constructs a string representing the state, with the actual symbolic meanings of the values.
        /// </summary>
        /// <param name="problem">Parent planning problem</param>
        /// <returns>String representation of the state, with symbolic meanings.</returns>
        public virtual string ToStringWithMeanings(Problem problem)
        {
            return string.Join(", ", Enumerable.Range(0, Values.Length).Select(i => problem.Variables[i].Values[Values[i]].Replace("Atom", "")));
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

            State other = obj as State;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(Values, other.Values);
        }
    }
}
