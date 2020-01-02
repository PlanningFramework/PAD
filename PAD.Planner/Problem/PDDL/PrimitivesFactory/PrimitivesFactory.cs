using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a factory conveniently creating PDDL primitives on demand.
    /// </summary>
    public class PrimitivesFactory
    {
        /// <summary>
        /// ID manager.
        /// </summary>
        IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the atom factory.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public PrimitivesFactory(IDManager idManager)
        {
            IDManager = idManager;
        }

        /// <summary>
        /// Creates a predicate atom with the specified name and the given constant arguments.
        /// </summary>
        /// <param name="name">Predicate name.</param>
        /// <param name="argumentNames">Constant/variable argument names.</param>
        /// <returns>New predicate atom.</returns>
        public IAtom CreatePredicate(string name, params string[] argumentNames)
        {
            return CreatePredicateOrFunction(false, name, argumentNames);
        }

        /// <summary>
        /// Creates a function atom with the specified name and the given constant arguments.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="argumentNames">Constant/variable argument names.</param>
        /// <returns>New function atom.</returns>
        public IAtom CreateFunction(string name, params string[] argumentNames)
        {
            return CreatePredicateOrFunction(true, name, argumentNames);
        }

        /// <summary>
        /// Internal method for creating predicate of function atom from the given constant arguments.
        /// </summary>
        /// <param name="isFunction">Function or predicate?</param>
        /// <param name="name">Atom name.</param>
        /// <param name="argumentNames">Constant/variable argument names.</param>
        /// <returns>New atom.</returns>
        private IAtom CreatePredicateOrFunction(bool isFunction, string name, params string[] argumentNames)
        {
            int nameID = (isFunction) ? IDManager.Functions.GetID(name, argumentNames.Length) : IDManager.Predicates.GetID(name, argumentNames.Length);
            var terms = new List<ITerm>();
            foreach (var argumentName in argumentNames)
            {
                terms.Add(CreateTerm(argumentName));
            }
            return new Atom(nameID, terms);
        }

        /// <summary>
        /// Creates a state with the specified contained predicates.
        /// </summary>
        /// <param name="predicates">Predicates.</param>
        /// <returns>New state.</returns>
        public IState CreateState(params IAtom[] predicates)
        {
            var inputPredicates = new HashSet<IAtom>();
            foreach (var predicate in predicates)
            {
                inputPredicates.Add(predicate);
            }
            return new State(inputPredicates, null, null, IDManager);
        }

        /// <summary>
        /// Creates a state with the specified contained predicates and functions.
        /// </summary>
        /// <param name="predicates">Predicates.</param>
        /// <param name="objectFunctions">Object functions with values.</param>
        /// <param name="numericFunctions">Numeric functions with values.</param>
        /// <returns>New state.</returns>
        public IState CreateState(HashSet<IAtom> predicates, Dictionary<IAtom, int> objectFunctions = null, Dictionary<IAtom, double> numericFunctions = null)
        {
            return new State(predicates, numericFunctions, objectFunctions, IDManager);
        }

        /// <summary>
        /// Creates a relative state with the specified contained predicates.
        /// </summary>
        /// <param name="predicates">Predicates.</param>
        /// <returns>New relative state.</returns>
        public IRelativeState CreateRelativeState(params IAtom[] predicates)
        {
            var inputPredicates = new HashSet<IAtom>();
            foreach (var predicate in predicates)
            {
                inputPredicates.Add(predicate);
            }
            return new RelativeState(inputPredicates.Count != 0 ? inputPredicates : null, null, null, null, IDManager);
        }

        /// <summary>
        /// Creates a relative state with the specified contained predicates.
        /// </summary>
        /// <param name="negatedPredicatesIndices">Indices of negated predicates in the given list.</param>
        /// <param name="predicates">Predicates.</param>
        /// <returns>New relative state.</returns>
        public IRelativeState CreateRelativeState(HashSet<int> negatedPredicatesIndices, params IAtom[] predicates)
        {
            var inputPredicates = new HashSet<IAtom>();
            var inputNegatedPredicates = new HashSet<IAtom>();

            for (int i = 0; i < predicates.Length; ++i)
            {
                if (negatedPredicatesIndices.Contains(i))
                {
                    inputNegatedPredicates.Add(predicates[i]);
                }
                else
                {
                    inputPredicates.Add(predicates[i]);
                }
            }

            return new RelativeState(inputPredicates, inputNegatedPredicates, null, null, IDManager);
        }

        /// <summary>
        /// Creates a specified constant from the constant name.
        /// </summary>
        /// <param name="name">Constant name.</param>
        /// <returns>Constant ID.</returns>
        public int CreateConstant(string name)
        {
            return IDManager.Constants.GetID(name);
        }

        /// <summary>
        /// Creates a simple constant or variable term from the constant/variable name.
        /// </summary>
        /// <param name="name">Constant/variable name.</param>
        /// <returns>Constant or variable term.</returns>
        public ITerm CreateTerm(string name)
        {
            if (name.StartsWith("?"))
            {
                return new VariableTerm(IDManager.Variables.GetID(name));
            }
            else
            {
                return new ConstantTerm(IDManager.Constants.GetID(name), IDManager);
            }
        }

        /// <summary>
        /// Creates a specified type from the type name.
        /// </summary>
        /// <param name="name">Type name.</param>
        /// <returns>Type ID.</returns>
        public int CreateType(string name)
        {
            return IDManager.Types.GetID(name);
        }

        /// <summary>
        /// Creates a substitution from the given parameters and substitued constants.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <param name="constNames">Constant names.</param>
        /// <returns>New substitution.</returns>
        public ISubstitution CreateSubstitution(Parameters parameters, params string[] constNames)
        {
            Debug.Assert(parameters.Count == constNames.Length);
            ISubstitution substitution = new Substitution();

            for (int i = 0; i < parameters.Count; ++i)
            {
                if (!string.IsNullOrEmpty(constNames[i]))
                {
                    substitution.Add(parameters[i].ParameterNameID, CreateConstant(constNames[i]));
                }
            }
            return substitution;
        }
    }
}
