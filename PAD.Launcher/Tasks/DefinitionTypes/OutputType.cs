
namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Output type of the planning task.
    /// </summary>
    public enum OutputType
    {
        Unspecified,
        ToConsole,
        ToFile,
        CustomResultsProcessor
    }

    /// <summary>
    /// Auxilliary class for converting output type parameters.
    /// </summary>
    public static class OutputTypeConverter
    {
        /// <summary>
        /// Converts the parameter name to the type.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>Corresponding type.</returns>
        public static OutputType Convert(string paramName)
        {
            switch (paramName.ToUpper())
            {
                case "TOCONSOLE":
                    return OutputType.ToConsole;
                case "TOFILE":
                    return OutputType.ToFile;
                default:
                    throw new TasksLoaderException($"Unknown output type '{paramName}'!");
            }
        }
    }
}
