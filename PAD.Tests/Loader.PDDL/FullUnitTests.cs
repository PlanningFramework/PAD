using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;
using PAD.InputData.PDDL;
using PAD.InputData;

namespace PAD.Tests.PDDL
{
    /// <summary>
    /// Testing suite for the PDDL input data loader (from a black-box perspective).
    /// Covers the entire phase of loading an input data from the given valid PDDL files into PDDLInputData structure.
    /// </summary>
    [TestClass]
    public class PDDLLoaderFullUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private string GetFilePath(string fileName)
        {
            return $@"..\..\Loader.PDDL\FullTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_Actions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Actions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(4, data.Domain.Actions.Count);

            var action0 = data.Domain.Actions[0];
            Assert.AreEqual("actionA", action0.Name);
            Assert.AreEqual(1, action0.Parameters.Count);
            Assert.AreEqual("?a", action0.Parameters[0].ParameterName);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, action0.Parameters[0].TypeNames[0]);
            Assert.AreEqual(0, action0.Preconditions.Count);
            Assert.AreEqual(0, action0.Effects.Count);

            var action1 = data.Domain.Actions[1];
            Assert.AreEqual("actionB", action1.Name);
            Assert.AreEqual(0, action1.Parameters.Count);
            Assert.AreEqual(0, action1.Preconditions.Count);
            Assert.AreEqual(0, action1.Effects.Count);

            var action2 = data.Domain.Actions[2];
            Assert.AreEqual("actionC", action2.Name);
            Assert.AreEqual(0, action2.Parameters.Count);
            Assert.AreEqual(0, action2.Preconditions.Count);
            Assert.AreEqual(0, action2.Effects.Count);

