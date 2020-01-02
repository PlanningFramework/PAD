
namespace PAD.Planner
{
    /// <summary>
    /// General interface for a successor (forward transition).
    /// </summary>
    public interface ISuccessor : ITransition
    {
        /// <summary>
        /// Gets the successor state.
        /// </summary>
        /// <returns>Successor state.</returns>
        IState GetSuccessorState();
    }
}
