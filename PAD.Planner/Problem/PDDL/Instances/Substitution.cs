using System.Collections.Generic;
using System.Diagnostics;
using VariableId = System.Int32;
using ConstantId = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Standard implementation of a PDDL variables substitution, in form of a collection of pairs (variableID:constantValueID).
    /// </summary>
    public class Substitution : Dictionary<VariableId, ConstantId>, ISubstitution
    {
        /// <summary>
        /// Constructs an empty substitution.
        /// </summary>
        public Substitution()
        {
        }

        /// <summary>
        /// Constructs the substitution from the parameters.
        /// </summary>
        /// <param name="valuesList">List of values.</param>
        /// <param name="parameters">Variable parameters (of operator or quantified sub-expression).</param>
        public Substitution(List<int> valuesList, Parameters parameters)
        {
            Debug.Assert(valuesList.Count == parameters.Count);
            for (int i = 0; i < valuesList.Count; ++i)
            {
                Add(parameters[i].ParameterNameId, valuesList[i]);
            }
        }

        /// <summary>
        /// Checks whether the substitution contains specified variable.
        /// </summary>
        /// <param name="variableId">Requested variable.</param>
        /// <returns>True if the given variable is substitutes, false otherwise.</returns>
        public bool Contains(VariableId variableId)
        {
            return ContainsKey(variableId);
        }

        /// <summary>
        /// Gets the value of the specified variable.
        /// </summary>
        /// <param name="variableId">Requested variable.</param>
        /// <returns>Substituted value of the specified variable.</returns>
        public ConstantId GetValue(VariableId variableId)
        {
            return this[variableId];
        }

        /// <summary>
        /// Checks whether the substitution is empty (no variable is substituted).
        /// </summary>
        /// <returns>True if the substitution is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return (Count == 0);
        }

        /// <summary>
        /// Registers the given local substitution in the current substitution.
        /// </summary>
        /// <param name="substitution">Local substitution.</param>
        public void AddLocalSubstitution(ISubstitution substitution)
        {
            foreach (var item in substitution)
            {
                if (ContainsKey(item.Key))
                {
                    continue;
                }
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Un-registers the given local substitution in the current substitution.
        /// </summary>
        /// <param name="substitution">Local substitution.</param>
        public void RemoveLocalSubstitution(ISubstitution substitution)
        {
            foreach (var item in substitution)
            {
                if (!ContainsKey(item.Key))
                {
                    continue;
                }
                Remove(item.Key);
            }
        }

        /// <summary>
        /// Creates a deep copy of the substitution.
        /// </summary>
        /// <returns>A copy of the substitution.</returns>
        public ISubstitution Clone()
        {
            Substitution newSubstitution = new Substitution();
            foreach (var item in this)
            {
                newSubstitution.Add(item.Key, item.Value);
            }
            return newSubstitution;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this);
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

            Substitution other = obj as Substitution;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }
    }
}
