
namespace PAD.InputData.PDDL.Validator
{
    /// <summary>
    /// Exception specifying errors while validation of the input data.
    /// </summary>
    public class ValidationException : InputDataException
    {
        /// <summary>
        /// Creates a validation exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        public ValidationException(string message) : base(message)
        {
        }
    }
}
