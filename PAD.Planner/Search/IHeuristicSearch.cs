
namespace PAD.Planner.Search
{
    /// <summary>
    /// Common interface for a search procedure with heuristics.
    /// </summary>
    public interface IHeuristicSearch
    {
        /// <summary>
        /// Starts the search procedure.
        /// </summary>
        /// <param name="type">Type of search.</param>
        /// <returns>Result of the search.</returns>
        ResultStatus Start(SearchType type = SearchType.Forward);

        /// <summary>
        /// Gets the full search results of the planning search.
        /// </summary>
        /// <param name="includingSolutionPlan">Include the solution plan?</param>
        /// <returns>Search results.</returns>
        SearchResults GetSearchResults(bool includingSolutionPlan = true);

        /// <summary>
        /// Gets the solution plan (i.e. a concrete sequence of applied operators).
        /// </summary>
        /// <returns>Solution plan.</returns>
        ISolutionPlan GetSolutionPlan();

        /// <summary>
        /// Gets the cost of the found solution plan.
        /// </summary>
        /// <returns>Solution plan cost.</returns>
        double GetSolutionCost();

        /// <summary>
        /// Gets the search procedure description.
        /// </summary>
        /// <returns>Search procedure description.</returns>
        string GetName();
    }
}
