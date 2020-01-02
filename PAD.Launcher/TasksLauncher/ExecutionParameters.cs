using PAD.Launcher.Tasks;

namespace PAD.Launcher
{
    /// <summary>
    /// Encapsulated planning tasks collection with execution parameters.
    /// </summary>
    public class PlanningTasksWithExecutionParameters
    {
        /// <summary>
        /// Collection of planning tasks for the execution.
        /// </summary>
        public PlanningTask[] PlanningTasks { set; get; } = null;

        /// <summary>
        /// Execution parameters.
        /// </summary>
        public ExecutionParameters Parameters { set; get; } = null;

        /// <summary>
        /// Creates the planning tasks with execution parameters.
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="parameters"></param>
        public PlanningTasksWithExecutionParameters(PlanningTask[] tasks, ExecutionParameters parameters)
        {
            PlanningTasks = tasks;
            Parameters = parameters;
        }
    }

    /// <summary>
    /// Execution parameters for the parallel calculation of planning tasks.
    /// </summary>
    public class ExecutionParameters
    {
        /// <summary>
        /// Maximum number of parallel tasks for the parallel calculation.
        /// </summary>
        public int MaxNumberOfParallelTasks { set; get; } = System.Environment.ProcessorCount - 1;
    };
}
