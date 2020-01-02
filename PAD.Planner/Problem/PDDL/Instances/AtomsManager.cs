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
        public AtomsManager(InputData.PDDLInputData inputData, IDManager idManager)
        {
            Func<int, InputData.PDDL.DefinitionTerms, Tuple<IAtom, Parameters>> ProcessAtom = (int atomID, InputData.PDDL.DefinitionTerms inputTerms) =>
            {
                List<ITerm> terms = new List<ITerm>();
                Parameters parameters = new Parameters();

                int freeVarID = 0;
                foreach (var term in inputTerms)
                {
                    List<int> typeIDs = new List<int>();
                    foreach (var typeName in term.TypeNames)
                    {
                        typeIDs.Add(idManager.Types.GetID(typeName));
                    }

                    terms.Add(new VariableTerm(freeVarID));
                    parameters.Add(new Parameter(freeVarID++, typeIDs, idManager));
                }

                IAtom atom = new Atom(atomID, terms);
                return Tuple.Create(atom, parameters);
            };

            foreach (var predicate in inputData.Domain.Predicates)
            {
                int atomName = idManager.Predicates.GetID(predicate.Name, predicate.Terms.Count);
                var processedAtom = ProcessAtom(atomName, predicate.Terms);

                LiftedPredicates.Add(processedAtom);
            }

            foreach (var function in inputData.Domain.Functions)
            {
                int atomName = idManager.Functions.GetID(function.Name, function.Terms.Count);
                var processedAtom = ProcessAtom(atomName, function.Terms);

                if (function.IsNumbericFunction())
                {
                    LiftedNumericFunctions.Add(processedAtom);
                }
                else
                {
                    List<int> returnTypeIDs = new List<int>();
                    foreach (var typeName in function.ReturnValueTypes)
                    {
                        returnTypeIDs.Add(idManager.Types.GetID(typeName));
                    }

                    LiftedObjectFunctions.Add(Tuple.Create(processedAtom.Item1, returnTypeIDs, processedAtom.Item2));
                }
            }
        }
    }
}
