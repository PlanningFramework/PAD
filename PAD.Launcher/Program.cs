
namespace PAD.Launcher
{
    /// <summary>
    /// Entry point of the launcher.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main method, executing tasks specified in input arguments of the program.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        public static void Main(string[] args)
        {
            TasksLauncher.ExecuteFromProgramArguments(args);
        }
    }
}
