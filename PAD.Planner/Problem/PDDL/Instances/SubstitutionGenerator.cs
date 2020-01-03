using System.Collections.Generic;
using System;
// ReSharper disable CommentTypo

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
        private Lazy<ConstantsManager> ConstantsManager { get; }

        /// <summary>
        /// Constructs the substitution generator.
        /// </summary>
        /// <param name="constantsManager">Constants manager.</param>
        public SubstitutionGenerator(Lazy<ConstantsManager> constantsManager)
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
                constructionList.Add(IdManager.InvalidId);
            }

            return SubstituteParams(0, constructionList, parameters);
        }

        /// <summary>
        /// Substitutes the given parameter and recursively calls the function on the next parameters (divide and conquer). Lazy-evaluated.
        /// </summary>
        /// <param name="currentIndex">Parameter index.</param>
        /// <param name="constructionList">List of a substitution being constructed.</param>
        /// <param name="parameters">Parameters being substituted.</param>
        private IEnumerable<ISubstitution> SubstituteParams(int currentIndex, List<int> constructionList, Parameters parameters)
        {
            if (currentIndex >= parameters.Count)
            {
                // last possible index, we have a valid substitution
                yield return new Substitution(constructionList, parameters);
            }
            else
            {
                foreach (var typeId in parameters[currentIndex].TypeNamesIDs)
                {
                    foreach (var constantId in ConstantsManager.Value.GetAllConstantsOfType(typeId))
                    {
                        constructionList[currentIndex] = constantId;
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
