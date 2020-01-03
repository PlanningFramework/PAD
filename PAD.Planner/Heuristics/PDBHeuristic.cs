using System.Collections.Generic;

namespace PAD.Planner.Heuristics
{
    /// <summary>
    /// Implementation of the PDB (Pattern Database) heuristic. The heuristic values are precomputed for chosen patterns in abstracted planning problem
    /// and stored in a look-up collection.
    /// </summary>
    public class PDBHeuristic : Heuristic
    {
        /// <summary>
        /// Calculated pattern database.
        /// </summary>
        private IPatternDatabase PatternDatabase { get; }

        /// <summary>
        /// Constructs the heuristic.
        /// </summary>
        /// <param name="problem">Original planning problem.</param>
        /// <param name="findAdditivePatterns">Should the additive pattern be automatically determined?</param>
        /// <param name="patternHints">Pattern hints for the database.</param>
        public PDBHeuristic(IProblem problem, bool findAdditivePatterns = true, List<HashSet<int>> patternHints = null) : base(problem)
        {
            PatternDatabase = problem.GetPatternDatabase(findAdditivePatterns, patternHints);
        }

        /// <summary>
        /// Gets the heuristic value for the given state (in the context of forward search).
        /// </summary>
        /// <param name="state">State to be evaluated.</param>
        /// <returns>Heuristic value for the specified state.</returns>
        protected override double GetValueImpl(IState state)
        {
            return PatternDatabase.GetValue(state);
        }

        /// <summary>
        /// Gets the heuristic value for the given conditions (in the context of backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Heuristic value for the specified conditions.</returns>
        protected override double GetValueImpl(IConditions conditions)
        {
            return PatternDatabase.GetValue(conditions);
        }

        /// <summary>
        /// Gets the heuristic description.
        /// </summary>
        /// <returns>Heuristic description.</returns>
        public override string GetName()
        {
            return "PDB Heuristic";
        }
    }
}
