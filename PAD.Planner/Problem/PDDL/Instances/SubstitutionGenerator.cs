using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure intended to handle generation of substitutions, which are then used for grounding of PDDL operators.
    /// </summary>
    public class SubstitutionGenerator
    {
        /// <summary>
        /// Constants manager, handling available constants for types.
        /// </summary>
        private Lazy<ConstantsManager> ConstantsManager { set; get; } = null;

        /// <summary>
        /// Constructs the substitution generator.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="constantsManager">Constants manager.</param>
        /// <param name="idManager">ID manager.</param>
        public SubstitutionGenerator(InputData.PDDLInputData inputData, Lazy<ConstantsManager> constantsManager, IDManager idManager)
        {
            ConstantsManager = constantsManager;
        }

        /// <summary>
        /// Generates all substitutions based on the given local parameters (e.g. operator arguments or forall expression arguments). Lazy-evaluated.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Collection of local substitutions.</returns>
        public IEnumerable<ISubstitution> GenerateAllLocalSubstitutions(Parameters parameters)
        {
            List<int> constructionList = new List<int>(parameters.Count);
            for (int i = 0; i < parameters.Count; ++i)
            {
                constructionList.Add(IDManager.INVALID_ID);
            }

            return SubstituteParams(0, constructionList, parameters);
        }

        /// <summary>
        /// Substitues the given parameter and recursively calls the function on the next parameters (divide and conquer). Lazy-evaluated.
        /// </summary>
        /// <param name="currentIndex">Parameter index.</param>
        /// <param name="constructionList">List of a substitution being constucted.</param>
        /// <param name="parameters">Parameters being substitued.</param>
        private IEnumerable<ISubstitution> SubstituteParams(int currentIndex, List<int> constructionList, Parameters parameters)
        {
            if (currentIndex >= parameters.Count)
            {
                // last possible index, we have a valid substitution
                yield return new Substitution(constructionList, parameters);
            }
            else
            {
                foreach (var typeID in parameters[currentIndex].TypeNamesIDs)
                {
                    foreach (var constantID in ConstantsManager.Value.GetAllConstantsOfType(typeID))
                    {
                        constructionList[currentIndex] = constantID;
                        foreach (var deeperSubstitution in SubstituteParams(currentIndex + 1, constructionList, parameters))
                        {
                            yield return deeperSubstitution;
                        }
                    }
                }
            }
        }
    }
}
