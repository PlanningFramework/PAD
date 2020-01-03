using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using PAD.InputData;
using PAD.Planner.PDDL;
// ReSharper disable CommentTypo
// ReSharper disable PossibleUnintendedReferenceComparison
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable ImplicitlyCapturedClosure

namespace PAD.Tests.PDDL
{
    /// <summary>
    /// Testing suite for the PDDL planner. Testing all components of the planning problem and the searching engine.
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
            return $@"..\..\PDDL\PlannerTestCases\{fileName}";
        }

        [TestMethod]
        public void TC_Atom()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Atom.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            var preconditions = problem.Operators[0].Preconditions;

            PredicateExpression pred0 = (PredicateExpression)preconditions[0];
            IAtom atom0 = pred0.PredicateAtom;
            Assert.AreEqual(factory.CreatePredicate("pred0").GetNameId(), atom0.GetNameId());
            Assert.AreEqual(0, atom0.GetArity());
            Assert.AreEqual(0, atom0.GetTerms().Count);
            Assert.AreEqual("pred0", ((Atom)atom0).GetFullName(idManager.Predicates));
            Assert.AreEqual("(pred0)", atom0.ToString(idManager.Predicates));
            IAtom atom0Clone = atom0.Clone();
            Assert.IsTrue(atom0 != atom0Clone);
            Assert.IsTrue(atom0.GetHashCode() == atom0Clone.GetHashCode());
            Assert.IsTrue(atom0.Equals(atom0Clone));

            PredicateExpression pred1 = (PredicateExpression)preconditions[1];
            IAtom atom1 = pred1.PredicateAtom;
            Assert.AreEqual(factory.CreatePredicate("pred1", "constA", "constA", "constA").GetNameId(), atom1.GetNameId());
            Assert.AreEqual(3, atom1.GetArity());
            Assert.AreEqual(Atom.NotGroundedTerm, atom1.GetGroundedTerm(0));
            Assert.AreEqual(idManager.Constants.GetId("constA"), atom1.GetGroundedTerm(1));
            Assert.AreEqual(Atom.NotGroundedTerm, atom1.GetGroundedTerm(2));
            Assert.AreEqual(3, atom1.GetTerms().Count);
            Assert.IsTrue(atom1.GetTerms()[0] is VariableTerm);
            Assert.IsTrue(atom1.GetTerms()[1] is ConstantTerm);
            Assert.IsTrue(atom1.GetTerms()[2] is ObjectFunctionTerm);
            Assert.AreEqual("pred1", ((Atom)atom1).GetFullName(idManager.Predicates));
            Assert.AreEqual("(pred1 ?var0 constA (objFunc))", atom1.ToString(idManager.Predicates));
            IAtom atom1Clone = atom1.Clone();
            Assert.IsTrue(atom1 != atom1Clone);
            Assert.IsTrue(atom1.GetHashCode() == atom1Clone.GetHashCode());
            Assert.IsTrue(atom1.Equals(atom1Clone));

            EqualsExpression equals2 = (EqualsExpression)preconditions[2];
            ObjectFunctionTerm objFunc2 = (ObjectFunctionTerm)equals2.LeftArgument;
            IAtom atom2 = objFunc2.FunctionAtom;
            Assert.AreEqual(factory.CreateFunction("objFunc").GetNameId(), atom2.GetNameId());
            Assert.AreEqual(0, atom2.GetArity());
            Assert.AreEqual(0, atom2.GetTerms().Count);
            Assert.AreEqual("objFunc", ((Atom)atom2).GetFullName(idManager.Functions));
            Assert.AreEqual("(objFunc)", atom2.ToString(idManager.Functions));
            IAtom atom2Clone = atom2.Clone();
            Assert.IsTrue(atom2 != atom2Clone);
            Assert.IsTrue(atom2.GetHashCode() == atom2Clone.GetHashCode());
            Assert.IsTrue(atom2.Equals(atom2Clone));

            NumericCompareExpression compare3 = (NumericCompareExpression)preconditions[3];
            NumericFunction numFunc3 = (NumericFunction)compare3.LeftArgument;
            IAtom atom3 = numFunc3.FunctionAtom;
            Assert.AreEqual(factory.CreateFunction("numFunc", "constA").GetNameId(), atom3.GetNameId());
            Assert.AreEqual(1, atom3.GetArity());
            Assert.AreEqual(idManager.Constants.GetId("constA"), atom3.GetGroundedTerm(0));
            Assert.AreEqual(1, atom3.GetTerms().Count);
            Assert.IsTrue(atom3.GetTerms()[0] is ConstantTerm);
            Assert.AreEqual("numFunc", ((Atom)atom3).GetFullName(idManager.Functions));
            Assert.AreEqual("(numFunc constA)", atom3.ToString(idManager.Functions));
            IAtom atom3Clone = atom3.Clone();
            Assert.IsTrue(atom3 != atom3Clone);
            Assert.IsTrue(atom3.GetHashCode() == atom3Clone.GetHashCode());
            Assert.IsTrue(atom3.Equals(atom3Clone));

            PredicateExpression pred4 = (PredicateExpression)preconditions[4];
            IAtom atom4 = pred4.PredicateAtom;
            idManager.Variables.Register("?var66");
            var unification1 = atom4.GetUnificationWith(factory.CreatePredicate("pred1", "constB", "?var66", "constA"));
            int value;
            Assert.IsTrue(unification1.TryGetValue(0, out value) && value == factory.CreateConstant("constB"));
            Assert.IsFalse(unification1.TryGetValue(1, out value));
            Assert.IsTrue(unification1.TryGetValue(2, out value) && value == factory.CreateConstant("constA"));
        }

        [TestMethod]
        public void TC_AtomsManager()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_AtomsManager.pddl"), GetFilePath("Dummy_P.pddl"));

            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            AtomsManager manager = new AtomsManager(data, idManager);

            idManager.Variables.Register("?a");
            idManager.Variables.Register("?b");

            Parameters parameters0 = new Parameters();
            Parameters parameters1 = new Parameters(new Parameter(idManager.Variables.GetId("?a"), new HashSet<int> { idManager.Types.GetId("typeA") }, idManager));
            Parameters parameters2 = new Parameters(new Parameter(idManager.Variables.GetId("?a"), new HashSet<int> { idManager.Types.GetId("typeA") }, idManager),
                                                    new Parameter(idManager.Variables.GetId("?b"), new HashSet<int> { idManager.Types.GetId("typeB") }, idManager));

            Assert.AreEqual(3, manager.LiftedPredicates.Count);
            Assert.IsTrue(manager.LiftedPredicates[0].Item1.Equals(factory.CreatePredicate("predA")));
            Assert.IsTrue(manager.LiftedPredicates[0].Item2.Equals(parameters0));
            Assert.IsTrue(manager.LiftedPredicates[1].Item1.Equals(factory.CreatePredicate("predB", "?a")));
            Assert.IsTrue(manager.LiftedPredicates[1].Item2.Equals(parameters1));
            Assert.IsTrue(manager.LiftedPredicates[2].Item1.Equals(factory.CreatePredicate("predC", "?a", "?b")));
            Assert.IsTrue(manager.LiftedPredicates[2].Item2.Equals(parameters2));

            Assert.AreEqual(3, manager.LiftedNumericFunctions.Count);
            Assert.IsTrue(manager.LiftedNumericFunctions[0].Item1.Equals(factory.CreateFunction("numFuncA")));
            Assert.IsTrue(manager.LiftedNumericFunctions[0].Item2.Equals(parameters0));
            Assert.IsTrue(manager.LiftedNumericFunctions[1].Item1.Equals(factory.CreateFunction("numFuncB", "?a")));
            Assert.IsTrue(manager.LiftedNumericFunctions[1].Item2.Equals(parameters1));
            Assert.IsTrue(manager.LiftedNumericFunctions[2].Item1.Equals(factory.CreateFunction("numFuncC", "?a", "?b")));
            Assert.IsTrue(manager.LiftedNumericFunctions[2].Item2.Equals(parameters2));

            Assert.AreEqual(3, manager.LiftedObjectFunctions.Count);
            Assert.IsTrue(manager.LiftedObjectFunctions[0].Item1.Equals(factory.CreateFunction("objFuncA")));
            Assert.IsTrue(manager.LiftedObjectFunctions[0].Item2.Contains(factory.CreateType("typeA")));
            Assert.IsTrue(manager.LiftedObjectFunctions[0].Item3.Equals(parameters0));
            Assert.IsTrue(manager.LiftedObjectFunctions[1].Item1.Equals(factory.CreateFunction("objFuncB", "?a")));
            Assert.IsTrue(manager.LiftedObjectFunctions[1].Item2.Contains(factory.CreateType("typeB")));
            Assert.IsTrue(manager.LiftedObjectFunctions[1].Item3.Equals(parameters1));
            Assert.IsTrue(manager.LiftedObjectFunctions[2].Item1.Equals(factory.CreateFunction("objFuncC", "?a", "?b")));
            Assert.IsTrue(manager.LiftedObjectFunctions[2].Item2.Contains(factory.CreateType("object")));
            Assert.IsTrue(manager.LiftedObjectFunctions[2].Item3.Equals(parameters2));
        }

        [TestMethod]
        public void TC_Conditions()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Conditions_D.pddl"), GetFilePath("TC_Conditions_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var predA = factory.CreatePredicate("predA");
            var predB = factory.CreatePredicate("predB");
            var predC = factory.CreatePredicate("predC");
            var predDConstA = factory.CreatePredicate("predD", "constA");
            var predDConstB = factory.CreatePredicate("predD", "constB");
            var predEConstA = factory.CreatePredicate("predE", "constA");
            var predEConstB = factory.CreatePredicate("predE", "constB");

            var conditions0 = problem.Operators[0].Preconditions;
            var conditions1 = problem.Operators[1].Preconditions;
            var conditions2 = problem.Operators[2].Preconditions;
            var conditions3 = problem.Operators[3].Preconditions;
            var goalConditions = (Conditions)problem.GoalConditions;

            Assert.IsNull(conditions0.Parameters);
            Assert.IsTrue(conditions1.Parameters != null && conditions1.Parameters.Count == 1);
            Assert.IsTrue(conditions2.Parameters != null && conditions2.Parameters.Count == 1);
            Assert.IsNull(conditions3.Parameters);
            Assert.IsNull(goalConditions.Parameters);

            Assert.IsFalse(conditions0.Evaluate(factory.CreateState(predA)));
            Assert.IsTrue(conditions0.Evaluate(factory.CreateState(predA, predB)));

            var state1 = factory.CreateState(predEConstA);
            var state2 = factory.CreateState(predA);

            Assert.IsTrue(conditions1.Evaluate(state1, factory.CreateSubstitution(conditions1.Parameters, "constA")));
            Assert.IsFalse(conditions1.Evaluate(state1, factory.CreateSubstitution(conditions1.Parameters, "constB")));
            Assert.IsTrue(conditions1.Evaluate(state1));
            Assert.IsFalse(conditions1.Evaluate(state2));

            Assert.IsTrue(conditions0.EvaluateRigidRelationsCompliance(null));
            Assert.IsFalse(conditions2.EvaluateRigidRelationsCompliance(factory.CreateSubstitution(conditions2.Parameters, "constA")));
            Assert.IsTrue(conditions2.EvaluateRigidRelationsCompliance(factory.CreateSubstitution(conditions2.Parameters, "constB")));

            var state3 = factory.CreateState(predA, predB);

            Assert.AreEqual(1, conditions0.GetNotAccomplishedConstraintsCount(state2));
            Assert.AreEqual(0, conditions0.GetNotAccomplishedConstraintsCount(state3));

            var usedPredicates = conditions0.GetUsedPredicates();
            Assert.AreEqual(2, usedPredicates.Count);
            Assert.IsTrue(usedPredicates.Contains(predA));
            Assert.IsTrue(usedPredicates.Contains(predB));
            idManager.Variables.Register("?a");
            Assert.IsTrue(conditions1.GetUsedPredicates().Contains(factory.CreatePredicate("predE", "?a")));
            Assert.IsTrue(conditions2.GetUsedPredicates().Contains(factory.CreatePredicate("predD", "?a")));
            idManager.Variables.Unregister("?a");

            StateLabels stateLabels = new StateLabels {{predA, 7}, {predB, 4}};
            Assert.AreEqual(11, goalConditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));
            Assert.AreEqual(7, goalConditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));

            var atoms1 = conditions1.GetSatisfyingAtoms(factory.CreateSubstitution(conditions1.Parameters, "constA"), state1);
            Assert.AreEqual(1, atoms1.Count);
            Assert.IsTrue(atoms1.Contains(predEConstA));

            var cnf0 = (ConditionsCNF)conditions0.GetCNF();
            Assert.IsNotNull(cnf0);
            Assert.IsNull(cnf0.Parameters);
            Assert.AreEqual(2, cnf0.Count);
            Assert.IsTrue(cnf0.Contains(new PredicateLiteralCNF(predA, false, idManager)));
            Assert.IsTrue(cnf0.Contains(new PredicateLiteralCNF(predB, false, idManager)));

            var wrapped0 = conditions0.GetWrappedConditions();
            AndExpression and0 = wrapped0 as AndExpression;
            Assert.IsNotNull(and0);
            Assert.AreEqual(2, and0.Children.Count);
            Assert.IsTrue(and0.Children.Contains(new PredicateExpression(predA, idManager)));
            Assert.IsTrue(and0.Children.Contains(new PredicateExpression(predB, idManager)));

            Assert.AreEqual(2, conditions0.GetSize());
            Assert.AreEqual(1, conditions1.GetSize());

            var states3 = conditions3.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(16, states3.Count);
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstA, predEConstB)));

            var relativeStates3 = conditions3.GetCorrespondingRelativeStates(problem).ToList();
            Assert.AreEqual(1, relativeStates3.Count);
            Assert.IsTrue(relativeStates3.Contains(factory.CreateRelativeState(new HashSet<int> { 2 }, predA, predC, predB))); // (predA), (predC), (not (predB))

            var emptyClone1 = conditions1.CloneEmpty();
            Assert.IsTrue(emptyClone1.Parameters.Equals(conditions1.Parameters));
            Assert.IsTrue(emptyClone1.Parameters != conditions1.Parameters);

            var clone0 = conditions0.Clone();
            Assert.IsTrue(conditions0 != clone0);
            Assert.IsTrue(conditions0.GetHashCode() == clone0.GetHashCode());
            Assert.IsTrue(conditions0.Equals(clone0));
        }

        [TestMethod]
        public void TC_ConditionsCNF()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsCNF_D.pddl"), GetFilePath("TC_ConditionsCNF_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var predA = factory.CreatePredicate("predA");
            var predB = factory.CreatePredicate("predB");
            var predC = factory.CreatePredicate("predC");
            var predDConstA = factory.CreatePredicate("predD", "constA");
            var predDConstB = factory.CreatePredicate("predD", "constB");
            var predEConstA = factory.CreatePredicate("predE", "constA");
            var predEConstB = factory.CreatePredicate("predE", "constB");

            var conditions0 = (ConditionsCNF)problem.Operators[0].Preconditions.GetCNF();
            var conditions1 = (ConditionsCNF)problem.Operators[1].Preconditions.GetCNF();
            var conditions2 = (ConditionsCNF)problem.Operators[2].Preconditions.GetCNF();
            var conditions3 = (ConditionsCNF)problem.Operators[3].Preconditions.GetCNF();
            var goalConditions = (ConditionsCNF)problem.GoalConditions.GetCNF();

            Assert.IsNull(conditions0.Parameters);
            Assert.IsTrue(conditions1.Parameters != null && conditions1.Parameters.Count == 1);
            Assert.IsTrue(conditions2.Parameters != null && conditions2.Parameters.Count == 1);
            Assert.IsNull(conditions3.Parameters);
            Assert.IsNull(goalConditions.Parameters);

            Assert.IsFalse(conditions0.Evaluate(factory.CreateState(predA)));
            Assert.IsTrue(conditions0.Evaluate(factory.CreateState(predA, predB)));

            var state1 = factory.CreateState(predEConstA);
            var state2 = factory.CreateState(predA);

            Assert.IsTrue(conditions1.Evaluate(state1, factory.CreateSubstitution(conditions1.Parameters, "constA")));
            Assert.IsFalse(conditions1.Evaluate(state1, factory.CreateSubstitution(conditions1.Parameters, "constB")));
            Assert.IsTrue(conditions1.Evaluate(state1));
            Assert.IsFalse(conditions1.Evaluate(state2));

            var state3 = factory.CreateState(predA, predB);

            Assert.AreEqual(1, conditions0.GetNotAccomplishedConstraintsCount(state2));
            Assert.AreEqual(0, conditions0.GetNotAccomplishedConstraintsCount(state3));

            var usedPredicates = conditions0.GetUsedPredicates();
            Assert.AreEqual(2, usedPredicates.Count);
            Assert.IsTrue(usedPredicates.Contains(predA));
            Assert.IsTrue(usedPredicates.Contains(predB));

            StateLabels stateLabels = new StateLabels {{predA, 7}, {predB, 4}};
            Assert.AreEqual(11, goalConditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));
            Assert.AreEqual(7, goalConditions.EvaluateOperatorPlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));

            var cnf0 = (ConditionsCNF)conditions0.GetCNF();
            Assert.IsNotNull(cnf0);
            Assert.IsNull(cnf0.Parameters);
            Assert.AreEqual(2, cnf0.Count);
            Assert.IsTrue(cnf0.Contains(new PredicateLiteralCNF(predA, false, idManager)));
            Assert.IsTrue(cnf0.Contains(new PredicateLiteralCNF(predB, false, idManager)));

            Assert.AreEqual(2, conditions0.GetSize());
            Assert.AreEqual(1, conditions1.GetSize());

            var states3 = conditions3.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(16, states3.Count);
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstB, predEConstA, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstA)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstB)));
            Assert.IsTrue(states3.Contains(factory.CreateState(predA, predC, predDConstA, predDConstB, predEConstA, predEConstB)));

            var relativeStates3 = conditions3.GetCorrespondingRelativeStates(problem).ToList();
            Assert.AreEqual(1, relativeStates3.Count);
            Assert.IsTrue(relativeStates3.Contains(factory.CreateRelativeState(new HashSet<int> { 2 }, predA, predC, predB))); // (predA), (predC), (not (predB))

            var emptyClone1 = conditions1.CloneEmpty();
            Assert.IsTrue(emptyClone1.Parameters.Equals(conditions1.Parameters));
            Assert.IsTrue(emptyClone1.Parameters != conditions1.Parameters);

            var clone0 = conditions0.Clone();
            Assert.IsTrue(conditions0 != clone0);
            Assert.IsTrue(conditions0.GetHashCode() == clone0.GetHashCode());
            Assert.IsTrue(conditions0.Equals(clone0));

            conditions1.Merge(conditions2);

            Assert.AreEqual(2, conditions1.Parameters.Count);
            Assert.AreEqual(2, conditions1.Count);
            PredicateLiteralCNF pred1 = conditions1.First() as PredicateLiteralCNF;
            Assert.IsNotNull(pred1);
            Assert.IsTrue(!pred1.IsNegated && pred1.PredicateAtom.GetNameId() == predEConstA.GetNameId());
            VariableTerm varTerm1 = (VariableTerm)pred1.PredicateAtom.GetTerms()[0];
            Assert.IsTrue(varTerm1.NameId == conditions1.Parameters[0].ParameterNameId);

            PredicateLiteralCNF pred2 = (PredicateLiteralCNF)conditions1.Last();
            Assert.IsNotNull(pred2);
            Assert.IsTrue(pred2.IsNegated && pred2.PredicateAtom.GetNameId() == predDConstA.GetNameId());
            VariableTerm varTerm2 = (VariableTerm)pred2.PredicateAtom.GetTerms()[0];
            Assert.IsTrue(varTerm2.NameId == conditions1.Parameters[1].ParameterNameId);
        }

        [TestMethod]
        public void TC_ConditionsCNFBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsCNFBuilder.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            GroundingManager groundingManager = new GroundingManager(data, new IdManager(data));
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager);
            ConditionsCNFBuilder transformer = new ConditionsCNFBuilder(evaluationManager);

            ConditionsCNF expression0 = transformer.Build(problem.Operators[0].Preconditions);
            Assert.AreEqual(1, expression0.Count);
            ClauseCNF clause0 = expression0.First() as ClauseCNF;
            Assert.IsNotNull(clause0);
            Assert.AreEqual(4, clause0.Count);
            foreach (var literal in clause0)
            {
                Assert.IsTrue(literal is PredicateLiteralCNF);
            }

            ConditionsCNF expression1 = transformer.Build(problem.Operators[1].Preconditions);
            Assert.AreEqual(4, expression1.Count);
            foreach (var element in expression1)
            {
                Assert.IsTrue(element is PredicateLiteralCNF);
            }

            ConditionsCNF expression2 = transformer.Build(problem.Operators[2].Preconditions);
            Assert.AreEqual(4, expression2.Count);
            foreach (var element in expression2)
            {
                ClauseCNF clause = element as ClauseCNF;
                Assert.IsNotNull(clause);
                Assert.AreEqual(3, clause.Count);
            }

            ConditionsCNF expression3 = transformer.Build(problem.Operators[3].Preconditions);
            Assert.AreEqual(2, expression3.Count);
            PredicateLiteralCNF pred30 = expression3.Last() as PredicateLiteralCNF;
            Assert.IsNotNull(pred30);
            Assert.IsFalse(pred30.IsNegated);
            PredicateLiteralCNF pred31 = expression3.First() as PredicateLiteralCNF;
            Assert.IsNotNull(pred31);
            Assert.IsTrue(pred31.IsNegated);

            ConditionsCNF expression4 = transformer.Build(problem.Operators[4].Preconditions);
            Assert.AreEqual(3, expression4.Count);
            var enumerator = expression4.GetEnumerator();
            enumerator.MoveNext();
            PredicateLiteralCNF pred42 = enumerator.Current as PredicateLiteralCNF;
            Assert.IsNotNull(pred42);
            Assert.IsFalse(pred42.IsNegated);
            enumerator.MoveNext();
            PredicateLiteralCNF pred41 = enumerator.Current as PredicateLiteralCNF;
            Assert.IsNotNull(pred41);
            Assert.IsFalse(pred41.IsNegated);
            enumerator.MoveNext();
            PredicateLiteralCNF pred40 = enumerator.Current as PredicateLiteralCNF;
            Assert.IsNotNull(pred40);
            Assert.IsTrue(pred40.IsNegated);
            enumerator.Dispose();

            ConditionsCNF expression5 = transformer.Build(problem.Operators[5].Preconditions);
            Assert.AreEqual(1, expression5.Count);
            ClauseCNF clause5 = expression5.First() as ClauseCNF;
            Assert.IsNotNull(clause5);
            Assert.AreEqual(2, clause5.Count);
            PredicateLiteralCNF pred50 = clause5.First() as PredicateLiteralCNF;
            Assert.IsNotNull(pred50);
            Assert.IsFalse(pred50.IsNegated);
            PredicateLiteralCNF pred51 = clause5.Last() as PredicateLiteralCNF;
            Assert.IsNotNull(pred51);
            Assert.IsFalse(pred51.IsNegated);
        }

        [TestMethod]
        public void TC_ConditionsCNFEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsCNFEvaluator_D.pddl"), GetFilePath("TC_ConditionsCNFEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            ConditionsCNFEvaluator evaluator = new ConditionsCNFEvaluator(new GroundingManager(data, idManager), problem.RigidRelations);

            Assert.IsTrue(evaluator.Evaluate((ConditionsCNF)problem.Operators[0].Preconditions.GetCNF(), new Substitution(), problem.InitialState));
            Assert.IsFalse(evaluator.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), new Substitution(), problem.InitialState));

            var substitutionA = factory.CreateSubstitution(problem.Operators[2].Parameters, "constA");
            var substitutionB = factory.CreateSubstitution(problem.Operators[2].Parameters, "constB");
            Assert.IsTrue(evaluator.Evaluate((ConditionsCNF)problem.Operators[2].Preconditions.GetCNF(), substitutionA, problem.InitialState));
            Assert.IsFalse(evaluator.Evaluate((ConditionsCNF)problem.Operators[2].Preconditions.GetCNF(), substitutionB, problem.InitialState));
        }

        [TestMethod]
        public void TC_ConditionsGrounder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsGrounder.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            Lazy<TermsGrounder> termsGrounder = new Lazy<TermsGrounder>(() => new TermsGrounder(idManager));
            Lazy<NumericExpressionsGrounder> numericExpressionGrounder = new Lazy<NumericExpressionsGrounder>(() => new NumericExpressionsGrounder(termsGrounder, idManager));
            Lazy<ExpressionsGrounder> expressionsGrounder = new Lazy<ExpressionsGrounder>(() => new ExpressionsGrounder(termsGrounder, numericExpressionGrounder, idManager));
            ConditionsGrounder grounder = new ConditionsGrounder(expressionsGrounder);

            var substitution0 = factory.CreateSubstitution(problem.Operators[0].Parameters, "", "constA");
            var grounded0 = grounder.Ground(problem.Operators[0].Preconditions, substitution0);
            Assert.AreEqual(1, grounded0.Parameters.Count);
            Assert.AreEqual(0, grounded0.Parameters[0].ParameterNameId);

            PredicateExpression predicate0 = (PredicateExpression)grounded0[0];
            VariableTerm variableTerm0 = (VariableTerm)predicate0.PredicateAtom.GetTerms()[0];
            Assert.IsNotNull(variableTerm0);
            Assert.IsTrue(problem.Operators[0].Parameters[0].ParameterNameId == variableTerm0.NameId);
            ConstantTerm constantTerm0 = (ConstantTerm)predicate0.PredicateAtom.GetTerms()[1];
            Assert.IsNotNull(constantTerm0);
            Assert.IsTrue(factory.CreateConstant("constA") == constantTerm0.NameId);

            var substitution1 = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB", "constB");
            var grounded1 = grounder.Ground(problem.Operators[0].Preconditions, substitution1);
            Assert.IsNull(grounded1.Parameters);

            PredicateExpression predicate1 = (PredicateExpression)grounded1[0];
            ConstantTerm constantTerm11 = (ConstantTerm)predicate1.PredicateAtom.GetTerms()[0];
            Assert.IsNotNull(constantTerm11);
            Assert.IsTrue(factory.CreateConstant("constB") == constantTerm11.NameId);
            ConstantTerm constantTerm12 = (ConstantTerm)predicate1.PredicateAtom.GetTerms()[1];
            Assert.IsNotNull(constantTerm12);
            Assert.IsTrue(factory.CreateConstant("constB") == constantTerm12.NameId);
        }

        [TestMethod]
        public void TC_ConditionsParametersCollector()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsParametersCollector_D.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            ConditionsParametersCollector collector = new ConditionsParametersCollector();

            var parameters = problem.Operators[0].Parameters;
            var preconds = problem.Operators[0].Preconditions;

            var usedVariables = collector.Collect(preconds);
            Assert.AreEqual(6, usedVariables.Count);

            Assert.IsFalse(usedVariables.Contains(parameters[0].ParameterNameId));
            Assert.IsFalse(usedVariables.Contains(parameters[1].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[2].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[3].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[4].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[5].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[6].ParameterNameId));
            Assert.IsTrue(usedVariables.Contains(parameters[7].ParameterNameId));
            Assert.IsFalse(usedVariables.Contains(parameters[8].ParameterNameId));
        }

        [TestMethod]
        public void TC_ConditionsParametersRenamer()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsParametersRenamer_D.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            ConditionsParametersRenamer renamer = new ConditionsParametersRenamer();

            var precond0 = (ConditionsCNF)problem.Operators[0].Preconditions.GetCNF();
            var precond1 = (ConditionsCNF)problem.Operators[1].Preconditions.GetCNF();

            Assert.AreEqual(0, precond0.Parameters[0].ParameterNameId);
            PredicateLiteralCNF pred0 = (PredicateLiteralCNF)precond0.First();
            VariableTerm varTerm0 = (VariableTerm)pred0.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(0, varTerm0.NameId);

            Assert.AreEqual(0, precond1.Parameters[0].ParameterNameId);
            PredicateLiteralCNF pred1 = (PredicateLiteralCNF)precond1.First();
            VariableTerm varTerm1 = (VariableTerm)pred1.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(0, varTerm1.NameId);

            Assert.IsTrue(precond0.Parameters.AreConflictedWith(precond1.Parameters));

            renamer.Rename(precond1, 4);

            Assert.IsFalse(precond0.Parameters.AreConflictedWith(precond1.Parameters));

            Assert.AreEqual(4, precond1.Parameters[0].ParameterNameId);
            PredicateLiteralCNF pred1P = (PredicateLiteralCNF)precond1.First();
            VariableTerm varTerm1P = (VariableTerm)pred1P.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(4, varTerm1P.NameId);

            precond0.Merge(precond1);

            Assert.AreEqual(2, precond0.Parameters.Count);
            Assert.AreEqual(0, precond0.Parameters[0].ParameterNameId);
            Assert.AreEqual(4, precond0.Parameters[1].ParameterNameId);
            Assert.AreEqual(4, precond0.Parameters.GetMaxUsedParameterId());

            Assert.AreEqual(2, precond0.Count);
            PredicateLiteralCNF mergedPred1 = (PredicateLiteralCNF)precond0.First();
            VariableTerm mergedVarTerm1 = (VariableTerm)mergedPred1.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(0, mergedVarTerm1.NameId);
            PredicateLiteralCNF mergedPred2 = (PredicateLiteralCNF)precond0.Last();
            VariableTerm mergedVarTerm2 = (VariableTerm)mergedPred2.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(4, mergedVarTerm2.NameId);
        }

        [TestMethod]
        public void TC_ConditionsUsedPredicatesCollector()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConditionsUsedPredicatesCollector.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            idManager.Variables.Register("?a");
            var conditions = problem.Operators[0].Preconditions;

            ConditionsUsedPredicatesCollector collector = new ConditionsUsedPredicatesCollector();
            var usedPredicates = collector.Collect(conditions);

            Assert.AreEqual(4, usedPredicates.Count);
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predA")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predB")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predC", "constB")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predC", "?a")));
        }

        [TestMethod]
        public void TC_ConstantsManager()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ConstantsManager_D.pddl"), GetFilePath("TC_ConstantsManager_P.pddl"));

            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            ConstantsManager constantsManager = new ConstantsManager(data, idManager);

            Assert.AreEqual(7, constantsManager.Count);

            var objectConstants = constantsManager[factory.CreateType("object")];
            Assert.AreEqual(6, objectConstants.Count);
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("object1")));
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("object2")));
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("constC")));
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("constD")));
            Assert.IsTrue(objectConstants.Contains(factory.CreateConstant("constF")));
            Assert.IsTrue(Planner.CollectionsEquality.Equals(objectConstants, constantsManager.GetAllConstantsOfType(factory.CreateType("object"))));

            var typeAConstants = constantsManager[factory.CreateType("typeA")];
            Assert.AreEqual(2, typeAConstants.Count);
            Assert.IsTrue(typeAConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(typeAConstants.Contains(factory.CreateConstant("constD")));

            var typeBConstants = constantsManager[factory.CreateType("typeB")];
            Assert.AreEqual(2, typeBConstants.Count);
            Assert.IsTrue(typeBConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(typeBConstants.Contains(factory.CreateConstant("constD")));

            var typeCConstants = constantsManager[factory.CreateType("typeC")];
            Assert.AreEqual(3, typeCConstants.Count);
            Assert.IsTrue(typeCConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(typeCConstants.Contains(factory.CreateConstant("constC")));
            Assert.IsTrue(typeCConstants.Contains(factory.CreateConstant("constD")));

            var typeDConstants = constantsManager[factory.CreateType("typeD")];
            Assert.AreEqual(1, typeDConstants.Count);
            Assert.IsTrue(typeDConstants.Contains(factory.CreateConstant("constD")));

            var typeEConstants = constantsManager[factory.CreateType("typeE")];
            Assert.AreEqual(3, typeEConstants.Count);
            Assert.IsTrue(typeEConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(typeEConstants.Contains(factory.CreateConstant("constC")));
            Assert.IsTrue(typeEConstants.Contains(factory.CreateConstant("constD")));

            var typeFConstants = constantsManager[factory.CreateType("typeF")];
            Assert.AreEqual(4, typeFConstants.Count);
            Assert.IsTrue(typeFConstants.Contains(factory.CreateConstant("constAB")));
            Assert.IsTrue(typeFConstants.Contains(factory.CreateConstant("constC")));
            Assert.IsTrue(typeFConstants.Contains(factory.CreateConstant("constD")));
            Assert.IsTrue(typeFConstants.Contains(factory.CreateConstant("constF")));

            var constAbTypes = constantsManager.GetTypesForConstant(factory.CreateConstant("constAB"));
            Assert.AreEqual(2, constAbTypes.Count);
            Assert.IsTrue(constAbTypes.Contains(factory.CreateType("typeA")));
            Assert.IsTrue(constAbTypes.Contains(factory.CreateType("typeB")));

            var constCTypes = constantsManager.GetTypesForConstant(factory.CreateConstant("constC"));
            Assert.AreEqual(1, constCTypes.Count);
            Assert.IsTrue(constCTypes.Contains(factory.CreateType("typeC")));

            var object2Types = constantsManager.GetTypesForConstant(factory.CreateConstant("object2"));
            Assert.AreEqual(1, object2Types.Count);
            Assert.IsTrue(object2Types.Contains(factory.CreateType("object")));

            var childrenTypesC = constantsManager.GetChildrenTypes(factory.CreateType("typeC")).ToList();
            Assert.AreEqual(2, childrenTypesC.Count);
            Assert.IsTrue(childrenTypesC.Contains(factory.CreateType("typeA")));
            Assert.IsTrue(childrenTypesC.Contains(factory.CreateType("typeB")));

            var childrenTypesObject = constantsManager.GetChildrenTypes(factory.CreateType("object")).ToList();
            Assert.AreEqual(2, childrenTypesObject.Count);
            Assert.IsTrue(childrenTypesObject.Contains(factory.CreateType("typeE")));
            Assert.IsTrue(childrenTypesObject.Contains(factory.CreateType("typeF")));
        }

        [TestMethod]
        public void TC_Effects()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Effects_D.pddl"), GetFilePath("TC_Effects_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            // effects structure

            var effects0 = problem.Operators[0].Effects;
            Assert.IsTrue(effects0[0] is PredicateEffect);
            Assert.IsTrue(effects0[1] is NotEffect);
            Assert.IsTrue(effects0[2] is ForallEffect);
            Assert.IsTrue(effects0[3] is NumericAssignEffect);
            Assert.IsTrue(effects0[4] is NumericIncreaseEffect);
            Assert.IsTrue(effects0[5] is NumericDecreaseEffect);
            Assert.IsTrue(effects0[6] is NumericScaleUpEffect);
            Assert.IsTrue(effects0[7] is NumericScaleDownEffect);
            Assert.IsTrue(effects0[8] is ObjectAssignEffect);
            Assert.IsTrue(effects0[9] is WhenEffect);
            Assert.IsTrue(effects0[10] is WhenEffect);

            // effects functions

            var effects1 = problem.Operators[1].Effects;
            var state = problem.InitialState;
            var goalConditions = (Conditions)problem.GoalConditions;
            var goalRelativeState = (IRelativeState)goalConditions.GetCorrespondingRelativeStates(problem).First();

            var predAConstA = factory.CreatePredicate("predA", "constA");
            var predAConstB = factory.CreatePredicate("predA", "constB");
            var predB = factory.CreatePredicate("predB");
            var predC = factory.CreatePredicate("predC");
            var predDConstA = factory.CreatePredicate("predD", "constA");
            var predDConstB = factory.CreatePredicate("predD", "constB");
            var numFunc = factory.CreateFunction("numFunc");
            var objFunc = factory.CreateFunction("objFunc");
            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");

            var substitutionA = factory.CreateSubstitution(problem.Operators[1].Parameters, "constA");
            var newStateA = effects1.Apply(state, substitutionA);
            Assert.IsTrue(newStateA.HasPredicate(predAConstA));
            Assert.IsTrue(!newStateA.HasPredicate(predB));
            Assert.IsTrue(newStateA.HasPredicate(predC));
            Assert.IsTrue(newStateA.HasPredicate(predDConstA));
            Assert.IsTrue(newStateA.HasPredicate(predDConstB));
            Assert.AreEqual(9, newStateA.GetNumericFunctionValue(numFunc));
            Assert.AreEqual(constA, newStateA.GetObjectFunctionValue(objFunc));

            var substitutionB = factory.CreateSubstitution(problem.Operators[1].Parameters, "constB");
            var newStateB = effects1.Apply(state, substitutionB);
            Assert.IsTrue(newStateB.HasPredicate(predAConstB));
            Assert.IsTrue(!newStateB.HasPredicate(predB));
            Assert.IsTrue(newStateB.HasPredicate(predC));
            Assert.IsTrue(newStateB.HasPredicate(predDConstA));
            Assert.IsTrue(newStateB.HasPredicate(predDConstB));
            Assert.AreEqual(9, newStateB.GetNumericFunctionValue(numFunc));
            Assert.AreEqual(constB, newStateB.GetObjectFunctionValue(objFunc));

            effects1.SetDeleteRelaxation();
            newStateB = effects1.Apply(state, substitutionB);
            Assert.IsTrue(newStateB.HasPredicate(predB));
            effects1.SetDeleteRelaxation(false);

            var resultAtoms = effects1.GetResultAtoms(substitutionB);
            Assert.AreEqual(3, resultAtoms.Count);
            Assert.IsTrue(resultAtoms.Contains(predAConstB));
            Assert.IsTrue(resultAtoms.Contains(predDConstA));
            Assert.IsTrue(resultAtoms.Contains(predDConstB));

            Assert.IsTrue(effects1.IsRelevant(goalConditions, substitutionA));
            Assert.IsFalse(effects1.IsRelevant(goalConditions, substitutionB));

            Assert.IsTrue(effects1.IsRelevant(goalRelativeState, substitutionA));
            Assert.IsFalse(effects1.IsRelevant(goalRelativeState, substitutionB));

            var newConditions = (ConditionsCNF)effects1.ApplyBackwards(goalConditions, substitutionA);
            Assert.AreEqual(2, newConditions.Count);
            Assert.IsTrue(newConditions.Contains(new PredicateLiteralCNF(new PredicateExpression(predAConstB, idManager), true)));
            Assert.IsTrue(newConditions.Contains(new PredicateLiteralCNF(new PredicateExpression(predC, idManager), false)));

            var newRelativeStates = effects1.ApplyBackwards(goalRelativeState, substitutionA).ToList();
            Assert.AreEqual(1, newRelativeStates.Count);
            var newRelativeState = (IRelativeState)newRelativeStates.First();
            Assert.IsTrue(newRelativeState.HasNegatedPredicate(predAConstB));
            Assert.IsTrue(newRelativeState.HasPredicate(predC));
            Assert.IsFalse(newRelativeState.HasPredicate(predAConstA));
            Assert.IsFalse(newRelativeState.HasNegatedPredicate(predB));
        }

        [TestMethod]
        public void TC_EffectsApplier()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsApplier_D.pddl"), GetFilePath("TC_EffectsApplier_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager, problem.RigidRelations);

            EffectsApplier effectsApplier = new EffectsApplier(evaluationManager);

            var substitution = new Substitution();
            var state = problem.InitialState;
            var effects = problem.Operators[0].Effects;
            Assert.AreEqual(11, effects.Count);

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");
            var pred0 = factory.CreatePredicate("pred0");
            var pred1ConstA = factory.CreatePredicate("pred1", "constA");
            var pred1ConstB = factory.CreatePredicate("pred1", "constB");
            var numericFunc = factory.CreateFunction("numericFunc");
            var objectFunc = factory.CreateFunction("objectFunc");

            Assert.IsTrue(state.HasPredicate(pred0));
            Assert.IsTrue(state.HasPredicate(pred1ConstA));
            Assert.IsTrue(!state.HasPredicate(pred1ConstB));
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));
            Assert.AreEqual(constA, state.GetObjectFunctionValue(objectFunc));

            effectsApplier.Apply(effects[0], state, substitution);
            Assert.IsTrue(state.HasPredicate(pred1ConstB));

            effectsApplier.Apply(effects[1], state, substitution);
            Assert.IsTrue(!state.HasPredicate(pred0));

            effectsApplier.Apply(effects[2], state, substitution);
            Assert.IsTrue(!state.HasPredicate(pred1ConstA));
            Assert.IsTrue(!state.HasPredicate(pred1ConstB));

            effectsApplier.Apply(effects[3], state, substitution);
            Assert.AreEqual(5, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[4], state, substitution);
            Assert.AreEqual(12, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[5], state, substitution);
            Assert.AreEqual(10, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[6], state, substitution);
            Assert.AreEqual(30, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[7], state, substitution);
            Assert.AreEqual(15, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[8], state, substitution);
            Assert.AreEqual(constB, state.GetObjectFunctionValue(objectFunc));

            effectsApplier.Apply(effects[9], state, substitution);
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));

            effectsApplier.Apply(effects[10], state, substitution);
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));
        }

        [TestMethod]
        public void TC_EffectsBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsBuilder.pddl"), GetFilePath("Dummy_P.pddl"));

            EffectsBuilder builder = new EffectsBuilder(new IdManager(data));

            var effects = data.Domain.Actions[0].Effects;
            Assert.IsTrue(builder.Build(effects[0]) is PredicateEffect);
            Assert.IsTrue(builder.Build(effects[1]) is NotEffect);
            Assert.IsTrue(builder.Build(effects[2]) is ForallEffect);
            Assert.IsTrue(builder.Build(effects[3]) is NumericAssignEffect);
            Assert.IsTrue(builder.Build(effects[4]) is NumericIncreaseEffect);
            Assert.IsTrue(builder.Build(effects[5]) is NumericDecreaseEffect);
            Assert.IsTrue(builder.Build(effects[6]) is NumericScaleUpEffect);
            Assert.IsTrue(builder.Build(effects[7]) is NumericScaleDownEffect);
            Assert.IsTrue(builder.Build(effects[8]) is ObjectAssignEffect);
            Assert.IsTrue(builder.Build(effects[9]) is WhenEffect);
            Assert.IsTrue(((ForallEffect)builder.Build(effects[2])).Effects[0] is PredicateEffect);
            Assert.IsTrue(((WhenEffect)builder.Build(effects[9])).Effects[0] is NotEffect);
        }

        [TestMethod]
        public void TC_EffectsBackwardsConditionsApplier()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsBackwardsConditionsApplier_D.pddl"), GetFilePath("TC_EffectsBackwardsConditionsApplier_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager, problem.RigidRelations);
            Conditions goalConds = (Conditions)problem.GoalConditions;

            Func<string, PredicateLiteralCNF> getPredicate = predicateName => new PredicateLiteralCNF(factory.CreatePredicate(predicateName), false, idManager);

            EffectsBackwardsConditionsApplier backwardsApplier0 = new EffectsBackwardsConditionsApplier(problem.Operators[0].Preconditions, problem.Operators[0].Effects, evaluationManager);
            IConditions resultConditions0 = backwardsApplier0.ApplyBackwards(new Conditions(goalConds[0], evaluationManager), new Substitution());
            ConditionsCNF resultConditionsCnf0 = (ConditionsCNF)resultConditions0;
            Assert.AreEqual(2, resultConditionsCnf0.Count);
            Assert.IsTrue(resultConditionsCnf0.Contains(getPredicate("predE")));
            Assert.IsTrue(resultConditionsCnf0.Contains(getPredicate("predZ")));

            EffectsBackwardsConditionsApplier backwardsApplier1 = new EffectsBackwardsConditionsApplier(problem.Operators[1].Preconditions, problem.Operators[1].Effects, evaluationManager);
            ISubstitution operatorSubstitution1 = factory.CreateSubstitution(problem.Operators[1].Parameters, "constA");
            IConditions resultConditions1 = backwardsApplier1.ApplyBackwards(new Conditions(goalConds[1], evaluationManager), operatorSubstitution1);
            ConditionsCNF resultConditionsCnf1 = (ConditionsCNF)resultConditions1;
            Assert.AreEqual(1, resultConditionsCnf1.Count);
            EqualsLiteralCNF equals10 = resultConditionsCnf1.First() as EqualsLiteralCNF;
            Assert.IsNotNull(equals10);
            ConstantTerm const10 = equals10.LeftArgument as ConstantTerm;
            Assert.IsNotNull(const10);
            Assert.AreEqual(factory.CreateConstant("constB"), const10.NameId);
            ObjectFunctionTerm objFunc10 = equals10.RightArgument as ObjectFunctionTerm;
            Assert.IsNotNull(objFunc10);
            Assert.AreEqual(factory.CreateFunction("objFunc6").GetNameId(), objFunc10.FunctionAtom.GetNameId());

            EffectsBackwardsConditionsApplier backwardsApplier2 = new EffectsBackwardsConditionsApplier(problem.Operators[2].Preconditions, problem.Operators[2].Effects, evaluationManager);
            IConditions resultConditions2 = backwardsApplier2.ApplyBackwards(new Conditions(goalConds[2], evaluationManager), new Substitution());
            ConditionsCNF resultConditionsCnf2 = (ConditionsCNF)resultConditions2;
            Assert.AreEqual(1, resultConditionsCnf2.Count);
            NumericCompareLiteralCNF numComp20 = resultConditionsCnf2.First() as NumericCompareLiteralCNF;
            Assert.IsNotNull(numComp20);
            Assert.AreEqual(NumericCompareExpression.RelationalOperator.EQ, numComp20.Operator);
            Minus minusExpr20 = numComp20.LeftArgument as Minus;
            Assert.IsNotNull(minusExpr20);
            Assert.IsTrue(minusExpr20.LeftChild is NumericFunction);
            Assert.IsTrue(minusExpr20.RightChild is Number);
            Assert.IsTrue(numComp20.RightArgument is Number);

            EffectsBackwardsConditionsApplier backwardsApplier3 = new EffectsBackwardsConditionsApplier(problem.Operators[3].Preconditions, problem.Operators[3].Effects, evaluationManager);
            ISubstitution substitution3 = factory.CreateSubstitution(problem.Operators[3].Parameters, "constA");
            IConditions resultConditions3 = backwardsApplier3.ApplyBackwards(new Conditions(goalConds[3], evaluationManager), substitution3);
            ConditionsCNF resultConditionsCnf3 = (ConditionsCNF)resultConditions3;
            Assert.AreEqual(2, resultConditionsCnf3.Count);
            PredicateLiteralCNF pred30 = resultConditionsCnf3.First() as PredicateLiteralCNF;
            Assert.IsNotNull(pred30);
            Assert.AreEqual(factory.CreatePredicate("predF", "constA", "constB").GetNameId(), pred30.PredicateAtom.GetNameId());
            PredicateLiteralCNF pred31 = resultConditionsCnf3.Last() as PredicateLiteralCNF;
            Assert.IsNotNull(pred31);
            Assert.AreEqual(factory.CreatePredicate("predF", "constA", "constB").GetNameId(), pred31.PredicateAtom.GetNameId());

            EffectsBackwardsConditionsApplier backwardsApplier4 = new EffectsBackwardsConditionsApplier(problem.Operators[4].Preconditions, problem.Operators[4].Effects, evaluationManager);
            IConditions resultConditions4 = backwardsApplier4.ApplyBackwards(new Conditions(goalConds[4], evaluationManager), new Substitution());
            ConditionsCNF resultConditionsCnf4 = (ConditionsCNF)resultConditions4;
            Assert.AreEqual(8, resultConditionsCnf4.Count);
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predA"), getPredicate("predB"), getPredicate("pred1") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predA"), getPredicate("pred1") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predB"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predA"), getPredicate("predB"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predA"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predA"), getPredicate("predB"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predB"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(getPredicate("predZ")));
        }

        [TestMethod]
        public void TC_EffectsBackwardsConditionsApplierLifted()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsBackwardsConditionsApplierLifted_D.pddl"), GetFilePath("TC_EffectsBackwardsConditionsApplierLifted_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager, problem.RigidRelations);
            Conditions goalConds = (Conditions)problem.GoalConditions;

            EffectsBackwardsConditionsApplier backwardsApplier0 = new EffectsBackwardsConditionsApplier(problem.Operators[0].Preconditions, problem.Operators[0].Effects, evaluationManager, true);
            EffectsBackwardsConditionsApplier backwardsApplier1 = new EffectsBackwardsConditionsApplier(problem.Operators[1].Preconditions, problem.Operators[1].Effects, evaluationManager, true);

            var substitution = factory.CreateSubstitution(problem.Operators[1].Parameters, "x1", "y1");
            var newConditions = (ConditionsCNF)backwardsApplier1.ApplyBackwards(goalConds, substitution);

            var minimalSubstitution = backwardsApplier1.ExtractMinimalOperatorSubstitution();
            Assert.AreEqual(1, minimalSubstitution.Count());
            int value;
            Assert.IsTrue(minimalSubstitution.TryGetValue(1, out value) && value == factory.CreateConstant("y1"));
            Assert.IsFalse(minimalSubstitution.TryGetValue(0, out value));

            Assert.AreEqual(1, newConditions.Parameters.Count);
            PredicateLiteralCNF predicate0 = (PredicateLiteralCNF)newConditions.First();
            Assert.AreEqual(factory.CreatePredicate("pred", "x1").GetNameId(), predicate0.PredicateAtom.GetNameId());
            VariableTerm variableTerm0 = predicate0.PredicateAtom.GetTerms()[0] as VariableTerm;
            Assert.IsNotNull(variableTerm0);
            Assert.IsTrue(variableTerm0.NameId == newConditions.Parameters[0].ParameterNameId);
            Assert.IsTrue(newConditions.Evaluate(factory.CreateState(factory.CreatePredicate("pred", "x1"))));
            Assert.IsTrue(newConditions.Evaluate(factory.CreateState(factory.CreatePredicate("pred", "x2"))));
            Assert.IsFalse(newConditions.Evaluate(factory.CreateState(factory.CreatePredicate("pred", "y1"))));

            var substitutionX1 = factory.CreateSubstitution(problem.Operators[0].Parameters, "x1");
            var newConditionsX1 = (ConditionsCNF)backwardsApplier0.ApplyBackwards(newConditions, substitutionX1);
            Assert.IsNull(newConditionsX1.Parameters);
            PredicateLiteralCNF predicateX1 = (PredicateLiteralCNF)newConditionsX1.First();
            Assert.IsTrue(factory.CreatePredicate("predStart").Equals(predicateX1.PredicateAtom));
            Assert.IsTrue(problem.IsStartConditions(newConditionsX1));

            var substitutionX2 = factory.CreateSubstitution(problem.Operators[0].Parameters, "x2");
            var newConditionsX2 = (ConditionsCNF)backwardsApplier0.ApplyBackwards(newConditions, substitutionX2);
            Assert.IsNull(newConditionsX2.Parameters);
            PredicateLiteralCNF predicateX2 = (PredicateLiteralCNF)newConditionsX2.First();
            Assert.IsTrue(factory.CreatePredicate("predStart").Equals(predicateX2.PredicateAtom));
            Assert.IsTrue(problem.IsStartConditions(newConditionsX2));
        }

        [TestMethod]
        public void TC_EffectsBackwardsRelativeStateApplier()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsBackwardsRelativeStateApplier_D.pddl"), GetFilePath("TC_EffectsBackwardsRelativeStateApplier_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager, problem.RigidRelations);
            Conditions goalConds = (Conditions)problem.GoalConditions;

            Func<Conditions, int, IEnumerable<Planner.IRelativeState>> getRelativeStates = (conditions, index) => new Conditions(conditions[index], evaluationManager).GetCorrespondingRelativeStates(problem);

            var predA = factory.CreatePredicate("predA");
            var predB = factory.CreatePredicate("predB");
            var predD = factory.CreatePredicate("predD");
            var predE = factory.CreatePredicate("predE");
            var predZ = factory.CreatePredicate("predZ");
            var predFConstBConstA = factory.CreatePredicate("predF", "constB", "constA");
            var predFConstBConstB = factory.CreatePredicate("predF", "constB", "constB");
            var pred1 = factory.CreatePredicate("pred1");
            var pred2 = factory.CreatePredicate("pred2");

            // primitive effects

            HashSet<IRelativeState> resultRelativeStates0 = new HashSet<IRelativeState>();
            EffectsBackwardsRelativeStateApplier backwardsApplier0 = new EffectsBackwardsRelativeStateApplier(problem.Operators[0].Preconditions, problem.Operators[0].Effects, evaluationManager);
            foreach (var relativeState in getRelativeStates(goalConds, 0))
            {
                var resultRelativeStates = backwardsApplier0.ApplyBackwards((IRelativeState)relativeState, new Substitution()).ToList();
                Assert.AreEqual(1, resultRelativeStates.Count);
                resultRelativeStates0.Add((IRelativeState)resultRelativeStates.First());
            }
            Assert.AreEqual(2, resultRelativeStates0.Count);
            Assert.IsTrue(resultRelativeStates0.Contains(factory.CreateRelativeState(new HashSet<int> { 2 }, predE, predZ, pred1))); // (predE), (predZ), (not (pred1))
            Assert.IsTrue(resultRelativeStates0.Contains(factory.CreateRelativeState(new HashSet<int> { 3 }, predD, predE, predZ, pred1))); // (predD), (predE), (predZ), (not (pred1))

            // object assign effects

            HashSet<IRelativeState> resultRelativeStates1 = new HashSet<IRelativeState>();
            EffectsBackwardsRelativeStateApplier backwardsApplier1 = new EffectsBackwardsRelativeStateApplier(problem.Operators[1].Preconditions, problem.Operators[1].Effects, evaluationManager);
            foreach (var relativeState in getRelativeStates(goalConds, 1))
            {
                var resultRelativeStates = backwardsApplier1.ApplyBackwards((IRelativeState)relativeState, new Substitution()).ToList();
                Assert.AreEqual(1, resultRelativeStates.Count);
                resultRelativeStates1.Add((IRelativeState)resultRelativeStates.First());
            }
            Assert.AreEqual(1, resultRelativeStates1.Count);
            Assert.IsTrue(resultRelativeStates1.Contains(new RelativeState(null, null, null, new Dictionary<IAtom, int> {{ factory.CreateFunction("objFunc2"), factory.CreateConstant("constB") }}, idManager)));

            // numeric assign effects

            HashSet<IRelativeState> resultRelativeStates2 = new HashSet<IRelativeState>();
            EffectsBackwardsRelativeStateApplier backwardsApplier2 = new EffectsBackwardsRelativeStateApplier(problem.Operators[2].Preconditions, problem.Operators[2].Effects, evaluationManager);
            foreach (var relativeState in getRelativeStates(goalConds, 2))
            {
                var resultRelativeStates = backwardsApplier2.ApplyBackwards((IRelativeState)relativeState, new Substitution()).ToList();
                Assert.AreEqual(1, resultRelativeStates.Count);
                resultRelativeStates2.Add((IRelativeState)resultRelativeStates.First());
            }
            Assert.AreEqual(1, resultRelativeStates2.Count);
            Assert.IsTrue(resultRelativeStates2.Contains(new RelativeState(null, null, new Dictionary<IAtom, double> { { factory.CreateFunction("numFunc2"), 22.3 } }, null, idManager)));

            // forall effects

            HashSet<IRelativeState> resultRelativeStates3 = new HashSet<IRelativeState>();
            EffectsBackwardsRelativeStateApplier backwardsApplier3 = new EffectsBackwardsRelativeStateApplier(problem.Operators[3].Preconditions, problem.Operators[3].Effects, evaluationManager);
            ISubstitution substitution3 = factory.CreateSubstitution(problem.Operators[3].Parameters, "constA");
            foreach (var relativeState in getRelativeStates(goalConds, 3))
            {
                var resultRelativeStates = backwardsApplier3.ApplyBackwards((IRelativeState)relativeState, substitution3).ToList();
                Assert.AreEqual(1, resultRelativeStates.Count);
                resultRelativeStates3.Add((IRelativeState)resultRelativeStates.First());
            }
            Assert.AreEqual(1, resultRelativeStates3.Count);
            Assert.IsTrue(resultRelativeStates3.Contains(factory.CreateRelativeState(predFConstBConstA, predFConstBConstB)));

            // when effects

            HashSet<IRelativeState> resultRelativeStates4 = new HashSet<IRelativeState>();
            EffectsBackwardsRelativeStateApplier backwardsApplier4 = new EffectsBackwardsRelativeStateApplier(problem.Operators[4].Preconditions, problem.Operators[4].Effects, evaluationManager);
            foreach (var relativeState in getRelativeStates(goalConds, 4))
            {
                var resultRelativeStates = backwardsApplier4.ApplyBackwards((IRelativeState)relativeState, new Substitution());
                foreach (var resultRelativeState in resultRelativeStates)
                {
                    resultRelativeStates4.Add((IRelativeState)resultRelativeState);
                }
            }
            Assert.AreEqual(4, resultRelativeStates4.Count);
            Assert.IsTrue(resultRelativeStates4.Contains(factory.CreateRelativeState(predZ, predA, predB)));
            Assert.IsTrue(resultRelativeStates4.Contains(factory.CreateRelativeState(predZ, predA, pred2)));
            Assert.IsTrue(resultRelativeStates4.Contains(factory.CreateRelativeState(predZ, pred1, predB)));
            Assert.IsTrue(resultRelativeStates4.Contains(factory.CreateRelativeState(predZ, pred1, pred2)));
        }

        [TestMethod]
        public void TC_EffectsPreprocessedCollection()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsPreprocessedCollection.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            EffectsPreprocessedCollection effects = new EffectsPreprocessedCollection();
            effects.CollectEffects(problem.Operators[0].Effects);

            idManager.Variables.Register("?a");
            idManager.Variables.Register("?x");

            // collected effects

            Assert.AreEqual(1, effects.PositivePredicateEffects.Count);
            Assert.IsTrue(effects.PositivePredicateEffects.Contains(factory.CreatePredicate("pred", "?a")));

            Assert.AreEqual(1, effects.NegativePredicateEffects.Count);
            Assert.IsTrue(effects.NegativePredicateEffects.Contains(factory.CreatePredicate("pred", "?a")));

            Assert.AreEqual(5, effects.NumericFunctionAssignmentEffects.Count);
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericAssignEffect(factory.CreateFunction("numFunc1", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericIncreaseEffect(factory.CreateFunction("numFunc2", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericDecreaseEffect(factory.CreateFunction("numFunc3", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericScaleUpEffect(factory.CreateFunction("numFunc4", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericScaleDownEffect(factory.CreateFunction("numFunc5", "?a"), new Number(5), idManager)));

            Assert.AreEqual(1, effects.ObjectFunctionAssignmentEffects.Count);
            Assert.IsTrue(effects.ObjectFunctionAssignmentEffects.Contains(new ObjectAssignEffect(factory.CreateFunction("objFunc", "?a"), factory.CreateTerm("?a"))));

            Assert.AreEqual(1, effects.ForallEffects.Count);
            Assert.IsTrue(effects.ForallEffects.Contains(new ForallEffect(new Parameters(((InputData.PDDL.ForallEffect)data.Domain.Actions[0].Effects[8]).Parameters, idManager),
                                                                          new List<IEffect> { new PredicateEffect(factory.CreatePredicate("pred", "?x")) })));

            Assert.AreEqual(1, effects.WhenEffects.Count);
            Assert.IsTrue(effects.WhenEffects.Contains(new WhenEffect(new PredicateExpression(factory.CreatePredicate("pred", "?a"), idManager),
                                                                      new List<PrimitiveEffect> { new PredicateEffect(factory.CreatePredicate("pred", "?a")) })));

            Assert.AreEqual(0, effects.GroundedPositivePredicateEffects.Count);
            Assert.AreEqual(0, effects.GroundedNegativePredicateEffects.Count);
            Assert.AreEqual(0, effects.GroundedNumericFunctionAssignmentEffects.Count);
            Assert.AreEqual(0, effects.GroundedObjectFunctionAssignmentEffects.Count);

            // first grounding

            ISubstitution substitutionA = factory.CreateSubstitution(problem.Operators[0].Parameters, "constA");
            effects.GroundEffectsByCurrentOperatorSubstitution(groundingManager, substitutionA);

            Assert.AreEqual(1, effects.GroundedPositivePredicateEffects.Count);
            Assert.IsTrue(effects.GroundedPositivePredicateEffects.Contains(factory.CreatePredicate("pred", "constA")));
            Assert.IsTrue(effects.PositivePredicateEffects.Contains(factory.CreatePredicate("pred", "?a")));

            Assert.AreEqual(1, effects.GroundedNegativePredicateEffects.Count);
            Assert.IsTrue(effects.GroundedNegativePredicateEffects.Contains(factory.CreatePredicate("pred", "constA")));
            Assert.IsTrue(effects.NegativePredicateEffects.Contains(factory.CreatePredicate("pred", "?a")));

            Assert.AreEqual(5, effects.GroundedNumericFunctionAssignmentEffects.Count);
            var numFunc1A = factory.CreateFunction("numFunc1", "constA");
            Assert.IsTrue(effects.GroundedNumericFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, INumericExpression>(numFunc1A, new Number(5))));
            var numFunc2A = factory.CreateFunction("numFunc2", "constA");
            Assert.IsTrue(effects.GroundedNumericFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, INumericExpression>(numFunc2A, new Minus(new NumericFunction(numFunc2A, idManager), new Number(5)))));
            var numFunc3A = factory.CreateFunction("numFunc3", "constA");
            Assert.IsTrue(effects.GroundedNumericFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, INumericExpression>(numFunc3A, new Plus(new NumericFunction(numFunc3A, idManager), new Number(5)))));
            var numFunc4A = factory.CreateFunction("numFunc4", "constA");
            Assert.IsTrue(effects.GroundedNumericFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, INumericExpression>(numFunc4A, new Divide(new NumericFunction(numFunc4A, idManager), new Number(5)))));
            var numFunc5A = factory.CreateFunction("numFunc5", "constA");
            Assert.IsTrue(effects.GroundedNumericFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, INumericExpression>(numFunc5A, new Multiply(new NumericFunction(numFunc5A, idManager), new Number(5)))));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericAssignEffect(factory.CreateFunction("numFunc1", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericIncreaseEffect(factory.CreateFunction("numFunc2", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericDecreaseEffect(factory.CreateFunction("numFunc3", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericScaleUpEffect(factory.CreateFunction("numFunc4", "?a"), new Number(5), idManager)));
            Assert.IsTrue(effects.NumericFunctionAssignmentEffects.Contains(new NumericScaleDownEffect(factory.CreateFunction("numFunc5", "?a"), new Number(5), idManager)));

            Assert.AreEqual(1, effects.GroundedObjectFunctionAssignmentEffects.Count);
            Assert.IsTrue(effects.GroundedObjectFunctionAssignmentEffects.Contains(new KeyValuePair<IAtom, ITerm>(factory.CreateFunction("objFunc", "constA"), factory.CreateTerm("constA"))));
            Assert.IsTrue(effects.ObjectFunctionAssignmentEffects.Contains(new ObjectAssignEffect(factory.CreateFunction("objFunc", "?a"), factory.CreateTerm("?a"))));

            Assert.AreEqual(6, effects.OriginalLiftedFunctions.Count);
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc1", "constA"), factory.CreateFunction("numFunc1", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc2", "constA"), factory.CreateFunction("numFunc2", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc3", "constA"), factory.CreateFunction("numFunc3", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc4", "constA"), factory.CreateFunction("numFunc4", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc5", "constA"), factory.CreateFunction("numFunc5", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("objFunc", "constA"), factory.CreateFunction("objFunc", "?a"))));

            // second grounding

            ISubstitution substitutionB = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB");
            effects.GroundEffectsByCurrentOperatorSubstitution(groundingManager, substitutionB);

            Assert.AreEqual(1, effects.GroundedPositivePredicateEffects.Count);
            Assert.IsTrue(effects.GroundedPositivePredicateEffects.Contains(factory.CreatePredicate("pred", "constB")));

            Assert.AreEqual(1, effects.GroundedNegativePredicateEffects.Count);
            Assert.IsTrue(effects.GroundedNegativePredicateEffects.Contains(factory.CreatePredicate("pred", "constB")));

            Assert.AreEqual(6, effects.OriginalLiftedFunctions.Count);
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("numFunc1", "constB"), factory.CreateFunction("numFunc1", "?a"))));
            Assert.IsTrue(effects.OriginalLiftedFunctions.Contains(new KeyValuePair<IAtom, IAtom>(factory.CreateFunction("objFunc", "constB"), factory.CreateFunction("objFunc", "?a"))));
        }

        [TestMethod]
        public void TC_EffectsRelevanceConditionsEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsRelevanceConditionsEvaluator_D.pddl"), GetFilePath("TC_EffectsRelevanceConditionsEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            ConditionsCNF goalExpr = (ConditionsCNF)problem.GoalConditions.GetCNF();
            ISubstitution emptySubstitution = new Substitution();

            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[0].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[1].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[2].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[3].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[4].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[5].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[6].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));

            var substitutionA = factory.CreateSubstitution(problem.Operators[7].Parameters, "constA");
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[7].Effects, groundingManager).Evaluate(goalExpr, substitutionA));

            var substitutionB = factory.CreateSubstitution(problem.Operators[7].Parameters, "constB");
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[7].Effects, groundingManager).Evaluate(goalExpr, substitutionB));

            var substitution8B = factory.CreateSubstitution(problem.Operators[8].Parameters, "constB");
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[8].Effects, groundingManager).Evaluate(goalExpr, substitution8B));

            var substitution9B = factory.CreateSubstitution(problem.Operators[9].Parameters, "constB");
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[9].Effects, groundingManager).Evaluate(goalExpr, substitution9B));

            var substitution10B = factory.CreateSubstitution(problem.Operators[10].Parameters, "constB");
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[10].Effects, groundingManager).Evaluate(goalExpr, substitution10B));

            var substitution11B = factory.CreateSubstitution(problem.Operators[11].Parameters, "constB");
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[11].Effects, groundingManager).Evaluate(goalExpr, substitution11B));

            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[12].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution));

            List<int> relevantContionalEffects = new List<int>();
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[13].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution, null, relevantContionalEffects));
            Assert.AreEqual(2, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));
            Assert.IsTrue(relevantContionalEffects.Contains(2));

            relevantContionalEffects.Clear();
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[14].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution, null, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);

            relevantContionalEffects.Clear();
            Assert.IsTrue(new EffectsRelevanceConditionsEvaluator(problem.Operators[15].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution, null, relevantContionalEffects));
            Assert.AreEqual(1, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));

            relevantContionalEffects.Clear();
            Assert.IsFalse(new EffectsRelevanceConditionsEvaluator(problem.Operators[16].Effects, groundingManager).Evaluate(goalExpr, emptySubstitution, null, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);
        }

        [TestMethod]
        public void TC_EffectsRelevanceRelativeStateEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsRelevanceRelativeStateEvaluator_D.pddl"), GetFilePath("TC_EffectsRelevanceRelativeStateEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            IRelativeState relativeState = (IRelativeState)problem.GoalConditions.GetCorrespondingRelativeStates(problem).First();
            ISubstitution emptySubstitution = new Substitution();

            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[0].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[1].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[2].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[3].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[4].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[5].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[6].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[7].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));

            var substitution8A = factory.CreateSubstitution(problem.Operators[8].Parameters, "constA");
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[8].Effects, groundingManager).Evaluate(relativeState, substitution8A));

            var substitution8B = factory.CreateSubstitution(problem.Operators[8].Parameters, "constB");
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[8].Effects, groundingManager).Evaluate(relativeState, substitution8B));

            var substitution9B = factory.CreateSubstitution(problem.Operators[9].Parameters, "constB");
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[9].Effects, groundingManager).Evaluate(relativeState, substitution9B));

            var substitution10B = factory.CreateSubstitution(problem.Operators[10].Parameters, "constB");
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[10].Effects, groundingManager).Evaluate(relativeState, substitution10B));

            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[11].Effects, groundingManager).Evaluate(relativeState, emptySubstitution));

            List<int> relevantContionalEffects = new List<int>();
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[12].Effects, groundingManager).Evaluate(relativeState, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(2, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));
            Assert.IsTrue(relevantContionalEffects.Contains(2));

            relevantContionalEffects.Clear();
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[13].Effects, groundingManager).Evaluate(relativeState, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);

            relevantContionalEffects.Clear();
            Assert.IsTrue(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[14].Effects, groundingManager).Evaluate(relativeState, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(1, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));

            relevantContionalEffects.Clear();
            Assert.IsFalse(new EffectsRelevanceRelativeStateEvaluator(problem.Operators[15].Effects, groundingManager).Evaluate(relativeState, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);
        }

        [TestMethod]
        public void TC_EffectsResultAtomsCollector()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EffectsResultAtomsCollector.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            EffectsResultAtomsCollector collector = new EffectsResultAtomsCollector(problem.Operators[0].Effects, groundingManager);

            var substitutionA = factory.CreateSubstitution(problem.Operators[0].Parameters, "constA");
            var atomsA = collector.Collect(substitutionA);
            Assert.AreEqual(4, atomsA.Count);
            Assert.IsTrue(atomsA.Contains(factory.CreatePredicate("predA")));
            Assert.IsTrue(atomsA.Contains(factory.CreatePredicate("predC", "constA")));
            Assert.IsTrue(atomsA.Contains(factory.CreatePredicate("predD", "constA")));
            Assert.IsTrue(atomsA.Contains(factory.CreatePredicate("predD", "constB")));

            var substitutionB = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB");
            var atomsB = collector.Collect(substitutionB);
            Assert.AreEqual(4, atomsB.Count);
            Assert.IsTrue(atomsB.Contains(factory.CreatePredicate("predA")));
            Assert.IsTrue(atomsB.Contains(factory.CreatePredicate("predC", "constB")));
            Assert.IsTrue(atomsB.Contains(factory.CreatePredicate("predD", "constA")));
            Assert.IsTrue(atomsB.Contains(factory.CreatePredicate("predD", "constB")));
        }

        [TestMethod]
        public void TC_EvaluationManager()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_EvaluationManager_D.pddl"), GetFilePath("TC_EvaluationManager_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, idManager), problem.RigidRelations);

            ISubstitution substitutionA = factory.CreateSubstitution(problem.Operators[2].Parameters, "constA");
            ISubstitution substitutionB = factory.CreateSubstitution(problem.Operators[2].Parameters, "constB");

            Assert.IsTrue(evaluationManager.Evaluate(problem.Operators[0].Preconditions, new Substitution(), problem.InitialState));
            Assert.IsTrue(evaluationManager.Evaluate((ConditionsCNF)problem.Operators[0].Preconditions.GetCNF(), new Substitution(), problem.InitialState));
            Assert.IsTrue(problem.Operators[0].Preconditions.TrueForAll(expr => evaluationManager.Evaluate(expr, new Substitution(), problem.InitialState)));

            Assert.IsFalse(evaluationManager.Evaluate(problem.Operators[1].Preconditions, new Substitution(), problem.InitialState));
            Assert.IsFalse(evaluationManager.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), new Substitution(), problem.InitialState));
            Assert.IsFalse(problem.Operators[1].Preconditions.TrueForAll(expr => evaluationManager.Evaluate(expr, new Substitution(), problem.InitialState)));

            Assert.IsTrue(evaluationManager.Evaluate(problem.Operators[2].Preconditions, substitutionA, problem.InitialState));
            Assert.IsTrue(evaluationManager.Evaluate((ConditionsCNF)problem.Operators[2].Preconditions.GetCNF(), substitutionA, problem.InitialState));
            Assert.IsTrue(problem.Operators[2].Preconditions.TrueForAll(expr => evaluationManager.Evaluate(expr, substitutionA, problem.InitialState)));

            Assert.IsFalse(evaluationManager.Evaluate(problem.Operators[2].Preconditions, substitutionB, problem.InitialState));
            Assert.IsFalse(evaluationManager.Evaluate((ConditionsCNF)problem.Operators[2].Preconditions.GetCNF(), substitutionB, problem.InitialState));
            Assert.IsFalse(problem.Operators[2].Preconditions.TrueForAll(expr => evaluationManager.Evaluate(expr, substitutionB, problem.InitialState)));

            Assert.IsTrue(evaluationManager.Evaluate(problem.Operators[2].Preconditions, null, problem.InitialState));
            Assert.IsTrue(evaluationManager.Evaluate((ConditionsCNF)problem.Operators[2].Preconditions.GetCNF(), null, problem.InitialState));

            Assert.IsTrue(evaluationManager.IsPredicateRigidRelation(factory.CreatePredicate("pred", "constA")));
            Assert.IsFalse(evaluationManager.IsPredicateRigidRelation(factory.CreatePredicate("predZ", "constA")));

            Assert.IsTrue(evaluationManager.EvaluateRigidRelationsCompliance(problem.Operators[2].Preconditions, substitutionA));
            Assert.IsTrue(problem.Operators[2].Preconditions.TrueForAll(expr => evaluationManager.EvaluateRigidRelationsCompliance(expr, substitutionA)));
            Assert.IsFalse(evaluationManager.EvaluateRigidRelationsCompliance(problem.Operators[2].Preconditions, substitutionB));
            Assert.IsFalse(problem.Operators[2].Preconditions.TrueForAll(expr => evaluationManager.EvaluateRigidRelationsCompliance(expr, substitutionB)));

            Assert.AreEqual(3, evaluationManager.GetNotAccomplishedConstraintsCount((Conditions)problem.GoalConditions, problem.InitialState));
            Assert.AreEqual(3, evaluationManager.GetNotAccomplishedConstraintsCount((ConditionsCNF)problem.GoalConditions.GetCNF(), problem.InitialState));

            StateLabels stateLabels = new StateLabels
            {
                {factory.CreatePredicate("predTrue"), 5},
                {factory.CreatePredicate("predFalse"), 3}
            };
            Assert.AreEqual(5, evaluationManager.EvaluateOperatorPlanningGraphLabel((Conditions)problem.GoalConditions, new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(8, evaluationManager.EvaluateOperatorPlanningGraphLabel((Conditions)problem.GoalConditions, new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            var refState = factory.CreateState(factory.CreatePredicate("predZ", "constA"));
            var satisfyingAtoms = evaluationManager.GetSatisfyingAtoms(problem.Operators[3].Preconditions, factory.CreateSubstitution(problem.Operators[3].Parameters, "constA", "constA"), refState);
            Assert.AreEqual(1, satisfyingAtoms.Count);
            Assert.IsTrue(satisfyingAtoms.Contains(factory.CreatePredicate("predZ", "constA")));

            var precond4 = (ConditionsCNF)problem.Operators[3].Preconditions.GetCNF();
            evaluationManager.RenameConditionParameters(precond4, 7);
            Assert.AreEqual(7, precond4.Parameters[0].ParameterNameId);
            PredicateLiteralCNF pred4 = (PredicateLiteralCNF)precond4.Last();
            VariableTerm var4 = (VariableTerm)pred4.PredicateAtom.GetTerms()[0];
            Assert.AreEqual(7, var4.NameId);

            var usedPredicates = evaluationManager.CollectUsedPredicates(problem.Operators[0].Preconditions);
            Assert.AreEqual(4, usedPredicates.Count);
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predTrue")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("predFalse")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("pred", "constA")));
            Assert.IsTrue(usedPredicates.Contains(factory.CreatePredicate("pred", "constB")));
        }

        [TestMethod]
        public void TC_ExpressionsBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ExpressionsBuilder.pddl"), GetFilePath("Dummy_P.pddl"));

            ExpressionsBuilder builder = new ExpressionsBuilder(new IdManager(data));

            var preconditions = data.Domain.Actions[0].Preconditions;
            Assert.IsTrue(builder.Build(preconditions[0]) is PreferenceExpression);
            Assert.IsTrue(builder.Build(preconditions[1]) is PredicateExpression);
            Assert.IsTrue(builder.Build(preconditions[2]) is NotExpression);
            Assert.IsTrue(builder.Build(preconditions[3]) is AndExpression);
            Assert.IsTrue(builder.Build(preconditions[4]) is OrExpression);
            Assert.IsTrue(builder.Build(preconditions[5]) is ImplyExpression);
            Assert.IsTrue(builder.Build(preconditions[6]) is ForallExpression);
            Assert.IsTrue(builder.Build(preconditions[7]) is ExistsExpression);
            Assert.IsTrue(builder.Build(preconditions[8]) is NumericCompareExpression);
            Assert.IsTrue(builder.Build(preconditions[9]) is EqualsExpression);
        }

        [TestMethod]
        public void TC_ExpressionEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ExpressionEvaluator_D.pddl"), GetFilePath("TC_ExpressionEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            ExpressionEvaluator evaluator = new ExpressionEvaluator(new GroundingManager(data, new IdManager(data)), problem.RigidRelations);

            foreach (IExpression expression in problem.Operators[0].Preconditions)
            {
                Assert.IsTrue(evaluator.Evaluate(expression, new Substitution(), problem.InitialState));
            }

            foreach (IExpression expression in problem.Operators[1].Preconditions)
            {
                Assert.IsFalse(evaluator.Evaluate(expression, new Substitution(), problem.InitialState));
            }
        }

        [TestMethod]
        public void TC_ExpressionsGrounder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ExpressionsGrounder.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            Lazy<TermsGrounder> termsGrounder = new Lazy<TermsGrounder>(() => new TermsGrounder(idManager));
            ExpressionsGrounder expressionsGrounder = new ExpressionsGrounder(termsGrounder, new Lazy<NumericExpressionsGrounder>(() => new NumericExpressionsGrounder(termsGrounder, idManager)), idManager);

            var constB = factory.CreateConstant("constB");
            var substitution0 = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB", "constB");

            var precond = problem.Operators[0].Preconditions;
            var groundedExpression = expressionsGrounder.Ground(precond.GetWrappedConditions(), substitution0);

            AndExpression andExpr = groundedExpression as AndExpression;
            Assert.IsNotNull(andExpr);
            Assert.AreEqual(4, andExpr.Children.Count);

            PredicateExpression pred0 = andExpr.Children[0] as PredicateExpression;
            Assert.IsNotNull(pred0);
            var pred0Terms = pred0.PredicateAtom.GetTerms();
            ConstantTerm pred0ConstTerm0 = pred0Terms[0] as ConstantTerm;
            Assert.IsNotNull(pred0ConstTerm0);
            Assert.AreEqual(constB, pred0ConstTerm0.NameId);
            ConstantTerm pred0ConstTerm1 = pred0Terms[1] as ConstantTerm;
            Assert.IsNotNull(pred0ConstTerm1);
            Assert.AreEqual(constB, pred0ConstTerm1.NameId);

            EqualsExpression equals1 = andExpr.Children[1] as EqualsExpression;
            Assert.IsNotNull(equals1);
            ObjectFunctionTerm objFunc1 = equals1.LeftArgument as ObjectFunctionTerm;
            Assert.IsNotNull(objFunc1);
            ConstantTerm objFunc1ConstTerm = objFunc1.FunctionAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(objFunc1ConstTerm);
            Assert.AreEqual(constB, objFunc1ConstTerm.NameId);
            ConstantTerm constTerm1 = equals1.RightArgument as ConstantTerm;
            Assert.IsNotNull(constTerm1);
            Assert.AreEqual(constB, constTerm1.NameId);

            NumericCompareExpression compare2 = andExpr.Children[2] as NumericCompareExpression;
            Assert.IsNotNull(compare2);
            NumericFunction numFunc2 = compare2.LeftArgument as NumericFunction;
            Assert.IsNotNull(numFunc2);
            ConstantTerm numFuncFunc2ConstTerm = numFunc2.FunctionAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(numFuncFunc2ConstTerm);
            Assert.AreEqual(constB, numFuncFunc2ConstTerm.NameId);

            ForallExpression forall3 = andExpr.Children[3] as ForallExpression;
            Assert.IsNotNull(forall3);
            PredicateExpression pred3 = forall3.Child as PredicateExpression;
            Assert.IsNotNull(pred3);
            ConstantTerm pred3ConstTerm = pred3.PredicateAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(pred3ConstTerm);
            Assert.AreEqual(constB, pred3ConstTerm.NameId);
            VariableTerm pred3VariableTerm = pred3.PredicateAtom.GetTerms()[1] as VariableTerm;
            Assert.IsNotNull(pred3VariableTerm);
        }

        [TestMethod]
        public void TC_TermsBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_TermsBuilder.pddl"), GetFilePath("Dummy_P.pddl"));

            IdManager idManager = new IdManager(data);
            TermsBuilder builder = new TermsBuilder(idManager);
            var preconditions = data.Domain.Actions[0].Preconditions;
            var parameters = data.Domain.Actions[0].Parameters;

            idManager.Variables.RegisterLocalParameters(parameters);

            InputData.PDDL.PredicateExpression pred0 = (InputData.PDDL.PredicateExpression)preconditions[0];
            Assert.IsTrue(builder.Build(pred0.Terms[0]) is ConstantTerm);

            InputData.PDDL.EqualsExpression equals1 = (InputData.PDDL.EqualsExpression)preconditions[1];
            Assert.IsTrue(builder.Build(equals1.Term1) is VariableTerm);
            Assert.IsTrue(builder.Build(equals1.Term2) is VariableTerm);

            InputData.PDDL.EqualsExpression equals2 = (InputData.PDDL.EqualsExpression)preconditions[2];
            Assert.IsTrue(builder.Build(equals2.Term1) is ObjectFunctionTerm);
            Assert.IsTrue(builder.Build(equals2.Term2) is ConstantTerm);

            idManager.Variables.UnregisterLocalParameters(parameters);
        }

        [TestMethod]
        public void TC_Terms()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Terms.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            var preconditions = problem.Operators[0].Preconditions;

            EqualsExpression equals0 = (EqualsExpression)preconditions[0];
            VariableTerm variable0 = equals0.LeftArgument as VariableTerm;
            Assert.IsNotNull(variable0);
            ITerm var0Clone = variable0.Clone();
            Assert.IsTrue(variable0 != var0Clone);
            Assert.IsTrue(variable0.GetHashCode() == var0Clone.GetHashCode());
            Assert.IsTrue(variable0.Equals(var0Clone));

            ConstantTerm const0 = equals0.RightArgument as ConstantTerm;
            Assert.IsNotNull(const0);
            ITerm const0Clone = const0.Clone();
            Assert.IsTrue(const0 != const0Clone);
            Assert.IsTrue(const0.GetHashCode() == const0Clone.GetHashCode());
            Assert.IsTrue(const0.Equals(const0Clone));

            EqualsExpression equals1 = (EqualsExpression)preconditions[1];
            ObjectFunctionTerm objFunc1 = equals1.LeftArgument as ObjectFunctionTerm;
            Assert.IsNotNull(objFunc1);
            ITerm objFunc1Clone = objFunc1.Clone();
            Assert.IsTrue(objFunc1 != objFunc1Clone);
            Assert.IsTrue(objFunc1.GetHashCode() == objFunc1Clone.GetHashCode());
            Assert.IsTrue(objFunc1.Equals(objFunc1Clone));
        }

        [TestMethod]
        public void TC_TermsGrounder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_TermsGrounder_D.pddl"), GetFilePath("TC_TermsGrounder_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            TermsGrounder termsGrounder = new TermsGrounder(idManager);

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");
            ISubstitution substitution0 = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB", "constA");
            var precond = problem.Operators[0].Preconditions;
            var refState = problem.InitialState;

            PredicateExpression pred0 = precond[0] as PredicateExpression;
            Assert.IsNotNull(pred0);
            ITerm pred0GroundedTerm0 = termsGrounder.GroundTerm(pred0.PredicateAtom.GetTerms()[0], substitution0);
            ConstantTerm pred0ConstTerm0 = pred0GroundedTerm0 as ConstantTerm;
            Assert.IsNotNull(pred0ConstTerm0);
            Assert.AreEqual(constB, pred0ConstTerm0.NameId);
            ITerm pred0GroundedTerm1 = termsGrounder.GroundTerm(pred0.PredicateAtom.GetTerms()[1], substitution0);
            ConstantTerm pred0ConstTerm1 = pred0GroundedTerm1 as ConstantTerm;
            Assert.IsNotNull(pred0ConstTerm1);
            Assert.AreEqual(constA, pred0ConstTerm1.NameId);

            IAtom groundedAtom0 = termsGrounder.GroundAtom(pred0.PredicateAtom, substitution0);
            Assert.AreEqual(pred0GroundedTerm0, groundedAtom0.GetTerms()[0]);
            Assert.AreEqual(pred0GroundedTerm1, groundedAtom0.GetTerms()[1]);

            PredicateExpression pred1 = precond[1] as PredicateExpression;
            Assert.IsNotNull(pred1);
            ITerm pred1GroundedTerm = termsGrounder.GroundTerm(pred1.PredicateAtom.GetTerms()[0], substitution0);
            ObjectFunctionTerm pred1ObjFuncTerm = pred1GroundedTerm as ObjectFunctionTerm;
            Assert.IsNotNull(pred1ObjFuncTerm);
            ConstantTerm pred1ConstTerm = pred1ObjFuncTerm.FunctionAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(pred1ConstTerm);
            Assert.AreEqual(constB, pred1ConstTerm.NameId);
            ITerm pred1GroundedTermDeep = termsGrounder.GroundTermDeep(pred1.PredicateAtom.GetTerms()[0], substitution0, refState);
            ConstantTerm pred1ConstTermDeep = pred1GroundedTermDeep as ConstantTerm;
            Assert.IsNotNull(pred1ConstTermDeep);
            Assert.AreEqual(constA, pred1ConstTermDeep.NameId);

            IAtom groundedAtom1Deep = termsGrounder.GroundAtomDeep(pred1.PredicateAtom, substitution0, refState);
            Assert.AreEqual(pred1GroundedTermDeep, groundedAtom1Deep.GetTerms()[0]);
        }

        [TestMethod]
        public void TC_ExpressionToCNFTransformer()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ExpressionToCNFTransformer.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, new IdManager(data)));
            ExpressionToCNFTransformer transformer = new ExpressionToCNFTransformer(evaluationManager);

            IExpression expression0 = transformer.Transform(problem.Operators[0].Preconditions.GetWrappedConditions());
            var expr0 = expression0 as OrExpression;
            Assert.IsNotNull(expr0);
            Assert.AreEqual(4, expr0.Children.Count);
            foreach (var child in expr0.Children)
            {
                Assert.IsTrue(child is PredicateExpression);
            }

            IExpression expression1 = transformer.Transform(problem.Operators[1].Preconditions.GetWrappedConditions());
            var expr1 = expression1 as AndExpression;
            Assert.IsNotNull(expr1);
            Assert.AreEqual(4, expr1.Children.Count);
            foreach (var child in expr1.Children)
            {
                Assert.IsTrue(child is PredicateExpression);
            }

            IExpression expression2 = transformer.Transform(problem.Operators[2].Preconditions.GetWrappedConditions());
            var expr2 = expression2 as AndExpression;
            Assert.IsNotNull(expr2);
            Assert.AreEqual(4, expr2.Children.Count);
            foreach (var child in expr2.Children)
            {
                OrExpression childExpr = child as OrExpression;
                Assert.IsNotNull(childExpr);
                Assert.AreEqual(3, childExpr.Children.Count);
            }

            IExpression expression3 = transformer.Transform(problem.Operators[3].Preconditions.GetWrappedConditions());
            var expr3 = expression3 as AndExpression;
            Assert.IsNotNull(expr3);
            Assert.AreEqual(2, expr3.Children.Count);
            Assert.IsTrue(expr3.Children[0] is PredicateExpression);
            Assert.IsTrue(expr3.Children[1] is NotExpression);

            IExpression expression4 = transformer.Transform(problem.Operators[4].Preconditions.GetWrappedConditions());
            var expr4 = expression4 as AndExpression;
            Assert.IsNotNull(expr4);
            Assert.AreEqual(3, expr4.Children.Count);
            Assert.IsTrue(expr4.Children[0] is NotExpression);
            Assert.IsTrue(expr4.Children[1] is PredicateExpression);
            Assert.IsTrue(expr4.Children[2] is PredicateExpression);

            IExpression expression5 = transformer.Transform(problem.Operators[5].Preconditions.GetWrappedConditions());
            var expr5 = expression5 as OrExpression;
            Assert.IsNotNull(expr5);
            Assert.AreEqual(2, expr5.Children.Count);
            Assert.IsTrue(expr5.Children[0] is PredicateExpression);
            Assert.IsTrue(expr5.Children[1] is PredicateExpression);
        }

        [TestMethod]
        public void TC_ExpressionToNNFTransformer()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_ExpressionToNNFTransformer.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, new IdManager(data)));
            IExpression expression = problem.Operators[0].Preconditions[0];

            ExpressionToNNFTransformer transformer = new ExpressionToNNFTransformer(evaluationManager);
            IExpression newExpression = transformer.Transform(expression);

            AndExpression andExpr = newExpression as AndExpression;
            Assert.IsNotNull(andExpr);
            Assert.AreEqual(5, andExpr.Children.Count);

            OrExpression child0 = andExpr.Children[0] as OrExpression;
            Assert.IsNotNull(child0);
            Assert.AreEqual(2, child0.Children.Count);
            Assert.IsTrue(child0.Children[0] is NotExpression);
            Assert.IsTrue(child0.Children[1] is NotExpression);

            AndExpression child1 = andExpr.Children[1] as AndExpression;
            Assert.IsNotNull(child1);
            Assert.AreEqual(2, child1.Children.Count);
            Assert.IsTrue(child1.Children[0] is PredicateExpression);
            Assert.IsTrue(child1.Children[1] is NotExpression);

            EqualsExpression child2 = andExpr.Children[2] as EqualsExpression;
            Assert.IsNotNull(child2);

            OrExpression child3 = andExpr.Children[3] as OrExpression;
            Assert.IsNotNull(child3);
            Assert.AreEqual(2, child3.Children.Count);
            Assert.IsTrue(child3.Children[0] is NotExpression);
            Assert.IsTrue(child3.Children[1] is NotExpression);

            AndExpression child4 = andExpr.Children[4] as AndExpression;
            Assert.IsNotNull(child4);
            Assert.AreEqual(2, child4.Children.Count);
            Assert.IsTrue(child4.Children[0] is PredicateExpression);
            Assert.IsTrue(child4.Children[1] is PredicateExpression);
        }

        [TestMethod]
        public void TC_Grounder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Grounder_D.pddl"), GetFilePath("TC_Grounder_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            Grounder grounder = new Grounder(idManager);

            ISubstitution substitution = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB", "constA");
            var precond = problem.Operators[0].Preconditions;
            var refState = problem.InitialState;

            PredicateExpression pred0 = precond[0] as PredicateExpression;
            Assert.IsNotNull(pred0);
            ITerm pred0GroundedTerm0 = grounder.GroundTerm(pred0.PredicateAtom.GetTerms()[0], substitution);
            ITerm pred0GroundedTerm1 = grounder.GroundTerm(pred0.PredicateAtom.GetTerms()[1], substitution);
            IAtom groundedAtom0 = grounder.GroundAtom(pred0.PredicateAtom, substitution);
            Assert.AreEqual(pred0GroundedTerm0, groundedAtom0.GetTerms()[0]);
            Assert.AreEqual(pred0GroundedTerm1, groundedAtom0.GetTerms()[1]);

            PredicateExpression pred1 = precond[1] as PredicateExpression;
            Assert.IsNotNull(pred1);
            ITerm pred1GroundedTermDeep = grounder.GroundTermDeep(pred1.PredicateAtom.GetTerms()[0], substitution, refState);
            IAtom groundedAtom1Deep = grounder.GroundAtomDeep(pred1.PredicateAtom, substitution, refState);
            Assert.AreEqual(pred1GroundedTermDeep, groundedAtom1Deep.GetTerms()[0]);

            IExpression groundedExpr0 = grounder.GroundExpression(pred0, substitution);
            PredicateExpression groundedExpr0Pred = groundedExpr0 as PredicateExpression;
            Assert.IsNotNull(groundedExpr0Pred);
            Assert.AreEqual(pred0GroundedTerm0, groundedExpr0Pred.PredicateAtom.GetTerms()[0]);
            Assert.AreEqual(pred0GroundedTerm1, groundedExpr0Pred.PredicateAtom.GetTerms()[1]);

            ISubstitution substitution1 = factory.CreateSubstitution(problem.Operators[0].Parameters, "", "constB");
            var groundedPrecond = grounder.GroundConditions(precond, substitution1);
            PredicateExpression groundedPrecond0 = groundedPrecond[0] as PredicateExpression;
            Assert.IsNotNull(groundedPrecond0);
            VariableTerm variableTerm0 = groundedPrecond0.PredicateAtom.GetTerms()[0] as VariableTerm;
            Assert.IsNotNull(variableTerm0);
            Assert.IsTrue(problem.Operators[0].Parameters[0].ParameterNameId == variableTerm0.NameId);
            ConstantTerm constantTerm0 = groundedPrecond0.PredicateAtom.GetTerms()[1] as ConstantTerm;
            Assert.IsNotNull(constantTerm0);
            Assert.IsTrue(factory.CreateConstant("constB") == constantTerm0.NameId);

            NumericCompareExpression numCompare2 = precond[2] as NumericCompareExpression;
            Assert.IsNotNull(numCompare2);
            var pred2GroundedNumericExpression0 = grounder.GroundNumericExpression(numCompare2.LeftArgument, substitution);
            var pred2GroundedNumericExpression1 = grounder.GroundNumericExpression(numCompare2.RightArgument, substitution);
            NumericFunction numFunc0 = pred2GroundedNumericExpression0 as NumericFunction;
            Assert.IsNotNull(numFunc0);
            Assert.IsTrue(numFunc0.FunctionAtom.GetGroundedTerm(0) == factory.CreateConstant("constB"));
            NumericFunction numFunc1 = pred2GroundedNumericExpression1 as NumericFunction;
            Assert.IsNotNull(numFunc1);
            Assert.IsTrue(numFunc1.FunctionAtom.GetGroundedTerm(0) == factory.CreateConstant("constA"));
        }

        [TestMethod]
        public void TC_GroundingManager()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_GroundingManager_D.pddl"), GetFilePath("TC_GroundingManager_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager manager = new GroundingManager(data, idManager);

            var parameters = problem.Operators[0].Parameters;
            var precond = problem.Operators[0].Preconditions;
            var precond0 = (PredicateExpression)precond[0];
            var precond1 = (PredicateExpression)precond[1];
            var precond2 = (NumericCompareExpression)precond[2];

            var substitutionAbc = factory.CreateSubstitution(parameters, "constA", "constB", "constC");
            var pred1Ac = factory.CreatePredicate("pred1", "constA", "constC");
            var pred2B = factory.CreatePredicate("pred2", "constB");
            var objFuncA = factory.CreateFunction("objFunc", "constA");
            var numFuncB = factory.CreateFunction("numFunc", "constB");
            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");

            Assert.AreEqual(pred1Ac, manager.GroundAtom(precond0.PredicateAtom, substitutionAbc));
            Assert.AreEqual(pred2B, manager.GroundAtomDeep(precond1.PredicateAtom, substitutionAbc, problem.InitialState));

            Assert.AreEqual(new ObjectFunctionTerm(objFuncA, idManager), manager.GroundTerm(precond1.PredicateAtom.GetTerms()[0], substitutionAbc));
            Assert.AreEqual(new ConstantTerm(constB, idManager), manager.GroundTermDeep(precond1.PredicateAtom.GetTerms()[0], substitutionAbc, problem.InitialState));

            Assert.AreEqual(new PredicateExpression(pred1Ac, idManager), manager.GroundExpression(precond0, substitutionAbc));
            Assert.AreEqual(new NumericFunction(numFuncB, idManager), manager.GroundNumericExpression(precond2.LeftArgument, substitutionAbc));

            var groundedConditions = manager.GroundConditions(precond, substitutionAbc);
            Assert.AreEqual(3, groundedConditions.Count);
            Assert.IsTrue(groundedConditions.Contains(new PredicateExpression(pred1Ac, idManager)));

            var localSubstitutions = manager.GenerateAllLocalSubstitutions(parameters).ToList();
            Assert.AreEqual(4, localSubstitutions.Count);
            Assert.IsTrue(localSubstitutions.Contains(factory.CreateSubstitution(parameters, "constA", "constA", "constC")));
            Assert.IsTrue(localSubstitutions.Contains(factory.CreateSubstitution(parameters, "constA", "constB", "constC")));
            Assert.IsTrue(localSubstitutions.Contains(factory.CreateSubstitution(parameters, "constB", "constA", "constC")));
            Assert.IsTrue(localSubstitutions.Contains(factory.CreateSubstitution(parameters, "constB", "constB", "constC")));

            var constantsOfType = manager.GetAllConstantsOfType(factory.CreateType("typeA")).ToList();
            Assert.AreEqual(2, constantsOfType.Count);
            Assert.IsTrue(constantsOfType.Contains(constA));
            Assert.IsTrue(constantsOfType.Contains(constB));

            var typesForConstant = manager.GetTypesForConstant(constA);
            Assert.AreEqual(1, typesForConstant.Count);
            Assert.IsTrue(typesForConstant.Contains(factory.CreateType("typeA")));

            var childrenTypes = manager.GetChildrenTypes(factory.CreateType("typeB")).ToList();
            Assert.AreEqual(1, childrenTypes.Count);
            Assert.IsTrue(childrenTypes.Contains(factory.CreateType("typeA")));

            var liftedPredicates = manager.GetAllLiftedPredicates();
            Assert.AreEqual(2, liftedPredicates.Count);
            var liftedNumericFunctions = manager.GetAllLiftedNumericFunctions();
            Assert.AreEqual(1, liftedNumericFunctions.Count);
            var liftedObjectFunctions = manager.GetAllLiftedObjectFunctions();
            Assert.AreEqual(1, liftedObjectFunctions.Count);

            var groundedPredicates = manager.GetAllGroundedPredicates();
            Assert.AreEqual(12, groundedPredicates.Count);
            var groundedNumericFunctions = manager.GetAllGroundedNumericFunctions();
            Assert.AreEqual(3, groundedNumericFunctions.Count);
            var groundedObjectFunctions = manager.GetAllGroundedObjectFunctions();
            Assert.AreEqual(3, groundedObjectFunctions.Count);
        }

        [TestMethod]
        public void TC_IDManager()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_IDManager_D.pddl"), GetFilePath("TC_IDManager_P.pddl"));

            IdManager idManager = new IdManager(data);

            Assert.AreEqual(3, idManager.Predicates.Count());
            Assert.AreEqual("predA", idManager.Predicates.GetNameFromId(idManager.Predicates.GetId("predA")));
            Assert.AreEqual("predB", idManager.Predicates.GetNameFromId(idManager.Predicates.GetId("predB", 1)));
            Assert.AreEqual("predB", idManager.Predicates.GetNameFromId(idManager.Predicates.GetId("predB", 2)));
            Assert.IsTrue(idManager.Predicates.GetId("predB", 1) != idManager.Predicates.GetId("predB", 2));

            Assert.AreEqual(2, idManager.Functions.Count());
            Assert.IsTrue(idManager.Functions.IsRegistered("objFunc", 1));
            Assert.IsFalse(idManager.Functions.IsRegistered("objFunc"));
            idManager.Functions.Register("objFunc");
            Assert.IsTrue(idManager.Functions.IsRegistered("objFunc"));
            Assert.IsTrue(idManager.Functions.GetId("objFunc") != idManager.Functions.GetId("objFunc", 1));
            idManager.Functions.Unregister("objFunc");
            Assert.IsFalse(idManager.Functions.IsRegistered("objFunc"));
            Assert.IsTrue(idManager.Functions.IsRegistered("numFunc", 1));
            Assert.AreEqual(1, idManager.Functions.GetNumberOfArgumentsFromId(idManager.Functions.GetId("numFunc", 1)));

            Assert.AreEqual(3, idManager.Constants.Count());
            Assert.IsTrue(idManager.Constants.IsRegistered("constA"));
            Assert.IsTrue(idManager.Constants.IsRegistered("constB"));
            Assert.IsTrue(idManager.Constants.IsRegistered("objectA"));
            Assert.IsFalse(idManager.Constants.IsRegistered("constC"));

            Assert.AreEqual(3, idManager.Types.Count());
            Assert.IsTrue(idManager.Types.IsRegistered("typeA"));
            Assert.IsTrue(idManager.Types.IsRegistered("typeB"));
            Assert.IsTrue(idManager.Types.IsRegistered("object"));

            var usedIDs = idManager.Types.GetUsedIDs().ToList();
            Assert.IsTrue(usedIDs.Contains(idManager.Types.GetId("typeA")));
            Assert.IsTrue(usedIDs.Contains(idManager.Types.GetId("typeB")));
            Assert.IsTrue(usedIDs.Contains(idManager.Types.GetId("object")));

            Assert.AreEqual(0, idManager.Preferences.Count());
            Assert.AreEqual(0, idManager.Variables.Count());

            idManager.Variables.RegisterLocalParameters(data.Domain.Actions[0].Parameters);
            Assert.AreEqual(2, idManager.Variables.Count());
            Assert.IsTrue(idManager.Variables.IsRegistered("?a"));
            Assert.IsTrue(idManager.Variables.IsRegistered("?b"));
            idManager.Variables.UnregisterLocalParameters(data.Domain.Actions[0].Parameters);
            Assert.AreEqual(0, idManager.Variables.Count());
        }

        [TestMethod]
        public void TC_InitialStateDataBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_InitialStateDataBuilder_D.pddl"), GetFilePath("TC_InitialStateDataBuilder_P.pddl"));

            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            InitialStateDataBuilder builder = new InitialStateDataBuilder(idManager);
            builder.Build(data.Problem.Init);

            Assert.IsTrue(builder.Predicates.Contains(factory.CreatePredicate("pred1")));
            Assert.IsTrue(builder.Predicates.Contains(factory.CreatePredicate("pred2", "constA")));
            Assert.IsFalse(builder.Predicates.Contains(factory.CreatePredicate("pred2", "constB")));
            Assert.IsFalse(builder.Predicates.Contains(factory.CreatePredicate("pred2", "constC")));

            var numericFuncConstA = factory.CreateFunction("numericFunc", "constA");
            var numericFuncConstB = factory.CreateFunction("numericFunc", "constB");
            var numericFuncConstC = factory.CreateFunction("numericFunc", "constC");
            Assert.IsTrue(builder.NumericFunctions.ContainsKey(numericFuncConstA));
            Assert.AreEqual(2, builder.NumericFunctions[numericFuncConstA]);
            Assert.IsTrue(builder.NumericFunctions.ContainsKey(numericFuncConstB));
            Assert.AreEqual(5, builder.NumericFunctions[numericFuncConstB]);
            Assert.IsFalse(builder.NumericFunctions.ContainsKey(numericFuncConstC));

            var objectFunc = factory.CreateFunction("objectFunc");
            Assert.IsTrue(builder.ObjectFunctions.ContainsKey(objectFunc));
            Assert.AreEqual(factory.CreateConstant("constA"), builder.ObjectFunctions[objectFunc]);
        }

        [TestMethod]
        public void TC_NotAccomplishedConstraintsCounter()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NotAccomplishedConstraintsCounter_D.pddl"), GetFilePath("TC_NotAccomplishedConstraintsCounter_P.pddl"));

            Problem problem = new Problem(data);
            GroundingManager groundingManager = new GroundingManager(data, new IdManager(data));
            Lazy<ExpressionEvaluator> expressionEvaluator = new Lazy<ExpressionEvaluator>(() => new ExpressionEvaluator(groundingManager, problem.RigidRelations));
            Conditions goalConditions = (Conditions)problem.GoalConditions;

            NotAccomplishedConstraintsCounter counter = new NotAccomplishedConstraintsCounter(groundingManager, expressionEvaluator);
            Assert.AreEqual(4, counter.Evaluate(goalConditions, problem.InitialState));

            IState newState = problem.Operators[0].Apply(problem.InitialState, new Substitution());
            Assert.AreEqual(3, counter.Evaluate(goalConditions, newState));

            IState newState2 = problem.Operators[1].Apply(newState, new Substitution());
            Assert.AreEqual(0, counter.Evaluate(goalConditions, newState2));
        }

        [TestMethod]
        public void TC_NotAccomplishedConstraintsCounterCNF()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NotAccomplishedConstraintsCounterCNF_D.pddl"), GetFilePath("TC_NotAccomplishedConstraintsCounterCNF_P.pddl"));

            Problem problem = new Problem(data);
            GroundingManager groundingManager = new GroundingManager(data, new IdManager(data));
            Lazy<ConditionsCNFEvaluator> conditionsCnfEvaluator = new Lazy<ConditionsCNFEvaluator>(() => new ConditionsCNFEvaluator(groundingManager));
            NotAccomplishedConstraintsCounterCNF counter = new NotAccomplishedConstraintsCounterCNF(conditionsCnfEvaluator);

            ConditionsCNF goalConditions = (ConditionsCNF)problem.GoalConditions.GetCNF();
            Assert.AreEqual(4, counter.Evaluate(goalConditions, problem.InitialState));

            IState newState = problem.Operators[0].Apply(problem.InitialState, new Substitution());
            Assert.AreEqual(3, counter.Evaluate(goalConditions, newState));

            IState newState2 = problem.Operators[1].Apply(newState, new Substitution());
            Assert.AreEqual(0, counter.Evaluate(goalConditions, newState2));
        }

        [TestMethod]
        public void TC_NumericAssignmentsBackwardsReplacer()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericAssignmentsBackwardsReplacer_D.pddl"), GetFilePath("TC_NumericAssignmentsBackwardsReplacer_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            ISubstitution operatorSubstitution = factory.CreateSubstitution(problem.Operators[0].Parameters, "constA");
            ISubstitution expressionSubstitution = new Substitution();
            Conditions goalConditions = (Conditions)problem.GoalConditions;

            EffectsPreprocessedCollection effects = new EffectsPreprocessedCollection();
            effects.CollectEffects(problem.Operators[0].Effects);
            effects.GroundEffectsByCurrentOperatorSubstitution(groundingManager, operatorSubstitution);

            NumericAssignmentsBackwardsReplacer replacer = new NumericAssignmentsBackwardsReplacer(effects.GroundedNumericFunctionAssignmentEffects, groundingManager, operatorSubstitution, expressionSubstitution);

            NumericCompareExpression expression0 = (NumericCompareExpression)goalConditions[0];
            INumericExpression leftExpression0 = replacer.Replace(expression0.LeftArgument);
            INumericExpression rightExpression0 = replacer.Replace(expression0.RightArgument);

            Number numberLeft0 = leftExpression0 as Number;
            Assert.IsNotNull(numberLeft0);
            Assert.AreEqual(20, numberLeft0.Value);

            Number numberRight0 = rightExpression0 as Number;
            Assert.IsNotNull(numberRight0);
            Assert.AreEqual(0.5, numberRight0.Value);

            NumericCompareExpression expression1 = (NumericCompareExpression)goalConditions[1];
            INumericExpression leftExpression1 = replacer.Replace(expression1.LeftArgument);
            INumericExpression rightExpression1 = replacer.Replace(expression1.RightArgument);

            Number numberLeft1 = leftExpression1 as Number;
            Assert.IsNotNull(numberLeft1);
            Assert.AreEqual(22, numberLeft1.Value);

            Number numberRight1 = rightExpression1 as Number;
            Assert.IsNotNull(numberRight1);
            Assert.AreEqual(0.5, numberRight1.Value);

            NumericCompareExpression expression2 = (NumericCompareExpression)goalConditions[2];
            INumericExpression leftExpression2 = replacer.Replace(expression2.LeftArgument);
            INumericExpression rightExpression2 = replacer.Replace(expression2.RightArgument);

            Number numberLeft2 = leftExpression2 as Number;
            Assert.IsNotNull(numberLeft2);
            Assert.AreEqual(-7, numberLeft2.Value);

            UnaryMinus unaryMinusRight2 = rightExpression2 as UnaryMinus;
            Assert.IsNotNull(unaryMinusRight2);

            NumericCompareExpression expression3 = (NumericCompareExpression)goalConditions[3];
            INumericExpression leftExpression3 = replacer.Replace(expression3.LeftArgument);
            INumericExpression rightExpression3 = replacer.Replace(expression3.RightArgument);

            Number numberLeft3 = leftExpression3 as Number;
            Assert.IsNotNull(numberLeft3);
            Assert.AreEqual(-19, numberLeft3.Value);

            Number numberRight3 = rightExpression3 as Number;
            Assert.IsNotNull(numberRight3);
            Assert.AreEqual(24, numberRight3.Value);

            NumericCompareExpression expression4 = (NumericCompareExpression)goalConditions[4];
            INumericExpression leftExpression4 = replacer.Replace(expression4.LeftArgument);
            INumericExpression rightExpression4 = replacer.Replace(expression4.RightArgument);

            Minus minusLeft4 = leftExpression4 as Minus;
            Assert.IsNotNull(minusLeft4);
            Assert.IsTrue(minusLeft4.LeftChild is NumericFunction);
            Assert.IsTrue(minusLeft4.RightChild is Number);

            Plus plusRight4 = rightExpression4 as Plus;
            Assert.IsNotNull(plusRight4);
            Assert.IsTrue(plusRight4.Children[0] is NumericFunction);
            Assert.IsTrue(plusRight4.Children[1] is Number);

            NumericCompareExpression expression5 = (NumericCompareExpression)goalConditions[5];
            INumericExpression leftExpression5 = replacer.Replace(expression5.LeftArgument);
            INumericExpression rightExpression5 = replacer.Replace(expression5.RightArgument);

            Divide divideLeft5 = leftExpression5 as Divide;
            Assert.IsNotNull(divideLeft5);
            Assert.IsTrue(divideLeft5.LeftChild is NumericFunction);
            Assert.IsTrue(divideLeft5.RightChild is Number);

            Multiply multiplyRight6 = rightExpression5 as Multiply;
            Assert.IsNotNull(multiplyRight6);
            Assert.IsTrue(multiplyRight6.Children[0] is NumericFunction);
            Assert.IsTrue(multiplyRight6.Children[1] is Number);
        }

        [TestMethod]
        public void TC_NumericExpressionsBuilder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericExpressionsBuilder.pddl"), GetFilePath("Dummy_P.pddl"));

            NumericExpressionsBuilder builder = new NumericExpressionsBuilder(new IdManager(data));

            var equalsExpr = (InputData.PDDL.NumericCompareExpression)data.Domain.Actions[0].Preconditions[0];
            var numericExpr = builder.Build(equalsExpr.NumericExpression1);

            Plus plusExpr = numericExpr as Plus;
            Assert.IsNotNull(plusExpr);
            Assert.AreEqual(6, plusExpr.Children.Count);

            Assert.IsTrue(plusExpr.Children[0] is UnaryMinus);
            Assert.IsTrue(plusExpr.Children[1] is Plus);
            Assert.IsTrue(plusExpr.Children[2] is Multiply);
            Assert.IsTrue(plusExpr.Children[3] is Divide);
            Assert.IsTrue(plusExpr.Children[4] is Minus);
            Assert.IsTrue(plusExpr.Children[5] is NumericFunction);
        }

        [TestMethod]
        public void TC_NumericExpressionEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericExpressionEvaluator_D.pddl"), GetFilePath("TC_NumericExpressionEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            ExpressionEvaluator evaluator = new ExpressionEvaluator(new GroundingManager(data, new IdManager(data)), problem.RigidRelations);

            foreach (IExpression expression in problem.Operators[0].Preconditions)
            {
                Assert.IsTrue(evaluator.Evaluate(expression, new Substitution(), problem.InitialState));
            }
        }

        [TestMethod]
        public void TC_NumericExpressionsGrounder()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericExpressionsGrounder.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            NumericExpressionsGrounder numericExpressionsGrounder = new NumericExpressionsGrounder(new Lazy<TermsGrounder>(() => new TermsGrounder(idManager)), idManager);

            ISubstitution substitution = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB", "constA");

            NumericCompareExpression compareExpr = problem.Operators[0].Preconditions[0] as NumericCompareExpression;
            Assert.IsNotNull(compareExpr);

            INumericExpression groundedExpr1 = numericExpressionsGrounder.Ground(compareExpr.LeftArgument, substitution);
            NumericFunction groundedNumFunc1 = groundedExpr1 as NumericFunction;
            Assert.IsNotNull(groundedNumFunc1);
            ConstantTerm constTerm1 = groundedNumFunc1.FunctionAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(constTerm1);
            Assert.AreEqual(factory.CreateConstant("constB"), constTerm1.NameId);

            INumericExpression groundedExpr2 = numericExpressionsGrounder.Ground(compareExpr.RightArgument, substitution);
            Plus groundedPlus2 = groundedExpr2 as Plus;
            Assert.IsNotNull(groundedPlus2);
            NumericFunction groundedNumFunc2 = groundedPlus2.Children[0] as NumericFunction;
            Assert.IsNotNull(groundedNumFunc2);
            ConstantTerm constTerm2 = groundedNumFunc2.FunctionAtom.GetTerms()[0] as ConstantTerm;
            Assert.IsNotNull(constTerm2);
            Assert.AreEqual(factory.CreateConstant("constA"), constTerm2.NameId);
        }

        [TestMethod]
        public void TC_NumericFunctionsCollector()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_NumericFunctionsCollector.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            NumericFunctionsCollector collector = new NumericFunctionsCollector();

            NumericCompareExpression expression0 = problem.Operators[0].Preconditions[0] as NumericCompareExpression;
            Assert.IsNotNull(expression0);
            idManager.Variables.Register("?a");

            var functions = collector.Collect(expression0.LeftArgument);
            Assert.AreEqual(3, functions.Count);
            Assert.IsTrue(functions.Contains(new NumericFunction(factory.CreateFunction("numFuncA", "constA", "?a"), idManager)));
            Assert.IsTrue(functions.Contains(new NumericFunction(factory.CreateFunction("numFuncB"), idManager)));
            Assert.IsTrue(functions.Contains(new NumericFunction(factory.CreateFunction("numFuncC", "constB"), idManager)));
        }

        [TestMethod]
        public void TC_Operator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Operator_D.pddl"), GetFilePath("TC_Operator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var predAConstA = factory.CreatePredicate("predA", "constA");
            var predAConstB = factory.CreatePredicate("predA", "constB");
            var predB = factory.CreatePredicate("predB");
            var predC = factory.CreatePredicate("predC");
            var predDConstA = factory.CreatePredicate("predD", "constA");
            var predDConstB = factory.CreatePredicate("predD", "constB");
            var predEConstA = factory.CreatePredicate("predE", "constA");
            var numFunc = factory.CreateFunction("numFunc");
            var objFunc = factory.CreateFunction("objFunc");
            var constA = factory.CreateConstant("constA");

            var operatorA = new Operator(problem.Operators[0], factory.CreateSubstitution(problem.Operators[0].Parameters, "constA"));
            var operatorB = new Operator(problem.Operators[0], factory.CreateSubstitution(problem.Operators[0].Parameters, "constB"));
            var operator1 = new Operator(problem.Operators[1], new Substitution());
            var state = problem.InitialState;
            var goalConditions = (Conditions)problem.GoalConditions;
            var goalRelativeState = (IRelativeState)goalConditions.GetCorrespondingRelativeStates(problem).First();

            Assert.IsNotNull(operatorA.GetLiftedOperator());
            Assert.IsNotNull(operatorA.GetSubstitution());

            Assert.AreEqual("actionName0(constA)", operatorA.GetName());
            Assert.AreEqual("actionName0(constB)", operatorB.GetName());

            Assert.IsTrue(operatorA.IsApplicable(state));
            Assert.IsFalse(operatorB.IsApplicable(state));

            var newState = (IState)operatorA.Apply(state);
            Assert.IsTrue(newState.HasPredicate(predAConstA));
            Assert.IsTrue(!newState.HasPredicate(predB));
            Assert.IsTrue(newState.HasPredicate(predC));
            Assert.IsTrue(newState.HasPredicate(predEConstA));
            Assert.IsTrue(newState.HasPredicate(predDConstA));
            Assert.IsTrue(newState.HasPredicate(predDConstB));
            Assert.AreEqual(9, newState.GetNumericFunctionValue(numFunc));
            Assert.AreEqual(constA, newState.GetObjectFunctionValue(objFunc));

            Assert.IsTrue(operatorA.IsRelevant(goalConditions));
            Assert.IsFalse(operatorB.IsRelevant(goalConditions));

            Assert.IsTrue(operatorA.IsRelevant(goalRelativeState));
            Assert.IsFalse(operatorB.IsRelevant(goalRelativeState));

            var newConditions = (ConditionsCNF)operatorA.ApplyBackwards(goalConditions);
            Assert.AreEqual(3, newConditions.Count);
            Assert.IsTrue(newConditions.Contains(new PredicateLiteralCNF(new PredicateExpression(predAConstB, idManager), true)));
            Assert.IsTrue(newConditions.Contains(new PredicateLiteralCNF(new PredicateExpression(predC, idManager), false)));
            Assert.IsTrue(newConditions.Contains(new PredicateLiteralCNF(new PredicateExpression(predEConstA, idManager), false)));

            var newRelativeStates = operatorA.ApplyBackwards(goalRelativeState).ToList();
            Assert.AreEqual(1, newRelativeStates.Count);
            var newRelativeState = (IRelativeState)newRelativeStates.First();
            Assert.IsTrue(newRelativeState.HasNegatedPredicate(predAConstB));
            Assert.IsTrue(newRelativeState.HasPredicate(predC));
            Assert.IsTrue(newRelativeState.HasPredicate(predEConstA));
            Assert.IsFalse(newRelativeState.HasPredicate(predAConstA));
            Assert.IsFalse(newRelativeState.HasNegatedPredicate(predB));

            Assert.AreEqual(LiftedOperator.DefaultOperatorCost, operatorA.GetCost());
            Assert.AreEqual(4, operator1.GetCost());

            StateLabels stateLabels = new StateLabels {{predAConstA, 5}, {predB, 3}, {predC, 7}};
            Assert.AreEqual(15, operator1.ComputePlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));
            Assert.AreEqual(7, operator1.ComputePlanningGraphLabel(stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));

            var effectivePreconditions = operatorA.GetEffectivePreconditions(state);
            Assert.AreEqual(1, effectivePreconditions.Count);
            Assert.IsTrue(effectivePreconditions.Contains(predEConstA));

            var effectiveEffects = operatorA.GetEffectiveEffects();
            Assert.AreEqual(3, effectiveEffects.Count);
            Assert.IsTrue(effectiveEffects.Contains(predAConstA));
            Assert.IsTrue(effectiveEffects.Contains(predDConstA));
            Assert.IsTrue(effectiveEffects.Contains(predDConstB));

            var operatorAClone = operatorA.Clone();
            Assert.IsTrue(operatorA != operatorAClone);
            Assert.IsTrue(operatorA.GetHashCode() == operatorAClone.GetHashCode());
            Assert.IsTrue(operatorA.Equals(operatorAClone));
        }

        [TestMethod]
        public void TC_OperatorApply()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_OperatorApply_D.pddl"), GetFilePath("TC_OperatorApply_P.pddl"));
            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var substitution = new Substitution();
            var state = problem.InitialState;
            var operators = problem.Operators;
            Assert.AreEqual(12, operators.Count);

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");
            var pred0 = factory.CreatePredicate("pred0");
            var pred1ConstA = factory.CreatePredicate("pred1", "constA");
            var pred1ConstB = factory.CreatePredicate("pred1", "constB");
            var numericFunc = factory.CreateFunction("numericFunc");
            var objectFunc = factory.CreateFunction("objectFunc");

            Assert.IsTrue(state.HasPredicate(pred0));
            Assert.IsTrue(state.HasPredicate(pred1ConstA));
            Assert.IsTrue(!state.HasPredicate(pred1ConstB));
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));
            Assert.AreEqual(constA, state.GetObjectFunctionValue(objectFunc));

            state = operators[0].Apply(state, substitution);
            Assert.IsTrue(state.HasPredicate(pred1ConstB));

            state = operators[1].Apply(state, substitution);
            Assert.IsTrue(!state.HasPredicate(pred0));

            state = operators[2].Apply(state, substitution);
            Assert.IsTrue(!state.HasPredicate(pred1ConstA));
            Assert.IsTrue(!state.HasPredicate(pred1ConstB));

            state = operators[3].Apply(state, substitution);
            Assert.AreEqual(5, state.GetNumericFunctionValue(numericFunc));

            state = operators[4].Apply(state, substitution);
            Assert.AreEqual(12, state.GetNumericFunctionValue(numericFunc));

            state = operators[5].Apply(state, substitution);
            Assert.AreEqual(10, state.GetNumericFunctionValue(numericFunc));

            state = operators[6].Apply(state, substitution);
            Assert.AreEqual(30, state.GetNumericFunctionValue(numericFunc));

            state = operators[7].Apply(state, substitution);
            Assert.AreEqual(15, state.GetNumericFunctionValue(numericFunc));

            state = operators[8].Apply(state, substitution);
            Assert.AreEqual(constB, state.GetObjectFunctionValue(objectFunc));

            state = operators[9].Apply(state, substitution);
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));

            state = operators[10].Apply(state, substitution);
            Assert.AreEqual(0, state.GetNumericFunctionValue(numericFunc));

            var substitutionConstA = factory.CreateSubstitution(operators[11].Parameters, "constA");
            state = operators[11].Apply(state, substitutionConstA);
            Assert.IsTrue(state.HasPredicate(pred1ConstA));
            Assert.IsTrue(!state.HasPredicate(pred1ConstB));

            var substitutionConstB = factory.CreateSubstitution(operators[11].Parameters, "constB");
            state = operators[11].Apply(state, substitutionConstB);
            Assert.IsTrue(state.HasPredicate(pred1ConstA));
            Assert.IsTrue(state.HasPredicate(pred1ConstB));
        }

        [TestMethod]
        public void TC_OperatorApplyBackwards()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_OperatorApplyBackwards_D.pddl"), GetFilePath("TC_OperatorApplyBackwards_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            Conditions goalConditions = (Conditions)problem.GoalConditions;

            Func<string, int> getConst = constName => factory.CreateConstant(constName);
            Func<string, PredicateLiteralCNF> getPredicate = predicateName => new PredicateLiteralCNF(factory.CreatePredicate(predicateName), false, idManager);
            Func<string, PredicateLiteralCNF> getNegPredicate = predicateName => new PredicateLiteralCNF(factory.CreatePredicate(predicateName), true, idManager);
            Func<string, ObjectFunctionTerm> getObjFuncTerm = funcName => new ObjectFunctionTerm(factory.CreateFunction(funcName), idManager);
            Func<string, ConstantTerm> getConstTerm = constName => new ConstantTerm(getConst(constName), idManager);
            Func<string, NumericFunction> getNumFunc = funcName => new NumericFunction(factory.CreateFunction(funcName), idManager);
            Func<string, string, string, PredicateLiteralCNF> getPredicate2Args = (predicateName, arg1, arg2)
                => new PredicateLiteralCNF(factory.CreatePredicate(predicateName, arg1, arg2), false, idManager);

            IConditions resultConditions0 = problem.Operators[0].ApplyBackwards(goalConditions, new Substitution());
            ConditionsCNF resultConditionsCnf0 = (ConditionsCNF)resultConditions0;
            Assert.IsFalse(resultConditionsCnf0.Contains(getPredicate("predA")));
            Assert.IsFalse(resultConditionsCnf0.Contains(getNegPredicate("predB")));
            Assert.IsFalse(resultConditionsCnf0.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predC"), getPredicate("predD") })));
            Assert.IsTrue(resultConditionsCnf0.Contains(getPredicate("predE")));
            Assert.IsTrue(resultConditionsCnf0.Contains(getPredicate("predZ")));

            ISubstitution operatorSubstitution1 = factory.CreateSubstitution(problem.Operators[1].Parameters, "constA");
            IConditions resultConditions1 = problem.Operators[1].ApplyBackwards(goalConditions, operatorSubstitution1);
            ConditionsCNF resultConditionsCnf1 = (ConditionsCNF)resultConditions1;
            Assert.IsFalse(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getObjFuncTerm("objFunc1"), getConstTerm("constA"), false)));
            Assert.IsFalse(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getObjFuncTerm("objFunc3"), getObjFuncTerm("objFunc2"), false)));
            Assert.IsFalse(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getObjFuncTerm("objFunc4"), getConstTerm("constA"), false)));
            Assert.IsFalse(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getObjFuncTerm("objFunc5"), getObjFuncTerm("objFunc6"), false)));
            Assert.IsFalse(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getObjFuncTerm("objFunc5"), getObjFuncTerm("objFunc6"), true)));
            Assert.IsTrue(resultConditionsCnf1.Contains(new EqualsLiteralCNF(getConstTerm("constB"), getObjFuncTerm("objFunc6"), false)));

            IConditions resultConditions2 = problem.Operators[2].ApplyBackwards(goalConditions, new Substitution());
            ConditionsCNF resultConditionsCnf2 = (ConditionsCNF)resultConditions2;
            Assert.IsFalse(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.LTE, new Plus(getNumFunc("numFunc1"), new Number(5)), new Number(8), false)));
            Assert.IsFalse(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.GT, new Plus(getNumFunc("numFunc1"), new Number(5)), new Number(8), true)));
            Assert.IsFalse(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.GTE, getNumFunc("numFunc1"), getNumFunc("numFunc2"), false)));
            Assert.IsFalse(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.LT, getNumFunc("numFunc1"), getNumFunc("numFunc2"), true)));
            Assert.IsFalse(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.EQ, getNumFunc("numFunc3"), new Number(0), false)));
            Assert.IsTrue(resultConditionsCnf2.Contains(new NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator.EQ, new Minus(getNumFunc("numFunc3"), new Number(10)), new Number(0), false)));

            ISubstitution substitution3 = factory.CreateSubstitution(problem.Operators[3].Parameters, "constA");
            IConditions resultConditions3 = problem.Operators[3].ApplyBackwards(goalConditions, substitution3);
            ConditionsCNF resultConditionsCnf3 = (ConditionsCNF)resultConditions3;
            Assert.IsFalse(resultConditionsCnf3.Contains(getPredicate2Args("predF", "constA", "constA")));
            Assert.IsFalse(resultConditionsCnf3.Contains(getPredicate2Args("predF", "constA", "constB")));
            Assert.IsTrue(resultConditionsCnf3.Contains(getPredicate2Args("predF", "constB", "constA")));
            Assert.IsTrue(resultConditionsCnf3.Contains(getPredicate2Args("predF", "constB", "constB")));

            IConditions resultConditions4 = problem.Operators[4].ApplyBackwards(goalConditions, substitution3);
            ConditionsCNF resultConditionsCnf4 = (ConditionsCNF)resultConditions4;
            Assert.IsFalse(resultConditionsCnf4.Contains(getPredicate("predX")));
            Assert.IsFalse(resultConditionsCnf4.Contains(getPredicate("predY")));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predX"), getPredicate("predY"), getPredicate("pred1") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predX"), getPredicate("pred1") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predY"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predX"), getPredicate("predY"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predX"), getPredicate("pred1"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predX"), getPredicate("predY"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(new ClauseCNF(new HashSet<LiteralCNF> { getPredicate("predY"), getPredicate("pred2") })));
            Assert.IsTrue(resultConditionsCnf4.Contains(getPredicate("predZ")));
        }

        [TestMethod]
        public void TC_OperatorCost()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_OperatorCost_D.pddl"), GetFilePath("TC_OperatorCost_P.pddl"));

            Problem problem = new Problem(data);

            var operators = problem.Operators;
            Assert.AreEqual(2, operators.Count);

            Planner.IOperator operator0 = new Operator(problem.Operators[0], new Substitution());
            Assert.AreEqual(4, operator0.GetCost());

            Planner.IOperator operator1 = new Operator(problem.Operators[1], new Substitution());
            Assert.AreEqual(7, operator1.GetCost());
        }

        [TestMethod]
        public void TC_OperatorIsApplicable()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_OperatorIsApplicable_D.pddl"), GetFilePath("TC_OperatorIsApplicable_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var operators = problem.Operators;
            Assert.AreEqual(3, operators.Count);

            Assert.IsTrue(operators[0].IsApplicable(problem.InitialState, new Substitution()));
            Assert.IsFalse(operators[1].IsApplicable(problem.InitialState, new Substitution()));

            var substitutionConstA = factory.CreateSubstitution(operators[2].Parameters, "constA");
            Assert.IsTrue(operators[2].IsApplicable(problem.InitialState, substitutionConstA));

            var substitutionConstB = factory.CreateSubstitution(operators[2].Parameters, "constB");
            Assert.IsFalse(operators[2].IsApplicable(problem.InitialState, substitutionConstB));
        }

        [TestMethod]
        public void TC_OperatorIsRelevant()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_OperatorIsRelevant_D.pddl"), GetFilePath("TC_OperatorIsRelevant_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var operators = problem.Operators;
            var goalConditions = problem.GoalConditions;
            Assert.AreEqual(17, operators.Count);

            var emptySubstitution = new Substitution();
            Assert.IsTrue(operators[0].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsFalse(operators[1].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[2].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsFalse(operators[3].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[4].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[5].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsFalse(operators[6].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[7].IsRelevant(goalConditions, factory.CreateSubstitution(operators[7].Parameters, "constA")));
            Assert.IsFalse(operators[7].IsRelevant(goalConditions, factory.CreateSubstitution(operators[7].Parameters, "constB")));
            Assert.IsTrue(operators[8].IsRelevant(goalConditions, factory.CreateSubstitution(operators[8].Parameters, "constB")));
            Assert.IsFalse(operators[9].IsRelevant(goalConditions, factory.CreateSubstitution(operators[9].Parameters, "constB")));
            Assert.IsTrue(operators[10].IsRelevant(goalConditions, factory.CreateSubstitution(operators[10].Parameters, "constB")));
            Assert.IsFalse(operators[11].IsRelevant(goalConditions, factory.CreateSubstitution(operators[11].Parameters, "constB")));
            Assert.IsTrue(operators[12].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[13].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsFalse(operators[14].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsTrue(operators[15].IsRelevant(goalConditions, emptySubstitution));
            Assert.IsFalse(operators[16].IsRelevant(goalConditions, emptySubstitution));

            List<int> relevantContionalEffects = new List<int>();
            Assert.IsTrue(operators[13].IsRelevant(goalConditions, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(2, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));
            Assert.IsTrue(relevantContionalEffects.Contains(2));

            relevantContionalEffects.Clear();
            Assert.IsFalse(operators[14].IsRelevant(goalConditions, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);

            relevantContionalEffects.Clear();
            Assert.IsTrue(operators[15].IsRelevant(goalConditions, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(1, relevantContionalEffects.Count);
            Assert.IsTrue(relevantContionalEffects.Contains(0));

            relevantContionalEffects.Clear();
            Assert.IsFalse(operators[16].IsRelevant(goalConditions, emptySubstitution, relevantContionalEffects));
            Assert.AreEqual(0, relevantContionalEffects.Count);
        }

        [TestMethod]
        public void TC_Parameters()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Parameters.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var params0 = problem.Operators[0].Parameters;
            Assert.AreEqual(2, params0.Count);
            Assert.AreEqual(0, params0[0].ParameterNameId);
            Assert.AreEqual(factory.CreateType("typeA"), params0[0].TypeNamesIDs.First());
            Assert.AreEqual(1, params0[1].ParameterNameId);
            Assert.AreEqual(factory.CreateType("typeB"), params0[1].TypeNamesIDs.First());
            Assert.AreEqual(1, params0.GetMaxUsedParameterId());

            var params1 = problem.Operators[1].Parameters;
            Assert.AreEqual(1, params1.Count);
            Assert.AreEqual(0, params1[0].ParameterNameId);
            Assert.IsTrue(params1[0].TypeNamesIDs.Contains(factory.CreateType("typeB")));
            Assert.IsTrue(params1[0].TypeNamesIDs.Contains(factory.CreateType("typeC")));
            Assert.AreEqual(0, params1.GetMaxUsedParameterId());
            Assert.IsTrue(params0.AreConflictedWith(params1));

            params1[0].ParameterNameId = 8;
            Assert.IsFalse(params0.AreConflictedWith(params1));
            Assert.AreEqual(8, params1.GetMaxUsedParameterId());

            var params0Clone = params0.Clone();
            Assert.IsTrue(params0 != params0Clone);
            Assert.IsTrue(params0.GetHashCode() == params0Clone.GetHashCode());
            Assert.IsTrue(params0.Equals(params0Clone));

            params0.Add(params1);
            Assert.AreEqual(3, params0.Count);
            Assert.AreEqual(8, params0.GetMaxUsedParameterId());
            Assert.IsFalse(params0.GetHashCode() == params0Clone.GetHashCode());
            Assert.IsFalse(params0.Equals(params0Clone));
        }

        [TestMethod]
        public void TC_PlanningGraphOperatorLabelEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_PlanningGraphOperatorLabelEvaluator_D.pddl"), GetFilePath("TC_PlanningGraphOperatorLabelEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            PlanningGraphOperatorLabelEvaluator evaluator = new PlanningGraphOperatorLabelEvaluator(groundingManager);

            StateLabels stateLabels = new StateLabels
            {
                {factory.CreatePredicate("predA"), 5},
                {factory.CreatePredicate("predB"), 3},
                {factory.CreatePredicate("predC"), 7},
                {factory.CreatePredicate("predD", "constA"), 8}
            };

            var substit1A = factory.CreateSubstitution(problem.Operators[1].Parameters, "constA");
            var substit1B = factory.CreateSubstitution(problem.Operators[1].Parameters, "constB");

            Assert.AreEqual(7, evaluator.Evaluate(problem.Operators[0].Preconditions, new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(15, evaluator.Evaluate(problem.Operators[0].Preconditions, new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(8, evaluator.Evaluate(problem.Operators[1].Preconditions, substit1A, stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(20, evaluator.Evaluate(problem.Operators[1].Preconditions, substit1A, stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(7, evaluator.Evaluate(problem.Operators[1].Preconditions, substit1B, stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(12, evaluator.Evaluate(problem.Operators[1].Preconditions, substit1B, stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));
        }

        [TestMethod]
        public void TC_PlanningGraphOperatorLabelEvaluatorCNF()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_PlanningGraphOperatorLabelEvaluatorCNF_D.pddl"), GetFilePath("TC_PlanningGraphOperatorLabelEvaluatorCNF_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);

            PlanningGraphOperatorLabelEvaluatorCNF evaluator = new PlanningGraphOperatorLabelEvaluatorCNF(groundingManager);

            StateLabels stateLabels = new StateLabels
            {
                {factory.CreatePredicate("predA"), 5},
                {factory.CreatePredicate("predB"), 3},
                {factory.CreatePredicate("predC"), 7},
                {factory.CreatePredicate("predD", "constA"), 8}
            };

            var substit1A = factory.CreateSubstitution(problem.Operators[1].Parameters, "constA");
            var substit1B = factory.CreateSubstitution(problem.Operators[1].Parameters, "constB");

            Assert.AreEqual(7, evaluator.Evaluate((ConditionsCNF)problem.Operators[0].Preconditions.GetCNF(), new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(15, evaluator.Evaluate((ConditionsCNF)problem.Operators[0].Preconditions.GetCNF(), new Substitution(), stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(8, evaluator.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), substit1A, stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(20, evaluator.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), substit1A, stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));

            Assert.AreEqual(7, evaluator.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), substit1B, stateLabels, Planner.ForwardCostEvaluationStrategy.MAX_VALUE));
            Assert.AreEqual(12, evaluator.Evaluate((ConditionsCNF)problem.Operators[1].Preconditions.GetCNF(), substit1B, stateLabels, Planner.ForwardCostEvaluationStrategy.ADDITIVE_VALUE));
        }

        [TestMethod]
        public void TC_Problem()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Problem_D.pddl"), GetFilePath("TC_Problem_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, idManager), problem.RigidRelations);

            var pred0 = factory.CreatePredicate("pred0");
            var pred1 = factory.CreatePredicate("pred1");
            var pred2 = factory.CreatePredicate("pred2");
            var predRigid = factory.CreatePredicate("predRigid");

            Assert.AreEqual("domainName", problem.DomainName);
            Assert.AreEqual("problemName", problem.ProblemName);
            Assert.AreEqual(2, problem.Operators.Count);
            Assert.AreEqual(factory.CreateState(pred0, pred2), problem.InitialState);
            Assert.AreEqual(new Conditions(new PredicateExpression(pred1, idManager), evaluationManager), problem.GoalConditions);
            Assert.IsTrue(problem.RigidRelations.Contains(predRigid));
            Assert.IsNotNull(problem.IdManager);
            Assert.IsNotNull(problem.EvaluationManager);

            Assert.AreEqual("domainName", problem.GetDomainName());
            Assert.AreEqual("problemName", problem.GetProblemName());
            Assert.AreEqual(GetFilePath("TC_Problem_P.pddl"), problem.GetInputFilePath());
            problem.SetInitialState(factory.CreateState(pred0));
            Assert.AreEqual(factory.CreateState(pred0), problem.GetInitialState());
            var newGoal = new Conditions(new PredicateExpression(pred2, idManager), evaluationManager);
            problem.SetGoalConditions(newGoal);
            Assert.AreEqual(newGoal, problem.GetGoalConditions());

            problem = new Problem(data);

            Assert.IsTrue(problem.IsGoalState(factory.CreateState(pred1, pred2)));
            Assert.IsFalse(problem.IsGoalState(factory.CreateState(pred2)));

            Assert.IsFalse(problem.IsStartConditions(problem.GoalConditions));
            Assert.IsTrue(problem.IsStartConditions(newGoal));

            Assert.IsFalse(problem.IsStartRelativeState(problem.GoalConditions.GetCorrespondingRelativeStates(problem).First()));
            Assert.IsTrue(problem.IsStartRelativeState(newGoal.GetCorrespondingRelativeStates(problem).First()));

            Assert.AreEqual(1, problem.GetNotAccomplishedGoalsCount(problem.InitialState));
            Assert.AreEqual(1, problem.GetNotAccomplishedGoalsCount(problem.GoalConditions));

            Assert.AreEqual(2, problem.GetSuccessors(problem.InitialState).Count());
            Assert.AreEqual(2, problem.GetNextSuccessors(problem.InitialState, 10).Count());
            Assert.IsNotNull(problem.GetRandomSuccessor(problem.InitialState));

            Assert.AreEqual(1, problem.GetPredecessors(problem.GoalConditions).Count());
            Assert.AreEqual(1, problem.GetNextPredecessors(problem.GoalConditions, 10).Count());
            Assert.IsNotNull(problem.GetRandomPredecessor(problem.GoalConditions));
            Assert.AreEqual(1, problem.GetPredecessors(factory.CreateRelativeState(pred1)).Count());
            Assert.AreEqual(1, problem.GetPredecessors(factory.CreateState(pred1)).Count());

            Assert.AreEqual(2, problem.GetSuccessorStates(problem.InitialState).Count());
            Assert.AreEqual(8, problem.GetPredecessorStates(factory.CreateState(pred1)).Count());

            Assert.IsNotNull(problem.GetRelaxedProblem());
        }

        [TestMethod]
        public void TC_RelativeState()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RelativeState.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            RelativeState state = (RelativeState)factory.CreateRelativeState();

            Assert.IsNull(state.Predicates);
            Assert.IsNull(state.NumericFunctions);
            Assert.IsNull(state.ObjectFunctions);

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");
            var constC = factory.CreateConstant("constC");
            var pred1 = factory.CreatePredicate("pred1");
            var pred2ConstA = factory.CreatePredicate("pred2", "constA");
            var pred2ConstB = factory.CreatePredicate("pred2", "constB");
            var pred2ConstC = factory.CreatePredicate("pred2", "constC");
            var numericFuncConstA = factory.CreateFunction("numericFunc", "constA");
            var numericFuncConstB = factory.CreateFunction("numericFunc", "constB");
            var objectFunc = factory.CreateFunction("objectFunc");

            Assert.IsFalse(state.HasNegatedPredicate(pred1));
            state.AddNegatedPredicate(pred1);
            Assert.IsTrue(state.HasNegatedPredicate(pred1));
            state.RemoveNegatedPredicate(pred1);
            Assert.IsFalse(state.HasNegatedPredicate(pred1));

            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstA));
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstB));
            Assert.AreEqual(ObjectFunctionTerm.UndefinedValue, state.GetObjectFunctionValue(objectFunc));

            state.IncreaseNumericFunction(numericFuncConstA, 15.0);
            Assert.AreEqual(15.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.DecreaseNumericFunction(numericFuncConstA, 8.0);
            Assert.AreEqual(7.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.ScaleUpNumericFunction(numericFuncConstA, 3.0);
            Assert.AreEqual(21.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.ScaleDownNumericFunction(numericFuncConstA, 2.0);
            Assert.AreEqual(10.5, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, 66.0);
            Assert.AreEqual(66.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, NumericFunction.UndefinedValue);
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, 66.0);
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstB));

            state.AssignObjectFunction(objectFunc, constA);
            Assert.AreEqual(constA, state.GetObjectFunctionValue(objectFunc));
            state.AssignObjectFunction(objectFunc, ObjectFunctionTerm.UndefinedValue);
            Assert.AreEqual(ObjectFunctionTerm.UndefinedValue, state.GetObjectFunctionValue(objectFunc));
            state.AssignObjectFunction(objectFunc, constA);

            state.AddNegatedPredicate(pred2ConstA);
            state.AddNegatedPredicate(pred1);

            Assert.AreEqual(0, state.GetPredicates().Count());
            Assert.AreEqual(2, state.GetNegatedPredicates().Count());
            Assert.AreEqual(1, state.GetObjectFunctions().Count());
            Assert.AreEqual(1, state.GetNumericFunctions().Count());
            Assert.AreEqual(2, state.GetSize());

            Assert.IsNotNull(state.GetRelaxedState());

            var conditions = (Conditions)state.GetDescribingConditions(problem);
            Assert.IsTrue(conditions.Contains(new NotExpression(new PredicateExpression(pred2ConstA, idManager))));
            Assert.IsTrue(conditions.Contains(new NotExpression(new PredicateExpression(pred1, idManager))));
            Assert.IsTrue(conditions.Contains(new NumericCompareExpression(NumericCompareExpression.RelationalOperator.EQ, new NumericFunction(numericFuncConstA, idManager), new Number(66.0))));
            Assert.IsTrue(conditions.Contains(new EqualsExpression(new ObjectFunctionTerm(objectFunc, idManager), new ConstantTerm(constA))));

            var state2 = factory.CreateRelativeState(new HashSet<int> { 1, 2 }, pred1, pred2ConstA, pred2ConstB); // (pred1), (not (pred2 constA)), (not (pred2 constB))

            Assert.IsTrue(state2.Evaluate(factory.CreateState(pred1, pred2ConstC)));
            Assert.IsFalse(state2.Evaluate(factory.CreateState(pred1, pred2ConstA)));
            Assert.IsFalse(state2.Evaluate(factory.CreateState(pred2ConstC)));

            var correspondingStates = state2.GetCorrespondingStates(problem).ToList();
            Assert.AreEqual(8, correspondingStates.Count);
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constC } })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(correspondingStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constC } })));

            IState stateClone = (IState)state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_RelaxedPlanningGraph()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RelaxedPlanningGraph_D.pddl"), GetFilePath("TC_RelaxedPlanningGraph_P.pddl"));

            Problem problem = new Problem(data);
            RelaxedPlanningGraph planningGraph = new RelaxedPlanningGraph(problem.GetRelaxedProblem());

            Assert.AreEqual(7, planningGraph.ComputeMaxForwardCost(problem.GetInitialState()));
            Assert.AreEqual(7, planningGraph.ComputeMaxForwardCost(problem.GetGoalConditions()));

            Assert.AreEqual(11, planningGraph.ComputeAdditiveForwardCost(problem.GetInitialState()));
            Assert.AreEqual(11, planningGraph.ComputeAdditiveForwardCost(problem.GetGoalConditions()));

            Assert.AreEqual(9, planningGraph.ComputeFFCost(problem.GetInitialState()));
            Assert.AreEqual(9, planningGraph.ComputeFFCost(problem.GetGoalConditions()));
        }

        [TestMethod]
        public void TC_RelaxedProblem()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RelaxedProblem_D.pddl"), GetFilePath("TC_RelaxedProblem_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var relaxedProblem = problem.GetRelaxedProblem();

            var state = (IState)relaxedProblem.GetRandomSuccessor(relaxedProblem.GetInitialState()).GetSuccessorState();
            Assert.IsTrue(state.HasPredicate(factory.CreatePredicate("pred0")));
            Assert.IsTrue(state.HasPredicate(factory.CreatePredicate("pred1")));
            Assert.IsTrue(state.HasPredicate(factory.CreatePredicate("pred2")));
        }

        [TestMethod]
        public void TC_RigidRelations()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RigidRelations_D.pddl"), GetFilePath("TC_RigidRelations_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var pred0 = factory.CreatePredicate("pred0");
            var predA = factory.CreatePredicate("predA");
            var predBConstA = factory.CreatePredicate("predB", "constA");
            var predBConstB = factory.CreatePredicate("predB", "constB");
            var predBConstC = factory.CreatePredicate("predB", "constC");

            Assert.AreEqual(3, problem.RigidRelations.Count);
            Assert.IsTrue(problem.RigidRelations.Contains(predA));
            Assert.IsTrue(problem.RigidRelations.Contains(predBConstA));
            Assert.IsTrue(problem.RigidRelations.Contains(predBConstB));

            Assert.IsFalse(problem.InitialState.HasPredicate(predA));
            Assert.IsFalse(problem.InitialState.HasPredicate(predBConstA));
            Assert.IsFalse(problem.InitialState.HasPredicate(predBConstB));

            Assert.IsFalse(problem.RigidRelations.IsPredicateRigidRelation(pred0));
            Assert.IsTrue(problem.RigidRelations.IsPredicateRigidRelation(predA));
            Assert.IsTrue(problem.RigidRelations.IsPredicateRigidRelation(predBConstA));
            Assert.IsTrue(problem.RigidRelations.IsPredicateRigidRelation(predBConstB));
            Assert.IsTrue(problem.RigidRelations.IsPredicateRigidRelation(predBConstC));
        }

        [TestMethod]
        public void TC_RigidRelationsComplianceEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_RigidRelationsComplianceEvaluator_D.pddl"), GetFilePath("TC_RigidRelationsComplianceEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            RigidRelationsComplianceEvaluator evaluator = new RigidRelationsComplianceEvaluator(new GroundingManager(data, idManager), problem.RigidRelations);

            ISubstitution substitutionA = factory.CreateSubstitution(problem.Operators[0].Parameters, "constA");
            ISubstitution substitutionB = factory.CreateSubstitution(problem.Operators[0].Parameters, "constB");

            foreach (IExpression expression in problem.Operators[0].Preconditions)
            {
                Assert.IsTrue(evaluator.Evaluate(expression, substitutionA));
            }

            foreach (IExpression expression in problem.Operators[0].Preconditions)
            {
                Assert.IsFalse(evaluator.Evaluate(expression, substitutionB));
            }
        }

        [TestMethod]
        public void TC_SatisfyingAtomsEvaluator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_SatisfyingAtomsEvaluator_D.pddl"), GetFilePath("TC_SatisfyingAtomsEvaluator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, idManager), problem.RigidRelations);

            var conditions = problem.Operators[0].Preconditions;
            var parameters = problem.Operators[0].Parameters;

            var refState = factory.CreateState(factory.CreatePredicate("predC"), factory.CreatePredicate("predA", "constA"), factory.CreatePredicate("predB", "constB"));

            ISubstitution substitutionA = factory.CreateSubstitution(parameters, "constA");
            var satisfyingAtomsA = evaluationManager.GetSatisfyingAtoms(conditions, substitutionA, refState);
            Assert.AreEqual(2, satisfyingAtomsA.Count);
            Assert.IsTrue(satisfyingAtomsA.Contains(factory.CreatePredicate("predA", "constA")));
            Assert.IsTrue(satisfyingAtomsA.Contains(factory.CreatePredicate("predC")));

            ISubstitution substitutionB = factory.CreateSubstitution(parameters, "constB");
            var satisfyingAtomsB = evaluationManager.GetSatisfyingAtoms(conditions, substitutionB, refState);
            Assert.AreEqual(2, satisfyingAtomsB.Count);
            Assert.IsTrue(satisfyingAtomsB.Contains(factory.CreatePredicate("predB", "constB")));
            Assert.IsTrue(satisfyingAtomsB.Contains(factory.CreatePredicate("predC")));
        }

        [TestMethod]
        public void TC_State()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_State.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            State state = (State)problem.InitialState;

            Assert.IsNull(state.Predicates);
            Assert.IsNull(state.NumericFunctions);
            Assert.IsNull(state.ObjectFunctions);

            var constA = factory.CreateConstant("constA");
            var pred1 = factory.CreatePredicate("pred1");
            var pred2ConstA = factory.CreatePredicate("pred2", "constA");
            var numericFuncConstA = factory.CreateFunction("numericFunc", "constA");
            var numericFuncConstB = factory.CreateFunction("numericFunc", "constB");
            var objectFunc = factory.CreateFunction("objectFunc");

            Assert.IsFalse(state.HasPredicate(pred1));
            state.AddPredicate(pred1);
            Assert.IsTrue(state.HasPredicate(pred1));
            state.RemovePredicate(pred1);
            Assert.IsFalse(state.HasPredicate(pred1));

            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstA));
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstB));
            Assert.AreEqual(ObjectFunctionTerm.UndefinedValue, state.GetObjectFunctionValue(objectFunc));

            state.IncreaseNumericFunction(numericFuncConstA, 15.0);
            Assert.AreEqual(15.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.DecreaseNumericFunction(numericFuncConstA, 8.0);
            Assert.AreEqual(7.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.ScaleUpNumericFunction(numericFuncConstA, 3.0);
            Assert.AreEqual(21.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.ScaleDownNumericFunction(numericFuncConstA, 2.0);
            Assert.AreEqual(10.5, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, 66.0);
            Assert.AreEqual(66.0, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, NumericFunction.UndefinedValue);
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstA));
            state.AssignNumericFunction(numericFuncConstA, 66.0);
            Assert.AreEqual(NumericFunction.UndefinedValue, state.GetNumericFunctionValue(numericFuncConstB));

            state.AssignObjectFunction(objectFunc, constA);
            Assert.AreEqual(constA, state.GetObjectFunctionValue(objectFunc));
            state.AssignObjectFunction(objectFunc, ObjectFunctionTerm.UndefinedValue);
            Assert.AreEqual(ObjectFunctionTerm.UndefinedValue, state.GetObjectFunctionValue(objectFunc));
            state.AssignObjectFunction(objectFunc, constA);

            state.AddPredicate(pred2ConstA);
            state.AddPredicate(pred1);

            Assert.AreEqual(2, state.GetPredicates().Count());
            Assert.AreEqual(1, state.GetObjectFunctions().Count());
            Assert.AreEqual(1, state.GetNumericFunctions().Count());
            Assert.AreEqual(4, state.GetSize());

            Assert.IsNotNull(state.GetRelaxedState());

            var conditions = (Conditions)state.GetDescribingConditions(problem);
            Assert.IsTrue(conditions.Contains(new PredicateExpression(pred2ConstA, idManager)));
            Assert.IsTrue(conditions.Contains(new PredicateExpression(pred1, idManager)));
            Assert.IsTrue(conditions.Contains(new NumericCompareExpression(NumericCompareExpression.RelationalOperator.EQ,
                                                                           new NumericFunction(numericFuncConstA, idManager),
                                                                           new Number(66.0))));
            Assert.IsTrue(conditions.Contains(new EqualsExpression(new ObjectFunctionTerm(objectFunc, idManager), new ConstantTerm(constA))));

            IState stateClone = (IState)state.Clone();
            Assert.IsTrue(state != stateClone);
            Assert.IsTrue(state.GetHashCode() == stateClone.GetHashCode());
            Assert.IsTrue(state.Equals(stateClone));
        }

        [TestMethod]
        public void TC_StatesEnumerator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_StatesEnumerator.pddl"), GetFilePath("Dummy_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            EvaluationManager evaluationManager = new EvaluationManager(new GroundingManager(data, idManager));

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");
            var constC = factory.CreateConstant("constC");
            var pred1 = factory.CreatePredicate("pred1");
            var pred2ConstA = factory.CreatePredicate("pred2", "constA");
            var pred2ConstB = factory.CreatePredicate("pred2", "constB");
            var pred2ConstC = factory.CreatePredicate("pred2", "constC");
            var objectFunc = factory.CreateFunction("objectFunc");

            var relativeState = factory.CreateRelativeState(new HashSet<int> { 1, 2 }, pred1, pred2ConstA, pred2ConstB); // (pred1), (not (pred2 constA)), (not (pred2 constB))
            var relativeStateStates = StatesEnumerator.EnumerateStates(relativeState, problem).ToList();
            Assert.AreEqual(8, relativeStateStates.Count);
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constC } })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(relativeStateStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constC } })));

            var conditions = new Conditions(evaluationManager)
            {
                new PredicateExpression(pred1, idManager),
                new NotExpression(new PredicateExpression(pred2ConstA, idManager)),
                new NotExpression(new PredicateExpression(pred2ConstB, idManager))
            };

            var conditionsStates = StatesEnumerator.EnumerateStates(conditions, problem).ToList();
            Assert.AreEqual(8, conditionsStates.Count);
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1 }, new Dictionary<IAtom, int> { { objectFunc, constC } })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constA } })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constB } })));
            Assert.IsTrue(conditionsStates.Contains(factory.CreateState(new HashSet<IAtom> { pred1, pred2ConstC }, new Dictionary<IAtom, int> { { objectFunc, constC } })));

            var conditionsRelativeStates = StatesEnumerator.EnumerateRelativeStates(conditions, problem).ToList();
            Assert.AreEqual(1, conditionsRelativeStates.Count);
            Assert.IsTrue(conditionsRelativeStates.Contains(relativeState));

            var conditions2 = new Conditions(evaluationManager)
            {
                new PredicateExpression(pred1, idManager),
                new OrExpression(new List<IExpression>
                {
                    new PredicateExpression(pred2ConstA, idManager),
                    new PredicateExpression(pred2ConstB, idManager)
                })
            };

            var conditions2RelativeStates = StatesEnumerator.EnumerateRelativeStates(conditions2, problem).ToList();
            Assert.AreEqual(2, conditions2RelativeStates.Count);
            Assert.IsTrue(conditions2RelativeStates.Contains(factory.CreateRelativeState(pred1, pred2ConstA)));
            Assert.IsTrue(conditions2RelativeStates.Contains(factory.CreateRelativeState(pred1, pred2ConstB)));
        }

        [TestMethod]
        public void TC_Substitution()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_Substitution.pddl"), GetFilePath("Dummy_P.pddl"));
            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);

            var constA = factory.CreateConstant("constA");
            var constB = factory.CreateConstant("constB");

            var operatorParams = problem.Operators[0].Parameters;
            var param0 = operatorParams[0].ParameterNameId;
            var param1 = operatorParams[1].ParameterNameId;
            var param2 = operatorParams[2].ParameterNameId;
            const int invalidParam = 333;

            ISubstitution substitution = factory.CreateSubstitution(operatorParams, "constA", "constB", "constA");
            ISubstitution emptySubstitution = new Substitution();

            Assert.IsTrue(substitution.Contains(param2));
            Assert.IsFalse(substitution.Contains(invalidParam));

            Assert.AreEqual(constA, substitution.GetValue(param0));
            Assert.AreEqual(constB, substitution.GetValue(param1));
            Assert.AreEqual(constA, substitution.GetValue(param2));

            int result;
            Assert.IsTrue(substitution.TryGetValue(param0, out result));
            Assert.AreEqual(constA, result);
            Assert.IsFalse(substitution.TryGetValue(invalidParam, out result));

            ForallExpression forallExpr = (ForallExpression)problem.Operators[0].Preconditions[0];
            ISubstitution localSubstitution = factory.CreateSubstitution(forallExpr.Parameters, "constC");
            var localParam = forallExpr.Parameters[0].ParameterNameId;

            Assert.IsFalse(substitution.TryGetValue(localParam, out result));
            substitution.AddLocalSubstitution(localSubstitution);
            Assert.IsTrue(substitution.TryGetValue(localParam, out result));
            substitution.RemoveLocalSubstitution(localSubstitution);
            Assert.IsFalse(substitution.TryGetValue(localParam, out result));

            Assert.IsFalse(substitution.IsEmpty());
            Assert.IsTrue(emptySubstitution.IsEmpty());
            emptySubstitution.Add(0, 5);
            Assert.IsFalse(emptySubstitution.IsEmpty());
            Assert.IsTrue(emptySubstitution.Contains(0));

            ISubstitution substitutionClone = substitution.Clone();
            Assert.IsTrue(substitution != substitutionClone);
            Assert.IsTrue(substitution.GetHashCode() == substitutionClone.GetHashCode());
            Assert.IsTrue(substitution.Equals(substitutionClone));
        }

        [TestMethod]
        public void TC_SubstitutionGenerator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_SubstitutionGenerator.pddl"), GetFilePath("Dummy_P.pddl"));

            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            SubstitutionGenerator substitutionGenerator = new SubstitutionGenerator(new Lazy<ConstantsManager>(() => new ConstantsManager(data, idManager)));

            Func<InputData.PDDL.Parameters, Parameters> processParameters = inputParameters =>
            {
                idManager.Variables.RegisterLocalParameters(inputParameters);
                Parameters parameters = new Parameters(inputParameters, idManager);
                idManager.Variables.UnregisterLocalParameters(inputParameters);
                return parameters;
            };

            Parameters parameters0 = processParameters(data.Domain.Actions[0].Parameters);
            Parameters parameters1 = processParameters(data.Domain.Actions[1].Parameters);
            Parameters parameters2 = processParameters(data.Domain.Actions[2].Parameters);
            Parameters parameters3 = processParameters(data.Domain.Actions[3].Parameters);
            Parameters parameters4 = processParameters(data.Domain.Actions[4].Parameters);
            Parameters parameters5 = processParameters(data.Domain.Actions[5].Parameters);

            var substitutions0 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters0).ToList();
            var substitutions1 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters1).ToList();
            var substitutions2 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters2).ToList();
            var substitutions3 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters3).ToList();
            var substitutions4 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters4).ToList();
            var substitutions5 = substitutionGenerator.GenerateAllLocalSubstitutions(parameters5).ToList();

            var constAb = factory.CreateConstant("constAB");
            var constC = factory.CreateConstant("constC");
            var constD = factory.CreateConstant("constD");
            var constF = factory.CreateConstant("constF");
            var constG = factory.CreateConstant("constG");

            Assert.AreEqual(2, substitutions0.Count);
            Assert.IsTrue(substitutions0.Contains(factory.CreateSubstitution(parameters0, "constAB")));
            Assert.IsTrue(substitutions0.Contains(factory.CreateSubstitution(parameters0, "constD")));

            Assert.AreEqual(4, substitutions1.Count);
            Assert.IsTrue(substitutions1.Contains(factory.CreateSubstitution(parameters1, "constAB")));
            Assert.IsTrue(substitutions1.Contains(factory.CreateSubstitution(parameters1, "constD")));

            Assert.AreEqual(1, substitutions2.Count);
            Assert.IsTrue(substitutions2.Contains(factory.CreateSubstitution(parameters2, "constD", "constD", "constD", "constD")));

            Assert.AreEqual(5, substitutions3.Count);
            Assert.IsTrue(substitutions3.Contains(factory.CreateSubstitution(parameters3, "constAB")));
            Assert.IsTrue(substitutions3.Contains(factory.CreateSubstitution(parameters3, "constC")));
            Assert.IsTrue(substitutions3.Contains(factory.CreateSubstitution(parameters3, "constD")));
            Assert.IsTrue(substitutions3.Contains(factory.CreateSubstitution(parameters3, "constF")));
            Assert.IsTrue(substitutions3.Contains(factory.CreateSubstitution(parameters3, "constG")));

            Assert.AreEqual(125, substitutions4.Count);
            int varIdx0 = parameters4[0].ParameterNameId;
            int varIdx1 = parameters4[1].ParameterNameId;
            int varIdx2 = parameters4[2].ParameterNameId;
            HashSet<int> validValuesTypeObject = new HashSet<int> { constAb, constC, constD, constF, constG };
            foreach (var substitution in substitutions4)
            {
                Assert.IsTrue(validValuesTypeObject.Contains(substitution.GetValue(varIdx0)));
                Assert.IsTrue(validValuesTypeObject.Contains(substitution.GetValue(varIdx1)));
                Assert.IsTrue(validValuesTypeObject.Contains(substitution.GetValue(varIdx2)));
            }

            Assert.AreEqual(8, substitutions5.Count);
            int varIdx50 = parameters5[0].ParameterNameId;
            int varIdx51 = parameters5[1].ParameterNameId;
            HashSet<int> validValuesTypeF = new HashSet<int> { constAb, constC, constD, constF };
            HashSet<int> validValuesTypeB = new HashSet<int> { constAb, constD };
            foreach (var substitution in substitutions5)
            {
                Assert.IsTrue(validValuesTypeF.Contains(substitution.GetValue(varIdx50)));
                Assert.IsTrue(validValuesTypeB.Contains(substitution.GetValue(varIdx51)));
            }
        }

        [TestMethod]
        public void TC_TransitionsGenerator()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_TransitionsGenerator_D.pddl"), GetFilePath("TC_TransitionsGenerator_P.pddl"));

            Problem problem = new Problem(data);
            IdManager idManager = new IdManager(data);
            PrimitivesFactory factory = new PrimitivesFactory(idManager);
            GroundingManager groundingManager = new GroundingManager(data, idManager);
            EvaluationManager evaluationManager = new EvaluationManager(groundingManager);

            TransitionsGenerator transitionsGenerator = new TransitionsGenerator(problem);

            var predA = factory.CreatePredicate("predA");
            var predB = factory.CreatePredicate("predB");
            var predC = factory.CreatePredicate("predC");
            var predD = factory.CreatePredicate("predD");
            var pred1ConstA = factory.CreatePredicate("pred1", "constA");
            var pred1ConstB = factory.CreatePredicate("pred1", "constB");
            var pred2ConstAConstA = factory.CreatePredicate("pred2", "constA", "constA");
            var pred2ConstAConstB = factory.CreatePredicate("pred2", "constA", "constB");
            var pred2ConstBConstA = factory.CreatePredicate("pred2", "constB", "constA");
            var pred2ConstBConstB = factory.CreatePredicate("pred2", "constB", "constB");
            var statePredC = factory.CreateState(predC);
            var conditionsPredD = new Conditions(new PredicateExpression(predD, idManager), evaluationManager);

            var pred1ConstAc = new PredicateLiteralCNF(pred1ConstA, false, idManager);
            var pred2ConstBAc = new PredicateLiteralCNF(pred2ConstBConstA, false, idManager);
            var predAc = new PredicateLiteralCNF(predA, false, idManager);
            var predBc = new PredicateLiteralCNF(predB, false, idManager);
            var predCc = new PredicateLiteralCNF(predC, false, idManager);
            var predDc = new PredicateLiteralCNF(predD, false, idManager);

            HashSet<IState> successorStates = new HashSet<IState>();
            foreach (var successor in transitionsGenerator.GetSuccessors(problem.InitialState))
            {
                successorStates.Add((IState)successor.GetSuccessorState());
            }
            Assert.AreEqual(6, successorStates.Count);
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predB, pred1ConstA)));
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predB, pred1ConstB)));
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predA, pred2ConstAConstA)));
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predA, pred2ConstAConstB)));
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predA, pred2ConstBConstA)));
            Assert.IsTrue(successorStates.Contains(factory.CreateState(predA, pred2ConstBConstB)));

            HashSet<IConditions> predecessorConditions = new HashSet<IConditions>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors(problem.GoalConditions))
            {
                predecessorConditions.Add((IConditions)predecessor.GetPredecessorConditions());
            }
            Assert.AreEqual(3, predecessorConditions.Count);
            Assert.IsTrue(predecessorConditions.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { predAc, pred2ConstBAc, predCc }, evaluationManager, null)));
            Assert.IsTrue(predecessorConditions.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { pred1ConstAc, predBc, predCc }, evaluationManager, null)));
            Assert.IsTrue(predecessorConditions.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { pred1ConstAc, pred2ConstBAc, predDc }, evaluationManager, null)));

            HashSet<IRelativeState> predecessorRelativeStates = new HashSet<IRelativeState>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors((IRelativeState)problem.GoalConditions.GetCorrespondingRelativeStates(problem).First()))
            {
                Assert.AreEqual(1, predecessor.GetPredecessorRelativeStates().Count());
                predecessorRelativeStates.Add((IRelativeState)predecessor.GetPredecessorRelativeStates().First());
            }
            Assert.AreEqual(3, predecessorRelativeStates.Count);
            Assert.IsTrue(predecessorRelativeStates.Contains(factory.CreateRelativeState(predA, pred2ConstBConstA, predC)));
            Assert.IsTrue(predecessorRelativeStates.Contains(factory.CreateRelativeState(pred1ConstA, predB, predC)));
            Assert.IsTrue(predecessorRelativeStates.Contains(factory.CreateRelativeState(pred1ConstA, pred2ConstBConstA, predD)));

            HashSet<IConditions> predecessorConditions2 = new HashSet<IConditions>();
            foreach (var predecessor in transitionsGenerator.GetPredecessors(factory.CreateState(pred1ConstA, pred2ConstBConstA, predC)))
            {
                predecessorConditions2.Add((IConditions)predecessor.GetPredecessorConditions());
            }
            Assert.AreEqual(3, predecessorConditions2.Count);
            Assert.IsTrue(predecessorConditions2.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { predAc, pred2ConstBAc, predCc }, evaluationManager, null)));
            Assert.IsTrue(predecessorConditions2.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { pred1ConstAc, predBc, predCc }, evaluationManager, null)));
            Assert.IsTrue(predecessorConditions2.Contains(new ConditionsCNF(new HashSet<IConjunctCNF> { pred1ConstAc, pred2ConstBAc, predDc }, evaluationManager, null)));

            Assert.AreEqual(0, transitionsGenerator.GetSuccessors(statePredC).Count());
            Assert.AreEqual(0, transitionsGenerator.GetPredecessors(conditionsPredD).Count());
            Assert.AreEqual(0, transitionsGenerator.GetPredecessors(factory.CreateRelativeState(predD)).Count());
            Assert.AreEqual(0, transitionsGenerator.GetPredecessors(factory.CreateState(predD)).Count());

            var successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 4));
            Assert.AreEqual(4, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(statePredC, 50));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 4));
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(0, successors.Count);
            successors = new List<Planner.ISuccessor>(transitionsGenerator.GetNextSuccessors(problem.InitialState, 40));
            Assert.AreEqual(6, successors.Count);
            Assert.IsTrue(successors.TrueForAll(item => successorStates.Contains(item.GetSuccessorState())));

            var predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 2));
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(conditionsPredD, 50));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 5));
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(0, predecessors.Count);
            predecessors = new List<Planner.IPredecessor>(transitionsGenerator.GetNextPredecessors(problem.GoalConditions, 40));
            Assert.AreEqual(3, predecessors.Count);
            Assert.IsTrue(predecessors.TrueForAll(item => predecessorConditions.Contains(item.GetPredecessorConditions())));

            Planner.ISuccessor randomSuccessor = transitionsGenerator.GetRandomSuccessor(problem.InitialState);
            Assert.IsNotNull(randomSuccessor);
            Assert.IsTrue(successorStates.Contains(randomSuccessor.GetSuccessorState()));
            randomSuccessor = transitionsGenerator.GetRandomSuccessor(statePredC);
            Assert.IsNull(randomSuccessor);

            Planner.IPredecessor randomPredecessor = transitionsGenerator.GetRandomPredecessor(problem.GoalConditions);
            Assert.IsNotNull(randomPredecessor);
            Assert.IsTrue(predecessorConditions.Contains(randomPredecessor.GetPredecessorConditions()));
            randomPredecessor = transitionsGenerator.GetRandomPredecessor(conditionsPredD);
            Assert.IsNull(randomPredecessor);

            var successorStates2 = transitionsGenerator.GetSuccessorStates(problem.InitialState).ToList();
            Assert.AreEqual(6, successorStates2.Count);
            foreach (var succState in successorStates2)
            {
                Assert.IsTrue(successorStates.Contains(succState));
            }
            Assert.AreEqual(384, transitionsGenerator.GetPredecessorStates(factory.CreateState(pred1ConstA, pred2ConstBConstA, predC)).Count());
        }

        [TestMethod]
        public void TC_TypeHierarchy()
        {
            PDDLInputData data = new PDDLInputData(GetFilePath("TC_TypeHierarchy.pddl"), GetFilePath("Dummy_P.pddl"));

            IdManager idManager = new IdManager(data);
            TypeHierarchy typeHierarchy = new TypeHierarchy(data, idManager);
            Func<string, int> getId = typeName => idManager.Types.GetId(typeName);

            Assert.AreEqual(7, typeHierarchy.Count);

            var objectChildren = typeHierarchy[getId("object")];
            Assert.AreEqual(2, objectChildren.Count);
            Assert.IsTrue(objectChildren.Contains(getId("typeE")));
            Assert.IsTrue(objectChildren.Contains(getId("typeF")));

            var typeAChildren = typeHierarchy[getId("typeA")];
            Assert.AreEqual(1, typeAChildren.Count);
            Assert.IsTrue(typeAChildren.Contains(getId("typeD")));

            var typeBChildren = typeHierarchy[getId("typeB")];
            Assert.AreEqual(1, typeBChildren.Count);
            Assert.IsTrue(typeBChildren.Contains(getId("typeD")));

            var typeCChildren = typeHierarchy[getId("typeC")];
            Assert.AreEqual(2, typeCChildren.Count);
            Assert.IsTrue(typeCChildren.Contains(getId("typeA")));
            Assert.IsTrue(typeCChildren.Contains(getId("typeB")));

            var typeDChildren = typeHierarchy[getId("typeD")];
            Assert.AreEqual(0, typeDChildren.Count);

            var typeEChildren = typeHierarchy[getId("typeE")];
            Assert.AreEqual(1, typeEChildren.Count);
            Assert.IsTrue(typeEChildren.Contains(getId("typeC")));

            var typeFChildren = typeHierarchy[getId("typeF")];
            Assert.AreEqual(1, typeFChildren.Count);
            Assert.IsTrue(typeFChildren.Contains(getId("typeC")));
        }
    }
}
