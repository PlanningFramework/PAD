
namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Type of the task.
    /// </summary>
    public enum Type
    {
        PDDL,
        SAS
    }

    /// <summary>
    /// Auxilliary class for converting type parameters.
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts the parameter name to the type.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>Corresponding type.</returns>
        public static Type Convert(string paramName)
        {
            switch (paramName.ToUpper())
            {
                case "PDDL":
                    return Type.PDDL;
                case "SAS+":
                    return Type.SAS;
                default:
                    throw new TasksLoaderException($"Unknown type name '{paramName}'!");
            }
        }
    }
}
