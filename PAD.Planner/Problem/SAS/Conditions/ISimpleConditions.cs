using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Interface representing a simple version of conditions (linear list of constraints).
    /// </summary>
    public interface ISimpleConditions : IConditions, IEnumerable<IAssignment>
    {
        /// <summary>
        /// Checks whether the specified variable is actually constrained.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <returns>True if the given variable is constrained in the conditions.</returns>
        bool IsVariableConstrained(int variable);

        /// <summary>
        /// Checks whether the specified variable is actually constrained. If the check is positive, returns also a constraining value.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        /// <param name="value">Constraining value.</param>
        /// <returns>True if the given variable is constrained in the conditions, false otherwise.</returns>
        bool IsVariableConstrained(int variable, out int value);

        /// <summary>
        /// Gets the list of assigned values, where order is given by variables. E.g. {(3, 5), (1, 6)} will generate list [6, 5].
        /// </summary>
        /// <returns>List of assigned values.</returns>
        int[] GetAssignedValues();
    }
}
