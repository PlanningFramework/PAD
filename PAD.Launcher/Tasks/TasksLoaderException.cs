using System;

namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Exception specifying errors while loading the tasks.
    /// </summary>
    [Serializable]
    public class TasksLoaderException : Exception
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
