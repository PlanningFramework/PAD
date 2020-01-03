using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a single PDDL parameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Parameter name ID.
        /// </summary>
        public int ParameterNameId { set; get; }

        /// <summary>
        /// Type name IDs of possible parameter types.
        /// </summary>
        public ICollection<int> TypeNamesIDs { set; get; }

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Construct the parameter.
        /// </summary>
        /// <param name="parameterNameId">Parameter name ID.</param>
        /// <param name="typeNamesIDs">Type names IDs.</param>
        /// <param name="idManager">ID manager.</param>
        public Parameter(int parameterNameId, ICollection<int> typeNamesIDs, IdManager idManager)
        {
            ParameterNameId = parameterNameId;
            TypeNamesIDs = typeNamesIDs;
            IdManager = idManager;
        }

        /// <summary>
        /// Creates a deep copy of the parameter.
        /// </summary>
        /// <returns>Deep copy of the parameter.</returns>
        public Parameter Clone()
        {
            return new Parameter(ParameterNameId, new List<int>(TypeNamesIDs), IdManager);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(TypeNamesIDs).CombineHashCode(ParameterNameId);
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

            Parameter other = obj as Parameter;
            if (other == null)
            {
                return false;
            }

            return (ParameterNameId == other.ParameterNameId && CollectionsEquality.Equals(TypeNamesIDs, other.TypeNamesIDs));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string parameterName = $"{IdManager.GenericVariablePrefix}{ParameterNameId.ToString()}";

            List<string> typeNames = new List<string>();
            foreach (var typeId in TypeNamesIDs)
            {
                typeNames.Add(IdManager.Types.GetNameFromId(typeId));
            }

            if (typeNames.Count == 0)
            {
                return parameterName;
            }

            string parameterTypes = (typeNames.Count == 1) ? typeNames[0] : $"(either {string.Join(" ", typeNames)})";

            return $"{parameterName} - {parameterTypes}";
        }
    }
}
