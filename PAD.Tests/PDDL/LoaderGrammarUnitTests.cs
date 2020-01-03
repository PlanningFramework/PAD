using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAD.InputData.PDDL.Loader.Grammar;
using PAD.InputData.PDDL.Loader.Ast;
using PAD.InputData.PDDL.Traits;
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.Tests.PDDL
{
    /// <summary>
    /// Testing suite for all the specified branches (rules) of PDDL grammar.
    /// Covers the entire phase from taking an input file, parsing it into a parsing tree and converting into an AST.
    /// </summary>
    [TestClass]
    public class LoaderGrammarUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private static string GetFilePath(string fileName)
        {
            return $@"..\..\PDDL\LoaderGrammarTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_GrammarAction()
        {
            var astNode = Utilities.LoadAst(typeof(Action), GetFilePath("TC_GrammarAction.pddl"));

            Assert.IsTrue(astNode is DomainActionAstNode);
            var action = (DomainActionAstNode)astNode;

            Assert.AreEqual("actionName", action.Name);

            Assert.IsNotNull(action.Parameters);
            Assert.AreEqual(2, action.Parameters.TypedIdentifiers.Count);

            Assert.IsNull(action.Preconditions);
            Assert.IsNull(action.Effects);
        }

        [TestMethod]
        public void TC_GrammarConGD()
        {
            var astNode = Utilities.LoadAst(typeof(ConGd), GetFilePath("TC_GrammarConGD.pddl"));

            Assert.IsTrue(astNode is AndConGdAstNode);
            var andExprArgs = ((AndConGdAstNode)astNode).Arguments;

            Assert.AreEqual(11, andExprArgs.Count);

            Assert.IsTrue(andExprArgs[0] is ForallConGdAstNode);
            Assert.IsTrue(andExprArgs[1] is AtEndConGdAstNode);
            Assert.IsTrue(andExprArgs[2] is AlwaysConGdAstNode);
            Assert.IsTrue(andExprArgs[3] is SometimeConGdAstNode);
            Assert.IsTrue(andExprArgs[4] is WithinConGdAstNode);
            Assert.IsTrue(andExprArgs[5] is AtMostOnceConGdAstNode);
            Assert.IsTrue(andExprArgs[6] is SometimeAfterConGdAstNode);
            Assert.IsTrue(andExprArgs[7] is SometimeBeforeConGdAstNode);
            Assert.IsTrue(andExprArgs[8] is AlwaysWithinConGdAstNode);
            Assert.IsTrue(andExprArgs[9] is HoldDuringConGdAstNode);
            Assert.IsTrue(andExprArgs[10] is HoldAfterConGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarDaEffect()
        {
            var astNode = Utilities.LoadAst(typeof(DaEffect), GetFilePath("TC_GrammarDaEffect.pddl"));

            Assert.IsTrue(astNode is AndDaEffectsAstNode);
            var effects = ((AndDaEffectsAstNode)astNode).Arguments;

            Assert.AreEqual(3, effects.Count);
            Assert.IsTrue(effects[0] is AtTimedEffectAstNode);
            Assert.IsTrue(effects[1] is ForallDaEffectAstNode);
            Assert.IsTrue(effects[2] is WhenDaEffectAstNode);

            var effect1 = (ForallDaEffectAstNode)effects[1];
            Assert.AreEqual(1, effect1.Parameters.TypedIdentifiers.Count);
            Assert.IsTrue(effect1.Effect is AtTimedEffectAstNode);

            var effect2 = (WhenDaEffectAstNode)effects[2];
            Assert.IsTrue(effect2.Condition is AtTimedDaGdAstNode);
            Assert.IsTrue(effect2.Effect is AtTimedEffectAstNode);
        }

        [TestMethod]
        public void TC_GrammarDaGD()
        {
            var astNode = Utilities.LoadAst(typeof(DaGd), GetFilePath("TC_GrammarDaGD.pddl"));

            Assert.IsTrue(astNode is AndDaGdAstNode);
            var andExprArgs = ((AndDaGdAstNode)astNode).Arguments;

            Assert.AreEqual(5, andExprArgs.Count);
            Assert.IsTrue(andExprArgs[0] is ForallDaGdAstNode);
            Assert.IsTrue(andExprArgs[1] is PreferenceDaGdAstNode);
            Assert.IsTrue(andExprArgs[2] is AtTimedDaGdAstNode);
            Assert.IsTrue(andExprArgs[3] is AtTimedDaGdAstNode);
            Assert.IsTrue(andExprArgs[4] is OverTimedDaGdAstNode);

            var forallExpr = (ForallDaGdAstNode)andExprArgs[0];
            Assert.IsTrue(forallExpr.Expression is AtTimedDaGdAstNode);

            var preferenceExpr = (PreferenceDaGdAstNode)andExprArgs[1];
            Assert.AreEqual("prefName", preferenceExpr.Name);
            Assert.IsTrue(preferenceExpr.Argument is AtTimedDaGdAstNode);

            var atExpr1 = (AtTimedDaGdAstNode)andExprArgs[2];
            Assert.AreEqual(TimeSpecifier.START, atExpr1.TimeSpecifier);
            Assert.IsTrue(atExpr1.Argument is PredicateGdAstNode);

            var atExpr2 = (AtTimedDaGdAstNode)andExprArgs[3];
            Assert.AreEqual(TimeSpecifier.END, atExpr2.TimeSpecifier);
            Assert.IsTrue(atExpr2.Argument is PredicateGdAstNode);

            var overExpr = (OverTimedDaGdAstNode)andExprArgs[4];
            Assert.AreEqual(IntervalSpecifier.ALL, overExpr.IntervalSpecifier);
            Assert.IsTrue(overExpr.Argument is PredicateGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarDerivedPred()
        {
            var astNode = Utilities.LoadAst(typeof(DerivedPred), GetFilePath("TC_GrammarDerivedPred.pddl"));

            Assert.IsTrue(astNode is DomainDerivedPredAstNode);
            var derivedPred = (DomainDerivedPredAstNode)astNode;

            Assert.IsNotNull(derivedPred.Predicate);
            Assert.IsTrue(derivedPred.Expression is ForallGdAstNode);

            Assert.AreEqual("predA", derivedPred.Predicate.Name);
            Assert.AreEqual(1, derivedPred.Predicate.Arguments.TypedIdentifiers.Count);
        }

        [TestMethod]
        public void TC_GrammarDomain()
        {
            var astNode = Utilities.LoadAst(typeof(Domain), GetFilePath("TC_GrammarDomain.pddl"));

            Assert.IsTrue(astNode is DomainAstNode);
            var domain = (DomainAstNode)astNode;
            var sections = domain.DomainSections;

            Assert.AreEqual("domainName", domain.DomainName);

            Assert.AreEqual(9, sections.Count);
            Assert.IsTrue(sections[0] is DomainRequirementsAstNode);
            Assert.IsTrue(sections[1] is DomainTypesAstNode);
            Assert.IsTrue(sections[2] is DomainConstantsAstNode);
            Assert.IsTrue(sections[3] is DomainPredicatesAstNode);
            Assert.IsTrue(sections[4] is DomainFunctionsAstNode);
            Assert.IsTrue(sections[5] is DomainConstraintsAstNode);
            Assert.IsTrue(sections[6] is DomainActionAstNode);
            Assert.IsTrue(sections[7] is DomainDurActionAstNode);
            Assert.IsTrue(sections[8] is DomainDerivedPredAstNode);

            var requirements = (DomainRequirementsAstNode)sections[0];
            Assert.AreEqual(2, requirements.RequirementsList.Count);

            var types = (DomainTypesAstNode)sections[1];
            Assert.AreEqual(2, types.TypesList.TypedIdentifiers.Count);

            var constants = (DomainConstantsAstNode)sections[2];
            Assert.AreEqual(2, constants.ConstantsList.TypedIdentifiers.Count);

            var predicates = (DomainPredicatesAstNode)sections[3];
            Assert.AreEqual(2, predicates.PredicatesList.Count);

            var functions = (DomainFunctionsAstNode)sections[4];
            Assert.AreEqual(2, functions.FunctionTypedList.FunctionsList.Count);

            var constraints = (DomainConstraintsAstNode)sections[5];
            Assert.IsTrue(constraints.Expression is AlwaysConGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarDurAction()
        {
            var astNode = Utilities.LoadAst(typeof(DurAction), GetFilePath("TC_GrammarDurAction.pddl"));

            Assert.IsTrue(astNode is DomainDurActionAstNode);
            var action = (DomainDurActionAstNode)astNode;

            Assert.AreEqual("actionName", action.Name);

            Assert.IsNotNull(action.Parameters);
            Assert.AreEqual(2, action.Parameters.TypedIdentifiers.Count);

            Assert.IsNull(action.DurationConstraint);
            Assert.IsNull(action.Condition);
            Assert.IsNull(action.Effect);
        }

        [TestMethod]
        public void TC_GrammarDurConstr()
        {
            var astNode = Utilities.LoadAst(typeof(DurConstr), GetFilePath("TC_GrammarDurConstr.pddl"));

            Assert.IsTrue(astNode is AndSimpleDurationConstraintsAstNode);
            var constrs = ((AndSimpleDurationConstraintsAstNode)astNode).Arguments;

            Assert.AreEqual(4, constrs.Count);
            Assert.IsTrue(constrs[0] is AtSimpleDurationConstraintAstNode);
            Assert.IsTrue(constrs[1] is AtSimpleDurationConstraintAstNode);
            Assert.IsTrue(constrs[2] is CompOpSimpleDurationConstraintAstNode);
            Assert.IsTrue(constrs[3] is CompOpSimpleDurationConstraintAstNode);

            var constr0 = (AtSimpleDurationConstraintAstNode)constrs[0];
            Assert.AreEqual(TimeSpecifier.START, constr0.TimeSpecifier);
            Assert.IsTrue(constr0.DurationConstraint is CompOpSimpleDurationConstraintAstNode);

            var constr1 = (AtSimpleDurationConstraintAstNode)constrs[1];
            Assert.AreEqual(TimeSpecifier.END, constr1.TimeSpecifier);
            Assert.IsTrue(constr1.DurationConstraint is CompOpSimpleDurationConstraintAstNode);

            var constr2 = (CompOpSimpleDurationConstraintAstNode)constrs[2];
            Assert.AreEqual(DurationComparer.LTE, constr2.DurationComparer);
            Assert.IsTrue(constr2.DurationArgument is NumericOpAstNode);

            var constr3 = (CompOpSimpleDurationConstraintAstNode)constrs[3];
            Assert.AreEqual(DurationComparer.EQ, constr3.DurationComparer);
            Assert.IsTrue(constr3.DurationArgument is FunctionTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarEffect()
        {
            var astNode = Utilities.LoadAst(typeof(Effect), GetFilePath("TC_GrammarEffect.pddl"));

            Assert.IsTrue(astNode is AndCEffectsAstNode);
            var effects = ((AndCEffectsAstNode)astNode).Arguments;

            Assert.AreEqual(4, effects.Count);
            Assert.IsTrue(effects[0] is PredicatePEffectAstNode);
            Assert.IsTrue(effects[1] is AssignPEffectAstNode);
            Assert.IsTrue(effects[2] is ForallCEffectAstNode);
            Assert.IsTrue(effects[3] is WhenCEffectAstNode);

            var effect2 = (ForallCEffectAstNode)effects[2];
            Assert.AreEqual(1, effect2.Parameters.TypedIdentifiers.Count);
            Assert.IsTrue(effect2.Effect is PredicatePEffectAstNode);

            var effect3 = (WhenCEffectAstNode)effects[3];
            Assert.IsTrue(effect3.Condition is PredicateGdAstNode);
            Assert.IsTrue(effect3.Effect is AndPEffectsAstNode);
        }
        
        [TestMethod]
        public void TC_GrammarFunctionTerm()
        {
            var astNode = Utilities.LoadAst(typeof(FunctionTerm), GetFilePath("TC_GrammarFunctionTerm.pddl"));

            Assert.IsTrue(astNode is FunctionTermAstNode);
            var function = (FunctionTermAstNode)astNode;
            var terms = function.Terms;

            Assert.AreEqual("funcName", function.Name);
            Assert.AreEqual(3, terms.Count);

            Assert.IsTrue(terms[0] is FunctionTermAstNode);
            var term0 = (FunctionTermAstNode)terms[0];
            Assert.AreEqual("funcA", term0.Name);
            Assert.AreEqual(0, term0.Terms.Count);

            Assert.IsTrue(terms[1] is IdentifierTermAstNode);
            var term1 = (IdentifierTermAstNode)terms[1];
            Assert.AreEqual("funcB", term1.Name);

            Assert.IsTrue(terms[2] is IdentifierTermAstNode);
            var term2 = (IdentifierTermAstNode)terms[2];
            Assert.AreEqual("?aa", term2.Name);
        }

        [TestMethod]
        public void TC_GrammarFunctionTermC()
        {
            var astNode = Utilities.LoadAst(typeof(FunctionTermC), GetFilePath("TC_GrammarFunctionTermC.pddl"));

            Assert.IsTrue(astNode is FunctionTermAstNode);
            var function = (FunctionTermAstNode)astNode;
            var terms = function.Terms;

            Assert.AreEqual("funcName", function.Name);
            Assert.AreEqual(3, terms.Count);

            Assert.IsTrue(terms[0] is IdentifierTermAstNode);
            var term0 = (IdentifierTermAstNode)terms[0];
            Assert.AreEqual("constA", term0.Name);

            Assert.IsTrue(terms[1] is IdentifierTermAstNode);
            var term1 = (IdentifierTermAstNode)terms[1];
            Assert.AreEqual("funcA", term1.Name);
            
            Assert.IsTrue(terms[2] is FunctionTermAstNode);
            var term2 = (FunctionTermAstNode)terms[2];
            Assert.AreEqual("funcB", term2.Name);
            Assert.AreEqual(0, term2.Terms.Count);
        }

        [TestMethod]
        public void TC_GrammarFunctionTypedList()
        {
            var astNode = Utilities.LoadAst(typeof(FunctionTypedList), GetFilePath("TC_GrammarFunctionTypedList.pddl"));

            Assert.IsTrue(astNode is FunctionTypedListAstNode);
            var list = ((FunctionTypedListAstNode)astNode).FunctionsList;
            Assert.AreEqual(5, list.Count);

            Assert.AreEqual("funcA", list[0].Item1);
            Assert.AreEqual("typeA", list[0].Item3);
            Assert.AreEqual("?aa", list[0].Item2.TypedIdentifiers[0].Item1);
            Assert.AreEqual("typeX", list[0].Item2.TypedIdentifiers[0].Item2);
            Assert.AreEqual("?bb", list[0].Item2.TypedIdentifiers[1].Item1);
            Assert.AreEqual("typeX", list[0].Item2.TypedIdentifiers[1].Item2);

            Assert.AreEqual("funcB", list[1].Item1);
            Assert.AreEqual("typeA", list[1].Item3);

            Assert.AreEqual("funcC", list[2].Item1);
            Assert.AreEqual("typeB;typeC", list[2].Item3);

            Assert.AreEqual("funcD", list[3].Item1);
            Assert.AreEqual("", list[3].Item3);

            Assert.AreEqual("funcE", list[4].Item1);
            Assert.AreEqual("", list[4].Item3);
        }

        [TestMethod]
        public void TC_GrammarGD()
        {
            var astNode = Utilities.LoadAst(typeof(Gd), GetFilePath("TC_GrammarGD.pddl"));

            Assert.IsTrue(astNode is AndGdAstNode);
            var andExprArgs = ((AndGdAstNode)astNode).Arguments;

            Assert.AreEqual(8, andExprArgs.Count);
            Assert.IsTrue(andExprArgs[0] is PredicateGdAstNode);
            Assert.IsTrue(andExprArgs[1] is NotGdAstNode);
            Assert.IsTrue(andExprArgs[2] is OrGdAstNode);
            Assert.IsTrue(andExprArgs[3] is ImplyGdAstNode);
            Assert.IsTrue(andExprArgs[4] is ExistsGdAstNode);
            Assert.IsTrue(andExprArgs[5] is ForallGdAstNode);
            Assert.IsTrue(andExprArgs[6] is EqualsOpGdAstNode);
            Assert.IsTrue(andExprArgs[7] is NumCompGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarIdentifier()
        {
            var astNode = Utilities.LoadAst(typeof(PredicateGd), GetFilePath("TC_GrammarIdentifier.pddl"));

            Assert.IsTrue(astNode is PredicateGdAstNode);
            var terms = ((PredicateGdAstNode)astNode).Terms;
            Assert.AreEqual(3, terms.Count);

            Assert.IsTrue(terms[0] is IdentifierTermAstNode);
            var term0 = (IdentifierTermAstNode)terms[0];
            Assert.AreEqual("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_", term0.Name);
            Assert.IsFalse(term0.IsVariable());

            Assert.IsTrue(terms[1] is IdentifierTermAstNode);
            var term1 = (IdentifierTermAstNode)terms[1];
            Assert.AreEqual("?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_", term1.Name);
            Assert.IsTrue(term1.IsVariable());

            Assert.IsTrue(terms[2] is IdentifierTermAstNode);
            var term2 = (IdentifierTermAstNode)terms[2];
            Assert.AreEqual("rgdgdg-vsfds-dvs_rgsvd-dfvs_df", term2.Name);
        }

        [TestMethod]
        public void TC_GrammarInitElem()
        {
            var astNode = Utilities.LoadAst(typeof(Problem), GetFilePath("TC_GrammarInitElem.pddl"));

            Assert.IsTrue(astNode is ProblemAstNode);
            var problem = (ProblemAstNode)astNode;

            Assert.IsTrue(problem.ProblemSections[0] is ProblemInitAstNode);
            var initElemList = ((ProblemInitAstNode)problem.ProblemSections[0]).InitElemList;
            Assert.AreEqual(5, initElemList.Count);

            Assert.IsTrue(initElemList[0] is PredicateInitElemAstNode);
            Assert.IsTrue(initElemList[1] is EqualsOpInitElemAstNode);
            Assert.IsTrue(initElemList[2] is EqualsOpInitElemAstNode);
            Assert.IsTrue(initElemList[3] is NotInitElemAstNode);
            Assert.IsTrue(initElemList[4] is AtInitElemAstNode);
        }

        [TestMethod]
        public void TC_GrammarMetricExpr()
        {
            var astNode = Utilities.LoadAst(typeof(MetricExpr), GetFilePath("TC_GrammarMetricExpr.pddl"));

            Assert.IsTrue(astNode is NumericOpAstNode);
            var plusOp = (NumericOpAstNode)astNode;
            Assert.AreEqual(6, plusOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.PLUS, plusOp.Operator);

            Assert.IsTrue(plusOp.Arguments[0] is FunctionTermAstNode);
            Assert.IsTrue(plusOp.Arguments[1] is NumberTermAstNode);
            Assert.IsTrue(plusOp.Arguments[2] is FunctionTermAstNode);
            Assert.IsTrue(plusOp.Arguments[3] is FunctionTermAstNode);
            Assert.IsTrue(plusOp.Arguments[4] is NumericOpAstNode);
            Assert.IsTrue(plusOp.Arguments[5] is MetricPreferenceViolationAstNode);
            Assert.AreEqual("prefName", ((MetricPreferenceViolationAstNode)plusOp.Arguments[5]).Name);
        }

        [TestMethod]
        public void TC_GrammarNumericExpr()
        {
            var astNode = Utilities.LoadAst(typeof(NumericExpr), GetFilePath("TC_GrammarNumericExpr.pddl"));

            Assert.IsTrue(astNode is NumericOpAstNode);
            var mulOp = (NumericOpAstNode)astNode;
            Assert.AreEqual(4, mulOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.MUL, mulOp.Operator);

            Assert.IsTrue(mulOp.Arguments[0] is NumericOpAstNode);
            Assert.IsTrue(mulOp.Arguments[1] is FunctionTermAstNode);
            Assert.IsTrue(mulOp.Arguments[2] is FunctionTermAstNode);
            Assert.IsTrue(mulOp.Arguments[3] is NumberTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarNumericExprDa()
        {
            var astNode = Utilities.LoadAst(typeof(NumericExprDa), GetFilePath("TC_GrammarNumericExprDa.pddl"));

            Assert.IsTrue(astNode is NumericOpAstNode);
            var plusOp = (NumericOpAstNode)astNode;
            Assert.AreEqual(5, plusOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.PLUS, plusOp.Operator);

            Assert.IsTrue(plusOp.Arguments[0] is NumberTermAstNode);
            Assert.IsTrue(plusOp.Arguments[1] is FunctionTermAstNode);
            Assert.IsTrue(plusOp.Arguments[2] is FunctionTermAstNode);
            Assert.IsTrue(plusOp.Arguments[3] is NumericOpAstNode);
            Assert.IsTrue(plusOp.Arguments[4] is DurationVariableTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarNumericExprT()
        {
            var astNode = Utilities.LoadAst(typeof(DaEffect), GetFilePath("TC_GrammarNumericExprT.pddl"));

            Assert.IsTrue(astNode is AndDaEffectsAstNode);
            var timedEffects = ((AndDaEffectsAstNode)astNode).Arguments;

            Assert.AreEqual(3, timedEffects.Count);
            Assert.IsTrue(timedEffects[0] is AssignTimedEffectAstNode);
            Assert.IsTrue(timedEffects[1] is AssignTimedEffectAstNode);
            Assert.IsTrue(timedEffects[2] is AssignTimedEffectAstNode);

            var effect0 = (AssignTimedEffectAstNode)timedEffects[0];
            var timedExpr0 = effect0.Expression;
            Assert.IsFalse(timedExpr0.IsProductExpression);
            Assert.IsNull(timedExpr0.ProductExprNumericFactor);

            var effect1 = (AssignTimedEffectAstNode)timedEffects[1];
            var timedExpr1 = effect1.Expression;
            Assert.IsTrue(timedExpr1.IsProductExpression);
            Assert.IsNotNull(timedExpr1.ProductExprNumericFactor);
            Assert.IsTrue(timedExpr1.ProductExprNumericFactor is NumberTermAstNode);

            var effect2 = (AssignTimedEffectAstNode)timedEffects[2];
            var timedExpr2 = effect2.Expression;
            Assert.IsTrue(timedExpr2.IsProductExpression);
            Assert.IsNotNull(timedExpr2.ProductExprNumericFactor);
            Assert.IsTrue(timedExpr2.ProductExprNumericFactor is NumericOpAstNode);
        }

        [TestMethod]
        public void TC_GrammarNumericOp()
        {
            var astNode = Utilities.LoadAst(typeof(NumericExpr), GetFilePath("TC_GrammarNumericOp.pddl"));

            Assert.IsTrue(astNode is NumericOpAstNode);
            var plusOp = (NumericOpAstNode)astNode;
            Assert.AreEqual(4, plusOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.PLUS, plusOp.Operator);

            Assert.IsTrue(plusOp.Arguments[0] is NumericOpAstNode);
            var mulOp = (NumericOpAstNode)plusOp.Arguments[0];
            Assert.AreEqual(3, mulOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.MUL, mulOp.Operator);
            Assert.IsTrue(mulOp.Arguments[0] is NumberTermAstNode);
            Assert.IsTrue(mulOp.Arguments[1] is NumberTermAstNode);
            Assert.IsTrue(mulOp.Arguments[2] is FunctionTermAstNode);

            Assert.IsTrue(plusOp.Arguments[1] is NumericOpAstNode);
            var minusOp = (NumericOpAstNode)plusOp.Arguments[1];
            Assert.AreEqual(1, minusOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.MINUS, minusOp.Operator);
            Assert.IsTrue(minusOp.Arguments[0] is NumericOpAstNode);

            Assert.IsTrue(plusOp.Arguments[2] is NumericOpAstNode);
            var minusOp2 = (NumericOpAstNode)plusOp.Arguments[2];
            Assert.AreEqual(2, minusOp2.Arguments.Count);
            Assert.AreEqual(NumericOperator.MINUS, minusOp2.Operator);
            Assert.IsTrue(minusOp2.Arguments[0] is FunctionTermAstNode);
            Assert.IsTrue(minusOp2.Arguments[1] is NumberTermAstNode);

            Assert.IsTrue(plusOp.Arguments[3] is NumericOpAstNode);
            var divOp = (NumericOpAstNode)plusOp.Arguments[3];
            Assert.AreEqual(2, divOp.Arguments.Count);
            Assert.AreEqual(NumericOperator.DIV, divOp.Operator);
            Assert.IsTrue(divOp.Arguments[0] is NumberTermAstNode);
            Assert.IsTrue(divOp.Arguments[1] is NumberTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarPEffect()
        {
            var astNode = Utilities.LoadAst(typeof(Effect), GetFilePath("TC_GrammarPEffect.pddl"));

            Assert.IsTrue(astNode is AndCEffectsAstNode);
            var andExprArgs = ((AndCEffectsAstNode)astNode).Arguments;

            Assert.AreEqual(6, andExprArgs.Count);

            Assert.IsTrue(andExprArgs[0] is PredicatePEffectAstNode);

            Assert.IsTrue(andExprArgs[1] is EqualsOpPEffectAstNode);
            var expr1 = (EqualsOpPEffectAstNode)andExprArgs[1];
            Assert.IsTrue(expr1.Term1 is IdentifierTermAstNode);
            Assert.IsTrue(expr1.Term2 is NumberTermAstNode);

            Assert.IsTrue(andExprArgs[2] is NotPEffectAstNode);
            var expr2 = (NotPEffectAstNode)andExprArgs[2];
            Assert.IsNotNull(expr2.Argument);

            Assert.IsTrue(andExprArgs[3] is AssignPEffectAstNode);
            var expr3 = (AssignPEffectAstNode)andExprArgs[3];
            Assert.AreEqual(AssignOperator.ASSIGN, expr3.AssignOperator);
            Assert.IsNotNull(expr3.Argument1);
            Assert.IsTrue(expr3.Argument2 is IdentifierTermAstNode);

            Assert.IsTrue(andExprArgs[4] is AssignPEffectAstNode);
            var expr4 = (AssignPEffectAstNode)andExprArgs[4];
            Assert.AreEqual(AssignOperator.ASSIGN, expr4.AssignOperator);
            Assert.IsNotNull(expr4.Argument1);
            Assert.IsTrue(expr4.Argument2 is UndefinedFuncValAstNode);

            Assert.IsTrue(andExprArgs[5] is AssignPEffectAstNode);
            var expr5 = (AssignPEffectAstNode)andExprArgs[5];
            Assert.AreEqual(AssignOperator.INCREASE, expr5.AssignOperator);
            Assert.IsNotNull(expr5.Argument1);
            Assert.IsTrue(expr5.Argument2 is NumberTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarPEffectT()
        {
            var astNode = Utilities.LoadAst(typeof(TimedEffect), GetFilePath("TC_GrammarPEffectT.pddl"));

            Assert.IsTrue(astNode is AtTimedEffectAstNode);
            var effect = ((AtTimedEffectAstNode)astNode).Effect;

            Assert.IsTrue(effect is AndPEffectsAstNode);
            var andExprArgs = ((AndPEffectsAstNode)effect).Arguments;

            Assert.AreEqual(4, andExprArgs.Count);

            Assert.IsTrue(andExprArgs[0] is PredicatePEffectAstNode);

            Assert.IsTrue(andExprArgs[1] is AssignPEffectAstNode);
            var equalExpr1 = (AssignPEffectAstNode)andExprArgs[1];
            Assert.AreEqual(AssignOperator.ASSIGN, equalExpr1.AssignOperator);
            Assert.IsNotNull(equalExpr1.Argument1);
            Assert.IsTrue(equalExpr1.Argument2 is DurationVariableTermAstNode);

            Assert.IsTrue(andExprArgs[2] is AssignPEffectAstNode);
            var equalExpr2 = (AssignPEffectAstNode)andExprArgs[2];
            Assert.AreEqual(AssignOperator.ASSIGN, equalExpr2.AssignOperator);
            Assert.IsNotNull(equalExpr2.Argument1);
            Assert.IsTrue(equalExpr2.Argument2 is NumericOpAstNode);

            Assert.IsTrue(andExprArgs[3] is AssignPEffectAstNode);
            var equalExpr3 = (AssignPEffectAstNode)andExprArgs[3];
            Assert.AreEqual(AssignOperator.INCREASE, equalExpr3.AssignOperator);
            Assert.IsNotNull(equalExpr3.Argument1);
            Assert.IsTrue(equalExpr3.Argument2 is DurationVariableTermAstNode);
        }

        [TestMethod]
        public void TC_GrammarPredicateGD()
        {
            var astNode = Utilities.LoadAst(typeof(PredicateGd), GetFilePath("TC_GrammarPredicateGD.pddl"));

            Assert.IsTrue(astNode is PredicateGdAstNode);
            var predicate = (PredicateGdAstNode)astNode;
            var terms = predicate.Terms;

            Assert.AreEqual("predName", predicate.Name);
            Assert.AreEqual(3, terms.Count);

            Assert.IsTrue(terms[0] is IdentifierTermAstNode);
            var term0 = (IdentifierTermAstNode)terms[0];
            Assert.AreEqual("?aa", term0.Name);

            Assert.IsTrue(terms[1] is FunctionTermAstNode);
            var term1 = (FunctionTermAstNode)terms[1];
            Assert.AreEqual("func", term1.Name);
            Assert.AreEqual(0, term1.Terms.Count);

            Assert.IsTrue(terms[2] is IdentifierTermAstNode);
            var term2 = (IdentifierTermAstNode)terms[2];
            Assert.AreEqual("cc", term2.Name);
        }

        [TestMethod]
        public void TC_GrammarPrefConGD()
        {
            var astNode = Utilities.LoadAst(typeof(PrefConGd), GetFilePath("TC_GrammarPrefConGD.pddl"));

            Assert.IsTrue(astNode is AndConGdAstNode);
            var andExprArgs = ((AndConGdAstNode)astNode).Arguments;

            Assert.AreEqual(13, andExprArgs.Count);

            Assert.IsTrue(andExprArgs[0] is ForallConGdAstNode);
            Assert.IsTrue(andExprArgs[1] is AtEndConGdAstNode);
            Assert.IsTrue(andExprArgs[2] is AlwaysConGdAstNode);
            Assert.IsTrue(andExprArgs[3] is SometimeConGdAstNode);
            Assert.IsTrue(andExprArgs[4] is WithinConGdAstNode);
            Assert.IsTrue(andExprArgs[5] is AtMostOnceConGdAstNode);
            Assert.IsTrue(andExprArgs[6] is SometimeAfterConGdAstNode);
            Assert.IsTrue(andExprArgs[7] is SometimeBeforeConGdAstNode);
            Assert.IsTrue(andExprArgs[8] is AlwaysWithinConGdAstNode);
            Assert.IsTrue(andExprArgs[9] is HoldDuringConGdAstNode);
            Assert.IsTrue(andExprArgs[10] is HoldAfterConGdAstNode);

            Assert.IsTrue(andExprArgs[11] is PreferenceConGdAstNode);
            var preference = (PreferenceConGdAstNode)andExprArgs[11];
            Assert.AreEqual("prefName", preference.Name);
            Assert.IsTrue(preference.Argument is AlwaysConGdAstNode);

            Assert.IsTrue(andExprArgs[12] is PreferenceConGdAstNode);
            var preferenceWithEmptyName = (PreferenceConGdAstNode)andExprArgs[12];
            Assert.AreEqual("", preferenceWithEmptyName.Name);
            Assert.IsTrue(preferenceWithEmptyName.Argument is AlwaysConGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarPreGD()
        {
            var astNode = Utilities.LoadAst(typeof(PreGd), GetFilePath("TC_GrammarPreGD.pddl"));

            Assert.IsTrue(astNode is AndGdAstNode);
            var andExprArgs = ((AndGdAstNode)astNode).Arguments;

            Assert.AreEqual(9, andExprArgs.Count);
            Assert.IsTrue(andExprArgs[0] is PredicateGdAstNode);
            Assert.IsTrue(andExprArgs[1] is NotGdAstNode);
            Assert.IsTrue(andExprArgs[2] is OrGdAstNode);
            Assert.IsTrue(andExprArgs[3] is ImplyGdAstNode);
            Assert.IsTrue(andExprArgs[4] is ExistsGdAstNode);
            Assert.IsTrue(andExprArgs[5] is ForallGdAstNode);
            Assert.IsTrue(andExprArgs[6] is EqualsOpGdAstNode);
            Assert.IsTrue(andExprArgs[7] is NumCompGdAstNode);

            Assert.IsTrue(andExprArgs[8] is PreferenceGdAstNode);
            var preference = (PreferenceGdAstNode)andExprArgs[8];
            Assert.AreEqual("prefName", preference.Name);
            Assert.IsTrue(preference.Argument is PredicateGdAstNode);
        }

        [TestMethod]
        public void TC_GrammarProblem()
        {
            var astNode = Utilities.LoadAst(typeof(Problem), GetFilePath("TC_GrammarProblem.pddl"));

            Assert.IsTrue(astNode is ProblemAstNode);
            var problem = (ProblemAstNode)astNode;
            var sections = problem.ProblemSections;

            Assert.AreEqual("problemName", problem.ProblemName);
            Assert.AreEqual("domainName", problem.CorrespondingDomainName);

            Assert.AreEqual(7, sections.Count);
            Assert.IsTrue(sections[0] is ProblemRequirementsAstNode);
            Assert.IsTrue(sections[1] is ProblemObjectsAstNode);
            Assert.IsTrue(sections[2] is ProblemInitAstNode);
            Assert.IsTrue(sections[3] is ProblemGoalAstNode);
            Assert.IsTrue(sections[4] is ProblemConstraintsAstNode);
            Assert.IsTrue(sections[5] is ProblemMetricAstNode);
            Assert.IsTrue(sections[6] is ProblemLengthAstNode);

            var requirements = (ProblemRequirementsAstNode)sections[0];
            Assert.AreEqual(2, requirements.RequirementsList.Count);

            var objects = (ProblemObjectsAstNode)sections[1];
            Assert.AreEqual(2, objects.ObjectsList.TypedIdentifiers.Count);

            var init = (ProblemInitAstNode)sections[2];
            Assert.AreEqual(3, init.InitElemList.Count);

            var goal = (ProblemGoalAstNode)sections[3];
            Assert.IsTrue(goal.Condition is PredicateGdAstNode);

            var constraints = (ProblemConstraintsAstNode)sections[4];
            Assert.IsTrue(constraints.Expression is AtMostOnceConGdAstNode);

            var metric = (ProblemMetricAstNode)sections[5];
            Assert.AreEqual(OptimizationSpecifier.MINIMIZE, metric.OptimizationSpecifier);
            Assert.IsTrue(metric.MetricExpression is MetricPreferenceViolationAstNode);

            var length = (ProblemLengthAstNode)sections[6];
            Assert.AreEqual(2, length.LengthSpecifications.Count);
            Assert.AreEqual(LengthSpecifier.SERIAL, length.LengthSpecifications[0].Item1);
            Assert.AreEqual(88, length.LengthSpecifications[0].Item2);
            Assert.AreEqual(LengthSpecifier.PARALLEL, length.LengthSpecifications[1].Item1);
            Assert.AreEqual(99, length.LengthSpecifications[1].Item2);
        }
        
        [TestMethod]
        public void TC_GrammarTerm()
        {
            var astNode = Utilities.LoadAst(typeof(PredicateGd), GetFilePath("TC_GrammarTerm.pddl"));

            Assert.IsTrue(astNode is PredicateGdAstNode);
            var terms = ((PredicateGdAstNode)astNode).Terms;

            Assert.AreEqual(3, terms.Count);
            Assert.IsTrue(terms[0] is IdentifierTermAstNode);
            Assert.IsTrue(terms[1] is IdentifierTermAstNode);
            Assert.IsTrue(terms[2] is FunctionTermAstNode);

            var term0 = (IdentifierTermAstNode)terms[0];
            Assert.AreEqual("?aa", term0.Name);

            var term1 = (IdentifierTermAstNode)terms[1];
            Assert.AreEqual("funcA", term1.Name);

            var term2 = (FunctionTermAstNode)terms[2];
            Assert.AreEqual("funcB", term2.Name);
            Assert.AreEqual(3, term2.Terms.Count);
        }

        [TestMethod]
        public void TC_GrammarTimedEffect()
        {
            var astNode = Utilities.LoadAst(typeof(DaEffect), GetFilePath("TC_GrammarTimedEffect.pddl"));

            Assert.IsTrue(astNode is AndDaEffectsAstNode);
            var timedEffects = ((AndDaEffectsAstNode)astNode).Arguments;

            Assert.AreEqual(4, timedEffects.Count);
            Assert.IsTrue(timedEffects[0] is AtTimedEffectAstNode);
            Assert.IsTrue(timedEffects[1] is AtTimedEffectAstNode);
            Assert.IsTrue(timedEffects[2] is AssignTimedEffectAstNode);
            Assert.IsTrue(timedEffects[3] is AssignTimedEffectAstNode);

            var effect0 = (AtTimedEffectAstNode)timedEffects[0];
            Assert.AreEqual(TimeSpecifier.START, effect0.TimeSpecifier);
            Assert.IsTrue(effect0.Effect is PredicatePEffectAstNode);

            var effect1 = (AtTimedEffectAstNode)timedEffects[1];
            Assert.AreEqual(TimeSpecifier.END, effect1.TimeSpecifier);
            Assert.IsTrue(effect1.Effect is AndPEffectsAstNode);

            var effect2 = (AssignTimedEffectAstNode)timedEffects[2];
            Assert.AreEqual(TimedEffectAssignOperator.INCREASE, effect2.AssignOperator);
            Assert.IsNotNull(effect2.Function);
            Assert.IsNotNull(effect2.Expression);

            var effect3 = (AssignTimedEffectAstNode)timedEffects[3];
            Assert.AreEqual(TimedEffectAssignOperator.DECREASE, effect3.AssignOperator);
            Assert.IsNotNull(effect3.Function);
            Assert.IsNotNull(effect3.Expression);
        }

        [TestMethod]
        public void TC_GrammarTypedList()
        {
            var astNode = Utilities.LoadAst(typeof(TypedList), GetFilePath("TC_GrammarTypedList.pddl"));

            Assert.IsTrue(astNode is TypedListAstNode);
            var list = ((TypedListAstNode)astNode).TypedIdentifiers;

            Assert.AreEqual(6, list.Count);

            Assert.AreEqual("?aa", list[0].Item1);
            Assert.AreEqual("typeA", list[0].Item2);

            Assert.AreEqual("?bb", list[1].Item1);
            Assert.AreEqual("typeA", list[1].Item2);

            Assert.AreEqual("?cc", list[2].Item1);
            Assert.AreEqual("typeB", list[2].Item2);

            Assert.AreEqual("?dd", list[3].Item1);
            Assert.AreEqual("typeC;typeD;typeE", list[3].Item2);

            Assert.AreEqual("?ee", list[4].Item1);
            Assert.AreEqual("", list[4].Item2);

            Assert.AreEqual("?ff", list[5].Item1);
            Assert.AreEqual("", list[5].Item2);
        }

        [TestMethod]
        public void TC_GrammarTypedListC()
        {
            var astNode = Utilities.LoadAst(typeof(TypedListC), GetFilePath("TC_GrammarTypedListC.pddl"));

            Assert.IsTrue(astNode is TypedListAstNode);
            var list = ((TypedListAstNode)astNode).TypedIdentifiers;

            Assert.AreEqual(5, list.Count);

            Assert.AreEqual("aa", list[0].Item1);
            Assert.AreEqual("typeA", list[0].Item2);

            Assert.AreEqual("bb", list[1].Item1);
            Assert.AreEqual("typeA", list[1].Item2);

            Assert.AreEqual("cc", list[2].Item1);
            Assert.AreEqual("typeB", list[2].Item2);

            Assert.AreEqual("dd", list[3].Item1);
            Assert.AreEqual("typeB", list[3].Item2);

            Assert.AreEqual("ee", list[4].Item1);
            Assert.AreEqual("", list[4].Item2);
        }

        [TestMethod]
        public void TC_GrammarValueOrTerm()
        {
            var astNode = Utilities.LoadAst(typeof(Gd), GetFilePath("TC_GrammarValueOrTerm.pddl"));

            Assert.IsTrue(astNode is AndGdAstNode);
            var andExprArgs = ((AndGdAstNode)astNode).Arguments;

            Assert.AreEqual(4, andExprArgs.Count);
            Assert.IsTrue(andExprArgs[0] is EqualsOpGdAstNode);
            Assert.IsTrue(andExprArgs[1] is EqualsOpGdAstNode);
            Assert.IsTrue(andExprArgs[2] is EqualsOpGdAstNode);
            Assert.IsTrue(andExprArgs[3] is EqualsOpGdAstNode);

            var equalExpr0 = (EqualsOpGdAstNode)andExprArgs[0];
            Assert.IsTrue(equalExpr0.Argument1 is FunctionTermAstNode);
            Assert.IsTrue(equalExpr0.Argument2 is IdentifierTermAstNode);

            var equalExpr1 = (EqualsOpGdAstNode)andExprArgs[1];
            Assert.IsTrue(equalExpr1.Argument1 is FunctionTermAstNode);
            Assert.IsTrue(equalExpr1.Argument2 is IdentifierTermAstNode);

            var equalExpr2 = (EqualsOpGdAstNode)andExprArgs[2];
            Assert.IsTrue(equalExpr2.Argument1 is IdentifierTermAstNode);
            Assert.IsTrue(equalExpr2.Argument2 is IdentifierTermAstNode);

            var equalExpr3 = (EqualsOpGdAstNode)andExprArgs[3];
            Assert.IsTrue(equalExpr3.Argument1 is NumberTermAstNode);
            Assert.IsTrue(equalExpr3.Argument2 is NumericOpAstNode);
        }

        [TestMethod]
        public void TC_GrammarValueOrTermT()
        {
            var astNode = Utilities.LoadAst(typeof(TimedEffect), GetFilePath("TC_GrammarValueOrTermT.pddl"));

            Assert.IsTrue(astNode is AtTimedEffectAstNode);
            var effect = ((AtTimedEffectAstNode)astNode).Effect;

            Assert.IsTrue(effect is AndPEffectsAstNode);
            var andExprArgs = ((AndPEffectsAstNode)effect).Arguments;

            Assert.AreEqual(3, andExprArgs.Count);

            Assert.IsTrue(andExprArgs[0] is AssignPEffectAstNode);
            var equalExpr0 = (AssignPEffectAstNode)andExprArgs[0];
            Assert.IsNotNull(equalExpr0.Argument1);
            Assert.IsTrue(equalExpr0.Argument2 is DurationVariableTermAstNode);

            Assert.IsTrue(andExprArgs[1] is AssignPEffectAstNode);
            var equalExpr1 = (AssignPEffectAstNode)andExprArgs[1];
            Assert.IsNotNull(equalExpr1.Argument1);
            Assert.IsTrue(equalExpr1.Argument2 is IdentifierTermAstNode);

            Assert.IsTrue(andExprArgs[2] is AssignPEffectAstNode);
            var equalExpr2 = (AssignPEffectAstNode)andExprArgs[2];
            Assert.IsNotNull(equalExpr2.Argument1);
            Assert.IsTrue(equalExpr2.Argument2 is DurationVariableTermAstNode);
        }
    }
}
