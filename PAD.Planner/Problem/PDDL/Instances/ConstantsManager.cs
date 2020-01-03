using System.Collections.Generic;
using ConstantId = System.Int32;
using TypeId = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure for management of constants/objects. The main purpose is to find and get all possible constants for the given type.
    /// The manager holds all constants for the concrete types, respecting type hierarchy. Also, the manager allows to obtain the 
    /// definition type(s) for the specific constant.
    /// </summary>
    public class ConstantsManager : Dictionary<TypeId, ConstantIdCollection>
    {
        /// <summary>
        /// ID manager.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Type hierarchy in the planning problem.
        /// </summary>
        private TypeHierarchy TypeHierarchy { get; }

        /// <summary>
        /// Definition types for all available constants (directly from the PDDL input definition, e.g. constA - typeA).
        /// Single constant can have multiple types via either clause, e.g. constB - (either typeA typeB).
        /// </summary>
        private Dictionary<ConstantId, ICollection<TypeId>> ConstantDefinitionTypes { get; }

        /// <summary>
        /// Constructs the constant manager.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public ConstantsManager(InputData.PDDLInputData inputData, IdManager idManager)
        {
            IdManager = idManager;
            TypeHierarchy = new TypeHierarchy(inputData, idManager);
            ConstantDefinitionTypes = new Dictionary<int, ICollection<int>>();

            foreach (var type in TypeHierarchy)
            {
                TypeId typeId = type.Key;
                Add(typeId, new ConstantIdCollection());
            }

            System.Action<string, List<string>> processConstantItem = (constantName, typeNames) =>
            {
                HashSet<TypeId> typeIDs = new HashSet<int>();

                ConstantId constantId = idManager.Constants.GetId(constantName);
                foreach (string type in typeNames)
                {
                    TypeId typeId = idManager.Types.GetId(type);
                    this[typeId].Add(constantId);
                    typeIDs.Add(typeId);
                }

                ConstantDefinitionTypes.Add(constantId, typeIDs);
            };

            foreach (var constant in inputData.Domain.Constants)
            {
                processConstantItem(constant.ConstantName, constant.TypeNames);
            }

            foreach (var obj in inputData.Problem.Objects)
            {
                processConstantItem(obj.ObjectName, obj.TypeNames);
            }

            System.Action<TypeId, TypeId> fillConstantsFromChildType = null;
            fillConstantsFromChildType = (typeId, outputTypeId) =>
            {
                this[outputTypeId].UnionWith(this[typeId]);

                TypeIdCollection childrenTypes = TypeHierarchy[typeId];
                foreach (TypeId childTypeId in childrenTypes)
                {
                    fillConstantsFromChildType(childTypeId, outputTypeId);
                }
            };

            foreach (TypeId typeId in Keys)
            {
                fillConstantsFromChildType(typeId, typeId);
            }
        }

        /// <summary>
        /// Gets all possible constants of the given type (i.e. including all constants of the ancestor types).
        /// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <returns>Collection of constants of the given type.</returns>
        public IEnumerable<ConstantId> GetAllConstantsOfType(TypeId typeId)
        {
            return this[typeId];
        }

        /// <summary>
        /// Gets the definition type(s) for the specified constant (more types = either clause)
        /// </summary>
        /// <param name="constantId">Constant ID.</param>
        /// <returns>Collection of definition types for the given constant.</returns>
        public ICollection<TypeId> GetTypesForConstant(ConstantId constantId)
        {
            return ConstantDefinitionTypes[constantId];
        }

        /// <summary>
        /// Gets the collection of direct children types for the specified type.
        /// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <returns>Direct children types of the given type.</returns>
        public IEnumerable<TypeId> GetChildrenTypes(TypeId typeId)
        {
            return TypeHierarchy.GetChildrenTypes(typeId);
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
                string typeName = IdManager.Types.GetNameFromId(item.Key);

                List<string> constantNames = new List<string>();
                foreach (ConstantId constantId in item.Value)
                {
                    string constantName = IdManager.Constants.GetNameFromId(constantId);
                    constantNames.Add(constantName);
                }

                string constantNamesList = string.Join(", ", constantNames);
                returnList.Add((constantNames.Count == 0) ? $"({typeName})" : $"({typeName} >> {constantNamesList})");
            }
            return string.Join(", ", returnList);
        }
    }

    /// <summary>
    /// Collection of constant IDs.
    /// </summary>
    public class ConstantIdCollection : HashSet<ConstantId>
    {
    }
}
