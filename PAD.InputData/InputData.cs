
namespace PAD.InputData
{
    /// <summary>
    /// General input data. Can be either PDDL specific or SAS+ specific.
    /// </summary>
    abstract public class InputData
    {
    }

    /// <summary>
    /// General input data exception. Can be thrown in a process of loading or validation of input data structures.
    /// </summary>
    abstract public class InputDataException : System.Exception
    {
        /// <summary>
        /// Creates an input data exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        public InputDataException(string message) : base(message)
        {
        }
    }
}
