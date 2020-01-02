
namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Variant of PlanningTask class allowing a direct specification of heuristic search engine (with corresponding problem, heuristic etc.).
    /// </summary>
    public class PlanningTaskExplicit : PlanningTaskBase
    {
        /// <summary>
        /// Explicitly created heuristic search, used for the planning task execution.
        /// </summary>
        public Planner.Search.IHeuristicSearch HeuristicSearch { set; get; } = null;

        /// <summary>
        /// Creates and returns the corresponding heuristic search engine.
        /// </summary>
        /// <returns>Corrsponding heuristic search engine.</returns>
        protected override Planner.Search.IHeuristicSearch GetHeuristicSearch()
        {
            return HeuristicSearch;
        }

        /// <summary>
        /// Creates a clone of the planning task.
        /// </summary>
        /// <returns>Clone of the planning task.</returns>
        public PlanningTaskExplicit Clone()
        {
            return new PlanningTaskExplicit()
            {
                HeuristicSearch = HeuristicSearch,
                OutputType = OutputType,
                OutputFile = OutputFile,
                OutputMutex = OutputMutex,
                ResultsProcessor = ResultsProcessor
            };
        }
    }
}