            var action3 = data.Domain.Actions[3];
            Assert.AreEqual("actionD", action3.Name);
            Assert.AreEqual(0, action3.Parameters.Count);
            Assert.AreEqual(0, action3.Preconditions.Count);
            Assert.AreEqual(0, action3.Effects.Count);
        }

        [TestMethod]
        public void TC_Constants()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Constants.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(5, data.Domain.Constants.Count);

            var const0 = data.Domain.Constants[0];
            Assert.AreEqual("constA", const0.ConstantName);
            Assert.AreEqual(2, const0.TypeNames.Count);
            Assert.AreEqual("typeA", const0.TypeNames[0]);
            Assert.AreEqual("typeB", const0.TypeNames[1]);

            var const1 = data.Domain.Constants[1];
            Assert.AreEqual("constB", const1.ConstantName);
            Assert.AreEqual(2, const1.TypeNames.Count);
            Assert.AreEqual("typeA", const1.TypeNames[0]);
            Assert.AreEqual("typeB", const1.TypeNames[1]);

            var const2 = data.Domain.Constants[2];
            Assert.AreEqual("constC", const2.ConstantName);
            Assert.AreEqual(2, const2.TypeNames.Count);
            Assert.AreEqual("typeA", const2.TypeNames[0]);
            Assert.AreEqual("typeB", const2.TypeNames[1]);

            var const3 = data.Domain.Constants[3];
            Assert.AreEqual("constD", const3.ConstantName);
            Assert.AreEqual(1, const3.TypeNames.Count);
            Assert.AreEqual(Constant.DEFAULT_TYPE, const3.TypeNames[0]);

            var const4 = data.Domain.Constants[4];
            Assert.AreEqual("constE", const4.ConstantName);
            Assert.AreEqual(1, const4.TypeNames.Count);
            Assert.AreEqual(Constant.DEFAULT_TYPE, const4.TypeNames[0]);
        }

        [TestMethod]
        public void TC_ConstraintsInDomain()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConstraintsInDomain.pddl"), GetFilePath("TC_ConstraintsInProblem.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(11, data.Domain.Constraints.Count);

            var constr0 = data.Domain.Constraints[0] as ForallConstraint;
            Assert.IsNotNull(constr0);
            Assert.AreEqual(1, constr0.Parameters.Count);
            Assert.AreEqual("?a", constr0.Parameters[0].ParameterName);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, constr0.Parameters[0].TypeNames[0]);

            var argConstraints = constr0.Constraints;
            Assert.AreEqual(1, argConstraints.Count);
            Assert.IsTrue(argConstraints[0] is AlwaysConstraint);

            var constr1 = data.Domain.Constraints[1] as AtEndConstraint;
            Assert.IsNotNull(constr1);
            Assert.IsTrue(constr1.Expression is PredicateExpression);

            var constr2 = data.Domain.Constraints[2] as AlwaysConstraint;
            Assert.IsNotNull(constr2);
            Assert.IsTrue(constr2.Expression is PredicateExpression);

            var constr3 = data.Domain.Constraints[3] as SometimeConstraint;
            Assert.IsNotNull(constr3);
            Assert.IsTrue(constr3.Expression is PredicateExpression);

            var constr4 = data.Domain.Constraints[4] as WithinConstraint;
            Assert.IsNotNull(constr4);
            Assert.AreEqual(6.0, constr4.Number);
            Assert.IsTrue(constr4.Expression is PredicateExpression);

            var constr5 = data.Domain.Constraints[5] as AtMostOnceConstraint;
            Assert.IsNotNull(constr5);
            Assert.IsTrue(constr5.Expression is PredicateExpression);

            var constr6 = data.Domain.Constraints[6] as SometimeAfterConstraint;
            Assert.IsNotNull(constr6);
            Assert.IsTrue(constr6.Expression1 is PredicateExpression);
            Assert.IsTrue(constr6.Expression2 is PredicateExpression);

            var constr7 = data.Domain.Constraints[7] as SometimeBeforeConstraint;
            Assert.IsNotNull(constr7);
            Assert.IsTrue(constr7.Expression1 is PredicateExpression);
            Assert.IsTrue(constr7.Expression2 is PredicateExpression);

            var constr8 = data.Domain.Constraints[8] as AlwaysWithinConstraint;
            Assert.IsNotNull(constr8);
            Assert.AreEqual(6.0, constr8.Number);
            Assert.IsTrue(constr8.Expression1 is PredicateExpression);
            Assert.IsTrue(constr8.Expression2 is PredicateExpression);

            var constr9 = data.Domain.Constraints[9] as HoldDuringConstraint;
            Assert.IsNotNull(constr9);
            Assert.AreEqual(6.0, constr9.Number1);
            Assert.AreEqual(6.0, constr9.Number2);
            Assert.IsTrue(constr9.Expression is PredicateExpression);

            var constr10 = data.Domain.Constraints[10] as HoldAfterConstraint;
            Assert.IsNotNull(constr10);
            Assert.AreEqual(6.0, constr10.Number);
            Assert.IsTrue(constr10.Expression is PredicateExpression);
        }

        [TestMethod]
        public void TC_ConstraintsInProblem()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConstraintsInDomain.pddl"), GetFilePath("TC_ConstraintsInProblem.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(11, data.Problem.Constraints.Count);

            var constr0 = data.Problem.Constraints[0] as ForallConstraint;
            Assert.IsNotNull(constr0);
            Assert.AreEqual(1, constr0.Parameters.Count);
            Assert.AreEqual("?a", constr0.Parameters[0].ParameterName);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, constr0.Parameters[0].TypeNames[0]);

            var argConstraints = constr0.Constraints;
            Assert.AreEqual(1, argConstraints.Count);
            Assert.IsTrue(argConstraints[0] is AlwaysConstraint);

            var constr1 = data.Problem.Constraints[1] as AtEndConstraint;
            Assert.IsNotNull(constr1);
            Assert.IsTrue(constr1.Expression is PredicateExpression);

            var constr2 = data.Problem.Constraints[2] as AlwaysConstraint;
            Assert.IsNotNull(constr2);
            Assert.IsTrue(constr2.Expression is PredicateExpression);

            var constr3 = data.Problem.Constraints[3] as SometimeConstraint;
            Assert.IsNotNull(constr3);
            Assert.IsTrue(constr3.Expression is PredicateExpression);

            var constr4 = data.Problem.Constraints[4] as WithinConstraint;
            Assert.IsNotNull(constr4);
            Assert.AreEqual(6.0, constr4.Number);
            Assert.IsTrue(constr4.Expression is PredicateExpression);

            var constr5 = data.Problem.Constraints[5] as AtMostOnceConstraint;
            Assert.IsNotNull(constr5);
            Assert.IsTrue(constr5.Expression is PredicateExpression);

            var constr6 = data.Problem.Constraints[6] as SometimeAfterConstraint;
            Assert.IsNotNull(constr6);
            Assert.IsTrue(constr6.Expression1 is PredicateExpression);
            Assert.IsTrue(constr6.Expression2 is PredicateExpression);

            var constr7 = data.Problem.Constraints[7] as SometimeBeforeConstraint;
            Assert.IsNotNull(constr7);
            Assert.IsTrue(constr7.Expression1 is PredicateExpression);
            Assert.IsTrue(constr7.Expression2 is PredicateExpression);

            var constr8 = data.Problem.Constraints[8] as AlwaysWithinConstraint;
            Assert.IsNotNull(constr8);
            Assert.AreEqual(6.0, constr8.Number);
            Assert.IsTrue(constr8.Expression1 is PredicateExpression);
            Assert.IsTrue(constr8.Expression2 is PredicateExpression);

            var constr9 = data.Problem.Constraints[9] as HoldDuringConstraint;
            Assert.IsNotNull(constr9);
            Assert.AreEqual(6.0, constr9.Number1);
            Assert.AreEqual(6.0, constr9.Number2);
            Assert.IsTrue(constr9.Expression is PredicateExpression);

            var constr10 = data.Problem.Constraints[10] as HoldAfterConstraint;
            Assert.IsNotNull(constr10);
            Assert.AreEqual(6.0, constr10.Number);
            Assert.IsTrue(constr10.Expression is PredicateExpression);
        }

        [TestMethod]
        public void TC_DerivedPredicates()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_DerivedPredicates.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(2, data.Domain.DerivedPredicates.Count);

            var pred0 = data.Domain.DerivedPredicates[0];
            Assert.AreEqual("notEquals", pred0.Predicate.Name);
            Assert.AreEqual(2, pred0.Predicate.Terms.Count);
            Assert.AreEqual("?a", pred0.Predicate.Terms[0].TermName);
            Assert.AreEqual("?b", pred0.Predicate.Terms[1].TermName);
            Assert.IsTrue(pred0.Expression is NotExpression);

            var pred1 = data.Domain.DerivedPredicates[1];
            Assert.AreEqual("totalTimeLesserThan10", pred1.Predicate.Name);
            Assert.AreEqual(0, pred1.Predicate.Terms.Count);
            Assert.IsTrue(pred1.Expression is NumericCompareExpression);
        }

        [TestMethod]
        public void TC_Domain()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Domain.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual("domainName", data.Domain.Name);

            Assert.AreEqual(0, data.Domain.Requirements.Count);
            Assert.AreEqual(0, data.Domain.Types.Count);
            Assert.AreEqual(0, data.Domain.Constants.Count);
            Assert.AreEqual(0, data.Domain.Predicates.Count);
            Assert.AreEqual(0, data.Domain.Functions.Count);
            Assert.AreEqual(0, data.Domain.Constraints.Count);
            Assert.AreEqual(0, data.Domain.Actions.Count);
            Assert.AreEqual(0, data.Domain.DurativeActions.Count);
            Assert.AreEqual(0, data.Domain.DerivedPredicates.Count);
        }

        [TestMethod]
        public void TC_DurativeActions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_DurativeActions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(2, data.Domain.DurativeActions.Count);

            var durativeAction0 = data.Domain.DurativeActions[0];
            Assert.AreEqual("actionA", durativeAction0.Name);
            Assert.AreEqual(1, durativeAction0.Parameters.Count);
            Assert.AreEqual("?a", durativeAction0.Parameters[0].ParameterName);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, durativeAction0.Parameters[0].TypeNames[0]);
            Assert.AreEqual(0, durativeAction0.Durations.Count);
            Assert.AreEqual(0, durativeAction0.Conditions.Count);
            Assert.AreEqual(0, durativeAction0.Effects.Count);

            var durativeAction1 = data.Domain.DurativeActions[1];
            Assert.AreEqual("actionB", durativeAction1.Name);
            Assert.AreEqual(0, durativeAction1.Parameters.Count);
            Assert.AreEqual(0, durativeAction1.Durations.Count);
            Assert.AreEqual(0, durativeAction1.Conditions.Count);
            Assert.AreEqual(0, durativeAction1.Effects.Count);
        }

        [TestMethod]
        public void TC_DurativeConstraints()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_DurativeConstraints.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.DurativeActions.Count);

            var durAction = data.Domain.DurativeActions[0];
            Assert.AreEqual(3, durAction.Durations.Count);

            var durConstr0 = durAction.Durations[0] as AtDurativeConstraint;
            Assert.IsNotNull(durConstr0);
            Assert.AreEqual(TimeSpecifier.START, durConstr0.TimeSpecifier);

            var durSubConstr0 = durConstr0.DurativeConstraint as CompareDurativeConstraint;
            Assert.IsNotNull(durSubConstr0);
            Assert.AreEqual(DurationComparer.LTE, durSubConstr0.DurationComparer);
            Assert.IsTrue(durSubConstr0.Value is Number);

            var durConstr1 = durAction.Durations[1] as AtDurativeConstraint;
            Assert.IsNotNull(durConstr1);
            Assert.AreEqual(TimeSpecifier.END, durConstr1.TimeSpecifier);
            Assert.IsTrue(durConstr1.DurativeConstraint is CompareDurativeConstraint);

            var durSubConstr1 = durConstr1.DurativeConstraint as CompareDurativeConstraint;
            Assert.IsNotNull(durSubConstr1);
            Assert.AreEqual(DurationComparer.EQ, durSubConstr1.DurationComparer);
            Assert.IsTrue(durSubConstr1.Value is NumericFunction);

            var durConstr2 = durAction.Durations[2] as CompareDurativeConstraint;
            Assert.IsNotNull(durConstr2);
            Assert.AreEqual(DurationComparer.GTE, durConstr2.DurationComparer);
            Assert.IsTrue(durConstr2.Value is Plus);
        }

        [TestMethod]
        public void TC_DurativeEffects()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_DurativeEffects.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.DurativeActions.Count);

            var durAction = data.Domain.DurativeActions[0];
            Assert.AreEqual(6, durAction.Effects.Count);

            var effect0 = durAction.Effects[0] as ForallDurativeEffect;
            Assert.IsNotNull(effect0);
            Assert.AreEqual(1, effect0.Parameters.Count);
            Assert.AreEqual("?b", effect0.Parameters[0].ParameterName);
            Assert.AreEqual(1, effect0.Effects.Count);
            Assert.IsTrue(effect0.Effects[0] is AtTimedEffect);

            var effect1 = durAction.Effects[1] as WhenDurativeEffect;
            Assert.IsNotNull(effect1);
            Assert.IsTrue(effect1.Expression is AtTimedExpression);
            Assert.IsTrue(effect1.Effect is AtTimedEffect);

            var effect2 = durAction.Effects[2] as AtTimedEffect;
            Assert.IsNotNull(effect2);
            Assert.AreEqual(TimeSpecifier.START, effect2.TimeSpecifier);
            Assert.AreEqual(1, effect2.Effects.Count);
            Assert.IsTrue(effect2.Effects[0] is PredicateEffect);

            var effect3 = durAction.Effects[3] as AtTimedEffect;
            Assert.IsNotNull(effect3);
            Assert.AreEqual(TimeSpecifier.END, effect3.TimeSpecifier);
            Assert.AreEqual(1, effect3.Effects.Count);
            Assert.IsTrue(effect3.Effects[0] is PredicateEffect);

            var effect4 = durAction.Effects[4] as AssignTimedEffect;
            Assert.IsNotNull(effect4);
            Assert.AreEqual(TimedEffectAssignOperator.INCREASE, effect4.AssignOperator);
            Assert.AreEqual("numFunc", effect4.Function.Name);
            Assert.AreEqual(0, effect4.Function.Terms.Count);
            Assert.IsTrue(effect4.Value is PrimitiveTimedNumericExpression);

            var effect5 = durAction.Effects[5] as AssignTimedEffect;
            Assert.IsNotNull(effect5);
            Assert.AreEqual(TimedEffectAssignOperator.DECREASE, effect5.AssignOperator);
            Assert.AreEqual("numFunc", effect5.Function.Name);
            Assert.AreEqual(0, effect5.Function.Terms.Count);
            Assert.IsTrue(effect5.Value is CompoundTimedNumericExpression);
            Assert.IsTrue(((CompoundTimedNumericExpression)effect5.Value).NumericExpression is Number);
        }

        [TestMethod]
        public void TC_DurativeExpressions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_DurativeExpressions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.DurativeActions.Count);

            var durAction = data.Domain.DurativeActions[0];
            Assert.AreEqual(6, durAction.Conditions.Count);

            var condition0 = durAction.Conditions[0] as ForallDurativeExpression;
            Assert.IsNotNull(condition0);
            Assert.AreEqual(1, condition0.Parameters.Count);
            Assert.AreEqual("?b", condition0.Parameters[0].ParameterName);
            Assert.IsTrue(condition0.Expression is AtTimedExpression);

            var condition1 = durAction.Conditions[1] as AndDurativeExpression;
            Assert.IsNotNull(condition1);
            Assert.AreEqual(1, condition1.Arguments.Count);
            Assert.IsTrue(condition1.Arguments[0] is AtTimedExpression);

            var condition2 = durAction.Conditions[2] as PreferencedTimedExpression;
            Assert.IsNotNull(condition2);
            Assert.AreEqual("prefName", condition2.Name);
            Assert.IsTrue(condition2.Expression is AtTimedExpression);

            var condition3 = durAction.Conditions[3] as AtTimedExpression;
            Assert.IsNotNull(condition3);
            Assert.AreEqual(TimeSpecifier.START, condition3.TimeSpecifier);
            Assert.IsTrue(condition3.Expression is PredicateExpression);

            var condition4 = durAction.Conditions[4] as AtTimedExpression;
            Assert.IsNotNull(condition4);
            Assert.AreEqual(TimeSpecifier.END, condition4.TimeSpecifier);
            Assert.IsTrue(condition4.Expression is PredicateExpression);

            var condition5 = durAction.Conditions[5] as OverTimedExpression;
            Assert.IsNotNull(condition5);
            Assert.AreEqual(IntervalSpecifier.ALL, condition5.IntervalSpecifier);
            Assert.IsTrue(condition5.Expression is PredicateExpression);
        }

        [TestMethod]
        public void TC_Effects()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Effects.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.Actions.Count);

            var effects = data.Domain.Actions[0].Effects;
            Assert.AreEqual(7, effects.Count);

            var effect0 = effects[0] as PredicateEffect;
            Assert.IsNotNull(effect0);
            Assert.AreEqual("pred", effect0.Name);
            Assert.AreEqual(1, effect0.Terms.Count);
            Assert.IsTrue(effect0.Terms[0] is VariableTerm);
            Assert.AreEqual("?aa", ((VariableTerm)effect0.Terms[0]).Name);

            var effect1 = effects[1] as EqualsEffect;
            Assert.IsNotNull(effect1);
            Assert.IsTrue(effect1.Term1 is ObjectFunctionTerm);
            Assert.IsTrue(effect1.Term2 is ConstantTerm);

            var effect2 = effects[2] as ForallEffect;
            Assert.IsNotNull(effect2);
            Assert.AreEqual(1, effect2.Parameters.Count);
            Assert.AreEqual("?cc", effect2.Parameters[0].ParameterName);
            Assert.AreEqual("typeA", effect2.Parameters[0].TypeNames[0]);
            Assert.AreEqual(1, effect2.Effects.Count);
            Assert.IsTrue(effect2.Effects[0] is PredicateEffect);

            var effect3 = effects[3] as WhenEffect;
            Assert.IsNotNull(effect3);
            Assert.IsTrue(effect3.Expression is PredicateExpression);
            Assert.AreEqual(2, effect3.Effects.Count);
            Assert.IsTrue(effect3.Effects[0] is PredicateEffect);
            Assert.IsTrue(effect3.Effects[1] is PredicateEffect);

            var effect4 = effects[4] as NumericAssignEffect;
            Assert.IsNotNull(effect4);
            Assert.AreEqual(AssignOperator.ASSIGN, effect4.AssignOperator);
            Assert.AreEqual("numFunc", effect4.Function.Name);
            Assert.AreEqual(0, effect4.Function.Terms.Count);
            Assert.IsTrue(effect4.Value is Number);

            var effect5 = effects[5] as ObjectAssignEffect;
            Assert.IsNotNull(effect5);
            Assert.AreEqual("objFunc", effect5.Function.Name);
            Assert.AreEqual(0, effect5.Function.Terms.Count);
            Assert.IsTrue(effect5.Value is VariableTerm);

            var effect6 = effects[6] as NumericAssignEffect;
            Assert.IsNotNull(effect6);
            Assert.AreEqual(AssignOperator.SCALE_UP, effect6.AssignOperator);
            Assert.AreEqual("numFunc", effect6.Function.Name);
            Assert.IsTrue(effect6.Value is Plus);
        }

        [TestMethod]
        public void TC_Expressions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Expressions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.Actions.Count);

            Assert.AreEqual("actionA", data.Domain.Actions[0].Name);
            Assert.AreEqual(2, data.Domain.Actions[0].Parameters.Count);

            var preconditions = data.Domain.Actions[0].Preconditions;
            Assert.AreEqual(9, preconditions.Count);
            Assert.IsTrue(preconditions[0] is PreferenceExpression);
            Assert.IsTrue(preconditions[1] is PredicateExpression);
            Assert.IsTrue(preconditions[2] is EqualsExpression);
            Assert.IsTrue(preconditions[3] is OrExpression);
            Assert.IsTrue(preconditions[4] is NotExpression);
            Assert.IsTrue(preconditions[5] is ImplyExpression);
            Assert.IsTrue(preconditions[6] is ExistsExpression);
            Assert.IsTrue(preconditions[7] is ForallExpression);
            Assert.IsTrue(preconditions[8] is NumericCompareExpression);
        }

        [TestMethod]
        public void TC_Functions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Functions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(4, data.Domain.Functions.Count);

            var func0 = data.Domain.Functions[0];
            Assert.AreEqual("functionA", func0.Name);
            Assert.AreEqual(2, func0.ReturnValueTypes.Count);
            Assert.AreEqual("typeA", func0.ReturnValueTypes[0]);
            Assert.AreEqual("typeB", func0.ReturnValueTypes[1]);
            Assert.AreEqual("?a", func0.Terms[0].TermName);
            Assert.AreEqual(DefinitionTerm.DEFAULT_TYPE, func0.Terms[0].TypeNames[0]);

            var func1 = data.Domain.Functions[1];
            Assert.AreEqual("functionB", func1.Name);
            Assert.AreEqual(1, func1.ReturnValueTypes.Count);
            Assert.AreEqual("object", func1.ReturnValueTypes[0]);
            Assert.AreEqual("?a", func1.Terms[0].TermName);
            Assert.AreEqual("typeA", func1.Terms[0].TypeNames[0]);

            var func2 = data.Domain.Functions[2];
            Assert.AreEqual("functionC", func2.Name);
            Assert.AreEqual(1, func2.ReturnValueTypes.Count);
            Assert.AreEqual("object", func2.ReturnValueTypes[0]);
            Assert.AreEqual("?a", func2.Terms[0].TermName);
            Assert.AreEqual("typeA", func2.Terms[0].TypeNames[0]);
            Assert.AreEqual("?b", func2.Terms[1].TermName);
            Assert.AreEqual("typeA", func2.Terms[1].TypeNames[0]);

            var func3 = data.Domain.Functions[3];
            Assert.AreEqual("functionD", func3.Name);
            Assert.AreEqual(1, func3.ReturnValueTypes.Count);
            Assert.AreEqual(Function.DEFAULT_RETURN_TYPE, func3.ReturnValueTypes[0]);
            Assert.AreEqual(0, func3.Terms.Count);
        }

        [TestMethod]
        public void TC_Goal()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_Goal.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            var goalCondition = data.Problem.Goal;

            Assert.AreEqual(9, goalCondition.Count);
            Assert.IsTrue(goalCondition[0] is PreferenceExpression);
            Assert.IsTrue(goalCondition[1] is PredicateExpression);
            Assert.IsTrue(goalCondition[2] is EqualsExpression);
            Assert.IsTrue(goalCondition[3] is OrExpression);
            Assert.IsTrue(goalCondition[4] is NotExpression);
            Assert.IsTrue(goalCondition[5] is ImplyExpression);
            Assert.IsTrue(goalCondition[6] is ExistsExpression);
            Assert.IsTrue(goalCondition[7] is ForallExpression);
            Assert.IsTrue(goalCondition[8] is NumericCompareExpression);
        }

        [TestMethod]
        public void TC_Init()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Init_D.pddl"), GetFilePath("TC_Init_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(8, data.Problem.Init.Count);

            var initElem0 = data.Problem.Init[0] as PredicateInitElement;
            Assert.IsNotNull(initElem0);
            Assert.AreEqual("pred", initElem0.Name);
            Assert.AreEqual(1, initElem0.Terms.Count);
            Assert.AreEqual("constA", initElem0.Terms[0].Name);

            var initElem1 = data.Problem.Init[1] as EqualsInitElement;
            Assert.IsNotNull(initElem1);
            Assert.AreEqual("constA", initElem1.Term1.Name);
            Assert.AreEqual("constB", initElem1.Term2.Name);

            var initElem2 = data.Problem.Init[2] as NotInitElement;
            Assert.IsNotNull(initElem2);
            Assert.IsTrue(initElem2.Element is PredicateInitElement);

            var initElem3 = data.Problem.Init[3] as AtInitElement;
            Assert.IsNotNull(initElem3);
            Assert.AreEqual(5, initElem3.Number);
            Assert.IsTrue(initElem3.Element is PredicateInitElement);

            var initElem4 = data.Problem.Init[4] as EqualsNumericFunctionInitElement;
            Assert.IsNotNull(initElem4);
            Assert.AreEqual("numFuncA", initElem4.Function.Name);
            Assert.AreEqual(0, initElem4.Function.Terms.Count);
            Assert.AreEqual(66, initElem4.Number);

            var initElem5 = data.Problem.Init[5] as EqualsObjectFunctionInitElement;
            Assert.IsNotNull(initElem5);
            Assert.AreEqual("objFuncA", initElem5.Function.Name);
            Assert.AreEqual(0, initElem5.Function.Terms.Count);
            Assert.AreEqual("constA", initElem5.Term.Name);

            var initElem6 = data.Problem.Init[6] as EqualsNumericFunctionInitElement;
            Assert.IsNotNull(initElem6);
            Assert.AreEqual("numFuncB", initElem6.Function.Name);
            Assert.AreEqual(1, initElem6.Function.Terms.Count);
            Assert.AreEqual("constA", initElem6.Function.Terms[0].Name);
            Assert.AreEqual(55, initElem6.Number);

            var initElem7 = data.Problem.Init[7] as EqualsObjectFunctionInitElement;
            Assert.IsNotNull(initElem7);
            Assert.AreEqual("objFuncB", initElem7.Function.Name);
            Assert.AreEqual(1, initElem7.Function.Terms.Count);
            Assert.AreEqual("constA", initElem7.Function.Terms[0].Name);
            Assert.AreEqual("constB", initElem7.Term.Name);
        }

        [TestMethod]
        public void TC_Length()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_Length.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(2, data.Problem.Length.Count);

            Assert.AreEqual(LengthSpecifier.SERIAL, data.Problem.Length[0].LengthSpecifier);
            Assert.AreEqual(55, data.Problem.Length[0].Parameter);

            Assert.AreEqual(LengthSpecifier.PARALLEL, data.Problem.Length[1].LengthSpecifier);
            Assert.AreEqual(66, data.Problem.Length[1].Parameter);
        }

        [TestMethod]
        public void TC_Metric()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_Metric.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(OptimizationSpecifier.MAXIMIZE, data.Problem.Metric.OptimizationSpecifier);

            var andExpr = data.Problem.Metric.Expression as MetricPlus;
            Assert.IsNotNull(andExpr);
            Assert.AreEqual(8, andExpr.Arguments.Count);

            var expr0 = andExpr.Arguments[0] as MetricMinus;
            Assert.IsNotNull(expr0);
            Assert.IsTrue(expr0.Argument1 is MetricNumber);
            Assert.IsTrue(expr0.Argument2 is MetricNumber);

            var expr1 = andExpr.Arguments[1] as MetricMultiply;
            Assert.IsNotNull(expr1);
            Assert.AreEqual(3, expr1.Arguments.Count);
            Assert.IsTrue(expr1.Arguments[0] is MetricNumber);
            Assert.IsTrue(expr1.Arguments[1] is MetricNumber);
            Assert.IsTrue(expr1.Arguments[2] is MetricNumber);

            var expr2 = andExpr.Arguments[2] as MetricDivide;
            Assert.IsNotNull(expr2);
            Assert.IsTrue(expr2.Argument1 is MetricNumber);
            Assert.IsTrue(expr2.Argument2 is MetricNumber);

            var expr3 = andExpr.Arguments[3] as MetricUnaryMinus;
            Assert.IsNotNull(expr3);
            Assert.IsTrue(expr3.Argument is MetricNumber);

            var expr4 = andExpr.Arguments[4] as MetricNumericFunction;
            Assert.IsNotNull(expr4);
            Assert.AreEqual("numFunc", expr4.Name);
            Assert.AreEqual(0, expr4.Terms.Count);

            var expr5 = andExpr.Arguments[5] as MetricNumericFunction;
            Assert.IsNotNull(expr5);
            Assert.AreEqual("numFunc", expr5.Name);
            Assert.AreEqual(0, expr5.Terms.Count);

            var expr6 = andExpr.Arguments[6] as MetricTotalTime;
            Assert.IsNotNull(expr6);

            var expr7 = andExpr.Arguments[7] as MetricPreferenceViolation;
            Assert.IsNotNull(expr7);
            Assert.AreEqual("prefName", expr7.PreferenceName);
        }

        [TestMethod]
        public void TC_NumericExpressions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericExpressions.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.Actions.Count);

            var preconditions = data.Domain.Actions[0].Preconditions;
            Assert.AreEqual(7, preconditions.Count);

            var expr0 = preconditions[0] as EqualsExpression;
            Assert.IsNotNull(expr0);
            Assert.IsTrue(expr0.Term1 is ObjectFunctionTerm);
            Assert.IsTrue(expr0.Term2 is ObjectFunctionTerm);

            var expr1 = preconditions[1] as EqualsExpression;
            Assert.IsNotNull(expr1);
            Assert.IsTrue(expr1.Term1 is ObjectFunctionTerm);
            Assert.IsTrue(expr1.Term2 is VariableTerm);

            var expr2 = preconditions[2] as NumericCompareExpression;
            Assert.IsNotNull(expr2);
            Assert.AreEqual(NumericComparer.EQ, expr2.NumericComparer);
            Assert.IsTrue(expr2.NumericExpression1 is NumericFunction);
            Assert.IsTrue(expr2.NumericExpression2 is NumericFunction);

            var expr3 = preconditions[3] as NumericCompareExpression;
            Assert.IsNotNull(expr3);
            Assert.AreEqual(NumericComparer.LT, expr3.NumericComparer);
            Assert.IsTrue(expr3.NumericExpression1 is NumericFunction);
            Assert.IsTrue(expr3.NumericExpression2 is Number);

            var expr4 = preconditions[4] as NumericCompareExpression;
            Assert.IsNotNull(expr4);
            Assert.AreEqual(NumericComparer.LTE, expr4.NumericComparer);
            Assert.IsTrue(expr4.NumericExpression1 is Plus);
            Assert.IsTrue(expr4.NumericExpression2 is Minus);

            var expr5 = preconditions[5] as NumericCompareExpression;
            Assert.IsNotNull(expr5);
            Assert.AreEqual(NumericComparer.GT, expr5.NumericComparer);
            Assert.IsTrue(expr5.NumericExpression1 is Multiply);
            Assert.IsTrue(expr5.NumericExpression2 is UnaryMinus);

            var expr6 = preconditions[6] as NumericCompareExpression;
            Assert.IsNotNull(expr6);
            Assert.AreEqual(NumericComparer.GTE, expr6.NumericComparer);
            Assert.IsTrue(expr6.NumericExpression1 is Number);
            Assert.IsTrue(expr6.NumericExpression2 is Divide);
        }

        [TestMethod]
        public void TC_Objects()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_Objects.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(5, data.Problem.Objects.Count);

            var object0 = data.Problem.Objects[0];
            Assert.AreEqual("objectA", object0.ObjectName);
            Assert.AreEqual(2, object0.TypeNames.Count);
            Assert.AreEqual("typeA", object0.TypeNames[0]);
            Assert.AreEqual("typeB", object0.TypeNames[1]);

            var object1 = data.Problem.Objects[1];
            Assert.AreEqual("objectB", object1.ObjectName);
            Assert.AreEqual(2, object1.TypeNames.Count);
            Assert.AreEqual("typeA", object1.TypeNames[0]);
            Assert.AreEqual("typeB", object1.TypeNames[1]);

            var object2 = data.Problem.Objects[2];
            Assert.AreEqual("objectC", object2.ObjectName);
            Assert.AreEqual(2, object2.TypeNames.Count);
            Assert.AreEqual("typeA", object2.TypeNames[0]);
            Assert.AreEqual("typeB", object2.TypeNames[1]);

            var object3 = data.Problem.Objects[3];
            Assert.AreEqual("objectD", object3.ObjectName);
            Assert.AreEqual(1, object3.TypeNames.Count);
            Assert.AreEqual(Object.DEFAULT_TYPE, object3.TypeNames[0]);

            var object4 = data.Problem.Objects[4];
            Assert.AreEqual("objectE", object4.ObjectName);
            Assert.AreEqual(1, object4.TypeNames.Count);
            Assert.AreEqual(Object.DEFAULT_TYPE, object4.TypeNames[0]);
        }

        [TestMethod]
        public void TC_Parameters()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Parameters.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.Actions.Count);

            var action = data.Domain.Actions[0];
            Assert.AreEqual(5, action.Parameters.Count);

            var param0 = action.Parameters[0];
            Assert.AreEqual("?a", param0.ParameterName);
            Assert.AreEqual(2, param0.TypeNames.Count);
            Assert.AreEqual("typeA", param0.TypeNames[0]);
            Assert.AreEqual("typeB", param0.TypeNames[1]);

            var param1 = action.Parameters[1];
            Assert.AreEqual("?b", param1.ParameterName);
            Assert.AreEqual(1, param1.TypeNames.Count);
            Assert.AreEqual("typeB", param1.TypeNames[0]);

            var param2 = action.Parameters[2];
            Assert.AreEqual("?c", param2.ParameterName);
            Assert.AreEqual(1, param2.TypeNames.Count);
            Assert.AreEqual("typeB", param2.TypeNames[0]);

            var param3 = action.Parameters[3];
            Assert.AreEqual("?d", param3.ParameterName);
            Assert.AreEqual(1, param3.TypeNames.Count);
            Assert.AreEqual("typeA", param3.TypeNames[0]);

            var param4 = action.Parameters[4];
            Assert.AreEqual("?e", param4.ParameterName);
            Assert.AreEqual(1, param4.TypeNames.Count);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, param4.TypeNames[0]);
        }

        [TestMethod]
        public void TC_Predicates()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Predicates.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(3, data.Domain.Predicates.Count);

            var pred0 = data.Domain.Predicates[0];
            Assert.AreEqual("predicateA", pred0.Name);
            Assert.AreEqual("?a", pred0.Terms[0].TermName);
            Assert.AreEqual(DefinitionTerm.DEFAULT_TYPE, pred0.Terms[0].TypeNames[0]);

            var pred1 = data.Domain.Predicates[1];
            Assert.AreEqual("predicateB", pred1.Name);
            Assert.AreEqual("?a", pred1.Terms[0].TermName);
            Assert.AreEqual("typeA", pred1.Terms[0].TypeNames[0]);
            Assert.AreEqual("?b", pred1.Terms[1].TermName);
            Assert.AreEqual("typeA", pred1.Terms[1].TypeNames[0]);

            var pred2 = data.Domain.Predicates[2];
            Assert.AreEqual("predicateC", pred2.Name);
            Assert.AreEqual(0, pred2.Terms.Count);
        }

        [TestMethod]
        public void TC_Problem()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_Problem.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual("problemName", data.Problem.Name);
            Assert.AreEqual("domainName", data.Problem.DomainName);

            Assert.AreEqual(0, data.Problem.Requirements.Count);
            Assert.AreEqual(0, data.Problem.Objects.Count);
            Assert.AreEqual(0, data.Problem.Init.Count);
            Assert.AreEqual(0, data.Problem.Goal.Count);
            Assert.AreEqual(0, data.Problem.Constraints.Count);
            Assert.IsNull(data.Problem.Metric.Expression);
            Assert.AreEqual(0, data.Problem.Length.Count);
        }

        [TestMethod]
        public void TC_RequirementsInDomain()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RequirementsInDomain.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(21, new HashSet<Requirement>(data.Domain.Requirements).Count);
        }

        [TestMethod]
        public void TC_RequirementsInProblem()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("Dummy_D.pddl"), GetFilePath("TC_RequirementsInProblem.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(21, new HashSet<Requirement>(data.Problem.Requirements).Count);
        }

        [TestMethod]
        public void TC_Terms()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Terms.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(1, data.Domain.Predicates.Count);

            var predicate = data.Domain.Predicates[0];
            Assert.AreEqual(5, predicate.Terms.Count);

            var param0 = predicate.Terms[0];
            Assert.AreEqual("?a", param0.TermName);
            Assert.AreEqual(2, param0.TypeNames.Count);
            Assert.AreEqual("typeA", param0.TypeNames[0]);
            Assert.AreEqual("typeB", param0.TypeNames[1]);

            var param1 = predicate.Terms[1];
            Assert.AreEqual("?b", param1.TermName);
            Assert.AreEqual(1, param1.TypeNames.Count);
            Assert.AreEqual("typeB", param1.TypeNames[0]);

            var param2 = predicate.Terms[2];
            Assert.AreEqual("?c", param2.TermName);
            Assert.AreEqual(1, param2.TypeNames.Count);
            Assert.AreEqual("typeB", param2.TypeNames[0]);

            var param3 = predicate.Terms[3];
            Assert.AreEqual("?d", param3.TermName);
            Assert.AreEqual(1, param3.TypeNames.Count);
            Assert.AreEqual("typeA", param3.TypeNames[0]);

            var param4 = predicate.Terms[4];
            Assert.AreEqual("?e", param4.TermName);
            Assert.AreEqual(1, param4.TypeNames.Count);
            Assert.AreEqual(Parameter.DEFAULT_TYPE, param4.TypeNames[0]);
        }

        [TestMethod]
        public void TC_Types()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Types.pddl"), GetFilePath("Dummy_P.pddl"));

            Assert.IsTrue(Utilities.CheckToStringExport(data));
            Assert.AreEqual(6, data.Domain.Types.Count);

            var type0 = data.Domain.Types[0];
            Assert.AreEqual("typeA", type0.TypeName);
            Assert.AreEqual(2, type0.BaseTypeNames.Count);
            Assert.AreEqual("typeA", type0.BaseTypeNames[0]);
            Assert.AreEqual("typeB", type0.BaseTypeNames[1]);

            var type1 = data.Domain.Types[1];
            Assert.AreEqual("typeB", type1.TypeName);
            Assert.AreEqual(1, type1.BaseTypeNames.Count);
            Assert.AreEqual("typeA", type1.BaseTypeNames[0]);

            var type2 = data.Domain.Types[2];
            Assert.AreEqual("typeC", type2.TypeName);
            Assert.AreEqual(1, type2.BaseTypeNames.Count);
            Assert.AreEqual("typeF", type2.BaseTypeNames[0]);

            var type3 = data.Domain.Types[3];
            Assert.AreEqual("typeD", type3.TypeName);
            Assert.AreEqual(2, type3.BaseTypeNames.Count);
            Assert.AreEqual("typeA", type3.BaseTypeNames[0]);
            Assert.AreEqual("typeB", type3.BaseTypeNames[1]);

            var type4 = data.Domain.Types[4];
            Assert.AreEqual("typeE", type4.TypeName);
            Assert.AreEqual(1, type4.BaseTypeNames.Count);
            Assert.AreEqual(Type.DEFAULT_BASE_TYPE, type4.BaseTypeNames[0]);

            var type5 = data.Domain.Types[5];
            Assert.AreEqual("typeF", type5.TypeName);
            Assert.AreEqual(1, type5.BaseTypeNames.Count);
            Assert.AreEqual(Type.DEFAULT_BASE_TYPE, type5.BaseTypeNames[0]);
        }
    }
}
