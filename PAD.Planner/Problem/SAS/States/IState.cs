
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface for a state in the SAS+ planning problem.
    /// </summary>
    public interface IState : Planner.IState
    {
        /// <summary>
        /// Gets the corresponding value for the specified variable.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Value for the specified variable.</returns>
        int GetValue(int variable);

        /// <summary>
        /// Gets corresponding values for the specified variable (identical to GetValue for standard
        /// states, but useful for relaxed states with multiple values).
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <returns>Values for the specified variable.</returns>
        int[] GetAllValues(int variable);

        /// <summary>
        /// Gets corresponding values for all the variables in the state.
        /// </summary>
        /// <returns>Values for all the variables.</returns>
        int[] GetAllValues();

        /// <summary>
        /// Gets corresponding values for the specified variables.
        /// </summary>
        /// <param name="variables">Target variables.</param>
        /// <returns>Values for the specified variables.</returns>
        int[] GetValues(int[] variables);

        /// <summary>
        /// Sets the specified value to the given variable in the state.
        /// </summary>
        /// <param name="variable">Target variable.</param>
        /// <param name="value">Value to be assigned.</param>
        void SetValue(int variable, int value);

        /// <summary>
        /// Sets the specified assignment in the state.
        /// </summary>
        /// <param name="assignment">Assignment ot be checked.</param>
        void SetValue(IAssignment assignment);

        /// <summary>
        /// Checks whether the state has given value for the specified variable.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Corresponding value to be checked.</param>
        /// <returns>True if the given variable has the given value, false otherwise.</returns>
        bool HasValue(int variable, int value);

        /// <summary>
        /// Checks whether the state has the specified assignment.
        /// </summary>
        /// <param name="assignment">Assignment ot be checked.</param>
        /// <returns>True if the state has the given assignment, false otherwise.</returns>
        bool HasValue(IAssignment assignment);

        /// <summary>
        /// Constructs a string representing the state in the concrete planning problem. Both the planning problem and the state can be later reconstructed from the string.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>String representation of the state.</returns>
        string GetInfoString(IProblem problem);

        /// <summary>
        /// Constructs a string representing the state, with the actual symbolic meanings of the values.
        /// </summary>
        /// <param name="problem">Parent planning problem</param>
        /// <returns>String representation of the state, with symbolic meanings.</returns>
        string ToStringWithMeanings(Problem problem);
    }
}
