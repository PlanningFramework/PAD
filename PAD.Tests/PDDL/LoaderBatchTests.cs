using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PAD.InputData;
// ReSharper disable StringLiteralTypo

namespace PAD.Tests.PDDL
{
    /// <summary>
    /// Batch testing suite for the PDDL input data loader.
    /// The tests consists of actual domains and problem from various years of International Planning Competition (IPC).
    /// </summary>
    [TestClass]
    public class LoaderBatchTests
    {
        /// <summary>
        /// Gets a pair of file paths for the domain and problem files of the specified planning problem.
        /// </summary>
        /// <param name="subFolder">Test case subfolder.</param>
        /// <param name="problemName">Problem name.</param>
        /// <returns>Pair of paths to domain and problem files.</returns>
        private static FilePaths GetFilePaths(string subFolder, string problemName)
        {
            string rootFilePath = $@"..\..\PDDL\LoaderBatchTestCases\{subFolder}\{problemName}";
            return new FilePaths(problemName, $"{rootFilePath}_domain.pddl", $"{rootFilePath}_problem.pddl");
        }

        [TestMethod]
        public void TC_FFBenchmark()
        {
            TestBatch("FF-Benchmark", "blocksworld", "ferry", "fridge", "hanoi", "miconic");
        }

        [TestMethod]
        public void TC_IPC1998()
        {
            TestBatch("IPC-1998", "assembly", "gripper", "logistics", "movie", "mprime", "mystery");
        }

        [TestMethod]
        public void TC_IPC2000()
        {
            TestBatch("IPC-2000", "blocks", "elevators", "freecell", "logistics", "schedule");
        }

        [TestMethod]
        public void TC_IPC2002()
        {
            TestBatch("IPC-2002", "depot", "driverlog", "rovers", "zenotravel");
        }

        [TestMethod]
        public void TC_IPC2004()
        {
            TestBatch("IPC-2004", "airport", "philosophers", "psr", "satellite", "settlers");
        }

        [TestMethod]
        public void TC_IPC2006()
        {
            TestBatch("IPC-2006", "openstacks", "pathways", "pipesworld", "rovers", "storage", "tpp", "trucks");
        }

        [TestMethod]
        public void TC_IPC2008()
        {
            TestBatch("IPC-2008", "elevators", "openstacks", "parcprinter", "pegsol", "scanalyzer", "sokoban", "transport", "woodworking");
        }

        [TestMethod]
        public void TC_IPC2011()
        {
            TestBatch("IPC-2011", "barman", "floortile", "nomystery", "openstacks", "parking", "tidybot", "visitall");
        }

        [TestMethod]
        public void TC_IPC2014()
        {
            TestBatch("IPC-2014", "cavediving", "citycar", "ged", "hiking", "childsnack", "maintenance", "tetris", "thoughtful");
        }

        [TestMethod]
        public void TC_IPC2018()
        {
            TestBatch("IPC-2018", "agricola", "caldera", "data_network", "flashfill", "nurikabe", "organic_synthesis", "petrinet", "settlers", "snake", "spider", "termes");
        }

        /// <summary>
        /// Tests a batch of problems in the specified subfolder. All test problems need to be loaded without errors/exceptions.
        /// </summary>
        /// <param name="subFolder">Test cases subfolder.</param>
        /// <param name="problems">List of problem names.</param>
        private static void TestBatch(string subFolder, params string[] problems)
        {
            List<FilePaths> filePathsList = new List<FilePaths>();
            foreach (var problem in problems)
            {
                filePathsList.Add(GetFilePaths(subFolder, problem));
            }

            foreach (var filePath in filePathsList)
            {
                try
                {
                    PDDLInputData data = new PDDLInputData(filePath.Domain, filePath.Problem);
                    Assert.IsNotNull(data.Domain);
                    Assert.IsNotNull(data.Problem);
                    Assert.IsTrue(Utilities.CheckToStringExport(data));
                }
                catch (System.Exception e)
                {
                    throw new System.Exception($"{filePath.ProblemId}: {e.Message}");
                }
            }
        }
    }
}
