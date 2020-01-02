using System.Collections.Generic;
using TypeID = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing type hierarchy of the PDDL planning problem.
    /// </summary>
    public class TypeHierarchy : Dictionary<TypeID, TypeIDCollection>
    {
        /// <summary>
        /// ID manager.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Gets the collection of direct children types for the specified type.
        /// </summary>
        /// <param name="typeID">Type ID.</param>
        /// <returns>Direct children types of the given type.</returns>
        public IEnumerable<TypeID> GetChildrenTypes(TypeID typeID)
        {
            return this[typeID];
        }

        /// <summary>
        /// Constructs the type hierarchy of the PDDL planning problem.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public TypeHierarchy(InputData.PDDLInputData inputData, IDManager idManager)
        {
            IDManager = idManager;

            Add(idManager.Types.GetID("object"), new TypeIDCollection());

            foreach (var type in inputData.Domain.Types)
            {
                TypeID typeID = idManager.Types.GetID(type.TypeName);
                if (!ContainsKey(typeID))
                {
                    Add(typeID, new TypeIDCollection());
                }

                foreach (var baseType in type.BaseTypeNames)
                {
                    TypeID baseTypeID = idManager.Types.GetID(baseType);
                    if (ContainsKey(baseTypeID))
                    {
                        this[baseTypeID].Add(typeID);
                    }
                    else
                    {
                        TypeIDCollection newChildrenSet = new TypeIDCollection();
                        newChildrenSet.Add(typeID);
                        Add(baseTypeID, newChildrenSet);
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
                string typeName = IDManager.Types.GetNameFromID(item.Key);

                List<string> childrenTypes = new List<string>();
                foreach (var childType in item.Value)
                {
                    string childTypeName = IDManager.Types.GetNameFromID(childType);
                    childrenTypes.Add(childTypeName);
                }

                string childrenTypesList = string.Join($", ", childrenTypes);
                if (childrenTypes.Count == 0)
                {
                    returnList.Add($"({typeName})");
                }
                else
                {
                    returnList.Add($"({typeName} >> {childrenTypesList})");
                }
            }
            return string.Join($", ", returnList);
        }
    }

    /// <summary>
    /// Collection of type IDs.
    /// </summary>
    public class TypeIDCollection : HashSet<TypeID>
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
            return $"{{{string.Join($", ", items)}}}";
        }
    }
}
