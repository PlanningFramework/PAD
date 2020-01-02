using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// PDDL parameters structure representing variables with corresponding type(s) that are unique in some local scope (i.e. operator
    /// arguments, forall parameters etc.). The parameter can have several disjunctive types (using PDDL "either" operator).
    /// </summary>
    public class Parameters : List<Parameter>
    {
        /// <summary>
        /// Constructs the object from the input data.
        /// </summary>
        /// <param name="parameters">PDDL parameters definition.</param>
        /// <param name="idManager">ID manager.</param>
        public Parameters(InputData.PDDL.Parameters parameters, IDManager idManager)
        {
            foreach (var parameter in parameters)
            {
                int parameterNameID = idManager.Variables.GetID(parameter.ParameterName);

                List<int> typeNamesIDs = new List<int>();
                foreach (var typeName in parameter.TypeNames)
                {
                    typeNamesIDs.Add(idManager.Types.GetID(typeName));
                }

                Add(new Parameter(parameterNameID, typeNamesIDs, idManager));
            }
        }

        /// <summary>
        /// Constructs the object from the given parameters.
        /// </summary>
        /// <param name="parameters">Parameters list.</param>
        public Parameters(params Parameter[] parameters) : base(parameters)
        {
        }

        /// <summary>
        /// Constructs empty parameters.
        /// </summary>
        public Parameters()
        {
        }

        /// <summary>
        /// Adds the other parameters. Uniqueness of the parameter names has to be ensured beforehand.
        /// </summary>
        /// <param name="other">Other parameters.</param>
        public void Add(Parameters other)
        {
            foreach (var param in other)
            {
                Add(param.Clone());
            }
        }

        /// <summary>
        /// Checks whether the current parameters are in conflict with other specified parameters, i.e. some of the parameters have a same name.
        /// </summary>
        /// <param name="other">Other parameters.</param>
        /// <returns>True if the parameters are in conflict with the other parameters, false otherwise.</returns>
        public bool AreConflictedWith(Parameters other)
        {
            if (other == null)
            {
                return false;
            }

            foreach (var parameter in this)
            {
                if (other.Exists(otherParameter => parameter.ParameterNameID == otherParameter.ParameterNameID))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the maximal used parameter ID.
        /// </summary>
        /// <returns>Maximal used parameter ID.</returns>
        public int GetMaxUsedParameterID()
        {
            int maxID = 0;
            foreach (var parameter in this)
            {
                maxID = Math.Max(maxID, parameter.ParameterNameID);
            }
            return maxID;
        }

        /// <summary>
        /// Creates a deep copy of the parameters.
        /// </summary>
        /// <returns>Deep copy of parameters.</returns>
        public Parameters Clone()
        {
            Parameters newParameters = new Parameters();
            foreach (var parameter in this)
            {
                newParameters.Add(parameter.Clone());
            }
            return newParameters;
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

            Parameters other = obj as Parameters;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> parametersStrings = new List<string>();
            foreach (var parameter in this)
            {
                parametersStrings.Add(parameter.ToString());
            }
            return $"({string.Join(" ", parametersStrings)})";
        }
    }
}
