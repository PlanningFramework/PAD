using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PAD.InputData;
using PAD.Planner.Heuristics;
using System.Linq;
using System;

namespace PAD.Tests
{
    /// <summary>
    /// Testing suite for the planner. Testing components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerHeuristicsUnitTests
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
        public void TC_AdditiveRelaxationHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new AdditiveRelaxationHeuristic(problemSAS);
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Additive Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new AdditiveRelaxationHeuristic(problemPDDL);
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Additive Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_BlindHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new BlindHeuristic();
            Assert.AreEqual(0, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(0, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual(0, heuristic.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(0, heuristic.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Blind Heuristic", heuristic.GetName());
            Assert.AreEqual(6, heuristic.GetCallsCount());
        }

        [TestMethod]
        public void TC_FFHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new FFHeuristic(problemSAS);
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("FF Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new FFHeuristic(problemPDDL);
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("FF Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_HeuristicStatistics()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));

            var heuristic = new StripsHeuristic(problemSAS);
            Assert.AreEqual(4, heuristic.GetValue(new Planner.SAS.State(0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(3, heuristic.GetValue(new Planner.SAS.State(1, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(2, heuristic.GetValue(new Planner.SAS.State(0, 1, 1, 0, 0, 0, 0)));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.State(1, 1, 1, 1, 0, 0, 0)));

            Assert.AreEqual(0, heuristic.Statistics.BestHeuristicValue);
            Assert.AreEqual(4, heuristic.Statistics.HeuristicCallsCount);
            Assert.AreEqual(9, heuristic.Statistics.SumOfHeuristicValues);
            Assert.AreEqual(2.25, heuristic.Statistics.AverageHeuristicValue);
        }

        [TestMethod]
        public void TC_MaxHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MaxHeuristic(new AdditiveRelaxationHeuristic(problemSAS), new MaxRelaxationHeuristic(problemSAS));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Max Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MaxHeuristic(new AdditiveRelaxationHeuristic(problemPDDL), new MaxRelaxationHeuristic(problemPDDL));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Max Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_MaxRelaxationHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MaxRelaxationHeuristic(problemSAS);
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Max Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MaxRelaxationHeuristic(problemPDDL);
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Max Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_MinHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MinHeuristic(new AdditiveRelaxationHeuristic(problemSAS), new MaxRelaxationHeuristic(problemSAS));
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(2, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Min Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MinHeuristic(new AdditiveRelaxationHeuristic(problemPDDL), new MaxRelaxationHeuristic(problemPDDL));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Min Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_PDBHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));

            var heuristic = new PDBHeuristic(problemSAS);
            Assert.AreEqual(8, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual("PDB Heuristic", heuristic.GetName());
            Assert.AreEqual(1, heuristic.GetCallsCount());
        }

        [TestMethod]
        public void TC_PerfectRelaxationHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new PerfectRelaxationHeuristic(problemSAS);
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(9, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Perfect Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new PerfectRelaxationHeuristic(problemPDDL);
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Perfect Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_StateSizeHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new StateSizeHeuristic();
            Assert.AreEqual(1.0/7.0, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(1.0/4.0, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(1.0/7.0, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("State Size Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new StateSizeHeuristic();
            Assert.AreEqual(1.0/5.0, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(1.0/2.0, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(1.0/2.0, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("State Size Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_StripsHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new StripsHeuristic(problemSAS);
            Assert.AreEqual(4, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(4, heuristic.GetValue(new Planner.SAS.State(0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(3, heuristic.GetValue(new Planner.SAS.State(1, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(2, heuristic.GetValue(new Planner.SAS.State(0, 1, 1, 0, 0, 0, 0)));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.State(1, 1, 1, 1, 0, 0, 0)));
            Assert.AreEqual(4, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.Conditions(new Planner.SAS.Assignment(0, 0))));
            Assert.AreEqual(2, heuristic.GetValue(new Planner.SAS.Conditions(new Planner.SAS.Assignment(1, 1), new Planner.SAS.Assignment(2, 1))));
            Assert.AreEqual(1, heuristic.GetValue(new Planner.SAS.ConditionsClause(new Planner.SAS.Conditions(new Planner.SAS.Assignment(0, 1), new Planner.SAS.Assignment(1, 1)), new Planner.SAS.Conditions(new Planner.SAS.Assignment(2, 1)))));
            Assert.AreEqual(int.MaxValue, heuristic.GetValue(new Planner.SAS.ConditionsContradiction()));
            Assert.AreEqual(4, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("STRIPS Heuristic", heuristic.GetName());
            Assert.AreEqual(11, heuristic.GetCallsCount());

            var data = new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl"));
            Planner.PDDL.IDManager idManager = new Planner.PDDL.IDManager(data);
            Planner.PDDL.PrimitivesFactory factory = new Planner.PDDL.PrimitivesFactory(idManager);
            Planner.PDDL.GroundingManager groundingManager = new Planner.PDDL.GroundingManager(data, idManager);
            Planner.PDDL.EvaluationManager evaluationManager = new Planner.PDDL.EvaluationManager(groundingManager, problemPDDL.RigidRelations);
            var atPred = factory.CreatePredicate("at", "ball1", "roomb");
            var atPred2 = factory.CreatePredicate("at", "ball2", "roomb");

            var heuristic2 = new StripsHeuristic(problemPDDL);
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.State(new HashSet<Planner.PDDL.IAtom> { atPred }, null, null, idManager)));
            Assert.AreEqual(0, heuristic2.GetValue(new Planner.PDDL.State(new HashSet<Planner.PDDL.IAtom> { atPred, atPred2 }, null, null, idManager)));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.Conditions(new Planner.PDDL.PredicateExpression(atPred, idManager), evaluationManager)));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.ConditionsCNF(new HashSet<Planner.PDDL.IConjunctCNF> { new Planner.PDDL.PredicateLiteralCNF(new Planner.PDDL.PredicateExpression(atPred, idManager), false) }, evaluationManager, null)));
            Assert.AreEqual(2, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("STRIPS Heuristic", heuristic2.GetName());
            Assert.AreEqual(7, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_SumHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new SumHeuristic(new AdditiveRelaxationHeuristic(problemSAS), new MaxRelaxationHeuristic(problemSAS));
            Assert.AreEqual(14, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(14, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(14, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Sum Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new SumHeuristic(new AdditiveRelaxationHeuristic(problemPDDL), new MaxRelaxationHeuristic(problemPDDL));
            Assert.AreEqual(7, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(7, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(7, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Sum Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_WeightedHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new WeightedHeuristic(new StripsHeuristic(problemSAS), 3);
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(new Planner.SAS.State(0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(6, heuristic.GetValue(new Planner.SAS.State(0, 1, 1, 0, 0, 0, 0)));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.State(1, 1, 1, 1, 0, 0, 0)));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()));
            Assert.AreEqual("Weighted STRIPS Heuristic (weight = 3)", heuristic.GetName());
            Assert.AreEqual(6, heuristic.GetCallsCount());

            var heuristic2 = new WeightedHeuristic(new StripsHeuristic(problemPDDL), 9);
            Assert.AreEqual(18, heuristic2.GetValue(problemPDDL.GetInitialState()));
            Assert.AreEqual(18, heuristic2.GetValue(problemPDDL.GetGoalConditions()));
            Assert.AreEqual(18, heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()));
            Assert.AreEqual("Weighted STRIPS Heuristic (weight = 9)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_WeightedSumHeuristic()
        {
            var problemSAS = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var problemPDDL = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new WeightedSumHeuristic(true, Tuple.Create((IHeuristic)new AdditiveRelaxationHeuristic(problemSAS), 3.0), Tuple.Create((IHeuristic)new MaxRelaxationHeuristic(problemSAS), 7.0));
            Assert.IsTrue(heuristic.GetValue(problemSAS.GetInitialState()) > 0);
            Assert.IsTrue(heuristic.GetValue(problemSAS.GetGoalConditions()) > 0);
            Assert.IsTrue(heuristic.GetValue(problemSAS.GetGoalConditions().GetCorrespondingRelativeStates(problemSAS).First()) > 0);
            Assert.IsTrue(heuristic.GetName().Contains("Weighted Sum Heuristic of"));
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new WeightedSumHeuristic(true, Tuple.Create((IHeuristic)new AdditiveRelaxationHeuristic(problemPDDL), 3.0), Tuple.Create((IHeuristic)new MaxRelaxationHeuristic(problemPDDL), 7.0));
            Assert.IsTrue(heuristic2.GetValue(problemPDDL.GetInitialState()) > 0);
            Assert.IsTrue(heuristic2.GetValue(problemPDDL.GetGoalConditions()) > 0);
            Assert.IsTrue(heuristic2.GetValue(problemPDDL.GetGoalConditions().GetCorrespondingRelativeStates(problemPDDL).First()) > 0);
            Assert.IsTrue(heuristic.GetName().Contains("Weighted Sum Heuristic of"));
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }
    }
}
