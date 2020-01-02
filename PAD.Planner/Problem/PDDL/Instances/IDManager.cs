using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Manager for the unique ID mappings used in the PDDL planning problem. Handles identifiers of predicates, functions, constants,
    /// types and preferences (global scope), and also identifiers of variables (local scope).
    /// </summary>
    public class IDManager
    {
        /// <summary>
        /// Predicates ID mapping. Stores the data about original predicate names (and number of arguments) and their mapped IDs.
        /// </summary>
        public EntityIDManager Predicates { set; get; } = new EntityIDManager();

        /// <summary>
        /// Functions ID mapping. Stores the data about original function names (and number of arguments) and their mapped IDs.
        /// </summary>
        public EntityIDManager Functions { set; get; } = new EntityIDManager();

        /// <summary>
        /// Constants ID mapping. Stores the data about original constant names and their mapped IDs.
        /// </summary>
        public EntityIDManager Constants { set; get; } = new EntityIDManager();

        /// <summary>
        /// Types ID mapping. Stores the data about original type names and their mapped IDs.
        /// </summary>
        public EntityIDManager Types { set; get; } = new EntityIDManager();

        /// <summary>
        /// Preferences ID mapping. Stores the data about original preference names and their mapped IDs.
        /// </summary>
        public EntityIDManager Preferences { set; get; } = new EntityIDManager();

        /// <summary>
        /// Variables ID mapping. Stores the data about original variable names and their mapped IDs (only locally unique!).
        /// </summary>
        public LocalEntityIDManager Variables { set; get; } = new LocalEntityIDManager();

        /// <summary>
        /// Invalid ID value.
        /// </summary>
        public const int INVALID_ID = -1;

        /// <summary>
        /// Undefined ID value (e.g. functions can have an undefined value).
        /// </summary>
        public const int UNDEFINED_ID = -2;

        /// <summary>
        /// Generic prefix for variable names (exact variable names in parameters are not stored, at the moment).
        /// </summary>
        public const string GENERIC_VARIABLE_PREFIX = "?var";

        /// <summary>
        /// Creates and initiliazes ID manager from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public IDManager(InputData.PDDLInputData inputData)
        {
            Types.Register("object");
            foreach (var type in inputData.Domain.Types)
            {
                if (!Types.IsRegistered(type.TypeName))
                {
                    Types.Register(type.TypeName);
                }
                foreach (var baseType in type.BaseTypeNames)
                {
                    if (!Types.IsRegistered(baseType))
                    {
                        Types.Register(baseType);
                    }
                }
            }

            inputData.Domain.Predicates.ForEach(predicate => Predicates.Register(predicate.Name, predicate.Terms.Count));
            inputData.Domain.Functions.ForEach(function => Functions.Register(function.Name, function.Terms.Count));
            inputData.Domain.Constants.ForEach(constant => Constants.Register(constant.ConstantName));
            inputData.Problem.Objects.ForEach(obj => Constants.Register(obj.ObjectName));
        }
    }

    /// <summary>
    /// Generic entity ID mapping. Stores the data about original entity names and their mapped IDs. The entity can be a predicate,
    /// a function, a constant or a type. Mapping key is a combination of an entity name and (optinaly) a number of its arguments -
    /// while e.g. constants are uniquely identified only by their name, the predicates are identified by their names and a number
    /// of their arguments.
    /// </summary>
    public class EntityIDManager
    {
        /// <summary>
        /// Mapping of the original entity name with a number of arguments to its corresponding ID.
        /// </summary>
        protected Dictionary<Tuple<string, int>, int> NameWithArgToID { set; get; } = new Dictionary<Tuple<string, int>, int>();

        /// <summary>
        /// Free ID to be used for the next entity registration.
        /// </summary>
        protected int NextFreeID { set; get; } = 0;

        /// <summary>
        /// Registers the specified entity and returns the registered ID value.
        /// </summary>
        /// <param name="name">Original name of the entity.</param>
        /// <param name="numberOfArguments">Number of entity arguments.</param>
        /// <returns>Registered entity ID.</returns>
        public int Register(string name, int numberOfArguments = 0)
        {
            var key = Tuple.Create(name, numberOfArguments);
            Debug.Assert(!NameWithArgToID.ContainsKey(key));
            NameWithArgToID.Add(key, NextFreeID);
            return NextFreeID++;
        }

        /// <summary>
        /// Checks whether the entity has been registered.
        /// </summary>
        /// <param name="name">Original entity name.</param>
        /// <param name="numberOfArguments">Number of entity arguments (if it has any).</param>
        /// <returns>True if the entity has been registered. False otherwise.</returns>
        public bool IsRegistered(string name, int numberOfArguments = 0)
        {
            var key = Tuple.Create(name, numberOfArguments);
            return NameWithArgToID.ContainsKey(key);
        }

        /// <summary>
        /// Unregisters the specified entity.
        /// </summary>
        /// <param name="name">Original name of the entity.</param>
        /// <param name="numberOfArguments">Number of entity arguments.</param>
        public void Unregister(string name, int numberOfArguments = 0)
        {
            var key = Tuple.Create(name, numberOfArguments);
            Debug.Assert(NameWithArgToID.ContainsKey(key));
            NameWithArgToID.Remove(key);
        }

        /// <summary>
        /// Gets the entity ID from its name.
        /// </summary>
        /// <param name="name">Original entity name.</param>
        /// <param name="numberOfArguments">Number of entity arguments (if it has any).</param>
        /// <returns>Mapped entity ID.</returns>
        public int GetID(string name, int numberOfArguments = 0)
        {
            var key = Tuple.Create(name, numberOfArguments);

            int value = -1;
            if (!NameWithArgToID.TryGetValue(key, out value))
            {
                Debug.Assert(false, "Invalid entity name requested!");
            }
            return value;
        }

        /// <summary>
        /// Gets the original entity name from its mapped ID.
        /// </summary>
        /// <param name="id">Mapped entity ID.</param>
        /// <returns>Original entity name.</returns>
        public string GetNameFromID(int id)
        {
            if (id == ObjectFunctionTerm.UNDEFINED_VALUE)
            {
                return "undefined";
            }

            foreach (var item in NameWithArgToID)
            {
                if (item.Value == id)
                {
                    return item.Key.Item1;
                }
            }

            Debug.Assert(false, "Invalid entity ID requested!");
            return "";
        }

        /// <summary>
        /// Gets the number of arguments for the specified entity (especially predicates/functions, other entities are nullary).
        /// </summary>
        /// <param name="id">Mapped entity ID.</param>
        /// <returns>Number of arguments of the entity.</returns>
        public int GetNumberOfArgumentsFromID(int id)
        {
            foreach (var item in NameWithArgToID)
            {
                if (item.Value == id)
                {
                    return item.Key.Item2;
                }
            }

            Debug.Assert(false, "Invalid entity ID requested!");
            return 0;
        }

        /// <summary>
        /// Gets the list of all used/mapped IDs.
        /// </summary>
        /// <returns>List of used IDs.</returns>
        public IEnumerable<int> GetUsedIDs()
        {
            return NameWithArgToID.Values;
        }

        /// <summary>
        /// Gets the count of mapped items.
        /// </summary>
        /// <returns>Number of mapped items.</returns>
        public int Count()
        {
            return NameWithArgToID.Count;
        }
    }

    /// <summary>
    /// Extended entity ID manager, handling entities with a local scope.
    /// </summary>
    public class LocalEntityIDManager : EntityIDManager
    {
        /// <summary>
        /// Registers local parameters.
        /// </summary>
        /// <param name="parameters">Input local parameters.</param>
        public void RegisterLocalParameters(InputData.PDDL.Parameters parameters)
        {
            parameters.ForEach(parameter => Register(parameter.ParameterName));
        }

        /// <summary>
        /// Unregisters local parameters.
        /// </summary>
        /// <param name="parameters">Input local parameters.</param>
        public void UnregisterLocalParameters(InputData.PDDL.Parameters parameters)
        {
            parameters.ForEach(parameter => Unregister(parameter.ParameterName));

            // if a full block was processed, we can reset the ID to zero
            if (NameWithArgToID.Count == 0)
            {
                NextFreeID = 0;
            }
        }
    }
}
