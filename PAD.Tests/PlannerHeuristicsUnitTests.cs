using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PAD.InputData;
using PAD.Planner.Heuristics;
using System.Linq;
using System;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

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
        private static string GetFilePath(string fileName)
        {
            return $@"..\..\PlannerTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_AdditiveRelaxationHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new AdditiveRelaxationHeuristic(sasProblem);
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Additive Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new AdditiveRelaxationHeuristic(pddlProblem);
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Additive Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_BlindHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new BlindHeuristic();
            Assert.AreEqual(0, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(0, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual(0, heuristic.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(0, heuristic.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Blind Heuristic", heuristic.GetName());
            Assert.AreEqual(6, heuristic.GetCallsCount());
        }

        [TestMethod]
        public void TC_FFHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new FFHeuristic(sasProblem);
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("FF Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new FFHeuristic(pddlProblem);
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("FF Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_HeuristicStatistics()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));

            var heuristic = new StripsHeuristic(sasProblem);
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
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MaxHeuristic(new AdditiveRelaxationHeuristic(sasProblem), new MaxRelaxationHeuristic(sasProblem));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Max Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MaxHeuristic(new AdditiveRelaxationHeuristic(pddlProblem), new MaxRelaxationHeuristic(pddlProblem));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Max Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_MaxRelaxationHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MaxRelaxationHeuristic(sasProblem);
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Max Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MaxRelaxationHeuristic(pddlProblem);
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Max Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_MinHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new MinHeuristic(new AdditiveRelaxationHeuristic(sasProblem), new MaxRelaxationHeuristic(sasProblem));
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(2, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Min Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new MinHeuristic(new AdditiveRelaxationHeuristic(pddlProblem), new MaxRelaxationHeuristic(pddlProblem));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Min Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_PDBHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));

            var heuristic = new PDBHeuristic(sasProblem);
            Assert.AreEqual(8, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual("PDB Heuristic", heuristic.GetName());
            Assert.AreEqual(1, heuristic.GetCallsCount());
        }

        [TestMethod]
        public void TC_PerfectRelaxationHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new PerfectRelaxationHeuristic(sasProblem);
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(9, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Perfect Relaxation Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new PerfectRelaxationHeuristic(pddlProblem);
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(5, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Perfect Relaxation Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_StateSizeHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new StateSizeHeuristic();
            Assert.AreEqual(1.0/7.0, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(1.0/4.0, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(1.0/7.0, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("State Size Heuristic", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new StateSizeHeuristic();
            Assert.AreEqual(1.0/5.0, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(1.0/2.0, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(1.0/2.0, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("State Size Heuristic", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_StripsHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new StripsHeuristic(sasProblem);
            Assert.AreEqual(4, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(4, heuristic.GetValue(new Planner.SAS.State(0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(3, heuristic.GetValue(new Planner.SAS.State(1, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(2, heuristic.GetValue(new Planner.SAS.State(0, 1, 1, 0, 0, 0, 0)));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.State(1, 1, 1, 1, 0, 0, 0)));
            Assert.AreEqual(4, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.Conditions(new Planner.SAS.Assignment(0, 0))));
            Assert.AreEqual(2, heuristic.GetValue(new Planner.SAS.Conditions(new Planner.SAS.Assignment(1, 1), new Planner.SAS.Assignment(2, 1))));
            Assert.AreEqual(1, heuristic.GetValue(new Planner.SAS.ConditionsClause(new Planner.SAS.Conditions(new Planner.SAS.Assignment(0, 1), new Planner.SAS.Assignment(1, 1)), new Planner.SAS.Conditions(new Planner.SAS.Assignment(2, 1)))));
            Assert.AreEqual(int.MaxValue, heuristic.GetValue(new Planner.SAS.ConditionsContradiction()));
            Assert.AreEqual(4, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("STRIPS Heuristic", heuristic.GetName());
            Assert.AreEqual(11, heuristic.GetCallsCount());

            var data = new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl"));
            Planner.PDDL.IdManager idManager = new Planner.PDDL.IdManager(data);
            Planner.PDDL.PrimitivesFactory factory = new Planner.PDDL.PrimitivesFactory(idManager);
            Planner.PDDL.GroundingManager groundingManager = new Planner.PDDL.GroundingManager(data, idManager);
            Planner.PDDL.EvaluationManager evaluationManager = new Planner.PDDL.EvaluationManager(groundingManager, pddlProblem.RigidRelations);
            var atPred = factory.CreatePredicate("at", "ball1", "roomb");
            var atPred2 = factory.CreatePredicate("at", "ball2", "roomb");

            var heuristic2 = new StripsHeuristic(pddlProblem);
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.State(new HashSet<Planner.PDDL.IAtom> { atPred }, null, null, idManager)));
            Assert.AreEqual(0, heuristic2.GetValue(new Planner.PDDL.State(new HashSet<Planner.PDDL.IAtom> { atPred, atPred2 }, null, null, idManager)));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.Conditions(new Planner.PDDL.PredicateExpression(atPred, idManager), evaluationManager)));
            Assert.AreEqual(1, heuristic2.GetValue(new Planner.PDDL.ConditionsCNF(new HashSet<Planner.PDDL.IConjunctCNF> { new Planner.PDDL.PredicateLiteralCNF(new Planner.PDDL.PredicateExpression(atPred, idManager), false) }, evaluationManager, null)));
            Assert.AreEqual(2, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("STRIPS Heuristic", heuristic2.GetName());
            Assert.AreEqual(7, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_SumHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new SumHeuristic(new AdditiveRelaxationHeuristic(sasProblem), new MaxRelaxationHeuristic(sasProblem));
            Assert.AreEqual(14, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(14, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(14, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Sum Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic.GetName());
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new SumHeuristic(new AdditiveRelaxationHeuristic(pddlProblem), new MaxRelaxationHeuristic(pddlProblem));
            Assert.AreEqual(7, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(7, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(7, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Sum Heuristic of (Additive Relaxation Heuristic, Max Relaxation Heuristic)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_WeightedHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new WeightedHeuristic(new StripsHeuristic(sasProblem), 3);
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetInitialState()));
            Assert.AreEqual(12, heuristic.GetValue(new Planner.SAS.State(0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual(6, heuristic.GetValue(new Planner.SAS.State(0, 1, 1, 0, 0, 0, 0)));
            Assert.AreEqual(0, heuristic.GetValue(new Planner.SAS.State(1, 1, 1, 1, 0, 0, 0)));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions()));
            Assert.AreEqual(12, heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()));
            Assert.AreEqual("Weighted STRIPS Heuristic (weight = 3)", heuristic.GetName());
            Assert.AreEqual(6, heuristic.GetCallsCount());

            var heuristic2 = new WeightedHeuristic(new StripsHeuristic(pddlProblem), 9);
            Assert.AreEqual(18, heuristic2.GetValue(pddlProblem.GetInitialState()));
            Assert.AreEqual(18, heuristic2.GetValue(pddlProblem.GetGoalConditions()));
            Assert.AreEqual(18, heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()));
            Assert.AreEqual("Weighted STRIPS Heuristic (weight = 9)", heuristic2.GetName());
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }

        [TestMethod]
        public void TC_WeightedSumHeuristic()
        {
            var sasProblem = new Planner.SAS.Problem(new SASInputData(GetFilePath("TC_Gripper.sas")));
            var pddlProblem = new Planner.PDDL.Problem(new PDDLInputData(GetFilePath("TC_Gripper_D.pddl"), GetFilePath("TC_Gripper_P.pddl")));

            var heuristic = new WeightedSumHeuristic(true, Tuple.Create((IHeuristic)new AdditiveRelaxationHeuristic(sasProblem), 3.0), Tuple.Create((IHeuristic)new MaxRelaxationHeuristic(sasProblem), 7.0));
            Assert.IsTrue(heuristic.GetValue(sasProblem.GetInitialState()) > 0);
            Assert.IsTrue(heuristic.GetValue(sasProblem.GetGoalConditions()) > 0);
            Assert.IsTrue(heuristic.GetValue(sasProblem.GetGoalConditions().GetCorrespondingRelativeStates(sasProblem).First()) > 0);
            Assert.IsTrue(heuristic.GetName().Contains("Weighted Sum Heuristic of"));
            Assert.AreEqual(3, heuristic.GetCallsCount());

            var heuristic2 = new WeightedSumHeuristic(true, Tuple.Create((IHeuristic)new AdditiveRelaxationHeuristic(pddlProblem), 3.0), Tuple.Create((IHeuristic)new MaxRelaxationHeuristic(pddlProblem), 7.0));
            Assert.IsTrue(heuristic2.GetValue(pddlProblem.GetInitialState()) > 0);
            Assert.IsTrue(heuristic2.GetValue(pddlProblem.GetGoalConditions()) > 0);
            Assert.IsTrue(heuristic2.GetValue(pddlProblem.GetGoalConditions().GetCorrespondingRelativeStates(pddlProblem).First()) > 0);
            Assert.IsTrue(heuristic.GetName().Contains("Weighted Sum Heuristic of"));
            Assert.AreEqual(3, heuristic2.GetCallsCount());
        }
    }
}
