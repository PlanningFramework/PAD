using System.Collections.Generic;
using VariableID = System.Int32;
using ConstantID = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for variable substitutions.
    /// </summary>
    public interface ISubstitution : IEnumerable<KeyValuePair<VariableID, ConstantID>>
    {
        /// <summary>
        /// Adds a variable-value assignment to the substitution.
        /// </summary>
        /// <param name="variable">Variable.</param>
        /// <param name="value">Value.</param>
        void Add(VariableID variable, ConstantID value);

        /// <summary>
        /// Checks whether the substitution contains specified variable.
        /// </summary>
        /// <param name="variableID">Requested variable.</param>
        /// <returns>True if the given variable is substitues, false otherwise.</returns>
        bool Contains(VariableID variableID);

        /// <summary>
        /// Tries to get the substitued value of the specified variable. If the variable is a part of substitution, then the value
        /// is returned via output parameter.
        /// </summary>
        /// <param name="variableID">Requested variable.</param>
        /// <param name="substituedValue">Output parameter of the substitued value.</param>
        /// <returns>True when the value for specified variable is found. False otherwise.</returns>
        bool TryGetValue(VariableID variableID, out ConstantID substituedValue);

        /// <summary>
        /// Gets the value of the specified variable.
        /// </summary>
        /// <param name="variableID">Requested variable.</param>
        /// <returns>Substitued value of the specified variable.</returns>
        ConstantID GetValue(VariableID variableID);

        /// <summary>
        /// Checks whether the substitution is empty (no variable is substituted).
        /// </summary>
        /// <returns>True if the substitution is empty, false otherwise.</returns>
        bool IsEmpty();

        /// <summary>
        /// Registers the given local substitution in the current substitution.
        /// </summary>
        /// <param name="substitution">Local substitution.</param>
        void AddLocalSubstitution(ISubstitution substitution);

        /// <summary>
        /// Un-registers the given local substitution in the current substitution.
        /// </summary>
        /// <param name="substitution">Local substitution.</param>
        void RemoveLocalSubstitution(ISubstitution substitution);

        /// <summary>
        /// Creates a deep copy of the substitution.
        /// </summary>
        /// <returns>A copy of the substitution.</returns>
        ISubstitution Clone();
    }
}
