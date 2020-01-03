using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using PAD.InputData;
using PAD.Planner.SAS;
// ReSharper disable PossibleUnintendedReferenceComparison
// ReSharper disable IdentifierTypo
// ReSharper disable CollectionNeverUpdated.Local

namespace PAD.Tests.SAS
{
    /// <summary>
    /// Testing suite for the SAS+ planner. Testing all components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private static string GetFilePath(string fileName)
        {
            return $@"..\..\SAS\PlannerTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_AbstractedState()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_AbstractedState.sas"));
            Problem problem = new Problem(data);

            AbstractedState.SetNotAbstractedVariables(new HashSet<int> { 0, 3, 4 });
            Assert.IsFalse(AbstractedState.IsVariableAbstracted(0));
            Assert.IsTrue(AbstractedState.IsVariableAbstracted(1));
            Assert.IsTrue(AbstractedState.IsVariableAbstracted(2));
            Assert.IsFalse(AbstractedState.IsVariableAbstracted(3));
            Assert.IsFalse(AbstractedState.IsVariableAbstracted(4));

            var state = (State)problem.GetInitialState();
            Assert.AreEqual(5, state.GetSize());
            Assert.AreEqual("Red, Black, Car, Yes, Bottom", state.ToStringWithMeanings(problem));

            var abstractedState = new AbstractedState(state);
            Assert.AreEqual(3, abstractedState.GetSize());
            Assert.AreEqual("Red, Yes, Bottom", abstractedState.ToStringWithMeanings(problem));

            Assert.AreEqual(0, abstractedState.GetValue(0));
            Assert.AreEqual(AbstractedState.WildCardValue, abstractedState.GetValue(1));
            Assert.AreEqual(AbstractedState.WildCardValue, abstractedState.GetValue(2));
            Assert.AreEqual(0, abstractedState.GetValue(3));
            Assert.AreEqual(1, abstractedState.GetValue(4));

            Assert.IsTrue(abstractedState.HasValue(0, 0));
            Assert.IsFalse(abstractedState.HasValue(0, 1));
            Assert.IsTrue(abstractedState.HasValue(1, 0));
            Assert.IsTrue(abstractedState.HasValue(1, 1));
            Assert.IsTrue(abstractedState.HasValue(1, 555));

            Assert.IsFalse(abstractedState.HasValue(new Assignment(3, 1)));
            abstractedState.SetValue(3, 1);
            Assert.IsTrue(abstractedState.HasValue(3, 1));
            abstractedState.SetValue(new Assignment(3, 0));

            Assert.AreEqual(1, abstractedState.GetAllValues(0).Length);
            Assert.AreEqual(0, abstractedState.GetAllValues(0)[0]);
            Assert.AreEqual(1, abstractedState.GetAllValues(1).Length);
            Assert.AreEqual(AbstractedState.WildCardValue, abstractedState.GetAllValues(1)[0]);
            Assert.IsTrue(abstractedState.GetAllValues().SequenceEqual(new[] { 0, 0, 1 }));

            Assert.IsTrue(abstractedState.GetValues(new[] { 2, 3 }).SequenceEqual(new[] { AbstractedState.WildCardValue, 0 }));
            Assert.IsTrue(abstractedState.GetValues(new[] { 4 }).SequenceEqual(new[] { 1 }));

            var relaxedState = abstractedState.GetRelaxedState() as RelaxedState;
            Assert.IsNotNull(relaxedState);

            var conditions = (IConditions)abstractedState.GetDescribingConditions(problem);
            Assert.AreEqual(conditions, new Conditions(new Assignment(0, 0), new Assignment(3, 0), new Assignment(4, 1)));

            Assert.AreEqual("PlannerTestCases_TC_AbstractedState.sas_[2x0 1]", abstractedState.GetInfoString(problem));
            Assert.AreEqual("[2x0 1]", abstractedState.GetCompressedDescription());
            Assert.AreEqual("[0 0 1]", abstractedState.ToString());

            var stateClone = abstractedState.Clone();
            Assert.IsTrue(abstractedState != stateClone);
            Assert.IsTrue(abstractedState.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(abstractedState.Equals(stateClone));
        }

        [TestMethod]
        public void TC_Assignment()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Assignment.sas"));
            Problem problem = new Problem(data);

            IAssignment assignment0 = ((PrimitiveEffect)problem.Operators[0].GetEffects()[0]).Assignment;
            IAssignment assignment1 = ((PrimitiveEffect)problem.Operators[0].GetEffects()[1]).Assignment;

            Assert.AreEqual(0, assignment0.GetVariable());
            Assert.AreEqual(1, assignment0.GetValue());
            Assert.IsTrue(assignment1.Equals(new Assignment(1, 2)));

            var assignmentClone = assignment1.Clone();
            Assert.IsTrue(assignment1 != assignmentClone);
            Assert.IsTrue(assignment1.GetHashCode() == assignmentClone.GetHashCode());
            Assert.IsTrue(assignment1.Equals(assignmentClone));
        }

        [TestMethod]
        public void TC_AxiomRules()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_AxiomRules.sas"));
            Problem problem = new Problem(data);
            AxiomRules axiomRules = problem.AxiomRules;

            // inner structure by ordered axiom layers

            Assert.AreEqual(3, axiomRules.Count);
            Assert.AreEqual(0, axiomRules[0].Count);
            Assert.AreEqual(3, axiomRules[1].Count);
            Assert.AreEqual(1, axiomRules[2].Count);

            // single axiom rule

            IState state = (IState)problem.InitialState.Clone();
            AxiomRule axiomRule2 = axiomRules[2][0];

            Assert.IsFalse(axiomRule2.IsApplicable(state));
            state.SetValue(0, 3);
            Assert.IsTrue(axiomRule2.IsApplicable(state));

            Assert.AreEqual(0, state.GetValue(2));
            Assert.IsTrue(axiomRule2.Apply(state));
            Assert.IsFalse(axiomRule2.Apply(state));
            Assert.AreEqual(1, state.GetValue(2));

            // application of the whole axiom layer

            IState state1 = (IState)problem.InitialState.Clone();
            AxiomLayer axiomLayer1 = axiomRules[1];

            axiomLayer1.Apply(state1);
            Assert.AreEqual(3, state1.GetValue(0));
            Assert.AreEqual(0, state1.GetValue(1));
            Assert.AreEqual(0, state1.GetValue(2));

            // application of all the axiom rules

            IState state2 = (IState)problem.InitialState.Clone();

            axiomRules.Apply(state2);
            Assert.AreEqual(3, state2.GetValue(0));
            Assert.AreEqual(0, state2.GetValue(1));
            Assert.AreEqual(1, state2.GetValue(2));

            state2.SetValue(1, 1);
            axiomRules.Apply(state2);
            Assert.AreEqual(3, state2.GetValue(0));
            Assert.AreEqual(0, state2.GetValue(1));
            Assert.AreEqual(1, state2.GetValue(2));
        }

        [TestMethod]
        public void TC_Conditions()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Conditions.sas"));
            Problem problem = new Problem(data);
            Assert.AreEqual(new Conditions(new Assignment(0, 1), new Assignment(2, 1), new Assignment(1, 0)), problem.GoalConditions);

            IConditions conditions = new Conditions(new Assignment(0, 0), new Assignment(1, 2));
            IConditions emptyConditions = new Conditions();
            IState state0 = new State(0, 0, 0, 0);
            IState state1 = new State(0, 2, 3, 0);

            Assert.IsFalse(conditions.Evaluate(state0));
            Assert.IsTrue(conditions.Evaluate(state1));
            Assert.IsTrue(emptyConditions.Evaluate(state0));
            Assert.IsTrue(emptyConditions.Evaluate(state1));

            var conjunction = conditions.ConjunctionWith(new Conditions(new Assignment(0, 0), new Assignment(2, 0)));
            Assert.IsTrue(conjunction.Equals(new Conditions(new Assignment(0, 0), new Assignment(1, 2), new Assignment(2, 0))));
            var conjunction1 = conditions.ConjunctionWith(new ConditionsClause(new Conditions(new Assignment(2, 3)), new Conditions(new Assignment(3, 3)), new Conditions(new Assignment(1, 1))));
            Assert.IsTrue(conjunction1.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 2), new Assignment(2, 3)), new Conditions(new Assignment(0, 0), new Assignment(1, 2), new Assignment(3, 3)))));
            var conjunction2 = conditions.ConjunctionWith(new Conditions(new Assignment(0, 1)));
            Assert.IsTrue(conjunction2.Equals(new ConditionsContradiction()));
            var conjunction3 = conditions.ConjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(conjunction3.Equals(new ConditionsContradiction()));
            var conjunction4 = emptyConditions.ConjunctionWith(new Conditions(new Assignment(2, 0)));
            Assert.IsTrue(conjunction4.Equals(new Conditions(new Assignment(2, 0))));

            var disjunction = conditions.DisjunctionWith(new Conditions(new Assignment(2, 0)));
            Assert.IsTrue(disjunction.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 2)), new Conditions(new Assignment(2, 0)))));
            var disjunction1 = conditions.DisjunctionWith(new ConditionsClause(new Conditions(new Assignment(2, 3)), new Conditions(new Assignment(3, 3))));
            Assert.IsTrue(disjunction1.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 2)), new Conditions(new Assignment(2, 3)), new Conditions(new Assignment(3, 3)))));
            var disjunction2 = conditions.DisjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(disjunction2.Equals(conditions));
            var disjunction3 = emptyConditions.DisjunctionWith(new Conditions(new Assignment(2, 0)));
            Assert.IsTrue(disjunction3.Equals(new Conditions()));

            Assert.IsFalse(conditions.IsConflictedWith(new Conditions(new Assignment(0, 0), new Assignment(1, 2), new Assignment(2, 0))));
            Assert.IsTrue(conditions.IsConflictedWith(new Conditions(new Assignment(0, 0), new Assignment(1, 0), new Assignment(2, 0))));
            Assert.IsFalse(conditions.IsConflictedWith(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)))));
            Assert.IsTrue(conditions.IsConflictedWith(new ConditionsContradiction()));
            Assert.IsFalse(emptyConditions.IsConflictedWith(new Conditions(new Assignment(0, 0))));

            var conditionsCopy = (IConditions)conditions.Clone();
            Assert.IsTrue(conditionsCopy.RemoveConstraint(new Assignment(0, 0)));
            Assert.IsFalse(conditionsCopy.RemoveConstraint(new Assignment(0, 0)));
            Assert.IsTrue(conditionsCopy.Equals(new Conditions(new Assignment(1, 2))));
            Assert.IsTrue(conditionsCopy.RemoveConstraint(new Assignment(1, 2)));
            Assert.IsTrue(conditionsCopy.Equals(new Conditions()));
            Assert.IsFalse(emptyConditions.RemoveConstraint(new Assignment(0, 0)));

            Assert.IsTrue(conditions.IsConstrained(new Assignment(0, 0)));
            Assert.IsFalse(conditions.IsConstrained(new Assignment(0, 1)));
            Assert.IsFalse(emptyConditions.IsConstrained(new Assignment(0, 0)));

            Assert.IsFalse(conditions.IsConflictedWith(new Assignment(0, 0)));
            Assert.IsTrue(conditions.IsConflictedWith(new Assignment(0, 1)));
            Assert.IsFalse(emptyConditions.IsConflictedWith(new Assignment(0, 0)));

            Assert.AreEqual(EffectRelevance.RELEVANT, conditions.IsEffectAssignmentRelevant(new Assignment(0, 0)));
            Assert.AreEqual(EffectRelevance.ANTI_RELEVANT, conditions.IsEffectAssignmentRelevant(new Assignment(0, 5)));
            Assert.AreEqual(EffectRelevance.IRRELEVANT, conditions.IsEffectAssignmentRelevant(new Assignment(3, 5)));
            Assert.AreEqual(EffectRelevance.IRRELEVANT, emptyConditions.IsEffectAssignmentRelevant(new Assignment(0, 0)));

            Assert.IsTrue(conditions.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0) }));
            Assert.IsFalse(conditions.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 2) }));
            Assert.IsTrue(emptyConditions.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0) }));

            int value;
            Assert.IsTrue(((ISimpleConditions)conditions).IsVariableConstrained(1, out value) && value == 2);
            Assert.IsFalse(((ISimpleConditions)conditions).IsVariableConstrained(2, out value));
            Assert.IsTrue(((ISimpleConditions)conditions).IsVariableConstrained(1));
            Assert.IsFalse(((ISimpleConditions)conditions).IsVariableConstrained(2));
            Assert.IsTrue(((ISimpleConditions)conditions).GetAssignedValues().SequenceEqual(new[] { 0, 2 }));
            Assert.IsFalse(((ISimpleConditions)conditions).GetAssignedValues().SequenceEqual(new[] { 2, 2 }));

            StateLabels stateLabels = new StateLabels {{new Assignment(0, 0), 3}, {new Assignment(1, 2), 7}};
            Assert.AreEqual(7, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(10, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(2, conditions.GetNotAccomplishedConstraintsCount(new State(1, 1, 1)));
            Assert.AreEqual(0, conditions.GetNotAccomplishedConstraintsCount(new State(0, 2, 1)));
            Assert.AreEqual(2, conditions.GetSize());

            HashSet<IState> expectedConditionsStates = new HashSet<IState> { new State(0, 2, 0), new State(0, 2, 1) };
            var conditionsStates = conditions.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(2, conditionsStates.Count);
            foreach (var state in conditionsStates)
            {
                Assert.IsTrue(expectedConditionsStates.Contains(state));
            }

            HashSet<IState> expectedEmptyConditionsStates = new HashSet<IState>
            {
                new State(0, 0, 0), new State(0, 0, 1), new State(0, 1, 0), new State(0, 1, 1), new State(0, 2, 0), new State(0, 2, 1),
                new State(1, 0, 0), new State(1, 0, 1), new State(1, 1, 0), new State(1, 1, 1), new State(1, 2, 0), new State(1, 2, 1),
                new State(2, 0, 0), new State(2, 0, 1), new State(2, 1, 0), new State(2, 1, 1), new State(2, 2, 0), new State(2, 2, 1)
            };
            var emptyConditionsStates = emptyConditions.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(18, emptyConditionsStates.Count);
            foreach (var state in emptyConditionsStates)
            {
                Assert.IsTrue(expectedEmptyConditionsStates.Contains(state));
            }

            var conditionsRelativeStates = conditions.GetCorrespondingRelativeStates(problem).ToList();
            Assert.AreEqual(1, conditionsRelativeStates.Count);
            Assert.IsTrue(conditionsRelativeStates.Contains(new RelativeState(0, 2, -1)));

            var emptyConditionsRelativeStates = emptyConditions.GetCorrespondingRelativeStates(problem).ToList();
            Assert.AreEqual(1, emptyConditionsRelativeStates.Count);
            Assert.IsTrue(emptyConditionsRelativeStates.Contains(new RelativeState(-1, -1, -1)));

            var simpleConditions = conditions.GetSimpleConditions().ToList();
            Assert.AreEqual(1, simpleConditions.Count);
            Assert.IsTrue(simpleConditions.First() == conditions);

            var conditionsClone = conditions.Clone();
            Assert.IsTrue(conditions != conditionsClone);
            Assert.IsTrue(conditions.GetHashCode() == conditionsClone.GetHashCode());
            Assert.IsTrue(conditions.Equals(conditionsClone));

            var emptyConditionsClone = emptyConditions.Clone();
            Assert.IsTrue(emptyConditions != emptyConditionsClone);
            Assert.IsTrue(emptyConditions.GetHashCode() == emptyConditionsClone.GetHashCode());
            Assert.IsTrue(emptyConditions.Equals(emptyConditionsClone));
        }

        [TestMethod]
        public void TC_ConditionsClause()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Conditions.sas"));
            Problem problem = new Problem(data);

            IConditions conditions = new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 2)), new Conditions(new Assignment(2, 0)));
            IConditions conditions1 = new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)));
            IConditions emptyConditions = new ConditionsClause();

            IState state0 = new State(0, 0, 0, 0);
            IState state1 = new State(0, 2, 3, 0);
            IState state2 = new State(1, 1, 1, 1);

            Assert.IsTrue(conditions.Evaluate(state0));
            Assert.IsTrue(conditions.Evaluate(state1));
            Assert.IsFalse(conditions.Evaluate(state2));
            Assert.IsFalse(emptyConditions.Evaluate(state0));

            var conjunction = conditions.ConjunctionWith(new Conditions(new Assignment(0, 0), new Assignment(2, 0)));
            Assert.IsTrue(conjunction.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 2), new Assignment(2, 0)), new Conditions(new Assignment(0, 0), new Assignment(2, 0)))));
            var conjunction1 = conditions1.ConjunctionWith(new ConditionsClause(new Conditions(new Assignment(2, 0)), new Conditions(new Assignment(1, 0))));
            Assert.IsTrue(conjunction1.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(2, 0)),
                                                                   new Conditions(new Assignment(1, 0), new Assignment(2, 0)),
                                                                   new Conditions(new Assignment(0, 0), new Assignment(1, 0)),
                                                                   new Conditions(new Assignment(1, 0)))));
            var conjunction2 = conditions1.ConjunctionWith(new Conditions(new Assignment(0, 1), new Assignment(1, 0)));
            Assert.IsTrue(conjunction2.Equals(new Conditions(new Assignment(0, 1), new Assignment(1, 0))));
            var conjunction3 = conditions1.ConjunctionWith(new Conditions(new Assignment(0, 1), new Assignment(1, 1)));
            Assert.IsTrue(conjunction3.Equals(new ConditionsContradiction()));
            var conjunction4 = conditions1.ConjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(conjunction4.Equals(new ConditionsContradiction()));
            var conjunction5 = emptyConditions.ConjunctionWith(new Conditions(new Assignment(0, 1)));
            Assert.IsTrue(conjunction5.Equals(new ConditionsContradiction()));

            var disjunction = conditions1.DisjunctionWith(new Conditions(new Assignment(2, 0)));
            Assert.IsTrue(disjunction.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)), new Conditions(new Assignment(2, 0)))));
            var disjunction1 = conditions1.DisjunctionWith(new ConditionsClause(new Conditions(new Assignment(2, 3)), new Conditions(new Assignment(0, 0))));
            Assert.IsTrue(disjunction1.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)), new Conditions(new Assignment(2, 3)))));
            var disjunction2 = conditions.DisjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(disjunction2.Equals(conditions));
            var disjunction3 = emptyConditions.DisjunctionWith(new Conditions(new Assignment(2, 0)));
            Assert.IsTrue(disjunction3.Equals(new ConditionsClause(new Conditions(new Assignment(2, 0)))));

            Assert.IsFalse(conditions.IsConflictedWith(new Conditions(new Assignment(0, 0), new Assignment(2, 1))));
            Assert.IsFalse(conditions.IsConflictedWith(new Conditions(new Assignment(0, 1), new Assignment(2, 0))));
            Assert.IsTrue(conditions.IsConflictedWith(new Conditions(new Assignment(0, 1), new Assignment(2, 1))));
            Assert.IsFalse(conditions.IsConflictedWith(new ConditionsClause(new Conditions(new Assignment(0, 1)), new Conditions(new Assignment(2, 1)))));
            Assert.IsTrue(conditions.IsConflictedWith(new ConditionsClause(new Conditions(new Assignment(0, 1), new Assignment(2, 1)), new Conditions(new Assignment(0, 2), new Assignment(2, 2)))));
            Assert.IsTrue(conditions.IsConflictedWith(new ConditionsContradiction()));
            Assert.IsTrue(emptyConditions.IsConflictedWith(new Conditions(new Assignment(0, 0))));

            var conditionsCopy = (IConditions)conditions.Clone();
            Assert.IsTrue(conditionsCopy.RemoveConstraint(new Assignment(0, 0)));
            Assert.IsFalse(conditionsCopy.RemoveConstraint(new Assignment(0, 0)));
            Assert.IsTrue(conditionsCopy.Equals(new ConditionsClause(new Conditions(new Assignment(1, 2)), new Conditions(new Assignment(2, 0)))));
            Assert.IsTrue(conditionsCopy.RemoveConstraint(new Assignment(1, 2)));
            Assert.IsTrue(conditionsCopy.Equals(new ConditionsClause(new Conditions())));
            Assert.IsFalse(emptyConditions.RemoveConstraint(new Assignment(0, 0)));

            Assert.IsTrue(conditions.IsConstrained(new Assignment(0, 0)));
            Assert.IsFalse(conditions.IsConstrained(new Assignment(0, 1)));
            Assert.IsFalse(emptyConditions.IsConstrained(new Assignment(0, 0)));

            IConditions conditions2 = new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(0, 1)));
            Assert.IsFalse(conditions2.IsConflictedWith(new Assignment(0, 0)));
            Assert.IsFalse(conditions2.IsConflictedWith(new Assignment(0, 1)));
            Assert.IsTrue(conditions2.IsConflictedWith(new Assignment(0, 2)));
            Assert.IsTrue(emptyConditions.IsConflictedWith(new Assignment(0, 0)));

            IConditions conditions3 = new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 0)), new Conditions(new Assignment(2, 0), new Assignment(1, 1)));
            Assert.AreEqual(EffectRelevance.RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(0, 0)));
            Assert.AreEqual(EffectRelevance.RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(1, 0)));
            Assert.AreEqual(EffectRelevance.RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(2, 0)));
            Assert.AreEqual(EffectRelevance.RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(1, 1)));
            Assert.AreEqual(EffectRelevance.ANTI_RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(0, 5)));
            Assert.AreEqual(EffectRelevance.ANTI_RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(1, 5)));
            Assert.AreEqual(EffectRelevance.ANTI_RELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(2, 5)));
            Assert.AreEqual(EffectRelevance.IRRELEVANT, conditions3.IsEffectAssignmentRelevant(new Assignment(3, 5)));
            Assert.AreEqual(EffectRelevance.IRRELEVANT, emptyConditions.IsEffectAssignmentRelevant(new Assignment(0, 0)));

            Assert.IsTrue(conditions3.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0), new Assignment(2, 0) }));
            Assert.IsFalse(conditions3.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0), new Assignment(2, 0), new Assignment(1, 1) }));
            Assert.IsFalse(emptyConditions.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0) }));

            StateLabels stateLabels = new StateLabels
            {
                {new Assignment(0, 0), 3}, {new Assignment(1, 2), 7}, {new Assignment(2, 0), 8}
            };
            Assert.AreEqual(8, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(10, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(1, conditions3.GetNotAccomplishedConstraintsCount(new State(1, 1, 1)));
            Assert.AreEqual(0, conditions3.GetNotAccomplishedConstraintsCount(new State(0, 0, 0)));
            Assert.AreEqual(2, conditions3.GetSize());

            HashSet<IState> expectedConditionsStates = new HashSet<IState>
            {
                new State(0, 2, 1),
                new State(0, 0, 0), new State(0, 1, 0), new State(0, 2, 0),
                new State(1, 0, 0), new State(1, 1, 0), new State(1, 2, 0),
                new State(2, 0, 0), new State(2, 1, 0), new State(2, 2, 0)
            };
            var conditionsStates = conditions.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(11, conditionsStates.Count);
            foreach (var state in conditionsStates)
            {
                Assert.IsTrue(expectedConditionsStates.Contains(state));
            }

            Assert.AreEqual(0, emptyConditions.GetCorrespondingStates(problem).Count());

            var conditionsRelativeStates = conditions.GetCorrespondingRelativeStates(problem).ToList();
            Assert.AreEqual(2, conditionsRelativeStates.Count);
            Assert.IsTrue(conditionsRelativeStates.Contains(new RelativeState(0, 2, -1)));
            Assert.IsTrue(conditionsRelativeStates.Contains(new RelativeState(-1, -1, 0)));

            Assert.AreEqual(0, emptyConditions.GetCorrespondingRelativeStates(problem).Count());

            var simpleConditions = conditions1.GetSimpleConditions().ToList();
            Assert.AreEqual(2, simpleConditions.Count);
            Assert.IsTrue(simpleConditions.Contains(new Conditions(new Assignment(0, 0))));
            Assert.IsTrue(simpleConditions.Contains(new Conditions(new Assignment(1, 0))));

            var conditionsClone = conditions.Clone();
            Assert.IsTrue(conditions != conditionsClone);
            Assert.IsTrue(conditions.GetHashCode() == conditionsClone.GetHashCode());
            Assert.IsTrue(conditions.Equals(conditionsClone));

            var emptyConditionsClone = emptyConditions.Clone();
            Assert.IsTrue(emptyConditions != emptyConditionsClone);
            Assert.IsTrue(emptyConditions.GetHashCode() == emptyConditionsClone.GetHashCode());
            Assert.IsTrue(emptyConditions.Equals(emptyConditionsClone));
        }

        [TestMethod]
        public void TC_ConditionsContradiction()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Conditions.sas"));
            Problem problem = new Problem(data);

            IConditions conditions = new ConditionsContradiction();
            IState state0 = new State(0, 0, 0, 0);

            Assert.IsFalse(conditions.Evaluate(state0));

            var conjunction = conditions.ConjunctionWith(new Conditions(new Assignment(0, 0), new Assignment(1, 0)));
            Assert.IsTrue(conjunction.Equals(new ConditionsContradiction()));
            var conjunction1 = conditions.ConjunctionWith(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0))));
            Assert.IsTrue(conjunction1.Equals(new ConditionsContradiction()));
            var conjunction2 = conditions.ConjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(conjunction2.Equals(new ConditionsContradiction()));

            var disjunction = conditions.DisjunctionWith(new Conditions(new Assignment(0, 0), new Assignment(1, 0)));
            Assert.IsTrue(disjunction.Equals(new Conditions(new Assignment(0, 0), new Assignment(1, 0))));
            var disjunction1 = conditions.DisjunctionWith(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0))));
            Assert.IsTrue(disjunction1.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)))));
            var disjunction2 = conditions.DisjunctionWith(new ConditionsContradiction());
            Assert.IsTrue(disjunction2.Equals(new ConditionsContradiction()));

            Assert.IsTrue(conditions.IsConflictedWith(new Conditions(new Assignment(0, 0), new Assignment(1, 0))));
            Assert.IsTrue(conditions.IsConflictedWith(new ConditionsClause(new Conditions(new Assignment(0, 0)), new Conditions(new Assignment(1, 0)))));
            Assert.IsTrue(conditions.IsConflictedWith(new ConditionsContradiction()));

            Assert.IsFalse(conditions.RemoveConstraint(new Assignment(0, 0)));
            Assert.IsFalse(conditions.IsConstrained(new Assignment(0, 0)));
            Assert.IsTrue(conditions.IsConflictedWith(new Assignment(0, 0)));
            Assert.AreEqual(EffectRelevance.ANTI_RELEVANT, conditions.IsEffectAssignmentRelevant(new Assignment(0, 0)));
            Assert.IsFalse(conditions.IsCompatibleWithMutexConstraints(new List<IAssignment> { new Assignment(0, 0), new Assignment(1, 0) }));

            StateLabels stateLabels = new StateLabels {{new Assignment(0, 0), 3}};
            Assert.AreEqual(int.MaxValue, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(int.MaxValue, conditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(int.MaxValue, conditions.GetNotAccomplishedConstraintsCount(new State(0, 0, 0)));
            Assert.AreEqual(int.MinValue, conditions.GetSize());

            Assert.AreEqual(0, conditions.GetCorrespondingStates(problem).Count());
            Assert.AreEqual(0, conditions.GetCorrespondingRelativeStates(problem).Count());

            var simpleConditions = conditions.GetSimpleConditions();
            Assert.AreEqual(0, simpleConditions.Count());

            var conditionsClone = conditions.Clone();
            Assert.IsTrue(conditions != conditionsClone);
            Assert.IsTrue(conditions.GetHashCode() == conditionsClone.GetHashCode());
            Assert.IsTrue(conditions.Equals(conditionsClone));
        }

        [TestMethod]
        public void TC_Effects()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Effects.sas"));
            Problem problem = new Problem(data);

            // effects structure

            Assert.AreEqual(3, problem.Operators.Count);
            var effects = problem.Operators[0].GetEffects();

            Assert.AreEqual(4, effects.Count);
            Assert.IsTrue(effects[0] is PrimitiveEffect);
            Assert.IsTrue(effects[1] is PrimitiveEffect);
            Assert.IsTrue(effects[2] is ConditionalEffect);
            Assert.IsTrue(effects[3] is ConditionalEffect);

            // single effect functions

            IState state0 = new State(0, 0, 0);
            IState state1 = new State(1, 0, 0);
            IState state2 = new State(0, 0, 0);

            IConditions conditions0 = new Conditions(new Assignment(0, 1), new Assignment(1, 1));
            IConditions conditions1 = new Conditions(new Assignment(0, 2));
            IConditions conditions2 = new Conditions(new Assignment(2, 1), new Assignment(1, 0));
            IConditions conditions3 = new ConditionsClause(new Conditions(new Assignment(0, 1)), new Conditions(new Assignment(0, 2)));
            IConditions conditions4 = new ConditionsClause(new Conditions(new Assignment(0, 2)), new Conditions(new Assignment(0, 3)));
            IConditions conditions5 = new ConditionsContradiction();
            IConditions conditions7 = new ConditionsClause(new Conditions(new Assignment(2, 0)), new Conditions(new Assignment(2, 2)));
            IConditions conditions8 = new Conditions(new Assignment(2, 1));
            IConditions conditions9 = new Conditions(new Assignment(2, 1), new Assignment(1, 1));
            IConditions conditions10 = new ConditionsClause(new Conditions(new Assignment(2, 2)), new Conditions(new Assignment(2, 1)));

            IRelativeState relativeState0 = new RelativeState(1, 1, -1);
            IRelativeState relativeState1 = new RelativeState(2, -1, -1);
            IRelativeState relativeState2 = new RelativeState(-1, 0, 1);
            IRelativeState relativeState8 = new RelativeState(-1, -1, 1);

            var effect0 = effects[0];
            Assert.IsTrue(effect0.GetConditions() == null);
            Assert.IsTrue(effect0.GetAssignment().Equals(new Assignment(0, 1)));
            Assert.IsTrue(effect0.IsApplicable(state0));
            effect0.Apply(state0);
            Assert.IsTrue(state0.Equals(new State(1, 0, 0)));
            Assert.IsTrue(effect0.IsRelevant(conditions0) == EffectRelevance.RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(conditions1) == EffectRelevance.ANTI_RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(conditions2) == EffectRelevance.IRRELEVANT);
            Assert.IsTrue(effect0.IsRelevant(conditions3) == EffectRelevance.RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(conditions4) == EffectRelevance.ANTI_RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(conditions5) == EffectRelevance.ANTI_RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(relativeState0) == EffectRelevance.RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(relativeState1) == EffectRelevance.ANTI_RELEVANT);
            Assert.IsTrue(effect0.IsRelevant(relativeState2) == EffectRelevance.IRRELEVANT);
            Assert.IsTrue(effect0.ApplyBackwards(conditions0).Equals(new Conditions(new Assignment(1, 1))));
            Assert.IsTrue(effect0.ApplyBackwards(conditions1).Equals(new ConditionsContradiction()));
            Assert.IsTrue(effect0.ApplyBackwards(conditions2).Equals(new Conditions(new Assignment(2, 1), new Assignment(1, 0))));
            Assert.IsTrue(effect0.ApplyBackwards(conditions3).Equals(new ConditionsClause(new Conditions())));
            Assert.IsTrue(effect0.ApplyBackwards(conditions4).Equals(new ConditionsContradiction()));
            Assert.IsTrue(effect0.ApplyBackwards(conditions5).Equals(new ConditionsContradiction()));
            Assert.IsTrue(effect0.ApplyBackwards((IRelativeState)relativeState0.Clone()).Equals(new RelativeState(-1, 1, -1)));
            Assert.IsTrue(effect0.ApplyBackwards((IRelativeState)relativeState2.Clone()).Equals(new RelativeState(-1, 0, 1)));

            var effect2 = effects[2];
            Assert.IsTrue(effect2.GetConditions().Equals(new Conditions(new Assignment(0, 1), new Assignment(1, 0))));
            Assert.IsTrue(effect2.GetAssignment().Equals(new Assignment(2, 1)));
            Assert.IsTrue(effect2.IsApplicable(state1));
            Assert.IsFalse(effect2.IsApplicable(state2));
            effect2.Apply(state1);
            Assert.IsTrue(state1.Equals(new State(1, 0, 1)));
            Assert.IsTrue(effect2.IsRelevant(conditions0) == EffectRelevance.IRRELEVANT);
            Assert.IsTrue(effect2.IsRelevant(conditions2) == EffectRelevance.RELEVANT);
            Assert.IsTrue(effect2.IsRelevant(conditions7) == EffectRelevance.IRRELEVANT);
            Assert.IsTrue(effect2.IsRelevant(relativeState0) == EffectRelevance.IRRELEVANT);
            Assert.IsTrue(effect2.IsRelevant(relativeState2) == EffectRelevance.RELEVANT);
            Assert.IsTrue(effect2.ApplyBackwards(conditions8).Equals(new ConditionsClause(new Conditions(new Assignment(2, 1)), new Conditions(new Assignment(0, 1), new Assignment(1, 0)))));
            Assert.IsTrue(effect2.ApplyBackwards(conditions9).Equals(new Conditions(new Assignment(2, 1), new Assignment(1, 1))));
            Assert.IsTrue(effect2.ApplyBackwards(conditions10).Equals(new ConditionsClause(new Conditions(new Assignment(2, 2)), new Conditions(new Assignment(2, 1)), new Conditions(new Assignment(0, 1), new Assignment(1, 0)))));
            Assert.IsTrue(effect2.ApplyBackwards(conditions5).Equals(new ConditionsContradiction()));
            Assert.IsTrue(effect2.ApplyBackwards(relativeState0) == null);
            Assert.IsTrue(effect2.ApplyBackwards(relativeState8).Equals(new RelativeState(1, 0, -1)));

            // entire operator effects tests

            var effects1 = problem.Operators[1].GetEffects();
            var preconditions1 = problem.Operators[1].GetPreconditions();
            var effects2 = problem.Operators[2].GetEffects();
            var preconditions2 = problem.Operators[2].GetPreconditions();

            int value;
            Assert.IsTrue(effects1.IsVariableAffected(0, out value) && value == 1);
            Assert.IsTrue(!effects1.IsVariableAffected(2, out value) && value == Assignment.InvalidValue);

            IState state3 = new State(0, 2, 0);
            IState state4 = new State(0, 0, 0);
            IState state5 = effects1.Apply(state3);
            Assert.IsTrue(state5.Equals(new State(1, 2, 0)));
            IState state6 = effects1.Apply(state4);
            Assert.IsTrue(state6.Equals(new State(1, 1, 0)));

            IConditions conditions12 = new Conditions(new Assignment(0, 1));
            IRelativeState relativeState12 = new RelativeState(1, -1, -1);
            Assert.IsTrue(effects1.IsRelevant(conditions12, preconditions1));
            Assert.IsTrue(effects1.IsRelevant(relativeState12, preconditions1));
            IConditions conditions13 = new Conditions(new Assignment(0, 2));
            IRelativeState relativeState13 = new RelativeState(2, -1, -1);
            Assert.IsFalse(effects1.IsRelevant(conditions13, preconditions1));
            Assert.IsFalse(effects1.IsRelevant(relativeState13, preconditions1));
            IConditions conditions14 = new Conditions(new Assignment(1, 1));
            IRelativeState relativeState14 = new RelativeState(-1, 1, -1);
            Assert.IsTrue(effects1.IsRelevant(conditions14, preconditions1));
            Assert.IsTrue(effects1.IsRelevant(relativeState14, preconditions1));
            IConditions conditions15 = new Conditions(new Assignment(1, 1), new Assignment(0, 0));
            IRelativeState relativeState15 = new RelativeState(0, 1, -1);
            Assert.IsTrue(effects2.IsRelevant(conditions15, preconditions2));
            Assert.IsTrue(effects2.IsRelevant(relativeState15, preconditions2));
            IConditions conditions16 = new Conditions(new Assignment(1, 1), new Assignment(0, 1));
            IRelativeState relativeState16 = new RelativeState(1, 1, -1);
            Assert.IsFalse(effects2.IsRelevant(conditions16, preconditions2));
            Assert.IsFalse(effects2.IsRelevant(relativeState16, preconditions2));

            var conditions17 = effects1.ApplyBackwards(new Conditions(new Assignment(0, 1)), preconditions1);
            Assert.IsTrue(conditions17.Equals(new Conditions(new Assignment(0, 0))));
            var relativeStates17 = effects1.ApplyBackwards(new RelativeState(1, -1, -1), preconditions1).ToList();
            Assert.IsTrue(relativeStates17.Count == 1 && relativeStates17.First().Equals(new RelativeState(0, -1, -1)));

            var conditions18 = effects1.ApplyBackwards(new Conditions(new Assignment(1, 1)), preconditions1);
            Assert.IsTrue(conditions18.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 0)), new Conditions(new Assignment(0, 0), new Assignment(1, 1)))));
            var relativeStates18 = effects1.ApplyBackwards(new RelativeState(-1, 1, -1), preconditions1).ToList();
            Assert.IsTrue(relativeStates18.Count == 2 && relativeStates18.Contains(new RelativeState(0, 0, -1)) && relativeStates18.Contains(new RelativeState(0, 1, -1)));

            var conditions19 = effects1.ApplyBackwards(new Conditions(new Assignment(0, 1), new Assignment(1, 1)), preconditions1);
            Assert.IsTrue(conditions19.Equals(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 0)), new Conditions(new Assignment(0, 0), new Assignment(1, 1)))));
            var relativeStates19 = effects1.ApplyBackwards(new RelativeState(1, 1, -1), preconditions1).ToList();
            Assert.IsTrue(relativeStates19.Count == 2 && relativeStates19.Contains(new RelativeState(0, 0, -1)) && relativeStates19.Contains(new RelativeState(0, 1, -1)));
        }

        [TestMethod]
        public void TC_MutexGroups()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_MutexGroups.sas"));
            Problem problem = new Problem(data);

            Assert.AreEqual(2, problem.MutexGroups.Count);
            Assert.AreEqual(3, problem.MutexGroups[0].Count);
            Assert.AreEqual(3, problem.MutexGroups[1].Count);

            int itemIndex;
            Assert.IsTrue(problem.MutexGroups[0].TryFindAffectedMutexItem(new Assignment(0, 2), out itemIndex));
            Assert.AreEqual(2, itemIndex);

            Assert.IsFalse(problem.MutexGroups[0].TryFindAffectedMutexItem(new Assignment(6, 6), out itemIndex));
            Assert.AreEqual(-1, itemIndex);
        }

        [TestMethod]
        public void TC_MutexChecker()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_MutexChecker.sas"));
            Problem problem = new Problem(data);

            MutexChecker checker = new MutexChecker(problem.MutexGroups);

            Assert.IsTrue(checker.CheckState(new State(1, 0, 0)));
            Assert.IsFalse(checker.CheckState(new State(1, 1, 0)));

            Assert.IsTrue(checker.CheckConditions(new Conditions(new Assignment(0, 1))));
            Assert.IsFalse(checker.CheckConditions(new Conditions(new Assignment(0, 1), new Assignment(1, 1))));
            Assert.IsTrue(checker.CheckConditions(new ConditionsClause(new Conditions(new Assignment(0, 1), new Assignment(1, 1)), new Conditions(new Assignment(1, 1)))));
            Assert.IsFalse(checker.CheckConditions(new ConditionsClause(new Conditions(new Assignment(0, 1), new Assignment(1, 1)), new Conditions(new Assignment(1, 1), new Assignment(2, 1)))));
            Assert.IsFalse(checker.CheckConditions(new ConditionsContradiction()));

            Assert.IsTrue(checker.CheckSuccessorCompatibility(new State(0, 0, 0), problem.Operators[0]));
            Assert.IsFalse(checker.CheckSuccessorCompatibility(new State(0, 1, 0), problem.Operators[0]));
            Assert.IsTrue(checker.CheckSuccessorCompatibility(new State(1, 0, 0), problem.Operators[0]));
            Assert.IsFalse(checker.CheckSuccessorCompatibility(new State(1, 0, 2), problem.Operators[0]));

            Assert.IsTrue(checker.CheckPredecessorCompatibility(new Conditions(new Assignment(0, 0)), problem.Operators[1]));
            Assert.IsFalse(checker.CheckPredecessorCompatibility(new Conditions(new Assignment(0, 0), new Assignment(2, 1)), problem.Operators[1]));
            Assert.IsFalse(checker.CheckPredecessorCompatibility(new Conditions(new Assignment(0, 0), new Assignment(1, 2)), problem.Operators[1]));
            Assert.IsTrue(checker.CheckPredecessorCompatibility(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(2, 1)), new Conditions(new Assignment(1, 1))), problem.Operators[1]));
            Assert.IsFalse(checker.CheckPredecessorCompatibility(new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(2, 1)), new Conditions(new Assignment(0, 2))), problem.Operators[1]));
            Assert.IsFalse(checker.CheckPredecessorCompatibility(new ConditionsContradiction(), problem.Operators[1]));
        }

        [TestMethod]
        public void TC_OperatorDecisionTreeBuilder()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_OperatorDecisionTreeBuilder.sas"));
            Problem problem = new Problem(data);

            // applicability tree
            {
                var tree = OperatorDecisionTreeBuilder.BuildApplicabilityTree(problem.Operators, problem.Variables);

                var node0 = tree as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0);
                Assert.AreEqual(0, node0.DecisionVariable);
                Assert.AreEqual(3, node0.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                var node00 = node0.OperatorsByDecisionVariableValue[0];
                var node0N = node0.OperatorsIndependentOnDecisionVariable;

                var node00X1 = node00 as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node00X1);
                Assert.AreEqual(1, node00X1.DecisionVariable);
                Assert.AreEqual(3, node00X1.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node00X1.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeLeafNode);
                Assert.AreEqual(2, ((OperatorDecisionTreeLeafNode)node00X1.OperatorsByDecisionVariableValue[0]).Operators.Count);
                Assert.AreEqual("operatorA", ((OperatorDecisionTreeLeafNode)node00X1.OperatorsByDecisionVariableValue[0]).Operators[0].GetName());
                Assert.AreEqual("operatorE", ((OperatorDecisionTreeLeafNode)node00X1.OperatorsByDecisionVariableValue[0]).Operators[1].GetName());
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node00X1.OperatorsIndependentOnDecisionVariable).Operators.Count);
                Assert.AreEqual("operatorB", ((OperatorDecisionTreeLeafNode)node00X1.OperatorsIndependentOnDecisionVariable).Operators[0].GetName());

                var node0Nx1 = node0N as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0Nx1);
                Assert.AreEqual(1, node0Nx1.DecisionVariable);
                Assert.AreEqual(3, node0Nx1.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0Nx1.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0Nx1.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                var node0Nx11 = node0Nx1.OperatorsByDecisionVariableValue[1];
                var node0Nx1N = node0Nx1.OperatorsIndependentOnDecisionVariable;

                var node0Nx11X2 = node0Nx11 as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0Nx11X2);
                Assert.AreEqual(2, node0Nx11X2.DecisionVariable);
                Assert.AreEqual(2, node0Nx11X2.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0Nx11X2.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0Nx11X2.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node0Nx11X2.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeEmptyLeafNode);
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node0Nx11X2.OperatorsByDecisionVariableValue[1]).Operators.Count);
                Assert.AreEqual("operatorC", ((OperatorDecisionTreeLeafNode)node0Nx11X2.OperatorsByDecisionVariableValue[1]).Operators[0].GetName());

                var node0Nx1Nx2 = node0Nx1N as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0Nx1Nx2);
                Assert.AreEqual(2, node0Nx1Nx2.DecisionVariable);
                Assert.AreEqual(2, node0Nx1Nx2.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0Nx1Nx2.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node0Nx1Nx2.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0Nx1Nx2.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeEmptyLeafNode);
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node0Nx1Nx2.OperatorsByDecisionVariableValue[0]).Operators.Count);
                Assert.AreEqual("operatorD", ((OperatorDecisionTreeLeafNode)node0Nx1Nx2.OperatorsByDecisionVariableValue[0]).Operators[0].GetName());
            }

            // relevance tree
            {
                var tree = OperatorDecisionTreeBuilder.BuildRelevanceTree(problem.Operators, problem.Variables);

                var node0 = tree as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0);
                Assert.AreEqual(0, node0.DecisionVariable);
                Assert.AreEqual(3, node0.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                var node00 = node0.OperatorsByDecisionVariableValue[0];
                var node01 = node0.OperatorsByDecisionVariableValue[1];
                var node0N = node0.OperatorsIndependentOnDecisionVariable;

                var node00X1 = node00 as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node00X1);
                Assert.AreEqual(1, node00X1.DecisionVariable);
                Assert.AreEqual(3, node00X1.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node00X1.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node00X1.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeLeafNode);
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node00X1.OperatorsByDecisionVariableValue[0]).Operators.Count);
                Assert.AreEqual("operatorC", ((OperatorDecisionTreeLeafNode)node00X1.OperatorsByDecisionVariableValue[0]).Operators[0].GetName());
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node00X1.OperatorsIndependentOnDecisionVariable).Operators.Count);
                Assert.AreEqual("operatorD", ((OperatorDecisionTreeLeafNode)node00X1.OperatorsIndependentOnDecisionVariable).Operators[0].GetName());

                var node01X1 = node01 as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node01X1);
                Assert.AreEqual(1, node01X1.DecisionVariable);
                Assert.AreEqual(3, node01X1.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node01X1.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node01X1.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node01X1.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node01X1.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeLeafNode);
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node01X1.OperatorsByDecisionVariableValue[2]).Operators.Count);
                Assert.AreEqual("operatorA", ((OperatorDecisionTreeLeafNode)node01X1.OperatorsByDecisionVariableValue[2]).Operators[0].GetName());
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node01X1.OperatorsIndependentOnDecisionVariable).Operators.Count);
                Assert.AreEqual("operatorB", ((OperatorDecisionTreeLeafNode)node01X1.OperatorsIndependentOnDecisionVariable).Operators[0].GetName());

                var node0Nx1 = node0N as OperatorDecisionTreeInnerNode;
                Assert.IsNotNull(node0Nx1);
                Assert.AreEqual(1, node0Nx1.DecisionVariable);
                Assert.AreEqual(3, node0Nx1.OperatorsByDecisionVariableValue.Length);
                Assert.IsTrue(node0Nx1.OperatorsByDecisionVariableValue[0] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0Nx1.OperatorsByDecisionVariableValue[1] is OperatorDecisionTreeLeafNode);
                Assert.IsTrue(node0Nx1.OperatorsByDecisionVariableValue[2] is OperatorDecisionTreeEmptyLeafNode);
                Assert.IsTrue(node0Nx1.OperatorsIndependentOnDecisionVariable is OperatorDecisionTreeEmptyLeafNode);
                Assert.AreEqual(1, ((OperatorDecisionTreeLeafNode)node0Nx1.OperatorsByDecisionVariableValue[1]).Operators.Count);
                Assert.AreEqual("operatorE", ((OperatorDecisionTreeLeafNode)node0Nx1.OperatorsByDecisionVariableValue[1]).Operators[0].GetName());
            }
        }

        [TestMethod]
        public void TC_Operators()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Operators.sas"));
            Problem problem = new Problem(data);

            // operator structure

            var oper = (Operator)problem.Operators[0];
            Assert.AreEqual("operator0", oper.GetName());
            Assert.AreEqual(0, oper.Id);
            Assert.AreEqual(5, oper.GetCost());

            Conditions conditions0 = oper.GetPreconditions();
            Assert.AreEqual(2, conditions0.Count);
            Assert.IsTrue(conditions0.Contains(new Assignment(0, 0)));
            Assert.IsTrue(conditions0.Contains(new Assignment(1, 0)));

            Assert.AreEqual(3, oper.GetEffects().Count);
            Assert.IsTrue(oper.Effects[0] is PrimitiveEffect);
            Assert.IsTrue(oper.Effects[1] is PrimitiveEffect);
            Assert.IsTrue(oper.Effects[2] is ConditionalEffect);
            Assert.IsTrue(((PrimitiveEffect)oper.Effects[0]).Assignment.Equals(new Assignment(0, 1)));
            Assert.IsTrue(((PrimitiveEffect)oper.Effects[1]).Assignment.Equals(new Assignment(1, 1)));
            Assert.IsTrue(((ConditionalEffect)oper.Effects[2]).Conditions.Equals(new Conditions(new Assignment(2, 1))));
            Assert.IsTrue(((ConditionalEffect)oper.Effects[2]).Assignment.Equals(new Assignment(2, 2)));

            // operator functions

            IState state = new State(0, 0, 0);
            IState state1 = new State(0, 1, 0);
            IState state2 = new State(0, 0, 1);

            Assert.IsTrue(oper.IsApplicable(state));
            Assert.IsFalse(oper.IsApplicable(state1));

            Assert.IsTrue(oper.Apply(state).Equals(new State(1, 1, 0)));
            Assert.IsTrue(oper.Apply(state2).Equals(new State(1, 1, 2)));

            IConditions conditions = new Conditions(new Assignment(0, 1));
            IConditions conditions1 = new Conditions(new Assignment(0, 1), new Assignment(1, 2));
            IConditions conditions2 = new Conditions(new Assignment(2, 2));
            IRelativeState relativeState = new RelativeState(1, -1, -1);
            IRelativeState relativeState1 = new RelativeState(1, 2, -1);
            IRelativeState relativeState2 = new RelativeState(-1, -1, 2);

            Assert.IsTrue(oper.IsRelevant(conditions));
            Assert.IsFalse(oper.IsRelevant(conditions1));
            Assert.IsTrue(oper.IsRelevant(conditions2));
            Assert.IsTrue(oper.IsRelevant(relativeState));
            Assert.IsFalse(oper.IsRelevant(relativeState1));
            Assert.IsTrue(oper.IsRelevant(relativeState2));

            Assert.IsTrue(oper.ApplyBackwards(conditions).Equals(new Conditions(new Assignment(0, 0), new Assignment(1, 0))));
            IConditions newConditions = (IConditions)oper.ApplyBackwards(conditions2);
            Assert.IsTrue(newConditions.Evaluate(new State(0, 0, 2)));
            Assert.IsTrue(newConditions.Evaluate(new State(0, 0, 1)));

            var newRelativeStates = oper.ApplyBackwards(relativeState).ToList();
            Assert.IsTrue(newRelativeStates.Count == 1 && newRelativeStates.First().Equals(new RelativeState(0, 0, -1)));
            var newRelativeStates2 = oper.ApplyBackwards(relativeState2).ToList();
            Assert.IsTrue(newRelativeStates2.Count == 2 && newRelativeStates2.Contains(new RelativeState(0, 0, 1)) && newRelativeStates2.Contains(new RelativeState(0, 0, 2)));

            var oper1 = problem.Operators[1];
            StateLabels stateLabels = new StateLabels
            {
                {new Assignment(0, 0), 3}, {new Assignment(1, 0), 7}, {new Assignment(2, 0), 2}
            };
            Assert.AreEqual(7, oper1.ComputePlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(12, oper1.ComputePlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            var operClone = oper.Clone();
            Assert.IsTrue(oper != operClone);
            Assert.IsTrue(oper.GetHashCode() == operClone.GetHashCode());
            Assert.IsTrue(oper.Equals(operClone));
        }

        [TestMethod]
        public void TC_PatternDatabase()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_PatternDatabase.sas"));
            Problem problem = new Problem(data);

            PatternValuesDistances valuesDistances = new PatternValuesDistances
            {
                {new[] {3, 3, 3}, 9},
                {new[] {2, 2, 2}, 7}
            };
            Assert.AreEqual(7, valuesDistances.GetDistance(new[] { 2, 2, 2 }));
            Assert.AreEqual(PatternValuesDistances.MaxDistance, valuesDistances.GetDistance(new[] { 2, 1, 2 }));

            Pattern pattern = new Pattern(new[] { 1, 2, 3 }, valuesDistances);
            Assert.AreEqual(9, pattern.GetDistance(new State(0, 3, 3, 3, 0)));
            Assert.AreEqual(PatternValuesDistances.MaxDistance, pattern.GetDistance(new State(0, 0, 0, 0, 0)));

            var userPatterns = new List<HashSet<int>> { new HashSet<int> { 0, 1 } };
            PatternDatabase database = new PatternDatabase(problem, false, userPatterns);
            Assert.AreEqual(1, database.Count);
            Assert.IsTrue(Planner.CollectionsEquality.Equals(database[0].PatternVariables, new[] { 0, 1 }));
            Assert.AreEqual(5, database[0].PatternValuesDistances.Count);
            Assert.AreEqual("{[0, 1] = 0}, {[1, 1] = 0}, {[2, 1] = 0}, {[1, 0] = 1}, {[1, 2] = 1}", database[0].PatternValuesDistances.ToString());

            Assert.AreEqual(PatternValuesDistances.MaxDistance, database.GetValue(new State(0, 0, 0)));
            Assert.AreEqual(0, database.GetValue(new State(0, 1, 0)));
            Assert.AreEqual(1, database.GetValue(new State(1, 0, 0)));
        }

        [TestMethod]
        public void TC_PatternsFinder()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_PatternsFinder.sas"));
            Problem problem = new Problem(data);

            PatternsFinder finder = new PatternsFinder(problem);
            var patterns = finder.FindAdditivePatterns();

            Assert.AreEqual(2, patterns.Count);
            Assert.IsTrue(patterns[0].SetEquals(new HashSet<int> { 0, 1, 2 }));
            Assert.IsTrue(patterns[1].SetEquals(new HashSet<int> { 4 }));

            PatternDatabase database = new PatternDatabase(problem);

            Assert.AreEqual(2, database.Count);
            Assert.IsTrue(Planner.CollectionsEquality.Equals(database[0].PatternVariables, new[] { 0, 1, 2 }));
            Assert.IsTrue(Planner.CollectionsEquality.Equals(database[1].PatternVariables, new[] { 4 }));
        }

        [TestMethod]
        public void TC_Problem()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Problem.sas"));
            Problem problem = new Problem(data);

            Assert.AreEqual("", problem.GetDomainName());
            Assert.AreEqual("TC_Problem", problem.GetProblemName());
            Assert.AreEqual(GetFilePath("TC_Problem.sas"), problem.GetInputFilePath());

            Assert.IsTrue(problem.GetInitialState().Equals(new State(0, 0, 0, 0)));
            Assert.IsTrue(problem.GetGoalConditions().Equals(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 1))));

            Assert.IsTrue(problem.IsGoalState(new State(1, 1, 1, 0)));
            Assert.IsFalse(problem.IsGoalState(new State(1, 1, 0, 0)));
            Assert.IsFalse(problem.IsGoalState(new State(1, 1, 1, 1))); // failed on mutex group 3

            Assert.IsTrue(problem.IsStartConditions(new Conditions(new Assignment(0, 0), new Assignment(1, 0))));
            Assert.IsFalse(problem.IsStartConditions(new Conditions(new Assignment(0, 0), new Assignment(1, 1))));

            Assert.IsTrue(problem.IsStartRelativeState(new RelativeState(0, 0, -1, -1)));
            Assert.IsFalse(problem.IsStartRelativeState(new RelativeState(0, 1, -1, -1)));

            Assert.AreEqual(0, problem.GetNotAccomplishedGoalsCount(new State(1, 1, 1, 0)));
            Assert.AreEqual(2, problem.GetNotAccomplishedGoalsCount(new State(0, 0, 1, 0)));
            Assert.AreEqual(0, problem.GetNotAccomplishedGoalsCount(new Conditions(new Assignment(0, 0), new Assignment(1, 0), new Assignment(2, 0), new Assignment(3, 0))));
            Assert.AreEqual(2, problem.GetNotAccomplishedGoalsCount(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 0))));

            HashSet<IState> successorStates = new HashSet<IState>();
            foreach (var successor in problem.GetSuccessors(problem.InitialState))
            {
                successorStates.Add((IState)successor.GetSuccessorState());
            }
            Assert.AreEqual(3, successorStates.Count);
            Assert.IsTrue(successorStates.Contains(new State(1, 0, 0, 0)));
            Assert.IsTrue(successorStates.Contains(new State(0, 0, 1, 1)));
            Assert.IsTrue(successorStates.Contains(new State(2, 2, 2, 1)));
            // filtered by mutex group 2: new State(0, 1, 0, 0)

            HashSet<IConditions> predecessorConditions = new HashSet<IConditions>();
            foreach (var predecessor in problem.GetPredecessors(problem.GoalConditions))
            {
                predecessorConditions.Add((IConditions)predecessor.GetPredecessorConditions());
            }
            Assert.AreEqual(2, predecessorConditions.Count);
            Assert.IsTrue(predecessorConditions.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 0), new Assignment(2, 1))));
            Assert.IsTrue(predecessorConditions.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 0))));
            // filtered by mutex group 2: new Conditions(new Assignment(0, 0), new Assignment(1, 1), new Assignment(2, 1)))

            Assert.AreEqual(0, problem.GetSuccessors(new State(1, 1, 1)).Count());
            Assert.AreEqual(0, problem.GetPredecessors(new Conditions(new Assignment(0, 0))).Count());

            var successors = new List<Planner.ISuccessor>(problem.GetNextSuccessors(problem.InitialState, 2));
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(problem.GetNextSuccessors(new State(1, 1, 1), 50));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(problem.GetNextSuccessors(problem.InitialState, 4));
            Assert.AreEqual(1, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(problem.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(problem.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(3, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));

            var predecessors = new List<Planner.IPredecessor>(problem.GetNextPredecessors(problem.GoalConditions, 1));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(problem.GetNextPredecessors(new Conditions(new Assignment(0, 0)), 50));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(problem.GetNextPredecessors(problem.GoalConditions, 5));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(problem.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(problem.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));

            Planner.ISuccessor randomSuccessor = problem.GetRandomSuccessor(problem.InitialState);
            Assert.IsNotNull(randomSuccessor);
            Assert.IsTrue(successorStates.Contains(randomSuccessor.GetSuccessorState()));
            randomSuccessor = problem.GetRandomSuccessor(new State(1, 1, 1));
            Assert.IsNull(randomSuccessor);

            Planner.IPredecessor randomPredecessor = problem.GetRandomPredecessor(problem.GoalConditions);
            Assert.IsNotNull(randomPredecessor);
            Assert.IsTrue(predecessorConditions.Contains(randomPredecessor.GetPredecessorConditions()));
            randomPredecessor = problem.GetRandomPredecessor(new Conditions(new Assignment(0, 0)));
            Assert.IsNull(randomPredecessor);

            HashSet<IConditions> predecessors2 = new HashSet<IConditions>();
            HashSet<IState> predecessors2States = new HashSet<IState>();
            foreach (var predecessor in problem.GetPredecessors(new State(1, 1, 1, 1)))
            {
                predecessors2.Add((IConditions)predecessor.GetPredecessorConditions());

                foreach (var state in predecessor.GetPredecessorStates(problem))
                {
                    predecessors2States.Add((IState)state);
                }
            }
            Assert.AreEqual(1, predecessors2.Count);
            Assert.IsTrue(predecessors2.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 0), new Assignment(3, 1))));

            Assert.AreEqual(1, predecessors2States.Count);
            Assert.IsTrue(predecessors2States.Contains(new State(1, 1, 0, 1)));

            HashSet<IRelativeState> predecessors3 = new HashSet<IRelativeState>();
            foreach (var predecessor in problem.GetPredecessors(new RelativeState(1, 1, -1)))
            {
                foreach (var state in predecessor.GetPredecessorRelativeStates())
                {
                    predecessors3.Add((IRelativeState)state);
                }
            }

            Assert.AreEqual(2, predecessors3.Count);
            Assert.IsTrue(predecessors3.Contains(new RelativeState(0, 1, -1)));
            Assert.IsTrue(predecessors3.Contains(new RelativeState(1, 0, -1)));

            var succStates = problem.GetSuccessorStates(new State(0, 0, 0, 0)).ToList();
            Assert.AreEqual(3, succStates.Count);
            Assert.IsTrue(succStates.Contains(new State(1, 0, 0, 0)));
            Assert.IsTrue(succStates.Contains(new State(0, 0, 1, 1)));
            Assert.IsTrue(succStates.Contains(new State(2, 2, 2, 1)));

            var predStates = problem.GetPredecessorStates(new State(1, 1, 1, 1)).ToList();
            Assert.AreEqual(1, predStates.Count);
            Assert.IsTrue(predStates.Contains(new State(1, 1, 0, 1)));

            Assert.IsNotNull(problem.GetRelaxedProblem());
            Assert.IsNotNull(problem.GetPatternDatabase());
        }

        [TestMethod]
        public void TC_PredecessorsGenerator()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_PredecessorsGenerator.sas"));
            Problem problem = new Problem(data);

            PredecessorsGenerator generator = new PredecessorsGenerator(problem.Operators, problem.Variables);

            Func<IConditions, HashSet<Planner.IConditions>> getPredecessors = conditions =>
            {
                var predecessorsSet = new HashSet<Planner.IConditions>();
                foreach (var predecessor in generator.GetPredecessors(conditions))
                {
                    predecessorsSet.Add(predecessor.GetPredecessorConditions());
                }
                return predecessorsSet;
            };

            var predecessors = getPredecessors(new Conditions(new Assignment(0, 1), new Assignment(1, 1)));
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(new Conditions(new Assignment(0, 0), new Assignment(1, 1))));
            Assert.IsTrue(predecessors.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 0))));

            predecessors = getPredecessors(new Conditions(new Assignment(2, 2)));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(new ConditionsClause(new Conditions(new Assignment(2, 2), new Assignment(0, 2)), new Conditions(new Assignment(3, 0), new Assignment(0, 2)))));

            predecessors = getPredecessors(new ConditionsClause(new Conditions(new Assignment(0, 1)), new Conditions(new Assignment(1, 1))));
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(new Conditions(new Assignment(0, 0))));
            Assert.IsTrue(predecessors.Contains(new Conditions(new Assignment(1, 0))));

            predecessors = getPredecessors(new ConditionsClause(new Conditions(new Assignment(2, 2)), new Conditions(new Assignment(1, 2))));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(new ConditionsClause(new Conditions(new Assignment(2, 2), new Assignment(0, 2)), new Conditions(new Assignment(1, 2), new Assignment(0, 2)), new Conditions(new Assignment(3, 0), new Assignment(0, 2)))));

            predecessors = getPredecessors(new ConditionsContradiction());
            Assert.AreEqual(0, predecessors.Count);
        }
        [TestMethod]
        public void TC_RedBlackState()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_RedBlackState.sas"));
            Problem problem = new Problem(data);

            problem.Variables[1].Abstracted = true;
            var state = new RedBlackState((State)problem.GetInitialState(), problem);

            Assert.AreEqual(0, state.GetValue(0));
            Assert.AreEqual(1, state.GetValue(1));
            Assert.AreEqual(2, state.GetValue(2));

            Assert.IsTrue(state.HasValue(0, 0));
            Assert.IsTrue(state.HasValue(1, 1));
            Assert.IsTrue(state.HasValue(2, 2));

            state.SetValue(0, 2);
            state.SetValue(1, 2);
            Assert.IsFalse(state.HasValue(0, 0));
            Assert.IsTrue(state.HasValue(0, 2));
            Assert.IsTrue(state.HasValue(1, 1));
            Assert.IsTrue(state.HasValue(1, 2));

            Assert.AreEqual(1, state.GetAllValues(0).Length);
            Assert.AreEqual(2, state.GetAllValues(0)[0]);
            Assert.AreEqual(2, state.GetAllValues(1).Length);
            Assert.AreEqual(1, state.GetAllValues(1)[0]);
            Assert.AreEqual(2, state.GetAllValues(1)[1]);
            Assert.IsTrue(state.GetAllValues().SequenceEqual(new[] { 2, 1, 2 }));

            Assert.IsTrue(state.GetValues(new[] { 2, 0 }).SequenceEqual(new[] { 2, 2 }));
            Assert.IsTrue(state.GetValues(new[] { 1 }).SequenceEqual(new[] { 1 }));

            Assert.AreEqual(4, state.GetSize());

            var relaxedState = state.GetRelaxedState() as RelaxedState;
            Assert.IsNotNull(relaxedState);

            var conditions = (IConditions)state.GetDescribingConditions(problem);
            Assert.AreEqual(conditions, new ConditionsClause(new Conditions(new Assignment(0, 2), new Assignment(1, 1), new Assignment(2, 2)),
                                                             new Conditions(new Assignment(0, 2), new Assignment(1, 2), new Assignment(2, 2))));

            Assert.AreEqual("PlannerTestCases_TC_RedBlackState.sas_[2 1|2 2]", state.GetInfoString(problem));
            Assert.AreEqual("[2 1|2 2]", state.ToString());
            Assert.AreEqual("Blue, White|Grey, Bike", state.ToStringWithMeanings(problem));

            var stateClone = state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_RelativeState()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_RelativeState.sas"));
            Problem problem = new Problem(data);

            var state = (IRelativeState)new RelativeState(1, -1, 2);

            Assert.AreEqual(1, state.GetValue(0));
            Assert.AreEqual(-1, state.GetValue(1));
            Assert.AreEqual(2, state.GetValue(2));

            Assert.IsTrue(state.HasValue(0, 1));
            Assert.IsTrue(state.HasValue(1, RelativeState.WildCardValue));
            Assert.IsTrue(state.HasValue(2, 2));

            Assert.IsFalse(state.HasValue(new Assignment(2, 0)));
            state.SetValue(2, 0);
            Assert.IsTrue(state.HasValue(2, 0));
            state.SetValue(new Assignment(2, 2));

            Assert.AreEqual(1, state.GetAllValues(0).Length);
            Assert.AreEqual(1, state.GetAllValues(0)[0]);
            Assert.AreEqual(1, state.GetAllValues(1).Length);
            Assert.AreEqual(-1, state.GetAllValues(1)[0]);
            Assert.AreEqual(1, state.GetAllValues(2).Length);
            Assert.AreEqual(2, state.GetAllValues(2)[0]);
            Assert.IsTrue(state.GetAllValues().SequenceEqual(new[] { 1, -1, 2 }));

            Assert.IsTrue(state.GetValues(new[] { 2, 0 }).SequenceEqual(new[] { 2, 1 }));
            Assert.IsTrue(state.GetValues(new[] { 1 }).SequenceEqual(new[] { -1 }));

            Assert.AreEqual(3, state.GetSize());

            var relaxedState = state.GetRelaxedState() as RelaxedState;
            Assert.IsNotNull(relaxedState);

            Assert.IsTrue(state.Evaluate(new State(1, 99, 2)));
            Assert.IsFalse(state.Evaluate(new State(1, 0, 1)));

            var conditions = (IConditions)state.GetDescribingConditions(problem);
            Assert.AreEqual(conditions, new Conditions(new Assignment(0, 1), new Assignment(2, 2)));

            var correspondingStates = state.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(2, correspondingStates.Count);
            Assert.IsTrue(correspondingStates.Contains(new State(1, 0, 2)) && correspondingStates.Contains(new State(1, 1, 2)));

            Assert.AreEqual(new State(-1, 2, 1, 1, 1), State.Parse("[-1 2 3x1]"));
            Assert.AreEqual(new State(2), State.Parse("[2]"));
            Assert.AreEqual(new State(), State.Parse("[]"));

            Assert.AreEqual("PlannerTestCases_TC_RelativeState.sas_[1 -1 2]", state.GetInfoString(problem));
            Assert.AreEqual("[1 2 3x-1 3]", new RelativeState(1, 2, -1, -1, -1, 3).GetCompressedDescription());
            Assert.AreEqual("[1 -1 2]", state.ToString());
            Assert.AreEqual("Green, *, Bike", state.ToStringWithMeanings(problem));

            var stateClone = state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_RelaxedPlanningGraph()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_RelaxedPlanningGraph.sas"));

            Problem problem = new Problem(data);
            RelaxedPlanningGraph planningGraph = new RelaxedPlanningGraph(problem.GetRelaxedProblem());

            Assert.AreEqual(9, planningGraph.ComputeMaxForwardCost(problem.InitialState));
            Assert.AreEqual(9, planningGraph.ComputeMaxForwardCost(problem.GoalConditions));

            Assert.AreEqual(25, planningGraph.ComputeAdditiveForwardCost(problem.InitialState));
            Assert.AreEqual(25, planningGraph.ComputeAdditiveForwardCost(problem.GoalConditions));

            Assert.AreEqual(12, planningGraph.ComputeFFCost(problem.GetInitialState()));
            Assert.AreEqual(12, planningGraph.ComputeFFCost(problem.GetGoalConditions()));
        }

        [TestMethod]
        public void TC_RelaxedProblem()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_RelaxedProblem.sas"));
            Problem problem = new Problem(data);

            var relaxedProblem = problem.GetRelaxedProblem();
            relaxedProblem.SetInitialState(new State(0, 1, 1));

            HashSet<IState> successorStates = new HashSet<IState>();
            foreach (var successor in relaxedProblem.GetSuccessors(relaxedProblem.GetInitialState()))
            {
                successorStates.Add((IState)successor.GetSuccessorState());
            }
            Assert.AreEqual(1, successorStates.Count);
            var relaxedState = successorStates.First();

            Assert.AreEqual(3, relaxedProblem.GetInitialState().GetSize());
            Assert.AreEqual(4, relaxedState.GetSize());

            var values0 = relaxedState.GetAllValues(0);
            Assert.AreEqual(2, values0.Length);
            Assert.AreEqual(0, values0[0]);
            Assert.AreEqual(1, values0[1]);

            Assert.IsTrue(relaxedProblem.IsGoalState(relaxedState));
        }

        [TestMethod]
        public void TC_RelaxedState()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_RelaxedState.sas"));
            Problem problem = new Problem(data);

            var state = (RelaxedState)problem.GetInitialState().GetRelaxedState();

            Assert.AreEqual(0, state.GetValue(0));
            Assert.AreEqual(1, state.GetValue(1));
            Assert.AreEqual(2, state.GetValue(2));

            state.SetValue(new Assignment(0, 1));
            state.SetValue(0, 2);
            state.SetValue(0, 2);

            Assert.IsTrue(state.HasValue(0, 0));
            Assert.IsTrue(state.HasValue(0, 1));
            Assert.IsTrue(state.HasValue(0, 2));
            Assert.IsTrue(state.HasValue(new Assignment(0, 2)));
            Assert.IsFalse(state.HasValue(new Assignment(0, 3)));

            Assert.AreEqual(3, state.GetAllValues(0).Length);
            Assert.AreEqual(0, state.GetAllValues(0)[0]);
            Assert.AreEqual(1, state.GetAllValues(0)[1]);
            Assert.AreEqual(2, state.GetAllValues(0)[2]);
            Assert.IsTrue(state.GetAllValues().SequenceEqual(new[] { 0, 1, 2 }));

            Assert.IsTrue(state.GetValues(new[] { 2, 0 }).SequenceEqual(new[] { 2, 0 }));
            Assert.IsTrue(state.GetValues(new[] { 1 }).SequenceEqual(new[] { 1 }));

            Assert.AreEqual(5, state.GetSize());
            Assert.AreEqual(3, state.GetLength());

            var relaxedState = state.GetRelaxedState() as RelaxedState;
            Assert.IsTrue(relaxedState == state);
            Assert.IsTrue(relaxedState.GetHashCode() == state.GetHashCode());
            Assert.IsTrue(relaxedState.Equals(state));

            var conditions = (IConditions)state.GetDescribingConditions(problem);
            Assert.AreEqual(conditions, new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 1), new Assignment(2, 2)),
                                                             new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 2)),
                                                             new Conditions(new Assignment(0, 2), new Assignment(1, 1), new Assignment(2, 2))));

            Assert.AreEqual("PlannerTestCases_TC_RelaxedState.sas_[0|1|2 1 2]", state.GetInfoString(problem));
            Assert.AreEqual("[0|1|2 1 2]", state.ToString());
            Assert.AreEqual("Red|Green|Blue, White, Bike", state.ToStringWithMeanings(problem));

            var stateClone = state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_State()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_State.sas"));
            Problem problem = new Problem(data);

            var state = (IState)problem.GetInitialState();

            Assert.AreEqual(0, state.GetValue(0));
            Assert.AreEqual(1, state.GetValue(1));
            Assert.AreEqual(2, state.GetValue(2));

            Assert.IsTrue(state.HasValue(0, 0));
            Assert.IsTrue(state.HasValue(1, 1));
            Assert.IsTrue(state.HasValue(2, 2));

            Assert.IsFalse(state.HasValue(new Assignment(2, 0)));
            state.SetValue(2, 0);
            Assert.IsTrue(state.HasValue(2, 0));
            state.SetValue(new Assignment(2, 2));

            Assert.AreEqual(1, state.GetAllValues(0).Length);
            Assert.AreEqual(0, state.GetAllValues(0)[0]);
            Assert.AreEqual(1, state.GetAllValues(1).Length);
            Assert.AreEqual(1, state.GetAllValues(1)[0]);
            Assert.AreEqual(1, state.GetAllValues(2).Length);
            Assert.AreEqual(2, state.GetAllValues(2)[0]);
            Assert.IsTrue(state.GetAllValues().SequenceEqual(new[] { 0, 1, 2 }));

            Assert.IsTrue(state.GetValues(new[] { 2, 0 }).SequenceEqual(new[] { 2, 0 }));
            Assert.IsTrue(state.GetValues(new[] { 1 }).SequenceEqual(new[] { 1 }));

            Assert.AreEqual(3, state.GetSize());

            var relaxedState = state.GetRelaxedState() as RelaxedState;
            Assert.IsNotNull(relaxedState);

            var conditions = (IConditions)state.GetDescribingConditions(problem);
            Assert.AreEqual(conditions, new Conditions(new Assignment(0, 0), new Assignment(1, 1), new Assignment(2, 2)));

            Assert.AreEqual(new State(1, 2, 1, 1, 1), State.Parse("[1 2 3x1]"));
            Assert.AreEqual(new State(2), State.Parse("[2]"));
            Assert.AreEqual(new State(), State.Parse("[]"));

            Assert.AreEqual("PlannerTestCases_TC_State.sas_[0 1 2]", state.GetInfoString(problem));
            Assert.AreEqual("[1 2 3x1 3]", new State(1, 2, 1, 1, 1, 3).GetCompressedDescription());
            Assert.AreEqual("[0 1 2]", state.ToString());
            Assert.AreEqual("Red, White, Bike", state.ToStringWithMeanings(problem));

            var stateClone = state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_StatesEnumerator()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_StatesEnumerator.sas"));
            Problem problem = new Problem(data);

            // simple conditions

            IConditions conditions = new Conditions(new Assignment(2, 0));
            HashSet<IState> expectedConditionsStates = new HashSet<IState>
            {
                new State(0, 0, 0), new State(0, 1, 0),
                new State(1, 0, 0), new State(1, 1, 0),
                new State(2, 0, 0), new State(2, 1, 0)
            };

            var conditionsStates = StatesEnumerator.EnumerateStates(conditions, problem.Variables).ToList();
            Assert.AreEqual(6, conditionsStates.Count);
            foreach (var state in conditionsStates)
            {
                Assert.IsTrue(expectedConditionsStates.Contains(state));
            }

            // empty simple conditions (= nothing constrained)

            IConditions emptyConditions = new Conditions();
            var emptyConditionsStates = StatesEnumerator.EnumerateStates(emptyConditions, problem.Variables);
            Assert.AreEqual(18, emptyConditionsStates.Count());

            // clause conditions

            IConditions conditionsClause = new ConditionsClause(new Conditions(new Assignment(0, 0), new Assignment(1, 0), new Assignment(2, 0)),
                                                                new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 1)),
                                                                new Conditions(new Assignment(0, 2)));
            HashSet<IState> expectedConditionsClauseStates = new HashSet<IState>
            {
                new State(0, 0, 0), new State(1, 1, 1),
                new State(2, 0, 0), new State(2, 1, 0),
                new State(2, 0, 1), new State(2, 1, 1),
                new State(2, 0, 2), new State(2, 1, 2)
            };

            var conditionsClauseStates = StatesEnumerator.EnumerateStates(conditionsClause, problem.Variables).ToList();
            Assert.AreEqual(8, conditionsClauseStates.Count);
            foreach (var state in conditionsClauseStates)
            {
                Assert.IsTrue(expectedConditionsClauseStates.Contains(state));
            }

            // empty clause conditions (= nothing allowed)

            IConditions emptyConditionsClause = new ConditionsClause();
            var emptyConditionsClauseStates = StatesEnumerator.EnumerateStates(emptyConditionsClause, problem.Variables);
            Assert.AreEqual(0, emptyConditionsClauseStates.Count());

            // relative states

            IRelativeState relativeState = new RelativeState(-1, -1, 0);
            HashSet<IState> expectedRelativeStateStates = new HashSet<IState>
            {
                new State(0, 0, 0), new State(0, 1, 0),
                new State(1, 0, 0), new State(1, 1, 0),
                new State(2, 0, 0), new State(2, 1, 0)
            };

            var relativeStateStates = StatesEnumerator.EnumerateStates(relativeState, problem.Variables).ToList();
            Assert.AreEqual(6, relativeStateStates.Count);
            foreach (var state in relativeStateStates)
            {
                Assert.IsTrue(expectedRelativeStateStates.Contains(state));
            }
        }

        [TestMethod]
        public void TC_SuccessorsGenerator()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_SuccessorsGenerator.sas"));
            Problem problem = new Problem(data);

            SuccessorsGenerator generator = new SuccessorsGenerator(problem.Operators, problem.Variables, problem.MutexGroups);

            HashSet<Planner.IState> successorStates = new HashSet<Planner.IState>();
            foreach (var successor in generator.GetSuccessors(new State(0, 0, 0)))
            {
                successorStates.Add(successor.GetSuccessorState());
            }
            Assert.AreEqual(4, successorStates.Count);
            Assert.IsTrue(successorStates.Contains(new State(1, 0, 0)));
            Assert.IsTrue(successorStates.Contains(new State(0, 1, 0)));
            Assert.IsTrue(successorStates.Contains(new State(0, 0, 1)));
            Assert.IsTrue(successorStates.Contains(new State(2, 2, 2)));
        }

        [TestMethod]
        public void TC_TransitionsEnumerator()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_TransitionsEnumerator.sas"));
            Problem problem = new Problem(data);

            Planner.TransitionsEnumerator<IState, Planner.ISuccessor> enumerator = new Planner.TransitionsEnumerator<IState, Planner.ISuccessor>();
            Func<IState, IEnumerable<Planner.ISuccessor>> generatorFunction = state => problem.GetSuccessors(state);

            State state0 = new State(0, 0, 0);
            State state1 = new State(1, 1, 1);
            State state2 = new State(0, 1, 1);
            var successors0 = new HashSet<Planner.ISuccessor>(problem.GetSuccessors(state0));
            var successors1 = new HashSet<Planner.ISuccessor>(problem.GetSuccessors(state1));
            var successors2 = new HashSet<Planner.ISuccessor>(problem.GetSuccessors(state2));

            var successors = enumerator.GetNextTransitions(state0, 2, generatorFunction).ToList();
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors0.Contains(item)));
            successors = enumerator.GetNextTransitions(state1, 2, generatorFunction).ToList();
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors1.Contains(item)));
            successors = enumerator.GetNextTransitions(state2, 2, generatorFunction).ToList();
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors2.Contains(item)));

            successors = enumerator.GetNextTransitions(state0, 2, generatorFunction).ToList();
            Assert.AreEqual(1, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors0.Contains(item)));
            successors = enumerator.GetNextTransitions(state1, 2, generatorFunction).ToList();
            Assert.AreEqual(1, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors1.Contains(item)));
            successors = enumerator.GetNextTransitions(state2, 2, generatorFunction).ToList();
            Assert.AreEqual(1, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successors2.Contains(item)));

            successors = enumerator.GetNextTransitions(state0, 2, generatorFunction).ToList();
            Assert.AreEqual(0, successors.Count);
            successors = enumerator.GetNextTransitions(state1, 2, generatorFunction).ToList();
            Assert.AreEqual(0, successors.Count);
            successors = enumerator.GetNextTransitions(state2, 2, generatorFunction).ToList();
            Assert.AreEqual(0, successors.Count);
        }

        [TestMethod]
        public void TC_TransitionsGenerator()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_TransitionsGenerator.sas"));
            Problem problem = new Problem(data);
            TransitionsGenerator transitionsGenerator = new TransitionsGenerator(problem);

            HashSet<IState> successorStates = new HashSet<IState>();
            foreach (var successor in transitionsGenerator.GetSuccessors(problem.InitialState))
            {
                successorStates.Add((IState)successor.GetSuccessorState());
            }
            Assert.AreEqual(4, successorStates.Count);
            Assert.IsTrue(successorStates.Contains(new State(1, 0, 0)));
            Assert.IsTrue(successorStates.Contains(new State(0, 1, 0)));
            Assert.IsTrue(successorStates.Contains(new State(0, 0, 1)));
            Assert.IsTrue(successorStates.Contains(new State(2, 2, 2)));

            HashSet<IConditions> predecessorConditions = new HashSet<IConditions>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors(problem.GoalConditions))
            {
                predecessorConditions.Add((IConditions)predecessor.GetPredecessorConditions());
            }
            Assert.AreEqual(3, predecessorConditions.Count);
            Assert.IsTrue(predecessorConditions.Contains(new Conditions(new Assignment(0, 0), new Assignment(1, 1), new Assignment(2, 1))));
            Assert.IsTrue(predecessorConditions.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 0), new Assignment(2, 1))));
            Assert.IsTrue(predecessorConditions.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 0))));

            Assert.AreEqual(0, transitionsGenerator.GetSuccessors(new State(1, 1, 1)).Count());
            Assert.AreEqual(0, transitionsGenerator.GetPredecessors(new Conditions(new Assignment(0, 0))).Count());

            var successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 2));
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(new State(1, 1, 1), 50));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 4));
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(4, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));

            var predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 1));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(new Conditions(new Assignment(0, 0)), 50));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 5));
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(3, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));

            Planner.ISuccessor randomSuccessor = transitionsGenerator.GetRandomSuccessor(problem.InitialState);
            Assert.IsNotNull(randomSuccessor);
            Assert.IsTrue(successorStates.Contains(randomSuccessor.GetSuccessorState()));
            randomSuccessor = transitionsGenerator.GetRandomSuccessor(new State(1, 1, 1));
            Assert.IsNull(randomSuccessor);

            Planner.IPredecessor randomPredecessor = transitionsGenerator.GetRandomPredecessor(problem.GoalConditions);
            Assert.IsNotNull(randomPredecessor);
            Assert.IsTrue(predecessorConditions.Contains(randomPredecessor.GetPredecessorConditions()));
            randomPredecessor = transitionsGenerator.GetRandomPredecessor(new Conditions(new Assignment(0, 0)));
            Assert.IsNull(randomPredecessor);

            HashSet<IConditions> predecessors2 = new HashSet<IConditions>();
            HashSet<IState> predecessors2States = new HashSet<IState>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors(new State(1, 1, 1)))
            {
                predecessors2.Add((IConditions)predecessor.GetPredecessorConditions());

                foreach (var state in predecessor.GetPredecessorStates(problem))
                {
                    predecessors2States.Add((IState)state);
                }
            }
            Assert.AreEqual(3, predecessors2.Count);
            Assert.IsTrue(predecessors2.Contains(new Conditions(new Assignment(0, 0), new Assignment(1, 1), new Assignment(2, 1))));
            Assert.IsTrue(predecessors2.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 0), new Assignment(2, 1))));
            Assert.IsTrue(predecessors2.Contains(new Conditions(new Assignment(0, 1), new Assignment(1, 1), new Assignment(2, 0))));

            Assert.AreEqual(3, predecessors2States.Count);
            Assert.IsTrue(predecessors2States.Contains(new State(0, 1, 1)));
            Assert.IsTrue(predecessors2States.Contains(new State(1, 0, 1)));
            Assert.IsTrue(predecessors2States.Contains(new State(1, 1, 0)));

            HashSet<IRelativeState> predecessors3 = new HashSet<IRelativeState>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors(new RelativeState(1, 1, -1)))
            {
                foreach (var state in predecessor.GetPredecessorRelativeStates())
                {
                    predecessors3.Add((IRelativeState)state);
                }
            }

            Assert.AreEqual(2, predecessors3.Count);
            Assert.IsTrue(predecessors3.Contains(new RelativeState(0, 1, -1)));
            Assert.IsTrue(predecessors3.Contains(new RelativeState(1, 0, -1)));

            var succStates = transitionsGenerator.GetSuccessorStates(new State(0, 0, 0)).ToList();
            Assert.AreEqual(4, succStates.Count);
            Assert.IsTrue(succStates.Contains(new State(1, 0, 0)));
            Assert.IsTrue(succStates.Contains(new State(0, 1, 0)));
            Assert.IsTrue(succStates.Contains(new State(0, 0, 1)));
            Assert.IsTrue(succStates.Contains(new State(2, 2, 2)));

            var predStates = transitionsGenerator.GetPredecessorStates(new State(1, 1, 1)).ToList();
            Assert.AreEqual(3, predStates.Count);
            Assert.IsTrue(predStates.Contains(new State(0, 1, 1)));
            Assert.IsTrue(predStates.Contains(new State(1, 0, 1)));
            Assert.IsTrue(predStates.Contains(new State(1, 1, 0)));
        }

        [TestMethod]
        public void TC_Variables()
        {
            SASInputData data = new SASInputData(GetFilePath("TC_Variables.sas"));
            Problem problem = new Problem(data);

            Assert.AreEqual(2, problem.Variables.Count);

            var variable0 = problem.Variables[0];
            Assert.AreEqual("var0", variable0.Name);
            Assert.AreEqual(-1, variable0.AxiomLayer);
            Assert.IsFalse(variable0.IsAxiomatic());
            Assert.AreEqual(3, variable0.GetDomainRange());
            Assert.AreEqual(3, variable0.Values.Count);
            Assert.AreEqual("Red", variable0.Values[0]);
            Assert.AreEqual("Green", variable0.Values[1]);
            Assert.AreEqual("Blue", variable0.Values[2]);
            Assert.IsFalse(variable0.IsRigid());

            var variable1 = problem.Variables[1];
            Assert.AreEqual("var1", variable1.Name);
            Assert.AreEqual(0, variable1.AxiomLayer);
            Assert.IsTrue(variable1.IsAxiomatic());
            Assert.AreEqual(2, variable1.GetDomainRange());
            Assert.AreEqual(2, variable1.Values.Count);
            Assert.AreEqual("Black", variable1.Values[0]);
            Assert.AreEqual("White", variable1.Values[1]);
            Assert.IsTrue(variable1.IsRigid());
        }
    }
}
