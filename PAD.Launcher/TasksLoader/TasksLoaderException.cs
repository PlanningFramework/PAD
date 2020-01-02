
namespace PAD.Launcher
{
    /// <summary>
    /// Exception specifying errors while loading the tasks.
    /// </summary>
    public class TasksLoaderException : System.Exception
    {
        /// <summary>
        /// Creates a loading exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        public TasksLoaderException(string message) : base(message)
        {
        }
    }
}
