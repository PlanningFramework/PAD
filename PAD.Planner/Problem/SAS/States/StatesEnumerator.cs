using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Auxiliary static class for generating all the states for specified conditions or a relative state.
    /// </summary>
    public static class StatesEnumerator
    {
        /// <summary>
        /// Generates all possible SAS+ states meeting conditions specified by the given conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>All possible SAS+ states meeting the conditions.</returns>
        public static IEnumerable<IState> EnumerateStates(IConditions conditions, Variables variables)
        {
            foreach (var simpleCondition in conditions.GetSimpleConditions())
            {
                foreach (var state in EnumerateStates(simpleCondition, variables))
                {
                    yield return state;
                }
            }
        }

        /// <summary>
        /// Generates all possible SAS+ states meeting conditions specified by the given conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>All possible SAS+ states meeting the conditions.</returns>
        public static IEnumerable<IState> EnumerateStates(ISimpleConditions conditions, Variables variables)
        {
            Func<int, int> checker = (variable) =>
            {
                int value;
                if (conditions.IsVariableConstrained(variable, out value))
                {
                    return value;
                }
                return -1;
            };
            return EnumerateStates(0, new State(new int[variables.Count]), checker, variables);
        }

        /// <summary>
        /// Generates all possible SAS+ states meeting conditions specified by the relative state. Lazy generated via yield return.
        /// </summary>
        /// <param name="relativeState">Reference relative state.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>All possible SAS+ states meeting the conditions.</returns>
        public static IEnumerable<IState> EnumerateStates(IRelativeState relativeState, Variables variables)
        {
            Func<int, int> checker = relativeState.GetValue;
            return EnumerateStates(0, new State(new int[variables.Count]), checker, variables);
        }

        /// <summary>
        /// Generates all possible SAS+ relative states meeting conditions specified by the given conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>All possible SAS+ relative states meeting the conditions.</returns>
        public static IEnumerable<IRelativeState> EnumerateRelativeStates(IConditions conditions, Variables variables)
        {
            foreach (var simpleCondition in conditions.GetSimpleConditions())
            {
                yield return GetCorrespondingRelativeState(simpleCondition, variables);
            }
        }

        /// <summary>
        /// Gets the corresponding SAS+ relative state meeting specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>Corresponding SAS+ relative state meeting the conditions.</returns>
        private static IRelativeState GetCorrespondingRelativeState(ISimpleConditions conditions, Variables variables)
        {
            int[] initWildCards = new int[variables.Count];
            for (int i = 0; i < variables.Count; ++i)
            {
                initWildCards[i] = -1;
            }

            IRelativeState newRelativeState = new RelativeState(initWildCards);

            foreach (var constraint in conditions)
            {
                newRelativeState.SetValue(constraint);
            }

            return newRelativeState;
        }

        /// <summary>
        /// Generates all possible SAS+ states meeting conditions specified by the given constrained values. Lazy generated recursively via yield return.
        /// </summary>
        /// <param name="variable">Current variable.</param>
        /// <param name="result">Current state being built.</param>
        /// <param name="constrainedValues">Constrained values checker.</param>
        /// <param name="variables">Variables of the planning problem.</param>
        /// <returns>All possible SAS+ states meeting the conditions.</returns>
        private static IEnumerable<IState> EnumerateStates(int variable, IState result, Func<int, int> constrainedValues, Variables variables)
        {
            if (variable >= variables.Count)
            {
                yield return (IState)result.Clone();
            }
            else
            {
                int constrainedValue = constrainedValues(variable);
                if (constrainedValue != -1)
                {
                    result.SetValue(variable, constrainedValue);
                    foreach (var item in EnumerateStates(variable + 1, result, constrainedValues, variables))
                    {
                        yield return item;
                    }
                }
                else
                {
                    for (int value = 0; value < variables[variable].GetDomainRange(); ++value)
                    {
                        result.SetValue(variable, value);
                        foreach (var item in EnumerateStates(variable + 1, result, constrainedValues, variables))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
