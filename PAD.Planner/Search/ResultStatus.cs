
namespace PAD.Planner.Search
{
    /// <summary>
    /// Possible result status of the search task.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// The search has not been performed.
        /// </summary>
        Idle,

        /// <summary>
        /// The solution of the planning problem has been found.
        /// </summary>
        SolutionFound,

        /// <summary>
        /// The searching procedure finished and hasn't found any solution to the planning problem.
        /// </summary>
        NoSolutionFound,

        /// <summary>
        /// The searching procedure finished with a failure - time limit has been excceded.
        /// </summary>
        TimeLimitExceeded,

        /// <summary>
        /// The searching procedure finished with a failure - memory limit of searched nodes has been excceded.
        /// </summary>
        MemoryLimitExceeded,
    }
}
