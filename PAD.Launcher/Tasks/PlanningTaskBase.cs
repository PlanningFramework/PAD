using System;

namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Base class for planning tasks, implementing execution and results processing.
    /// </summary>
    public abstract class PlanningTaskBase : IPlanningTask
    {
        /// <summary>
        /// Output type for the planning task (if not specified, then results processor or writing to console is chosen).
        /// </summary>
        public OutputType OutputType { set; get; } = OutputType.Unspecified;

        /// <summary>
        /// Filepath to the result output file (if the output target is a file).
        /// </summary>
        public string OutputFile { set; get; } = "";

        /// <summary>
        /// Mutex for output writting. Needed if the output is writen to console or the same file.
        /// </summary>
        public object OutputMutex { set; get; } = new object();

        /// <summary>
        /// Result processing function, specifying what to do with the search results.
        /// </summary>
        public Action<Planner.Search.SearchResults> ResultsProcessor { set; get; } = null;

        /// <summary>
        /// Executes the planning task and writes the result.
        /// </summary>
        public void Execute()
        {
            Planner.Search.IHeuristicSearch searchEngine = GetHeuristicSearch();
            searchEngine.Start();

            Planner.Search.SearchResults results = searchEngine.GetSearchResults();
            ProcessResults(results);
        }

        /// <summary>
        /// Processes the results of search.
        /// </summary>
        /// <param name="results">Search results.</param>
        private void ProcessResults(Planner.Search.SearchResults results)
        {
            if (OutputType == OutputType.Unspecified)
            {
                OutputType = (ResultsProcessor != null) ? OutputType.CustomResultsProcessor : OutputType.ToConsole;
            }

            switch (OutputType)
            {
                case OutputType.CustomResultsProcessor:
                {
                    ResultsProcessor(results);
                    break;
                }
                case OutputType.ToConsole:
                {
                    string taskOutput = results.GetFullDescription();
                    lock (OutputMutex)
                    {
                        Console.WriteLine(taskOutput);
                    }
                    break;
                }
                case OutputType.ToFile:
                {
                    if (System.IO.File.Exists(OutputFile))
                    {
                        System.IO.File.Delete(OutputFile);
                    }

                    string taskOutput = results.GetFullDescription();
                    using (var writer = System.IO.File.CreateText(OutputFile))
                    {
                        writer.Write(taskOutput);
                    }
                    break;
                }
                default:
                {
                    throw new NotImplementedException("Unknown output type!");
                }
            }
        }

        /// <summary>
        /// Creates and returns the corresponding heuristic search engine.
        /// </summary>
        /// <returns>Corrsponding heuristic search engine.</returns>
        protected abstract Planner.Search.IHeuristicSearch GetHeuristicSearch();
    }
}
