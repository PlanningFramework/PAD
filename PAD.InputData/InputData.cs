using System;

namespace PAD.InputData
{
    /// <summary>
    /// General input data. Can be either PDDL specific or SAS+ specific.
    /// </summary>
    public abstract class InputData
    {
    }

    /// <summary>
    /// General input data exception. Can be thrown in a process of loading or validation of input data structures.
    /// </summary>
    [Serializable]
    public abstract class InputDataException : Exception
    {
        /// <summary>
        /// Creates an input data exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        protected InputDataException(string message) : base(message)
        {
        }
    }
}
