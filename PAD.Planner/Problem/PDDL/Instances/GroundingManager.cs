using System.Collections.Generic;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Grounding manager handling substitutions generation and making ground instances of lifted entities.
    /// </summary>
    public class GroundingManager
    {
        /// <summary>
        /// Grounder instance.
        /// </summary>
        private Lazy<Grounder> Grounder { get; }

        /// <summary>
        /// Substitution generator.
        /// </summary>
        private Lazy<SubstitutionGenerator> SubstitutionGenerator { get; }

        /// <summary>
        /// Constants manager, handling available constants for types.
        /// </summary>
        private Lazy<ConstantsManager> ConstantsManager { get; }

        /// <summary>
        /// Atoms manager, handling available grounded instances for predicates/functions.
        /// </summary>
        private Lazy<AtomsManager> AtomsManager { get; }

        /// <summary>
        /// Constructs the grounding manager.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public GroundingManager(InputData.PDDLInputData inputData, IdManager idManager)
        {
            ConstantsManager = new Lazy<ConstantsManager>(() => new ConstantsManager(inputData, idManager));
            AtomsManager = new Lazy<AtomsManager>(() => new AtomsManager(inputData, idManager));
            SubstitutionGenerator = new Lazy<SubstitutionGenerator>(() => new SubstitutionGenerator(ConstantsManager));
            Grounder = new Lazy<Grounder>(() => new Grounder(idManager));
        }

        /// <summary>
        /// Grounds the specified atom by the given substitution and returns it.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtom(IAtom atom, ISubstitution substitution)
        {
            return Grounder.Value.GroundAtom(atom, substitution);
        }

        /// <summary>
        /// Grounds the specified atom by the given substitution and returns it. The "deep" version of terms grounding is used.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtomDeep(IAtom atom, ISubstitution substitution, IState state)
        {
            return Grounder.Value.GroundAtomDeep(atom, substitution, state);
        }

        /// <summary>
        /// Grounds the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTerm(ITerm term, ISubstitution substitution)
        {
            return Grounder.Value.GroundTerm(term, substitution);
        }

        /// <summary>
        /// Grounds the term. This version does "deep" grounding, in the sense that even object function terms are 
        /// grounded into constant terms (the value of the object function term is gotten from the given reference state).
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTermDeep(ITerm term, ISubstitution substitution, IState referenceState)
        {
            return Grounder.Value.GroundTermDeep(term, substitution, referenceState);
        }

        /// <summary>
        /// Grounds the expression by the given substitution and returns it.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>Grounded expression.</returns>
        public IExpression GroundExpression(IExpression expression, ISubstitution substitution)
        {
            return Grounder.Value.GroundExpression(expression, substitution);
        }

        /// <summary>
        /// Grounds the numeric expression by the given substitution and returns it.
        /// </summary>
        /// <param name="numericExpression">Numeric expression.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>Grounded numeric expression.</returns>
        public INumericExpression GroundNumericExpression(INumericExpression numericExpression, ISubstitution substitution)
        {
            return Grounder.Value.GroundNumericExpression(numericExpression, substitution);
        }

        /// <summary>
        /// Grounds the given conditions by the substitution. Variables in the conditions that are not contained in the substitution remain lifted and are added to the
        /// conditions parameters. The result is partially grounded/lifted conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>(Partially) grounded conditions.</returns>
        public Conditions GroundConditions(Conditions conditions, ISubstitution substitution)
        {
            return Grounder.Value.GroundConditions(conditions, substitution);
        }

        /// <summary>
        /// Generates all substitutions based on the given local parameters (e.g. operator arguments or forall expression arguments). Lazy-evaluated.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Collection of local substitutions.</returns>
        public IEnumerable<ISubstitution> GenerateAllLocalSubstitutions(Parameters parameters)
        {
            return SubstitutionGenerator.Value.GenerateAllLocalSubstitutions(parameters);
        }

        /// <summary>
        /// Gets all possible constants of the given type (i.e. including all constants of the ancestor types).
        /// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <returns>Collection of constants of the given type.</returns>
        public IEnumerable<int> GetAllConstantsOfType(int typeId)
        {
            return ConstantsManager.Value.GetAllConstantsOfType(typeId);
        }

        /// <summary>
        /// Gets the definition type(s) for the specified constant (more types = either clause)
        /// </summary>
        /// <param name="constantId">Constant ID.</param>
        /// <returns>Collection of definition types for the given constant.</returns>
        public ICollection<int> GetTypesForConstant(int constantId)
        {
            return ConstantsManager.Value.GetTypesForConstant(constantId);
        }

        /// <summary>
        /// Gets the collection of direct children types for the specified type.
        /// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <returns>Direct children types of the given type.</returns>
        public IEnumerable<int> GetChildrenTypes(int typeId)
        {
            return ConstantsManager.Value.GetChildrenTypes(typeId);
        }

        /// <summary>
        /// Gets all available lifted predicates in the planning problem.
        /// </summary>
        /// <returns>List of lifted predicates with the parameters.</returns>
        public List<Tuple<IAtom, Parameters>> GetAllLiftedPredicates()
        {
            return AtomsManager.Value.LiftedPredicates;
        }

        /// <summary>
        /// Gets all available grounded predicate instances in the planning problem.
        /// </summary>
        /// <returns>List of all grounded predicates.</returns>
        public List<IAtom> GetAllGroundedPredicates()
        {
            List<IAtom> resultList = new List<IAtom>();

            foreach (var predicate in GetAllLiftedPredicates())
            {
                IAtom predicateAtom = predicate.Item1;
                Parameters predicateVariables = predicate.Item2;

                foreach (var substitution in GenerateAllLocalSubstitutions(predicateVariables))
                {
                    resultList.Add(GroundAtom(predicateAtom, substitution));
                }
            }

            return resultList;
        }

        /// <summary>
        /// Gets all available lifted numeric functions in the planning problem.
        /// </summary>
        /// <returns>List of lifted numeric functions with the parameters.</returns>
        public List<Tuple<IAtom, Parameters>> GetAllLiftedNumericFunctions()
        {
            return AtomsManager.Value.LiftedNumericFunctions;
        }

        /// <summary>
        /// Gets all available grounded numeric functions in the planning problem.
        /// </summary>
        /// <returns>List of all grounded numeric functions.</returns>
        public List<IAtom> GetAllGroundedNumericFunctions()
        {
            List<IAtom> resultList = new List<IAtom>();

            foreach (var function in GetAllLiftedNumericFunctions())
            {
                IAtom functionAtom = function.Item1;
                Parameters functionParameters = function.Item2;

                foreach (var substitution in GenerateAllLocalSubstitutions(functionParameters))
                {
                    resultList.Add(GroundAtom(functionAtom, substitution));
                }
            }

            return resultList;
        }

        /// <summary>
        /// Gets all available lifted object functions in the planning problem.
        /// </summary>
        /// <returns>List of lifted object functions with the parameters and return types.</returns>
        public List<Tuple<IAtom, List<int>, Parameters>> GetAllLiftedObjectFunctions()
        {
            return AtomsManager.Value.LiftedObjectFunctions;
        }

        /// <summary>
        /// Gets all available grounded object functions with values in the planning problem.
        /// </summary>
        /// <returns>List of all grounded object functions with values.</returns>
        public List<Tuple<IAtom, List<int>>> GetAllGroundedObjectFunctions()
        {
            var resultList = new List<Tuple<IAtom, List<int>>>();

            foreach (var function in GetAllLiftedObjectFunctions())
            {
                IAtom functionAtom = function.Item1;
                List<int> functionReturnTypes = function.Item2;
                Parameters functionVariables = function.Item3;

                HashSet<int> possibleReturnValues = new HashSet<int> {ObjectFunctionTerm.UndefinedValue};

                foreach (var returnType in functionReturnTypes)
                {
                    possibleReturnValues.UnionWith(GetAllConstantsOfType(returnType));
                }

                foreach (var substitution in GenerateAllLocalSubstitutions(functionVariables))
                {
                    resultList.Add(Tuple.Create(GroundAtom(functionAtom, substitution), new List<int>(possibleReturnValues)));
                }
            }

            return resultList;
        }
    }
}
