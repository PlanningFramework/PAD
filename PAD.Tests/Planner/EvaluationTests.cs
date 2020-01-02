using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData;
using PAD.Planner;
using PAD.Planner.Heaps;
using PAD.Planner.Search;
using PAD.Planner.Heuristics;

namespace PAD.Tests
{
    /// <summary>
    /// Testing suite for the planner. Testing components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerEvaluationTests
    {
        /// <summary>
        /// Gets full filepath to the specified PDDL test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private string GetPDDLFilePath(string fileName)
        {
            return $@"..\..\TestCases_IPC\PDDL\{fileName}";
        }

        /// <summary>
        /// Gets full filepath to the specified SAS+ test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private string GetSASFilePath(string fileName)
        {
            return $@"..\..\TestCases_IPC\SAS\{fileName}";
        }

        [TestMethod]
        public void TC_Airport()
        {
            Assert.AreEqual(8, PDDLTest("airport"));
            Assert.AreEqual(8, SASTest("airport"));
        }

        [TestMethod]
        public void TC_Assembly()
        {
            //Assert.AreEqual(28, PDDLTest("assembly"));
            //Assert.AreEqual(28, SASTest("assembly"));
        }

        [TestMethod]
        public void TC_Blocks()
        {
            //Assert.AreEqual(6, PDDLTest("blocks"));
            Assert.AreEqual(6, SASTest("blocks"));
        }

        [TestMethod]
        public void TC_Depot()
        {
            //Assert.AreEqual(10, PDDLTest("depot"));
            Assert.AreEqual(10, SASTest("depot"));
        }

        [TestMethod]
        public void TC_DriverLog()
        {
            //Assert.AreEqual(7, PDDLTest("driverlog"));
            Assert.AreEqual(7, SASTest("driverlog"));
        }

        [TestMethod]
        public void TC_Elevators()
        {
            //Assert.AreEqual(42, PDDLTest("elevators"));
            Assert.AreEqual(42, SASTest("elevators"));
        }

        [TestMethod]
        public void TC_FreeCell()
        {
            //Assert.AreEqual(8, PDDLTest("freecell"));
            Assert.AreEqual(8, SASTest("freecell"));
        }

        [TestMethod]
        public void TC_Gripper()
        {
            Assert.AreEqual(11, PDDLTest("gripper"));
            Assert.AreEqual(11, SASTest("gripper"));
        }

        [TestMethod]
        public void TC_Movie()
        {
            Assert.AreEqual(7, PDDLTest("movie"));
            Assert.AreEqual(7, SASTest("movie"));
        }

        [TestMethod]
        public void TC_Openstacks()
        {
            //Assert.AreEqual(2, PDDLTest("openstacks"));
            Assert.AreEqual(2, SASTest("openstacks"));
        }

        [TestMethod]
        public void TC_Parcprinter()
        {
            //Assert.AreEqual(169009, PDDLTest("parcprinter"));
            Assert.AreEqual(169009, SASTest("parcprinter"));
        }

        [TestMethod]
        public void TC_Pathways()
        {
            //Assert.AreEqual(6, PDDLTest("pathways"));
            Assert.AreEqual(6, SASTest("pathways"));
        }

        [TestMethod]
        public void TC_Pegsol()
        {
            //Assert.AreEqual(2, PDDLTest("pegsol"));
            Assert.AreEqual(2, SASTest("pegsol"));
        }

        [TestMethod]
        public void TC_Philosophers()
        {
            //Assert.AreEqual(18, PDDLTest("philosophers"));
            Assert.AreEqual(18, SASTest("philosophers"));
        }

        [TestMethod]
        public void TC_PipesWorld()
        {
            Assert.AreEqual(5, PDDLTest("pipesworld"));
            Assert.AreEqual(5, SASTest("pipesworld"));
        }

        [TestMethod]
        public void TC_Psr()
        {
            Assert.AreEqual(8, PDDLTest("psr"));
            Assert.AreEqual(8, SASTest("psr"));
        }

        [TestMethod]
        public void TC_Rovers()
        {
            //Assert.AreEqual(8, PDDLTest("rovers"));
            Assert.AreEqual(8, SASTest("rovers"));
        }

        [TestMethod]
        public void TC_Satellite()
        {
            Assert.AreEqual(9, PDDLTest("satellite"));
            Assert.AreEqual(9, SASTest("satellite"));
        }

        [TestMethod]
        public void TC_Settlers()
        {
            //Assert.AreEqual(8, PDDLTest("settlers"));
            //Assert.AreEqual(8, SASTest("settlers"));
        }

        [TestMethod]
        public void TC_Sokoban()
        {
            //Assert.AreEqual(9, PDDLTest("sokoban"));
            //Assert.AreEqual(9, SASTest("sokoban"));
        }

        [TestMethod]
        public void TC_Storage()
        {
            Assert.AreEqual(3, PDDLTest("storage"));
            //Assert.AreEqual(3, SASTest("storage"));
        }

        [TestMethod]
        public void TC_Tpp()
        {
            Assert.AreEqual(5, PDDLTest("tpp"));
            //Assert.AreEqual(5, SASTest("tpp"));
        }

        [TestMethod]
        public void TC_Trucks()
        {
            //Assert.AreEqual(13, PDDLTest("trucks"));
            //Assert.AreEqual(13, SASTest("trucks"));
        }

        [TestMethod]
        public void TC_Zenotravel()
        {
            Assert.AreEqual(1, PDDLTest("zenotravel"));
            //Assert.AreEqual(1, SASTest("zenotravel"));
        }

        private double PDDLTest(string name)
        {
            return Search(new Planner.PDDL.Problem(new PDDLInputData(GetPDDLFilePath($"{name}_domain.pddl"), GetPDDLFilePath($"{name}_problem.pddl"))));
        }

        private double SASTest(string name)
        {
            return Search(new Planner.SAS.Problem(new SASInputData(GetSASFilePath($"{name}.sas"))));
        }

        private double Search(IProblem problem)
        {
            var searchEngine = new AStarSearch(problem, new StripsHeuristic(problem), new RegularBinaryHeap());
            searchEngine.Start();
            return searchEngine.GetSolutionCost();
        }
    }
}
