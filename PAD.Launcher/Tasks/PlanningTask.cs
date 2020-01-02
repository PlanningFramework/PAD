using System;

namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Encapsulated structure of a single planning task to be executed, typically within a parallel execution. The planning problem,
    /// search algorithm, heuristic, heap, input files, and memory and time limits are all created/set from the specified parameters.
    /// </summary>
    public class PlanningTask : PlanningTaskBase
    {
        /// <summary>
        /// Planning task type (currently PDDL or SAS+).
        /// </summary>
        public Type Type { set; get; } = Type.SAS;

        /// <summary>
        /// Planning task search algorithm.
        /// </summary>
        public Algorithm Algorithm { set; get; } = Algorithm.AStarSearch;

        /// <summary>
        /// Planning task heuristic used.
        /// </summary>
        public Heuristic Heuristic { set; get; } = Heuristic.BlindHeuristic;

        /// <summary>
        /// Planning task heap used.
        /// </summary>
        public Heap Heap { set; get; } = Heap.RedBlackTreeHeap;

        /// <summary>
        /// Filepath to the domain input file (if the used representation has any).
        /// </summary>
        public string DomainFile { set; get; } = "";

        /// <summary>
        /// Filepath to the problem input file..
        /// </summary>
        public string ProblemFile { set; get; } = "";

        /// <summary>
        /// Time limit of the planning task in minutes.
        /// </summary>
        public int TimeLimit { set; get; } = Planner.Search.HeuristicSearch.DEFAULT_TIME_LIMIT_MINUTES;

        /// <summary>
        /// Memory limit (of searched nodes) for the planning task.
        /// </summary>
        public long MemoryLimit { set; get; } = Planner.Search.HeuristicSearch.DEFAULT_MEMORY_LIMIT_NODES;

        /// <summary>
        /// Creates and returns the corresponding heuristic search engine.
        /// </summary>
        /// <returns>Corrsponding heuristic search engine.</returns>
        protected override Planner.Search.IHeuristicSearch GetHeuristicSearch()
        {
            Planner.IProblem problem = GetProblem();
            Planner.Heuristics.IHeuristic heuristic = GetHeuristic(problem);
            Planner.Heaps.IHeap heap = GetHeap();
            TimeSpan timeLimit = TimeSpan.FromMinutes(TimeLimit);
            long memoryLimit = MemoryLimit;

            switch (Algorithm)
            {
                case Algorithm.AStarSearch:
                    return new Planner.Search.AStarSearch(problem, heuristic, heap, false, timeLimit, memoryLimit);
                case Algorithm.BeamSearch:
                    return new Planner.Search.BeamSearch(problem, heuristic, heap, 2, false, timeLimit, memoryLimit);
                case Algorithm.HillClimbingSearch:
                    return new Planner.Search.HillClimbingSearch(problem, heuristic, false, timeLimit, memoryLimit);
                default:
                    throw new NotImplementedException("Unknown search algorithm type!");
            }
        }

        /// <summary>
        /// Creates and returs the corresponding planning problem for the search engine.
        /// </summary>
        /// <returns>Corresponding planning problem for the search engine.</returns>
        private Planner.IProblem GetProblem()
        {
            switch (Type)
            {
                case Type.PDDL:
                    return new Planner.PDDL.Problem(DomainFile, ProblemFile, false);
                case Type.SAS:
                    return new Planner.SAS.Problem(ProblemFile, false);
                default:
                    throw new NotImplementedException("Unknown planning task type!");
            }
        }

        /// <summary>
        /// Creates and returs the corresponding heuristic for the search engine.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Corresponding heuristic for the search engine.</returns>
        private Planner.Heuristics.IHeuristic GetHeuristic(Planner.IProblem problem)
        {
            switch (Heuristic)
            {
                case Heuristic.BlindHeuristic:
                    return new Planner.Heuristics.BlindHeuristic();
                case Heuristic.StripsHeuristic:
                    return new Planner.Heuristics.StripsHeuristic(problem);
                case Heuristic.PerfectRelaxationHeuristic:
                    return new Planner.Heuristics.PerfectRelaxationHeuristic(problem);
                case Heuristic.AdditiveRelaxationHeuristic:
                    return new Planner.Heuristics.AdditiveRelaxationHeuristic(problem);
                case Heuristic.MaxRelaxationHeuristic:
                    return new Planner.Heuristics.MaxRelaxationHeuristic(problem);
                case Heuristic.FFHeuristic:
                    return new Planner.Heuristics.FFHeuristic(problem);
                case Heuristic.PDBHeuristic:
                    return new Planner.Heuristics.PDBHeuristic(problem);
                default:
                    throw new NotImplementedException("Unknown heuristic type!");
            }
        }

        /// <summary>
        /// Creates and returs the corresponding heap for the search engine.
        /// </summary>
        /// <returns>Corresponding planning problem for the search engine.</returns>
        private Planner.Heaps.IHeap GetHeap()
        {
            switch (Heap)
            {
                case Heap.BinomialHeap:
                    return new Planner.Heaps.BinomialHeap();
                case Heap.FibonacciHeap:
                    return new Planner.Heaps.FibonacciHeap();
                case Heap.FibonacciHeap2:
                    return new Planner.Heaps.FibonacciHeap2();
                case Heap.LeftistHeap:
                    return new Planner.Heaps.LeftistHeap();
                case Heap.RedBlackTreeHeap:
                    return new Planner.Heaps.RedBlackTreeHeap();
                case Heap.RegularBinaryHeap:
                    return new Planner.Heaps.RegularBinaryHeap();
                case Heap.RegularTernaryHeap:
                    return new Planner.Heaps.RegularTernaryHeap();
                default:
                    throw new NotImplementedException("Unknown heap type!");
            }
        }

        /// <summary>
        /// Creates a clone of the planning task.
        /// </summary>
        /// <returns>Clone of the planning task.</returns>
        public PlanningTask Clone()
        {
            return new PlanningTask()
            {
                Type = Type,
                Algorithm = Algorithm,
                Heuristic = Heuristic,
                Heap = Heap,
                DomainFile = DomainFile,
                ProblemFile = ProblemFile,
                OutputType = OutputType,
                OutputFile = OutputFile,
                OutputMutex = OutputMutex,
                ResultsProcessor = ResultsProcessor,
                TimeLimit = TimeLimit,
                MemoryLimit = MemoryLimit
            };
        }
    }
}
