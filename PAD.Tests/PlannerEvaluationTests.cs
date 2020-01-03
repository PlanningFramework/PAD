using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData;
using PAD.Planner;
using PAD.Planner.Heaps;
using PAD.Planner.Search;
using PAD.Planner.Heuristics;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.Tests
{
    /// <summary>
    /// Testing suite for the planner. Testing components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerEvaluationTests
    {
        [TestMethod]
        public void TC_Airport()
        {
            Assert.AreEqual(8, PTest("airport"));
            Assert.AreEqual(8, STest("airport"));
        }

        [TestMethod]
        public void TC_Blocks()
        {
            Assert.AreEqual(6, STest("blocks"));
        }

        [TestMethod]
        public void TC_Depot()
        {
            Assert.AreEqual(10, STest("depot"));
        }

        [TestMethod]
        public void TC_DriverLog()
        {
            Assert.AreEqual(7, STest("driverlog"));
        }

        [TestMethod]
        public void TC_Elevators()
        {
            Assert.AreEqual(42, STest("elevators"));
        }

        [TestMethod]
        public void TC_FreeCell()
        {
            Assert.AreEqual(8, STest("freecell"));
        }

        [TestMethod]
        public void TC_Gripper()
        {
            Assert.AreEqual(11, PTest("gripper"));
            Assert.AreEqual(11, STest("gripper"));
        }

        [TestMethod]
        public void TC_Movie()
        {
            Assert.AreEqual(7, PTest("movie"));
            Assert.AreEqual(7, STest("movie"));
        }

        [TestMethod]
        public void TC_Openstacks()
        {
            Assert.AreEqual(2, STest("openstacks"));
        }

        [TestMethod]
        public void TC_Parcprinter()
        {
            Assert.AreEqual(169009, STest("parcprinter"));
        }

        [TestMethod]
        public void TC_Pathways()
        {
            Assert.AreEqual(6, STest("pathways"));
        }

        [TestMethod]
        public void TC_Pegsol()
        {
            Assert.AreEqual(2, STest("pegsol"));
        }

        [TestMethod]
        public void TC_Philosophers()
        {
            Assert.AreEqual(18, STest("philosophers"));
        }

        [TestMethod]
        public void TC_PipesWorld()
        {
            Assert.AreEqual(5, PTest("pipesworld"));
            Assert.AreEqual(5, STest("pipesworld"));
        }

        [TestMethod]
        public void TC_Psr()
        {
            Assert.AreEqual(8, PTest("psr"));
            Assert.AreEqual(8, STest("psr"));
        }

        [TestMethod]
        public void TC_Rovers()
        {
            Assert.AreEqual(8, STest("rovers"));
        }

        [TestMethod]
        public void TC_Satellite()
        {
            Assert.AreEqual(9, PTest("satellite"));
            Assert.AreEqual(9, STest("satellite"));
        }

        [TestMethod]
        public void TC_Storage()
        {
            Assert.AreEqual(3, PTest("storage"));
        }

        [TestMethod]
        public void TC_Tpp()
        {
            Assert.AreEqual(5, PTest("tpp"));
        }

        [TestMethod]
        public void TC_Zenotravel()
        {
            Assert.AreEqual(1, PTest("zenotravel"));
        }

        private static double PTest(string name)
        {
            string domainFilePath = $@"..\..\TestCases_IPC\PDDL\{name}_domain.pddl";
            string problemFilePath = $@"..\..\TestCases_IPC\PDDL\{name}_problem.pddl";
            return Search(new Planner.PDDL.Problem(new PDDLInputData(domainFilePath, problemFilePath)));
        }

        private static double STest(string name)
        {
            string filePath = $@"..\..\TestCases_IPC\SAS\{name}.sas";
            return Search(new Planner.SAS.Problem(new SASInputData(filePath)));
        }

        private static double Search(IProblem problem)
        {
            var searchEngine = new AStarSearch(problem, new StripsHeuristic(problem), new RegularBinaryHeap());
            searchEngine.Start();
            return searchEngine.GetSolutionCost();
        }
    }
}
