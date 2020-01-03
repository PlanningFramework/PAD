using System.Collections.Generic;
using TypeId = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing type hierarchy of the PDDL planning problem.
    /// </summary>
    public class TypeHierarchy : Dictionary<TypeId, TypeIdCollection>
    {
        /// <summary>
        /// ID manager.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Gets the collection of direct children types for the specified type.
        /// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <returns>Direct children types of the given type.</returns>
        public IEnumerable<TypeId> GetChildrenTypes(TypeId typeId)
        {
            return this[typeId];
        }

        /// <summary>
        /// Constructs the type hierarchy of the PDDL planning problem.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public TypeHierarchy(InputData.PDDLInputData inputData, IdManager idManager)
        {
            IdManager = idManager;

            Add(idManager.Types.GetId("object"), new TypeIdCollection());

            foreach (var type in inputData.Domain.Types)
            {
                TypeId typeId = idManager.Types.GetId(type.TypeName);
                if (!ContainsKey(typeId))
                {
                    Add(typeId, new TypeIdCollection());
                }

                foreach (var baseType in type.BaseTypeNames)
                {
                    TypeId baseTypeId = idManager.Types.GetId(baseType);
                    if (ContainsKey(baseTypeId))
                    {
                        this[baseTypeId].Add(typeId);
                    }
                    else
                    {
                        TypeIdCollection newChildrenSet = new TypeIdCollection {typeId};
                        Add(baseTypeId, newChildrenSet);
                    }
                }
            }
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

                List<string> childrenTypes = new List<string>();
                foreach (var childType in item.Value)
                {
                    string childTypeName = IdManager.Types.GetNameFromId(childType);
                    childrenTypes.Add(childTypeName);
                }

                string childrenTypesList = string.Join(", ", childrenTypes);
                returnList.Add(childrenTypes.Count == 0 ? $"({typeName})" : $"({typeName} >> {childrenTypesList})");
            }
            return string.Join(", ", returnList);
        }
    }

    /// <summary>
    /// Collection of type IDs.
    /// </summary>
    public class TypeIdCollection : HashSet<TypeId>
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> items = new List<string>();
            foreach (var type in this)
            {
                items.Add(type.ToString());
            }
            return $"{{{string.Join(", ", items)}}}";
        }
    }
}
