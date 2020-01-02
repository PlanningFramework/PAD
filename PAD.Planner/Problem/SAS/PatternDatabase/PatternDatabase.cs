using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of the SAS+ pattern database of the abstracted planning problem, used in PDB heuristic. The heuristic values are precomputed for
    /// chosen patterns in abstracted planning problem and stored in a look-up collection for the actual heuristic calls. The database can actually contain
    /// multiple different patterns which are treated as additive (the result value is calculated by adding the values from all the individual patterns).
    /// </summary>
    public class PatternDatabase : List<Pattern>, IPatternDatabase
    {
        /// <summary>
        /// Constructs the pattern database.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <param name="findAdditivePatterns">Should the additive patterns be automatically found?</param>
        /// <param name="patternHints">Explicitly requested patterns for the database (only if findAdditivePatterns is false).</param>
        public PatternDatabase(Problem problem, bool findAdditivePatterns = true, List<HashSet<int>> patternHints = null)
        {
            PatternDatabaseBuilder builder = new PatternDatabaseBuilder(problem);
            builder.BuildPatterns(findAdditivePatterns, patternHints).ForEach(entry => Add(entry));
        }

        /// <summary>
        /// Gets the calculated heuristic value for the specified state.
        /// </summary>
        /// <param name="generalState">State.</param>
        /// <returns>Heuristic value for the state.</returns>
        public double GetValue(Planner.IState generalState)
        {
            IState state = (IState)generalState;
            double result = 0;

            foreach (var pattern in this)
            {
                double distance = pattern.GetDistance(state);
                if (distance == PatternValuesDistances.MAX_DISTANCE)
                {
                    return PatternValuesDistances.MAX_DISTANCE;
                }

                result += distance;
            }

            return result;
        }

        /// <summary>
        /// Gets the calculated heuristic value for the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Heuristic value for the conditions.</returns>
        public double GetValue(Planner.IConditions conditions)
        {
            throw new NotSupportedException("PDB heuristic for the backward search is not supported, at the momement.");
        }
    }
}
