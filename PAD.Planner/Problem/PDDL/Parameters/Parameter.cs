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
        public int ParameterNameID { set; get; } = IDManager.INVALID_ID;

        /// <summary>
        /// Type name IDs of possible parameter types.
        /// </summary>
        public ICollection<int> TypeNamesIDs { set; get; } = null;

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Construct the parameter.
        /// </summary>
        /// <param name="parameterNameID">Parameter name ID.</param>
        /// <param name="typeNamesIDs">Type names IDs.</param>
        /// <param name="idManager">ID manager.</param>
        public Parameter(int parameterNameID, ICollection<int> typeNamesIDs, IDManager idManager)
        {
            ParameterNameID = parameterNameID;
            TypeNamesIDs = typeNamesIDs;
            IDManager = idManager;
        }

        /// <summary>
        /// Creates a deep copy of the parameter.
        /// </summary>
        /// <returns>Deep copy of the parameter.</returns>
        public Parameter Clone()
        {
            return new Parameter(ParameterNameID, new List<int>(TypeNamesIDs), IDManager);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(TypeNamesIDs).CombineHashCode(ParameterNameID);
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

            return (ParameterNameID == other.ParameterNameID && CollectionsEquality.Equals(TypeNamesIDs, other.TypeNamesIDs));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string parameterName = $"{IDManager.GENERIC_VARIABLE_PREFIX}{ParameterNameID.ToString()}";

            List<string> typeNames = new List<string>();
            foreach (var typeID in TypeNamesIDs)
            {
                typeNames.Add(IDManager.Types.GetNameFromID(typeID));
            }

            if (typeNames.Count == 0)
            {
                return parameterName;
            }

            string parameterTypes = "";
            if (typeNames.Count == 1)
            {
                parameterTypes = typeNames[0];
            }
            else
            {
                parameterTypes = $"(either {string.Join(" ", typeNames)})";
            }

            return $"{parameterName} - {parameterTypes}";
        }
    }
}
