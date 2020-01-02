
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface for the general assignment (a value assigned to a variable).
    /// </summary>
    public interface IAssignment
    {
        /// <summary>
        /// Gets the assignment variable.
        /// </summary>
        /// <returns>Assignment variable.</returns>
        int GetVariable();

        /// <summary>
        /// Gets the assigned value.
        /// </summary>
        /// <returns>Assigned value.</returns>
        int GetValue();

        /// <summary>
        /// Makes a deep copy of the assigned value.
        /// </summary>
        /// <returns>Deep copy of the assigned value.</returns>
        IAssignment Clone();

        /// <summary>
        /// Checks the equality with other assigned value.
        /// </summary>
        /// <param name="other">Other assigned value.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        bool Equals(IAssignment other);
    }
}
