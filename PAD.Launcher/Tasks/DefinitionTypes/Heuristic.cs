// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace PAD.Launcher.Tasks.DefinitionTypes
{
    /// <summary>
    /// Heuristic used in the planning task.
    /// </summary>
    public enum Heuristic
    {
        BlindHeuristic,
        StripsHeuristic,
        PerfectRelaxationHeuristic,
        AdditiveRelaxationHeuristic,
        MaxRelaxationHeuristic,
        FFHeuristic,
        PDBHeuristic
    }

    /// <summary>
    /// Auxiliary class for converting heuristic parameters.
    /// </summary>
    public static class HeuristicConverter
    {
        /// <summary>
        /// Converts the parameter name to the type.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>Corresponding type.</returns>
        public static Heuristic Convert(string paramName)
        {
            switch (paramName.ToUpper())
            {
                case "BLINDHEURISTIC":
                    return Heuristic.BlindHeuristic;
                case "STRIPSHEURISTIC":
                    return Heuristic.StripsHeuristic;
                case "PERFECTRELAXATIONHEURISTIC":
                    return Heuristic.PerfectRelaxationHeuristic;
                case "ADDITIVERELAXATIONHEURISTIC":
                    return Heuristic.AdditiveRelaxationHeuristic;
                case "MAXRELAXATIONHEURISTIC":
                    return Heuristic.MaxRelaxationHeuristic;
                case "FFHEURISTIC":
                    return Heuristic.FFHeuristic;
                case "PDBHEURISTIC":
                    return Heuristic.PDBHeuristic;
                default:
                    throw new TasksLoaderException($"Unknown heuristic name '{paramName}'!");
            }
        }
    }
}
