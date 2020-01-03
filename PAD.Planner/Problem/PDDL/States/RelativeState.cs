using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a relative state in the PDDL planning problem. Relative state is an extension of a standard state, representing
    /// a whole class of states. It is an alternative way to express conditions in the backwards planning (an alternative to the more general
    /// IConditions). Relative states in PDDL contain only the predicates and function values that are common for a group of states and
    /// additionally allow to explicitly express that something cannot be true, via "negated" predicates. Note the difference between a
    /// standard state and a relative state: everything not expressed in the state is implicitly not true, while everything not expressed
    /// in the relative state is implicitly both true and false - that's why the negated predicates have a good purpose here.
    /// </summary>
    public class RelativeState : State, IRelativeState
    {
        /// <summary>
        /// Set of negated predicates, expressing the fact that something is not true in the relative state.
        /// </summary>
        public HashSet<IAtom> NegatedPredicates { set; get; }

        /// <summary>
        /// Constructs an empty relative state.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public RelativeState(IdManager idManager) : base(idManager)
        {
        }

        /// <summary>
        /// Constructs the relative state from the specified data.
        /// </summary>
        /// <param name="predicates">Predicates of the state.</param>
        /// <param name="negatedPredicates">Negated predicates of the state.</param>
        /// <param name="numericFunctions">Numeric function values.</param>
        /// <param name="objectFunctions">Object function values.</param>
        /// <param name="idManager">ID manager.</param>
        public RelativeState(HashSet<IAtom> predicates, HashSet<IAtom> negatedPredicates, Dictionary<IAtom, double> numericFunctions, Dictionary<IAtom, int> objectFunctions, IdManager idManager)
            : base(predicates, numericFunctions, objectFunctions, idManager)
        {
            NegatedPredicates = negatedPredicates;
        }

        /// <summary>
        /// Adds the negated predicate to the relative state.
        /// </summary>
        /// <param name="predicate">Predicate to be added.</param>
        public void AddNegatedPredicate(IAtom predicate)
        {
            if (NegatedPredicates == null)
            {
                NegatedPredicates = new HashSet<IAtom>();
            }
            NegatedPredicates.Add(predicate);
        }

        /// <summary>
        /// Removes the negated predicate from the relative state.
        /// </summary>
        /// <param name="predicate">Predicate to be removed.</param>
        public void RemoveNegatedPredicate(IAtom predicate)
        {
            NegatedPredicates?.Remove(predicate);
        }

        /// <summary>
        /// Checks whether the relative state contains the given negated predicate.
        /// </summary>
        /// <param name="predicate">Predicate to be checked.</param>
        /// <returns>True if the relative state contains the negated predicate, false otherwise.</returns>
        public bool HasNegatedPredicate(IAtom predicate)
        {
            if (NegatedPredicates == null)
            {
                return false;
            }
            return NegatedPredicates.Contains(predicate);
        }

        /// <summary>
        /// Enumerates the contained negated predicates.
        /// </summary>
        /// <returns>Enumeration of negated predicates.</returns>
        public IEnumerable<IAtom> GetNegatedPredicates()
        {
            if (NegatedPredicates == null)
            {
                yield break;
            }

            foreach (var predicate in NegatedPredicates)
            {
                yield return predicate;
            }
        }

        /// <summary>
        /// Checks whether the specified state is meeting conditions given by the relative state (i.e. belong to the corresponding class of states).
        /// </summary>
        /// <param name="state">State to be checked.</param>
        /// <returns>True if the state is meeting conditions of the relative state, false otherwise.</returns>
        public bool Evaluate(Planner.IState state)
        {
            IState evaluatedState = (IState)state;

            if (Predicates != null)
            {
                foreach (var predicate in Predicates)
                {
                    if (!evaluatedState.HasPredicate(predicate))
                    {
                        return false;
                    }
                }
            }

            if (NegatedPredicates != null)
            {
                foreach (var predicate in NegatedPredicates)
                {
                    if (evaluatedState.HasPredicate(predicate))
                    {
                        return false;
                    }
                }
            }

            if (NumericFunctions != null)
            {
                foreach (var numericFunction in NumericFunctions)
                {
                    if (!evaluatedState.GetNumericFunctionValue(numericFunction.Key).Equals(numericFunction.Value))
                    {
                        return false;
                    }
                }
            }

            if (ObjectFunctions != null)
            {
                foreach (var objectFunction in ObjectFunctions)
                {
                    if (evaluatedState.GetObjectFunctionValue(objectFunction.Key) != objectFunction.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Enumerates all possible states meeting the conditions of the current relative state (i.e. are in the same class of states).
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>All possible states meeting the conditions of the relative state.</returns>
        public IEnumerable<Planner.IState> GetCorrespondingStates(IProblem problem)
        {
            return StatesEnumerator.EnumerateStates(this, (Problem)problem);
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public override Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            Conditions newConditions = new Conditions(((Problem)problem).EvaluationManager);

            if (Predicates != null)
            {
                foreach (var predicate in Predicates)
                {
                    newConditions.Add(new PredicateExpression(predicate.Clone(), IdManager));
                }
            }

            if (NegatedPredicates != null)
            {
                foreach (var predicate in NegatedPredicates)
                {
                    newConditions.Add(new NotExpression(new PredicateExpression(predicate.Clone(), IdManager)));
                }
            }

            if (NumericFunctions != null)
            {
                foreach (var numericFunction in NumericFunctions)
                {
                    newConditions.Add(new NumericCompareExpression(NumericCompareExpression.RelationalOperator.EQ,
                                                                   new NumericFunction(numericFunction.Key.Clone(), IdManager),
                                                                   new Number(numericFunction.Value)));
                }
            }

            if (ObjectFunctions != null)
            {
                foreach (var objectFunction in ObjectFunctions)
                {
                    newConditions.Add(new EqualsExpression(new ObjectFunctionTerm(objectFunction.Key.Clone(), IdManager), new ConstantTerm(objectFunction.Value, IdManager)));
                }
            }

            return newConditions;
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public override Planner.IState Clone()
        {
            return new RelativeState(
                (Predicates == null) ? null : new HashSet<IAtom>(Predicates),
                (NegatedPredicates == null) ? null : new HashSet<IAtom>(NegatedPredicates),
                (NumericFunctions == null) ? null : new Dictionary<IAtom, double>(NumericFunctions),
                (ObjectFunctions == null) ? null : new Dictionary<IAtom, int>(ObjectFunctions),
                IdManager);
        }

        /// <summary>
        /// Clears the content of the state.
        /// </summary>
        public override void ClearContent()
        {
            base.ClearContent();
            NegatedPredicates = null;
        }

        /// <summary>
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public new bool DetermineGoalNode(IProblem problem)
        {
            return problem.IsStartRelativeState(this);
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public new double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return heuristic.GetValue(this);
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public new IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            return problem.GetPredecessors(this);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string baseDescription = base.ToString();

            if (NegatedPredicates == null || NegatedPredicates.Count == 0)
            {
                return baseDescription;
            }

            List<string> negatedPredicates = new List<string>();
            foreach (var predicateAtom in NegatedPredicates)
            {
                negatedPredicates.Add($"(not {predicateAtom.ToString(IdManager.Predicates)})");
            }
            
            return $"{baseDescription}, {string.Join(", ", negatedPredicates)}";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCodeForOrderNoMatterCollection(Predicates)
                .CombineHashCode(HashHelper.GetHashCodeForOrderNoMatterCollection(NumericFunctions))
                .CombineHashCode(HashHelper.GetHashCodeForOrderNoMatterCollection(ObjectFunctions))
                .CombineHashCode(HashHelper.GetHashCodeForOrderNoMatterCollection(NegatedPredicates));
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            RelativeState other = obj as RelativeState;
            if (other == null)
            {
                return false;
            }

            if (!CollectionsEquality.Equals(Predicates, other.Predicates) ||
                !CollectionsEquality.Equals(NumericFunctions, other.NumericFunctions) ||
                !CollectionsEquality.Equals(ObjectFunctions, other.ObjectFunctions) ||
                !CollectionsEquality.Equals(NegatedPredicates, other.NegatedPredicates))
            {
                return false;
            }

            return true;
        }
    }
}
