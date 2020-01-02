using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData;

namespace PAD.Tests.SAS
{
    /// <summary>
    /// Testing suite for the SAS+ input data loader. Covers the entire phase of loading an input data from the given SAS+
    /// problem file into SASInputData structure.
    /// </summary>
    [TestClass]
    public class SASLoaderUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private string GetFilePath(string fileName)
        {
            return $@"..\..\Loader.SAS\TestCases\{fileName}";
        }

        [TestMethod]
        public void TC_AxiomRules()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_AxiomRules.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.AxiomRules.Count);

            var rule0 = data.Problem.AxiomRules[0];
            var conditions0 = rule0.Conditions;
            Assert.AreEqual(2, conditions0.Count);
            Assert.AreEqual(0, conditions0[0].Variable);
            Assert.AreEqual(0, conditions0[0].Value);
            Assert.AreEqual(1, conditions0[1].Variable);
            Assert.AreEqual(0, conditions0[1].Value);
            Assert.AreEqual(1, rule0.PrimitiveEffect.Variable);
            Assert.AreEqual(1, rule0.PrimitiveEffect.Value);

            var rule1 = data.Problem.AxiomRules[1];
            var conditions1 = rule1.Conditions;
            Assert.AreEqual(2, conditions1.Count);
            Assert.AreEqual(0, conditions1[0].Variable);
            Assert.AreEqual(0, conditions1[0].Value);
            Assert.AreEqual(1, conditions1[1].Variable);
            Assert.AreEqual(0, conditions1[1].Value);
            Assert.AreEqual(1, rule1.PrimitiveEffect.Variable);
            Assert.AreEqual(1, rule1.PrimitiveEffect.Value);
        }

        [TestMethod]
        public void TC_FullTest_Blocks()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_FullTest_Blocks.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));
        }

        [TestMethod]
        public void TC_FullTest_Gripper()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_FullTest_Gripper.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));
        }

        [TestMethod]
        public void TC_FullTest_PSR()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_FullTest_PSR.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));
        }

        [TestMethod]
        public void TC_GoalConditions()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_GoalConditions.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.GoalConditions.Count);

            var goal0 = data.Problem.GoalConditions[0];
            Assert.AreEqual(0, goal0.Variable);
            Assert.AreEqual(2, goal0.Value);

            var goal1 = data.Problem.GoalConditions[1];
            Assert.AreEqual(1, goal1.Variable);
            Assert.AreEqual(1, goal1.Value);
        }

        [TestMethod]
        public void TC_InitialState()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_InitialState.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.InitialState.Count);

            var initValues = data.Problem.InitialState;
            Assert.AreEqual(2, initValues[0]);
            Assert.AreEqual(1, initValues[1]);
        }

        [TestMethod]
        public void TC_Metric()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Metric.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.IsTrue(data.Problem.Metric.IsUsed);
        }

        [TestMethod]
        public void TC_MutexGroups()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_MutexGroups.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.MutexGroups.Count);

            var group0 = data.Problem.MutexGroups[0];
            Assert.AreEqual(3, group0.Count);
            Assert.AreEqual(0, group0[0].Variable);
            Assert.AreEqual(0, group0[0].Value);
            Assert.AreEqual(0, group0[1].Variable);
            Assert.AreEqual(1, group0[1].Value);
            Assert.AreEqual(0, group0[2].Variable);
            Assert.AreEqual(2, group0[2].Value);

            var group1 = data.Problem.MutexGroups[1];
            Assert.AreEqual(2, group1.Count);
            Assert.AreEqual(0, group1[0].Variable);
            Assert.AreEqual(1, group1[0].Value);
            Assert.AreEqual(1, group1[1].Variable);
            Assert.AreEqual(1, group1[1].Value);
        }

        [TestMethod]
        public void TC_Operators()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Operators.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.Operators.Count);

            var operator0 = data.Problem.Operators[0];
            Assert.AreEqual("operator0", operator0.Name);
            Assert.AreEqual(0, operator0.Conditions.Count);
            Assert.AreEqual(0, operator0.Effects.Count);
            Assert.AreEqual(0, operator0.Cost);

            var operator1 = data.Problem.Operators[1];

            var conditions = operator1.Conditions;
            Assert.AreEqual(2, conditions.Count);
            Assert.AreEqual(0, conditions[0].Variable);
            Assert.AreEqual(0, conditions[0].Value);
            Assert.AreEqual(1, conditions[1].Variable);
            Assert.AreEqual(0, conditions[1].Value);

            var effects = operator1.Effects;
            Assert.AreEqual(2, effects.Count);
            Assert.AreEqual(0, effects[0].Conditions.Count);
            Assert.AreEqual(1, effects[0].PrimitiveEffect.Variable);
            Assert.AreEqual(1, effects[0].PrimitiveEffect.Value);

            Assert.AreEqual(1, effects[1].Conditions.Count);
            Assert.AreEqual(1, effects[1].Conditions[0].Variable);
            Assert.AreEqual(1, effects[1].Conditions[0].Value);
            Assert.AreEqual(0, effects[1].PrimitiveEffect.Variable);
            Assert.AreEqual(1, effects[1].PrimitiveEffect.Value);
        }

        [TestMethod]
        public void TC_Problem()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Problem.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            var problem = data.Problem;
            Assert.AreEqual("TC_Problem", problem.Name);
            Assert.AreEqual(3, problem.Version.Number);
            Assert.IsFalse(problem.Metric.IsUsed);
            Assert.AreEqual(0, problem.Variables.Count);
            Assert.AreEqual(0, problem.MutexGroups.Count);
            Assert.AreEqual(0, problem.InitialState.Count);
            Assert.AreEqual(0, problem.GoalConditions.Count);
            Assert.AreEqual(0, problem.Operators.Count);
            Assert.AreEqual(0, problem.AxiomRules.Count);
            Assert.AreEqual(GetFilePath("TC_Problem.sas"), problem.FilePath);
        }

        [TestMethod]
        public void TC_Variables()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Variables.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.Variables.Count);

            var var0 = data.Problem.Variables[0];
            Assert.AreEqual("var0", var0.Name);
            Assert.AreEqual(-1, var0.AxiomLayer);
            Assert.IsFalse(var0.IsAxiomatic());
            Assert.AreEqual(3, var0.GetDomainRange());
            Assert.AreEqual(3, var0.Values.Count);
            Assert.AreEqual("Red", var0.Values[0]);
            Assert.AreEqual("Green", var0.Values[1]);
            Assert.AreEqual("Blue", var0.Values[2]);

            var var1 = data.Problem.Variables[1];
            Assert.AreEqual("var1", var1.Name);
            Assert.AreEqual(0, var1.AxiomLayer);
            Assert.IsTrue(var1.IsAxiomatic());
            Assert.AreEqual(2, var1.GetDomainRange());
            Assert.AreEqual(2, var1.Values.Count);
            Assert.AreEqual("Black", var1.Values[0]);
            Assert.AreEqual("White", var1.Values[1]);
        }

        [TestMethod]
        public void TC_Version1()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Version1.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(1, data.Problem.Version.Number);
        }

        [TestMethod]
        public void TC_Version2()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Version2.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(2, data.Problem.Version.Number);
        }

        [TestMethod]
        public void TC_Version3()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Version3.sas"));
            Assert.IsTrue(Utilities.CheckToStringExport(data));

            Assert.AreEqual(3, data.Problem.Version.Number);
        }
    }
}
