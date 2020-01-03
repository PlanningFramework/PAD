using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Auxiliary static class for generating all the states for specified conditions or a relative state.
    /// </summary>
    public static class StatesEnumerator
    {
        /// <summary>
        /// Generates all possible PDDL states meeting conditions specified by the given conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="problem">Planning problem.</param>
        /// <returns>All possible PDDL states meeting the conditions.</returns>
        public static IEnumerable<IState> EnumerateStates(IConditions conditions, Problem problem)
        {
            foreach (var relativeState in EnumerateRelativeStates(conditions, problem))
            {
                foreach (var state in EnumerateStates(relativeState, problem))
                {
                    yield return state;
                }
            }
        }

        /// <summary>
        /// Generates all possible PDDL states meeting conditions specified by the relative state. Lazy generated via yield return.
        /// </summary>
        /// <param name="relativeState">Reference relative state.</param>
        /// <param name="problem">Planning problem.</param>
        /// <returns>All possible PDDL states meeting the conditions.</returns>
        public static IEnumerable<IState> EnumerateStates(IRelativeState relativeState, Problem problem)
        {
            // note: numeric function assignments are not enumerated (as it is generally infinite), but only fixed by the values of the source state

            Func<IAtom, bool?> predicateChecker = (predicate) =>
            {
                if (relativeState.HasPredicate(predicate))
                {
                    return true;
                }
                else if (relativeState.HasNegatedPredicate(predicate))
                {
                    return false;
                }
                return null;
            };

            Func<IAtom, int> objectFunctionChecker = relativeState.GetObjectFunctionValue;

            IState initState = new State(problem.IdManager);
            foreach (var numericFunction in relativeState.GetNumericFunctions())
            {
                initState.AssignNumericFunction(numericFunction.Key, numericFunction.Value);
            }

            var predicates = problem.EvaluationManager.GroundingManager.GetAllGroundedPredicates();
            var objectFunctions = problem.EvaluationManager.GroundingManager.GetAllGroundedObjectFunctions();

            foreach (var state in EnumerateStatesByPredicates(0, predicates, initState, predicateChecker))
            {
                foreach (var resultState in EnumerateStatesByObjectFunctions(0, objectFunctions, state, objectFunctionChecker))
                {
                    yield return resultState;
                }
            }
        }

        /// <summary>
        /// Generates all possible PDDL relative states meeting conditions specified by the given conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="problem">Planning problem.</param>
        /// <returns>All possible PDDL relative states meeting the conditions.</returns>
        public static IEnumerable<IRelativeState> EnumerateRelativeStates(IConditions conditions, Problem problem)
        {
            return EnumerateRelativeStatesByCNF(0, new List<IConjunctCNF>((ConditionsCNF)conditions.GetCNF()), new RelativeState(problem.IdManager));
        }

        /// <summary>
        /// Generates all possible PDDL states meeting conditions specified by the given constraint function. Lazy generated recursively via yield return.
        /// </summary>
        /// <param name="index">Current index in the predicates list.</param>
        /// <param name="predicates">List of all grounded predicates.</param>
        /// <param name="result">Current state being built.</param>
        /// <param name="predicateConstraint">Predicate constraint function (returns true for positively constrained predicate, false for negatively constrained
        /// predicate, or null for not constrained).</param>
        /// <returns>All possible PDDL states meeting the conditions.</returns>
        private static IEnumerable<IState> EnumerateStatesByPredicates(int index, List<IAtom> predicates, IState result, Func<IAtom, bool?> predicateConstraint)
        {
            if (index >= predicates.Count)
            {
                yield return (IState)result.Clone();
            }
            else
            {
                var predicate = predicates[index];

                // predicate can be positively constrained, negatively constrained (not), or not constrained at all
                var constraint = predicateConstraint(predicate);

                if (constraint != null)
                {
                    if (constraint.Value)
                    {
                        result.AddPredicate(predicate);
                    }

                    foreach (var item in EnumerateStatesByPredicates(index + 1, predicates, result, predicateConstraint))
                    {
                        yield return item;
                    }

                    if (constraint.Value)
                    {
                        result.RemovePredicate(predicate);
                    }
                }
                else
                {
                    // the predicate won't be contained
                    foreach (var item in EnumerateStatesByPredicates(index + 1, predicates, result, predicateConstraint))
                    {
                        yield return item;
                    }

                    // the predicate will be contained
                    result.AddPredicate(predicate);
                    foreach (var item in EnumerateStatesByPredicates(index + 1, predicates, result, predicateConstraint))
                    {
                        yield return item;
                    }
                    result.RemovePredicate(predicate);
                }
            }
        }

        /// <summary>
        /// Generates all possible PDDL states meeting conditions specified by the given constraint function. Lazy generated recursively via yield return.
        /// </summary>
        /// <param name="index">Current index in the object functions list.</param>
        /// <param name="objectFunctions">List of all grounded object functions with all possible values.</param>
        /// <param name="result">Current state being built.</param>
        /// <param name="constrainedValue">Object function value constrained value (-1 if not constrained).</param>
        /// <returns>All possible PDDL states meeting the conditions.</returns>
        private static IEnumerable<IState> EnumerateStatesByObjectFunctions(int index, List<Tuple<IAtom, List<int>>> objectFunctions, IState result, Func<IAtom, int> constrainedValue)
        {
            if (index >= objectFunctions.Count)
            {
                yield return (IState)result.Clone();
            }
            else
            {
                var objectFunction = objectFunctions[index];
                var value = constrainedValue(objectFunction.Item1);

                if (value != ObjectFunctionTerm.UndefinedValue)
                {
                    result.AssignObjectFunction(objectFunction.Item1, value);

                    foreach (var item in EnumerateStatesByObjectFunctions(index + 1, objectFunctions, result, constrainedValue))
                    {
                        yield return item;
                    }
                }
                else
                {
                    foreach (int functionValue in objectFunction.Item2)
                    {
                        result.AssignObjectFunction(objectFunction.Item1, functionValue);

                        foreach (var item in EnumerateStatesByObjectFunctions(index + 1, objectFunctions, result, constrainedValue))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates all possible PDDL relative states meeting given CNF conditions (in the form of a list of conjuncts). Lazy generated recursively via yield return.
        /// </summary>
        /// <param name="index">Current index in the conjuncts list.</param>
        /// <param name="conjuncts">List of conjuncts of the CNF conditions.</param>
        /// <param name="result">Current relative state being built.</param>
        /// <returns>All possible PDDL relative states meeting the CNF conditions.</returns>
        private static IEnumerable<IRelativeState> EnumerateRelativeStatesByCNF(int index, List<IConjunctCNF> conjuncts, IRelativeState result)
        {
            if (index == 0)
            {
                // the constructed state can have trailing values from the previous unfinished enumeration!
                result.ClearContent();
            }

            Action<IRelativeState, LiteralCNF> addLiteral = (state, literal) =>
            {
                // Note: At the moment, there is limited support for object and numeric function assignments.
                // For example, numeric comparison literals like (< (numFunc) 5) will be omitted in the resulting relative state.

                PredicateLiteralCNF predicateLiteral = literal as PredicateLiteralCNF;
                if (predicateLiteral != null)
                {
                    if (literal.IsNegated)
                    {
                        state.AddNegatedPredicate(predicateLiteral.PredicateAtom.Clone());
                    }
                    else
                    {
                        state.AddPredicate(predicateLiteral.PredicateAtom.Clone());
                    }
                    return;
                }

                EqualsLiteralCNF equalsLiteral = literal as EqualsLiteralCNF;
                if (equalsLiteral != null)
                {
                    var assignment = equalsLiteral.TryGetObjectFunctionAssignment();
                    if (assignment != null)
                    {
                        if (!literal.IsNegated)
                        {
                            state.AssignObjectFunction(assignment.Item1.FunctionAtom.Clone(), assignment.Item2.NameId);
                        }
                    }
                    return;
                }

                NumericCompareLiteralCNF compareLiteral = literal as NumericCompareLiteralCNF;
                if (compareLiteral != null)
                {
                    var assignment = compareLiteral.TryGetNumericFunctionAssignment();
                    if (assignment != null)
                    {
                        if (!compareLiteral.IsNegated)
                        {
                            state.AssignNumericFunction(assignment.Item1.FunctionAtom.Clone(), assignment.Item2.Value);
                        }
                    }
                }
            };

            Action<IRelativeState, LiteralCNF> removeLiteral = (state, literal) =>
            {
                PredicateLiteralCNF predicateLiteral = literal as PredicateLiteralCNF;
                if (predicateLiteral != null)
                {
                    if (literal.IsNegated)
                    {
                        state.RemoveNegatedPredicate(predicateLiteral.PredicateAtom.Clone());
                    }
                    else
                    {
                        state.RemovePredicate(predicateLiteral.PredicateAtom.Clone());
                    }
                    return;
                }

                EqualsLiteralCNF equalsLiteral = literal as EqualsLiteralCNF;
                if (equalsLiteral != null)
                {
                    var assignment = equalsLiteral.TryGetObjectFunctionAssignment();
                    if (assignment != null)
                    {
                        if (!literal.IsNegated)
                        {
                            state.AssignObjectFunction(assignment.Item1.FunctionAtom.Clone(), ObjectFunctionTerm.UndefinedValue);
                        }
                    }
                    return;
                }

                NumericCompareLiteralCNF compareLiteral = literal as NumericCompareLiteralCNF;
                if (compareLiteral != null)
                {
                    var assignment = compareLiteral.TryGetNumericFunctionAssignment();
                    if (assignment != null)
                    {
                        if (!compareLiteral.IsNegated)
                        {
                            state.AssignNumericFunction(assignment.Item1.FunctionAtom.Clone(), NumericFunction.UndefinedValue);
                        }
                    }
                }
            };

            if (index >= conjuncts.Count)
            {
                yield return (IRelativeState)result.Clone();
            }
            else
            {
                var conjunct = conjuncts[index];

                ClauseCNF clause = conjunct as ClauseCNF;
                if (clause != null)
                {
                    foreach (var literal in clause)
                    {
                        addLiteral(result, literal);

                        foreach (var item in EnumerateRelativeStatesByCNF(index + 1, conjuncts, result))
                        {
                            yield return item;
                        }

                        removeLiteral(result, literal);
                    }
                }
                else
                {
                    LiteralCNF literal = conjunct as LiteralCNF;
                    Debug.Assert(literal != null);

                    addLiteral(result, literal);

                    foreach (var item in EnumerateRelativeStatesByCNF(index + 1, conjuncts, result))
                    {
                        yield return item;
                    }

                    removeLiteral(result, literal);
                }
            }
        }
    }
}
