using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData;
using PAD.Planner;
using PAD.Planner.Heaps;
using PAD.Planner.Search;
using PAD.Planner.Heuristics;
using System.Linq;
using System;

namespace PAD.Tests
{
    /// <summary>
    /// Testing suite for the planner. Testing components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerSearchUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private static string GetFilePath(string fileName)
        {
            return $@"..\..\PlannerTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_AStarSearch()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var sasSearch = new AStarSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap());
            sasSearch.Start();
            var sasSolution = sasSearch.GetSearchResults();
            Assert.AreEqual("A* Search", sasSolution.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution.ResultStatus);
            Assert.IsTrue(sasSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution.SolutionCost);
            Assert.IsNotNull(sasSolution.SolutionPlan);
            Assert.AreEqual(11, sasSolution.SolutionPlan.GetCost());

            var sasSearch2 = new AStarSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap());
            sasSearch2.Start(SearchType.BackwardWithConditions);
            var sasSolution2 = sasSearch2.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", sasSolution2.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution2.ResultStatus);
            Assert.IsTrue(sasSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution2.SolutionCost);
            Assert.IsNotNull(sasSolution2.SolutionPlan);
            Assert.AreEqual(11, sasSolution2.SolutionPlan.GetCost());

            var sasSearch3 = new AStarSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap());
            sasSearch3.Start(SearchType.BackwardWithStates);
            var sasSolution3 = sasSearch3.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", sasSolution3.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution3.ResultStatus);
            Assert.IsTrue(sasSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution3.SolutionCost);
            Assert.IsNotNull(sasSolution3.SolutionPlan);
            Assert.AreEqual(11, sasSolution3.SolutionPlan.GetCost());

            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var pddlSearch = new AStarSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap());
            pddlSearch.Start();
            var pddlSolution = pddlSearch.GetSearchResults();
            Assert.AreEqual("A* Search", pddlSolution.Algorithm);
            Assert.AreEqual("gripper", pddlSolution.DomainName);
            Assert.AreEqual("problem-1", pddlSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution.ResultStatus);
            Assert.IsTrue(pddlSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution.SolutionCost);
            Assert.IsNotNull(pddlSolution.SolutionPlan);
            Assert.AreEqual(5, pddlSolution.SolutionPlan.GetCost());

            var pddlSearch2 = new AStarSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap());
            pddlSearch2.Start(SearchType.BackwardWithConditions);
            var pddlSolution2 = pddlSearch2.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", pddlSolution2.Algorithm);
            Assert.AreEqual("gripper", pddlSolution2.DomainName);
            Assert.AreEqual("problem-1", pddlSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution2.ResultStatus);
            Assert.IsTrue(pddlSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution2.SolutionCost);
            Assert.IsNotNull(pddlSolution2.SolutionPlan);
            Assert.AreEqual(5, pddlSolution2.SolutionPlan.GetCost());

            var pddlSearch3 = new AStarSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap());
            pddlSearch3.Start(SearchType.BackwardWithStates);
            var pddlSolution3 = pddlSearch3.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", pddlSolution3.Algorithm);
            Assert.AreEqual("gripper", pddlSolution3.DomainName);
            Assert.AreEqual("problem-1", pddlSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution3.ResultStatus);
            Assert.IsTrue(pddlSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution3.SolutionCost);
            Assert.IsNotNull(pddlSolution3.SolutionPlan);
            Assert.AreEqual(5, pddlSolution3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_BeamSearch()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var sasSearch = new BeamSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            sasSearch.Start();
            var sasSolution = sasSearch.GetSearchResults();
            Assert.AreEqual("Beam Search", sasSolution.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution.ResultStatus);
            Assert.IsTrue(sasSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution.SolutionCost);
            Assert.IsNotNull(sasSolution.SolutionPlan);
            Assert.AreEqual(11, sasSolution.SolutionPlan.GetCost());

            var sasSearch2 = new BeamSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            sasSearch2.Start(SearchType.BackwardWithConditions);
            var sasSolution2 = sasSearch2.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", sasSolution2.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution2.ResultStatus);
            Assert.IsTrue(sasSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution2.SolutionCost);
            Assert.IsNotNull(sasSolution2.SolutionPlan);
            Assert.AreEqual(11, sasSolution2.SolutionPlan.GetCost());

            var sasSearch3 = new BeamSearch(sasProblem, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            sasSearch3.Start(SearchType.BackwardWithStates);
            var sasSolution3 = sasSearch3.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", sasSolution3.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution3.ResultStatus);
            Assert.IsTrue(sasSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution3.SolutionCost);
            Assert.IsNotNull(sasSolution3.SolutionPlan);
            Assert.AreEqual(11, sasSolution3.SolutionPlan.GetCost());

            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var pddlSearch = new BeamSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            pddlSearch.Start();
            var pddlSolution = pddlSearch.GetSearchResults();
            Assert.AreEqual("Beam Search", pddlSolution.Algorithm);
            Assert.AreEqual("gripper", pddlSolution.DomainName);
            Assert.AreEqual("problem-1", pddlSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution.ResultStatus);
            Assert.IsTrue(pddlSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution.SolutionCost);
            Assert.IsNotNull(pddlSolution.SolutionPlan);
            Assert.AreEqual(5, pddlSolution.SolutionPlan.GetCost());

            var pddlSearch2 = new BeamSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            pddlSearch2.Start(SearchType.BackwardWithConditions);
            var pddlSolution2 = pddlSearch2.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", pddlSolution2.Algorithm);
            Assert.AreEqual("gripper", pddlSolution2.DomainName);
            Assert.AreEqual("problem-1", pddlSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution2.ResultStatus);
            Assert.IsTrue(pddlSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution2.SolutionCost);
            Assert.IsNotNull(pddlSolution2.SolutionPlan);
            Assert.AreEqual(5, pddlSolution2.SolutionPlan.GetCost());

            var pddlSearch3 = new BeamSearch(pddlProblem, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            pddlSearch3.Start(SearchType.BackwardWithStates);
            var pddlSolution3 = pddlSearch3.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", pddlSolution3.Algorithm);
            Assert.AreEqual("gripper", pddlSolution3.DomainName);
            Assert.AreEqual("problem-1", pddlSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution3.ResultStatus);
            Assert.IsTrue(pddlSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution3.SolutionCost);
            Assert.IsNotNull(pddlSolution3.SolutionPlan);
            Assert.AreEqual(5, pddlSolution3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_HillClimbingSearch()
        {
            // The search path is basically random, no point to test the solution here

            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var sasSearch = new HillClimbingSearch(sasProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            sasSearch.Start();
            var sasSolution = sasSearch.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search", sasSolution.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution.Heuristic);
            Assert.IsTrue(sasSolution.SearchTime.TotalMilliseconds >= 0);

            var sasSearch2 = new HillClimbingSearch(sasProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            sasSearch2.Start(SearchType.BackwardWithConditions);
            var sasSolution2 = sasSearch2.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", sasSolution2.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution2.Heuristic);
            Assert.IsTrue(sasSolution2.SearchTime.TotalMilliseconds >= 0);

            var sasSearch3 = new HillClimbingSearch(sasProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            sasSearch3.Start(SearchType.BackwardWithStates);
            var sasSolution3 = sasSearch3.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", sasSolution3.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution3.Heuristic);
            Assert.IsTrue(sasSolution3.SearchTime.TotalMilliseconds >= 0);

            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var pddlSearch = new HillClimbingSearch(pddlProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            pddlSearch.Start();
            var pddlSolution = pddlSearch.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search", pddlSolution.Algorithm);
            Assert.AreEqual("gripper", pddlSolution.DomainName);
            Assert.AreEqual("problem-1", pddlSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution.Heuristic);
            Assert.IsTrue(pddlSolution.SearchTime.TotalMilliseconds >= 0);

            var pddlSearch2 = new HillClimbingSearch(pddlProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            pddlSearch2.Start(SearchType.BackwardWithConditions);
            var pddlSolution2 = pddlSearch2.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", pddlSolution2.Algorithm);
            Assert.AreEqual("gripper", pddlSolution2.DomainName);
            Assert.AreEqual("problem-1", pddlSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution2.Heuristic);
            Assert.IsTrue(pddlSolution2.SearchTime.TotalMilliseconds >= 0);

            var pddlSearch3 = new HillClimbingSearch(pddlProblem, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            pddlSearch3.Start(SearchType.BackwardWithStates);
            var pddlSolution3 = pddlSearch3.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", pddlSolution3.Algorithm);
            Assert.AreEqual("gripper", pddlSolution3.DomainName);
            Assert.AreEqual("problem-1", pddlSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution3.Heuristic);
            Assert.IsTrue(pddlSolution3.SearchTime.TotalMilliseconds >= 0);
        }

        [TestMethod]
        public void TC_IterativeDeepeningAStarSearch()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var sasSearch = new IterativeDeepeningAStarSearch(sasProblem, new FFHeuristic(sasProblem));
            sasSearch.Start();
            var sasSolution = sasSearch.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search", sasSolution.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution.ProblemName);
            Assert.AreEqual("FF Heuristic", sasSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution.ResultStatus);
            Assert.IsTrue(sasSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution.SolutionCost);
            Assert.IsNotNull(sasSolution.SolutionPlan);
            Assert.AreEqual(11, sasSolution.SolutionPlan.GetCost());

            var sasSearch2 = new IterativeDeepeningAStarSearch(sasProblem, new FFHeuristic(sasProblem));
            sasSearch2.Start(SearchType.BackwardWithConditions);
            var sasSolution2 = sasSearch2.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", sasSolution2.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution2.ProblemName);
            Assert.AreEqual("FF Heuristic", sasSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution2.ResultStatus);
            Assert.IsTrue(sasSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution2.SolutionCost);
            Assert.IsNotNull(sasSolution2.SolutionPlan);
            Assert.AreEqual(11, sasSolution2.SolutionPlan.GetCost());

            var sasSearch3 = new IterativeDeepeningAStarSearch(sasProblem, new FFHeuristic(sasProblem));
            sasSearch3.Start(SearchType.BackwardWithStates);
            var sasSolution3 = sasSearch3.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", sasSolution3.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution3.ProblemName);
            Assert.AreEqual("FF Heuristic", sasSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution3.ResultStatus);
            Assert.IsTrue(sasSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution3.SolutionCost);
            Assert.IsNotNull(sasSolution3.SolutionPlan);
            Assert.AreEqual(11, sasSolution3.SolutionPlan.GetCost());

            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var pddlSearch = new IterativeDeepeningAStarSearch(pddlProblem, new AdditiveRelaxationHeuristic(pddlProblem));
            pddlSearch.Start();
            var pddlSolution = pddlSearch.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search", pddlSolution.Algorithm);
            Assert.AreEqual("gripper", pddlSolution.DomainName);
            Assert.AreEqual("problem-1", pddlSolution.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", pddlSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution.ResultStatus);
            Assert.IsTrue(pddlSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution.SolutionCost);
            Assert.IsNotNull(pddlSolution.SolutionPlan);
            Assert.AreEqual(5, pddlSolution.SolutionPlan.GetCost());

            var pddlSearch2 = new IterativeDeepeningAStarSearch(pddlProblem, new AdditiveRelaxationHeuristic(pddlProblem));
            pddlSearch2.Start(SearchType.BackwardWithConditions);
            var pddlSolution2 = pddlSearch2.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", pddlSolution2.Algorithm);
            Assert.AreEqual("gripper", pddlSolution2.DomainName);
            Assert.AreEqual("problem-1", pddlSolution2.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", pddlSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution2.ResultStatus);
            Assert.IsTrue(pddlSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution2.SolutionCost);
            Assert.IsNotNull(pddlSolution2.SolutionPlan);
            Assert.AreEqual(5, pddlSolution2.SolutionPlan.GetCost());

            var pddlSearch3 = new IterativeDeepeningAStarSearch(pddlProblem, new AdditiveRelaxationHeuristic(pddlProblem));
            pddlSearch3.Start(SearchType.BackwardWithStates);
            var pddlSolution3 = pddlSearch3.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", pddlSolution3.Algorithm);
            Assert.AreEqual("gripper", pddlSolution3.DomainName);
            Assert.AreEqual("problem-1", pddlSolution3.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", pddlSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution3.ResultStatus);
            Assert.IsTrue(pddlSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution3.SolutionCost);
            Assert.IsNotNull(pddlSolution3.SolutionPlan);
            Assert.AreEqual(5, pddlSolution3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_MultiHeuristicAStarSearch()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var sasSearch = new MultiHeuristicAStarSearch(sasProblem, new BlindHeuristic());
            sasSearch.Start();
            var sasSolution = sasSearch.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search", sasSolution.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution.ResultStatus);
            Assert.IsTrue(sasSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution.SolutionCost);
            Assert.IsNotNull(sasSolution.SolutionPlan);
            Assert.AreEqual(11, sasSolution.SolutionPlan.GetCost());

            var sasSearch2 = new MultiHeuristicAStarSearch(sasProblem, new BlindHeuristic());
            sasSearch2.Start(SearchType.BackwardWithConditions);
            var sasSolution2 = sasSearch2.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", sasSolution2.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution2.ResultStatus);
            Assert.IsTrue(sasSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution2.SolutionCost);
            Assert.IsNotNull(sasSolution2.SolutionPlan);
            Assert.AreEqual(11, sasSolution2.SolutionPlan.GetCost());

            var sasSearch3 = new MultiHeuristicAStarSearch(sasProblem, new BlindHeuristic());
            sasSearch3.Start(SearchType.BackwardWithStates);
            var sasSolution3 = sasSearch3.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", sasSolution3.Algorithm);
            Assert.AreEqual("TC_Gripper", sasSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", sasSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, sasSolution3.ResultStatus);
            Assert.IsTrue(sasSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, sasSolution3.SolutionCost);
            Assert.IsNotNull(sasSolution3.SolutionPlan);
            Assert.AreEqual(11, sasSolution3.SolutionPlan.GetCost());

            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var pddlSearch = new MultiHeuristicAStarSearch(pddlProblem, new BlindHeuristic());
            pddlSearch.Start();
            var pddlSolution = pddlSearch.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search", pddlSolution.Algorithm);
            Assert.AreEqual("gripper", pddlSolution.DomainName);
            Assert.AreEqual("problem-1", pddlSolution.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution.ResultStatus);
            Assert.IsTrue(pddlSolution.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution.SolutionCost);
            Assert.IsNotNull(pddlSolution.SolutionPlan);
            Assert.AreEqual(5, pddlSolution.SolutionPlan.GetCost());

            var pddlSearch2 = new MultiHeuristicAStarSearch(pddlProblem, new BlindHeuristic());
            pddlSearch2.Start(SearchType.BackwardWithConditions);
            var pddlSolution2 = pddlSearch2.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", pddlSolution2.Algorithm);
            Assert.AreEqual("gripper", pddlSolution2.DomainName);
            Assert.AreEqual("problem-1", pddlSolution2.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution2.ResultStatus);
            Assert.IsTrue(pddlSolution2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution2.SolutionCost);
            Assert.IsNotNull(pddlSolution2.SolutionPlan);
            Assert.AreEqual(5, pddlSolution2.SolutionPlan.GetCost());

            var pddlSearch3 = new MultiHeuristicAStarSearch(pddlProblem, new BlindHeuristic());
            pddlSearch3.Start(SearchType.BackwardWithStates);
            var pddlSolution3 = pddlSearch3.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", pddlSolution3.Algorithm);
            Assert.AreEqual("gripper", pddlSolution3.DomainName);
            Assert.AreEqual("problem-1", pddlSolution3.ProblemName);
            Assert.AreEqual("Blind Heuristic", pddlSolution3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, pddlSolution3.ResultStatus);
            Assert.IsTrue(pddlSolution3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, pddlSolution3.SolutionCost);
            Assert.IsNotNull(pddlSolution3.SolutionPlan);
            Assert.AreEqual(5, pddlSolution3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_IStateOrConditions()
        {
            var problem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var heuristic = new FFHeuristic(problem);

            IStateOrConditions state = problem.InitialState;
            IStateOrConditions state2 = new Planner.SAS.State(1, 1, 1, 1, 0, 4, 4);

            Assert.IsTrue(CollectionsEquality.Equals(problem.GetSuccessors((IState)state), state.DetermineTransitions(problem)));
            Assert.IsFalse(state.DetermineGoalNode(problem));
            Assert.IsTrue(state2.DetermineGoalNode(problem));
            Assert.AreEqual(heuristic.GetValue((IState)state), state.DetermineHeuristicValue(heuristic));

            IStateOrConditions conditions = problem.GoalConditions;
            IStateOrConditions conditions2 = new Planner.SAS.Conditions(new Planner.SAS.Assignment(5, 4), new Planner.SAS.Assignment(6, 4));

            Assert.IsTrue(CollectionsEquality.Equals(problem.GetPredecessors((IConditions)conditions), conditions.DetermineTransitions(problem)));
            Assert.IsFalse(conditions.DetermineGoalNode(problem));
            Assert.IsTrue(conditions2.DetermineGoalNode(problem));
            Assert.AreEqual(heuristic.GetValue((IConditions)conditions), conditions.DetermineHeuristicValue(heuristic));

            IStateOrConditions relativeState = problem.GoalConditions.GetCorrespondingRelativeStates(problem).First();
            IStateOrConditions relativeState2 = new Planner.SAS.RelativeState(-1, -1, -1, -1, -1, 4, 4);

            Assert.IsTrue(CollectionsEquality.Equals(problem.GetPredecessors((IRelativeState)relativeState), relativeState.DetermineTransitions(problem)));
            Assert.IsFalse(relativeState.DetermineGoalNode(problem));
            Assert.IsTrue(relativeState2.DetermineGoalNode(problem));
            Assert.AreEqual(heuristic.GetValue((IRelativeState)relativeState), relativeState.DetermineHeuristicValue(heuristic));
        }

        [TestMethod]
        public void TC_SearchResults()
        {
            var problem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_SearchResults.sas")));
            var heuristic = new PDBHeuristic(problem);
            var problem2 = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_SearchResults2.sas")));
            var heuristic2 = new PDBHeuristic(problem2);
            var heap = new RegularBinaryHeap();

            var search1 = new AStarSearch(problem, heuristic, heap);
            var solution1 = search1.Start();
            Assert.AreEqual(ResultStatus.SolutionFound, solution1);
            Assert.IsNotNull(search1.GetSolutionPlan());

            var search2 = new AStarSearch(problem2, heuristic2, heap);
            var solution2 = search2.Start();
            Assert.AreEqual(ResultStatus.NoSolutionFound, solution2);
            Assert.IsNull(search2.GetSolutionPlan());

            var search3 = new AStarSearch(problem, heuristic, heap, false, new TimeSpan(0), 5000);
            var solution3 = search3.Start();
            Assert.AreEqual(ResultStatus.TimeLimitExceeded, solution3);
            Assert.IsNull(search3.GetSolutionPlan());

            var search4 = new AStarSearch(problem, heuristic, heap, false, new TimeSpan(0, 1, 0), 0);
            var solution4 = search4.Start();
            Assert.AreEqual(ResultStatus.MemoryLimitExceeded, solution4);
            Assert.IsNull(search4.GetSolutionPlan());
        }

        [TestMethod]
        public void TC_Transitions()
        {
            var problem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));

            ISuccessor successor = problem.GetSuccessors(problem.InitialState).First();
            Assert.IsFalse(successor.IsComplexTransition());
            Assert.IsTrue(successor.GetSuccessorState().Equals(successor.GetTransitionResult()));
            var successorResults = successor.GetComplexTransitionResults().ToList();
            Assert.AreEqual(1, successorResults.Count);
            Assert.IsTrue(successorResults.First().Equals(successor.GetTransitionResult()));

            IPredecessor predecessor = problem.GetPredecessors(problem.GoalConditions).First();
            Assert.IsFalse(predecessor.IsComplexTransition());
            Assert.IsTrue(predecessor.GetPredecessorConditions().Equals(predecessor.GetTransitionResult()));
            var predecessorResults = predecessor.GetComplexTransitionResults().ToList();
            Assert.AreEqual(1, predecessorResults.Count);
            Assert.IsTrue(predecessorResults.First().Equals(predecessor.GetTransitionResult()));

            IPredecessor predecessor2 = problem.GetPredecessors(problem.GoalConditions.GetCorrespondingRelativeStates(problem).First()).First();
            Assert.IsTrue(predecessor2.IsComplexTransition());
            Assert.IsTrue(CollectionsEquality.Equals(predecessor2.GetPredecessorRelativeStates(), predecessor2.GetComplexTransitionResults()));
            var predecessorResults2 = predecessor2.GetComplexTransitionResults().ToList();
            Assert.AreEqual(1, predecessorResults2.Count);
            Assert.IsTrue(predecessorResults2.First().Equals(predecessor2.GetTransitionResult()));
        }
    }
}
