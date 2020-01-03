using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure for management of atoms for predicates and functions. The main purpose is to hold all possible instances of
    /// predicates/functions in the given PDDL planning problem.
    /// </summary>
    public class AtomsManager
    {
        /// <summary>
        /// Available lifted predicates in the planning problem.
        /// </summary>
        public List<Tuple<IAtom, Parameters>> LiftedPredicates { set; get; } = new List<Tuple<IAtom, Parameters>>();

        /// <summary>
        /// Available lifted numeric functions in the planning problem.
        /// </summary>
        public List<Tuple<IAtom, Parameters>> LiftedNumericFunctions { set; get; } = new List<Tuple<IAtom, Parameters>>();

        /// <summary>
        /// Available lifted object functions in the planning problem.
        /// </summary>
        public List<Tuple<IAtom, List<int>, Parameters>> LiftedObjectFunctions { set; get; } = new List<Tuple<IAtom, List<int>, Parameters>>();

        /// <summary>
        /// Constructs the atoms manager.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public AtomsManager(InputData.PDDLInputData inputData, IdManager idManager)
        {
            Func<int, InputData.PDDL.DefinitionTerms, Tuple<IAtom, Parameters>> processAtom = (atomId, inputTerms) =>
            {
                List<ITerm> terms = new List<ITerm>();
                Parameters parameters = new Parameters();

                int freeVarId = 0;
                foreach (var term in inputTerms)
                {
                    List<int> typeIDs = new List<int>();
                    foreach (var typeName in term.TypeNames)
                    {
                        typeIDs.Add(idManager.Types.GetId(typeName));
                    }

                    terms.Add(new VariableTerm(freeVarId));
                    parameters.Add(new Parameter(freeVarId++, typeIDs, idManager));
                }

                IAtom atom = new Atom(atomId, terms);
                return Tuple.Create(atom, parameters);
            };

            foreach (var predicate in inputData.Domain.Predicates)
            {
                int atomName = idManager.Predicates.GetId(predicate.Name, predicate.Terms.Count);
                var processedAtom = processAtom(atomName, predicate.Terms);

                LiftedPredicates.Add(processedAtom);
            }

            foreach (var function in inputData.Domain.Functions)
            {
                int atomName = idManager.Functions.GetId(function.Name, function.Terms.Count);
                var processedAtom = processAtom(atomName, function.Terms);

                if (function.IsNumericFunction())
                {
                    LiftedNumericFunctions.Add(processedAtom);
                }
                else
                {
                    List<int> returnTypeIds = new List<int>();
                    foreach (var typeName in function.ReturnValueTypes)
                    {
                        returnTypeIds.Add(idManager.Types.GetId(typeName));
                    }

                    LiftedObjectFunctions.Add(Tuple.Create(processedAtom.Item1, returnTypeIds, processedAtom.Item2));
                }
            }
        }
    }
}
