
namespace PAD.InputData.PDDL.Loader
{
    /// <summary>
    /// Exception specifying errors while loading the input files.
    /// </summary>
    public class LoadingException : InputDataException
    {
        /// <summary>
        /// Creates a loading exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        public LoadingException(string message) : base(message)
        {
        }
    }
}
