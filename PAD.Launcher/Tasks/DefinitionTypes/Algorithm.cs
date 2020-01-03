// ReSharper disable StringLiteralTypo

namespace PAD.Launcher.Tasks.DefinitionTypes
{
    /// <summary>
    /// Search algorithm used in the planning task.
    /// </summary>
    public enum Algorithm
    {
        AStarSearch,
        BeamSearch,
        HillClimbingSearch
    }

    /// <summary>
    /// Auxiliary class for converting algorithms parameters.
    /// </summary>
    public static class AlgorithmConverter
    {
        /// <summary>
        /// Converts the parameter name to the type.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>Corresponding type.</returns>
        public static Algorithm Convert(string paramName)
        {
            switch (paramName.ToUpper())
            {
                case "ASTARSEARCH":
                    return Algorithm.AStarSearch;
                case "BEAMSEARCH":
                    return Algorithm.BeamSearch;
                case "HILLCLIMBINGSEARCH":
                    return Algorithm.HillClimbingSearch;
                default:
                    throw new TasksLoaderException($"Unknown algorithm name '{paramName}'!");
            }
        }
    }
}
