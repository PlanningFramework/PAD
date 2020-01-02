
namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Common interface for a planning task to be executed, typically within a parallel execution.
    /// </summary>
    public interface IPlanningTask
    {
        /// <summary>
        /// Executes the planning task and processes the result.
        /// </summary>
        void Execute();
    }
}
