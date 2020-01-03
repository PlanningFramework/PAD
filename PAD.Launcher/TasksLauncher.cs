using System.Threading.Tasks;
using System;
using PAD.Launcher.Tasks;

namespace PAD.Launcher
{
    /// <summary>
    /// Static class launching parallel execution of specified planning tasks. These tasks can be manually created
    /// by the user, or specified via input arguments of the program (including launching config file).
    /// </summary>
    public static class TasksLauncher
    {
        /// <summary>
        /// Processes the input program arguments and executes the specified planning task(s).
        /// </summary>
        /// <param name="args">Program input arguments.</param>
        public static void ExecuteFromProgramArguments(string[] args)
        {
            PlanningTasksWithExecutionParameters executionDetails;
            try
            {
                executionDetails = TasksLoader.FromProgramArguments(args);
            }
            catch (TasksLoaderException e)
            {
                Console.WriteLine($"Error while loading input parameters: {e.Message}");
                return;
            }

            Execute(executionDetails.PlanningTasks, executionDetails.Parameters);
        }

        /// <summary>
        /// Executes the given planning tasks, with default execution parameters.
        /// </summary>
        /// <param name="tasks">Planning tasks to execute.</param>
        public static void Execute(params IPlanningTask[] tasks)
        {
            Execute(tasks, new ExecutionParameters());
        }

        /// <summary>
        /// Executes the given planning tasks, with the given execution parameters.
        /// </summary>
        /// <param name="tasks">Planning tasks to execute.</param>
        /// <param name="parameters">Execution parameters.</param>
        public static void Execute(IPlanningTask[] tasks, ExecutionParameters parameters)
        {
            foreach (var task in tasks)
            {
                PlanningTaskBase taskBase = task as PlanningTaskBase;
                if (taskBase != null)
                {
                    taskBase.OutputMutex = OutputWriteMutex;
                }
            }

            Action[] actions = Array.ConvertAll(tasks, task => new Action(task.Execute));

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = parameters.MaxNumberOfParallelTasks }, actions);
        }

        /// <summary>
        /// Mutex for the concurrent output writing of tasks results.
        /// </summary>
        private static readonly object OutputWriteMutex = new object();
    }
}
