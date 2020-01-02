using System.Collections.Generic;
using ConstantID = System.Int32;
using TypeID = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure for management of constants/objects. The main purpose is to find and get all possible constants for the given type.
    /// The manager holds all constants for the concrete types, respecting type hierarchy. Also, the manager allows to obtain the 
    /// definition type(s) for the specific constant.
    /// </summary>
    public class ConstantsManager : Dictionary<TypeID, ConstantIDCollection>
    {
        /// <summary>
        /// ID manager.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Type hierarchy in the planning problem.
        /// </summary>
        private TypeHierarchy TypeHierarchy { set; get; } = null;

        /// <summary>
        /// Definition types for all available constants (directly from the PDDL input definition, e.g. constA - typeA).
        /// Single constant can have multiple types via either clause, e.g. constB - (either typeA typeB).
        /// </summary>
        private Dictionary<ConstantID, ICollection<TypeID>> ConstantDefinitionTypes { set; get; } = null;

        /// <summary>
        /// Constructs the constant manager.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public ConstantsManager(InputData.PDDLInputData inputData, IDManager idManager)
        {
            IDManager = idManager;
            TypeHierarchy = new TypeHierarchy(inputData, idManager);
            ConstantDefinitionTypes = new Dictionary<int, ICollection<int>>();

            foreach (var type in TypeHierarchy)
            {
                TypeID typeID = type.Key;
                Add(typeID, new ConstantIDCollection());
            }

            System.Action<string, List<string>> ProcessConstantItem = (string constantName, List<string> typeNames) =>
            {
                HashSet<TypeID> typeIDs = new HashSet<int>();

                ConstantID constantID = idManager.Constants.GetID(constantName);
                foreach (string type in typeNames)
                {
                    TypeID typeID = idManager.Types.GetID(type);
                    this[typeID].Add(constantID);
                    typeIDs.Add(typeID);
                }

                ConstantDefinitionTypes.Add(constantID, typeIDs);
            };

            foreach (var constant in inputData.Domain.Constants)
            {
                ProcessConstantItem(constant.ConstantName, constant.TypeNames);
            }

            foreach (var obj in inputData.Problem.Objects)
            {
                ProcessConstantItem(obj.ObjectName, obj.TypeNames);
            }

            System.Action<TypeID, TypeID> FillConstantsFromChildType = null;
            FillConstantsFromChildType = (TypeID typeID, TypeID outputTypeID) =>
            {
                this[outputTypeID].UnionWith(this[typeID]);

                TypeIDCollection childrenTypes = TypeHierarchy[typeID];
                foreach (TypeID childTypeID in childrenTypes)
                {
                    FillConstantsFromChildType(childTypeID, outputTypeID);
                }
            };

            foreach (TypeID typeID in Keys)
            {
                FillConstantsFromChildType(typeID, typeID);
            }
        }

        /// <summary>
        /// Gets all possible constants of the given type (i.e. including all constants of the ancestor types).
        /// </summary>
        /// <param name="typeID">Type ID.</param>
        /// <returns>Collection of constants of the given type.</returns>
        public IEnumerable<ConstantID> GetAllConstantsOfType(TypeID typeID)
        {
            return this[typeID];
        }

        /// <summary>
        /// Gets the definition type(s) for the specified constant (more types = either clause)
        /// </summary>
        /// <param name="constantID">Constant ID.</param>
        /// <returns>Collection of definition types for the given constant.</returns>
        public ICollection<TypeID> GetTypesForConstant(ConstantID constantID)
        {
            return ConstantDefinitionTypes[constantID];
        }

        /// <summary>
        /// Gets the collection of direct children types for the specified type.
        /// </summary>
        /// <param name="typeID">Type ID.</param>
        /// <returns>Direct children types of the given type.</returns>
        public IEnumerable<TypeID> GetChildrenTypes(TypeID typeID)
        {
            return TypeHierarchy.GetChildrenTypes(typeID);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> returnList = new List<string>();
            foreach (var item in this)
            {
                string typeName = IDManager.Types.GetNameFromID(item.Key);

                List<string> constantNames = new List<string>();
                foreach (ConstantID constantID in item.Value)
                {
                    string constantName = IDManager.Constants.GetNameFromID(constantID);
                    constantNames.Add(constantName);
                }

                string constantNamesList = string.Join($", ", constantNames);
                if (constantNames.Count == 0)
                {
                    returnList.Add($"({typeName})");
                }
                else
                {
                    returnList.Add($"({typeName} >> {constantNamesList})");
                }
            }
            return string.Join($", ", returnList);
        }
    }

    /// <summary>
    /// Collection of constant IDs.
    /// </summary>
    public class ConstantIDCollection : HashSet<ConstantID>
    {
    }
}
