using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData;
using PAD.Planner;
using PAD.Planner.Heaps;
using PAD.Planner.Search;
using PAD.Planner.Heuristics;
using System.Linq;
using System;
using System.Collections.Generic;

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
        private string GetFilePath(string fileName)
        {
            return $@"..\..\Planner\TestCases\{fileName}";
        }

        [TestMethod]
        public void TC_AStarSearch()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var searchSAS = new AStarSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap());
            searchSAS.Start();
            var solutionSAS = searchSAS.GetSearchResults();
            Assert.AreEqual("A* Search", solutionSAS.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS.ResultStatus);
            Assert.IsTrue(solutionSAS.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS.SolutionCost);
            Assert.IsNotNull(solutionSAS.SolutionPlan);
            Assert.AreEqual(11, solutionSAS.SolutionPlan.GetCost());

            var searchSAS2 = new AStarSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap());
            searchSAS2.Start(SearchType.BackwardWithConditions);
            var solutionSAS2 = searchSAS2.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", solutionSAS2.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS2.ResultStatus);
            Assert.IsTrue(solutionSAS2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS2.SolutionCost);
            Assert.IsNotNull(solutionSAS2.SolutionPlan);
            Assert.AreEqual(11, solutionSAS2.SolutionPlan.GetCost());

            var searchSAS3 = new AStarSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap());
            searchSAS3.Start(SearchType.BackwardWithStates);
            var solutionSAS3 = searchSAS3.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", solutionSAS3.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS3.ResultStatus);
            Assert.IsTrue(solutionSAS3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS3.SolutionCost);
            Assert.IsNotNull(solutionSAS3.SolutionPlan);
            Assert.AreEqual(11, solutionSAS3.SolutionPlan.GetCost());

            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var searchPDDL = new AStarSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap());
            searchPDDL.Start();
            var solutionPDDL = searchPDDL.GetSearchResults();
            Assert.AreEqual("A* Search", solutionPDDL.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL.ResultStatus);
            Assert.IsTrue(solutionPDDL.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL.SolutionCost);
            Assert.IsNotNull(solutionPDDL.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL.SolutionPlan.GetCost());

            var searchPDDL2 = new AStarSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap());
            searchPDDL2.Start(SearchType.BackwardWithConditions);
            var solutionPDDL2 = searchPDDL2.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", solutionPDDL2.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL2.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL2.ResultStatus);
            Assert.IsTrue(solutionPDDL2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL2.SolutionCost);
            Assert.IsNotNull(solutionPDDL2.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL2.SolutionPlan.GetCost());

            var searchPDDL3 = new AStarSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap());
            searchPDDL3.Start(SearchType.BackwardWithStates);
            var solutionPDDL3 = searchPDDL3.GetSearchResults();
            Assert.AreEqual("A* Search (Backward)", solutionPDDL3.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL3.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL3.ResultStatus);
            Assert.IsTrue(solutionPDDL3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL3.SolutionCost);
            Assert.IsNotNull(solutionPDDL3.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_BeamSearch()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var searchSAS = new BeamSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            searchSAS.Start();
            var solutionSAS = searchSAS.GetSearchResults();
            Assert.AreEqual("Beam Search", solutionSAS.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS.ResultStatus);
            Assert.IsTrue(solutionSAS.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS.SolutionCost);
            Assert.IsNotNull(solutionSAS.SolutionPlan);
            Assert.AreEqual(11, solutionSAS.SolutionPlan.GetCost());

            var searchSAS2 = new BeamSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            searchSAS2.Start(SearchType.BackwardWithConditions);
            var solutionSAS2 = searchSAS2.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", solutionSAS2.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS2.ResultStatus);
            Assert.IsTrue(solutionSAS2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS2.SolutionCost);
            Assert.IsNotNull(solutionSAS2.SolutionPlan);
            Assert.AreEqual(11, solutionSAS2.SolutionPlan.GetCost());

            var searchSAS3 = new BeamSearch(problemSAS, new BlindHeuristic(), new RegularBinaryHeap(), 10);
            searchSAS3.Start(SearchType.BackwardWithStates);
            var solutionSAS3 = searchSAS3.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", solutionSAS3.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS3.ResultStatus);
            Assert.IsTrue(solutionSAS3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS3.SolutionCost);
            Assert.IsNotNull(solutionSAS3.SolutionPlan);
            Assert.AreEqual(11, solutionSAS3.SolutionPlan.GetCost());

            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var searchPDDL = new BeamSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            searchPDDL.Start();
            var solutionPDDL = searchPDDL.GetSearchResults();
            Assert.AreEqual("Beam Search", solutionPDDL.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL.ResultStatus);
            Assert.IsTrue(solutionPDDL.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL.SolutionCost);
            Assert.IsNotNull(solutionPDDL.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL.SolutionPlan.GetCost());

            var searchPDDL2 = new BeamSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            searchPDDL2.Start(SearchType.BackwardWithConditions);
            var solutionPDDL2 = searchPDDL2.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", solutionPDDL2.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL2.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL2.ResultStatus);
            Assert.IsTrue(solutionPDDL2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL2.SolutionCost);
            Assert.IsNotNull(solutionPDDL2.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL2.SolutionPlan.GetCost());

            var searchPDDL3 = new BeamSearch(problemPDDL, new BlindHeuristic(), new RegularBinaryHeap(), 20);
            searchPDDL3.Start(SearchType.BackwardWithStates);
            var solutionPDDL3 = searchPDDL3.GetSearchResults();
            Assert.AreEqual("Beam Search (Backward)", solutionPDDL3.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL3.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL3.ResultStatus);
            Assert.IsTrue(solutionPDDL3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL3.SolutionCost);
            Assert.IsNotNull(solutionPDDL3.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_HillClimbingSearch()
        {
            // The search path is basically random, no point to test the solution here

            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var searchSAS = new HillClimbingSearch(problemSAS, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchSAS.Start();
            var solutionSAS = searchSAS.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search", solutionSAS.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS.Heuristic);
            Assert.IsTrue(solutionSAS.SearchTime.TotalMilliseconds >= 0);

            var searchSAS2 = new HillClimbingSearch(problemSAS, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchSAS2.Start(SearchType.BackwardWithConditions);
            var solutionSAS2 = searchSAS2.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", solutionSAS2.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS2.Heuristic);
            Assert.IsTrue(solutionSAS2.SearchTime.TotalMilliseconds >= 0);

            var searchSAS3 = new HillClimbingSearch(problemSAS, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchSAS3.Start(SearchType.BackwardWithStates);
            var solutionSAS3 = searchSAS3.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", solutionSAS3.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS3.Heuristic);
            Assert.IsTrue(solutionSAS3.SearchTime.TotalMilliseconds >= 0);

            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var searchPDDL = new HillClimbingSearch(problemPDDL, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchPDDL.Start();
            var solutionPDDL = searchPDDL.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search", solutionPDDL.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL.Heuristic);
            Assert.IsTrue(solutionPDDL.SearchTime.TotalMilliseconds >= 0);

            var searchPDDL2 = new HillClimbingSearch(problemPDDL, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchPDDL2.Start(SearchType.BackwardWithConditions);
            var solutionPDDL2 = searchPDDL2.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", solutionPDDL2.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL2.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL2.Heuristic);
            Assert.IsTrue(solutionPDDL2.SearchTime.TotalMilliseconds >= 0);

            var searchPDDL3 = new HillClimbingSearch(problemPDDL, new BlindHeuristic(), false, new TimeSpan(0, 0, 1), 50000);
            searchPDDL3.Start(SearchType.BackwardWithStates);
            var solutionPDDL3 = searchPDDL3.GetSearchResults();
            Assert.AreEqual("Hill-Climbing Search (Backward)", solutionPDDL3.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL3.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL3.Heuristic);
            Assert.IsTrue(solutionPDDL3.SearchTime.TotalMilliseconds >= 0);
        }

        [TestMethod]
        public void TC_IterativeDeepeningAStarSearch()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var searchSAS = new IterativeDeepeningAStarSearch(problemSAS, new FFHeuristic(problemSAS));
            searchSAS.Start();
            var solutionSAS = searchSAS.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search", solutionSAS.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS.ProblemName);
            Assert.AreEqual("FF Heuristic", solutionSAS.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS.ResultStatus);
            Assert.IsTrue(solutionSAS.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS.SolutionCost);
            Assert.IsNotNull(solutionSAS.SolutionPlan);
            Assert.AreEqual(11, solutionSAS.SolutionPlan.GetCost());

            var searchSAS2 = new IterativeDeepeningAStarSearch(problemSAS, new FFHeuristic(problemSAS));
            searchSAS2.Start(SearchType.BackwardWithConditions);
            var solutionSAS2 = searchSAS2.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", solutionSAS2.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS2.ProblemName);
            Assert.AreEqual("FF Heuristic", solutionSAS2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS2.ResultStatus);
            Assert.IsTrue(solutionSAS2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS2.SolutionCost);
            Assert.IsNotNull(solutionSAS2.SolutionPlan);
            Assert.AreEqual(11, solutionSAS2.SolutionPlan.GetCost());

            var searchSAS3 = new IterativeDeepeningAStarSearch(problemSAS, new FFHeuristic(problemSAS));
            searchSAS3.Start(SearchType.BackwardWithStates);
            var solutionSAS3 = searchSAS3.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", solutionSAS3.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS3.ProblemName);
            Assert.AreEqual("FF Heuristic", solutionSAS3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS3.ResultStatus);
            Assert.IsTrue(solutionSAS3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS3.SolutionCost);
            Assert.IsNotNull(solutionSAS3.SolutionPlan);
            Assert.AreEqual(11, solutionSAS3.SolutionPlan.GetCost());

            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var searchPDDL = new IterativeDeepeningAStarSearch(problemPDDL, new AdditiveRelaxationHeuristic(problemPDDL));
            searchPDDL.Start();
            var solutionPDDL = searchPDDL.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search", solutionPDDL.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", solutionPDDL.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL.ResultStatus);
            Assert.IsTrue(solutionPDDL.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL.SolutionCost);
            Assert.IsNotNull(solutionPDDL.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL.SolutionPlan.GetCost());

            var searchPDDL2 = new IterativeDeepeningAStarSearch(problemPDDL, new AdditiveRelaxationHeuristic(problemPDDL));
            searchPDDL2.Start(SearchType.BackwardWithConditions);
            var solutionPDDL2 = searchPDDL2.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", solutionPDDL2.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL2.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL2.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", solutionPDDL2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL2.ResultStatus);
            Assert.IsTrue(solutionPDDL2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL2.SolutionCost);
            Assert.IsNotNull(solutionPDDL2.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL2.SolutionPlan.GetCost());

            var searchPDDL3 = new IterativeDeepeningAStarSearch(problemPDDL, new AdditiveRelaxationHeuristic(problemPDDL));
            searchPDDL3.Start(SearchType.BackwardWithStates);
            var solutionPDDL3 = searchPDDL3.GetSearchResults();
            Assert.AreEqual("Iterative Deepening A* Search (Backward)", solutionPDDL3.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL3.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL3.ProblemName);
            Assert.AreEqual("Additive Relaxation Heuristic", solutionPDDL3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL3.ResultStatus);
            Assert.IsTrue(solutionPDDL3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL3.SolutionCost);
            Assert.IsNotNull(solutionPDDL3.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL3.SolutionPlan.GetCost());
        }

        [TestMethod]
        public void TC_MultiHeuristicAStarSearch()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var searchSAS = new MultiHeuristicAStarSearch(problemSAS, new BlindHeuristic());
            searchSAS.Start();
            var solutionSAS = searchSAS.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search", solutionSAS.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS.ResultStatus);
            Assert.IsTrue(solutionSAS.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS.SolutionCost);
            Assert.IsNotNull(solutionSAS.SolutionPlan);
            Assert.AreEqual(11, solutionSAS.SolutionPlan.GetCost());

            var searchSAS2 = new MultiHeuristicAStarSearch(problemSAS, new BlindHeuristic());
            searchSAS2.Start(SearchType.BackwardWithConditions);
            var solutionSAS2 = searchSAS2.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", solutionSAS2.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS2.ResultStatus);
            Assert.IsTrue(solutionSAS2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS2.SolutionCost);
            Assert.IsNotNull(solutionSAS2.SolutionPlan);
            Assert.AreEqual(11, solutionSAS2.SolutionPlan.GetCost());

            var searchSAS3 = new MultiHeuristicAStarSearch(problemSAS, new BlindHeuristic());
            searchSAS3.Start(SearchType.BackwardWithStates);
            var solutionSAS3 = searchSAS3.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", solutionSAS3.Algorithm);
            Assert.AreEqual("TC_Gripper", solutionSAS3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionSAS3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionSAS3.ResultStatus);
            Assert.IsTrue(solutionSAS3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(11, solutionSAS3.SolutionCost);
            Assert.IsNotNull(solutionSAS3.SolutionPlan);
            Assert.AreEqual(11, solutionSAS3.SolutionPlan.GetCost());

            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));
            var searchPDDL = new MultiHeuristicAStarSearch(problemPDDL, new BlindHeuristic());
            searchPDDL.Start();
            var solutionPDDL = searchPDDL.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search", solutionPDDL.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL.ResultStatus);
            Assert.IsTrue(solutionPDDL.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL.SolutionCost);
            Assert.IsNotNull(solutionPDDL.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL.SolutionPlan.GetCost());

            var searchPDDL2 = new MultiHeuristicAStarSearch(problemPDDL, new BlindHeuristic());
            searchPDDL2.Start(SearchType.BackwardWithConditions);
            var solutionPDDL2 = searchPDDL2.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", solutionPDDL2.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL2.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL2.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL2.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL2.ResultStatus);
            Assert.IsTrue(solutionPDDL2.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL2.SolutionCost);
            Assert.IsNotNull(solutionPDDL2.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL2.SolutionPlan.GetCost());

            var searchPDDL3 = new MultiHeuristicAStarSearch(problemPDDL, new BlindHeuristic());
            searchPDDL3.Start(SearchType.BackwardWithStates);
            var solutionPDDL3 = searchPDDL3.GetSearchResults();
            Assert.AreEqual("Multi-Heuristic A* Search (Backward)", solutionPDDL3.Algorithm);
            Assert.AreEqual("gripper", solutionPDDL3.DomainName);
            Assert.AreEqual("problem-1", solutionPDDL3.ProblemName);
            Assert.AreEqual("Blind Heuristic", solutionPDDL3.Heuristic);
            Assert.AreEqual(ResultStatus.SolutionFound, solutionPDDL3.ResultStatus);
            Assert.IsTrue(solutionPDDL3.SearchTime.TotalMilliseconds > 0);
            Assert.AreEqual(5, solutionPDDL3.SolutionCost);
            Assert.IsNotNull(solutionPDDL3.SolutionPlan);
            Assert.AreEqual(5, solutionPDDL3.SolutionPlan.GetCost());
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
            var successorResults = successor.GetComplexTransitionResults();
            Assert.AreEqual(1, successorResults.Count());
            Assert.IsTrue(successorResults.First().Equals(successor.GetTransitionResult()));

            IPredecessor predecessor = problem.GetPredecessors(problem.GoalConditions).First();
            Assert.IsFalse(predecessor.IsComplexTransition());
            Assert.IsTrue(predecessor.GetPredecessorConditions().Equals(predecessor.GetTransitionResult()));
            var predecessorResults = predecessor.GetComplexTransitionResults();
            Assert.AreEqual(1, predecessorResults.Count());
            Assert.IsTrue(predecessorResults.First().Equals(predecessor.GetTransitionResult()));

            IPredecessor predecessor2 = problem.GetPredecessors(problem.GoalConditions.GetCorrespondingRelativeStates(problem).First()).First();
            Assert.IsTrue(predecessor2.IsComplexTransition());
            Assert.IsTrue(CollectionsEquality.Equals(predecessor2.GetPredecessorRelativeStates(), predecessor2.GetComplexTransitionResults()));
            var predecessorResults2 = predecessor2.GetComplexTransitionResults();
            Assert.AreEqual(1, predecessorResults2.Count());
            Assert.IsTrue(predecessorResults2.First().Equals(predecessor2.GetTransitionResult()));
        }
    }
}
