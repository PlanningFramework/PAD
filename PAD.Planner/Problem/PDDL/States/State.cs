using System.Collections.Generic;
using System.Globalization;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a state in the PDDL planning problem. A state is basically defined by a set of PDDL predicates,
    /// and PDDL function values. The function values not specifically stated are undefined in the state.
    /// </summary>
    public class State : IState
    {
        /// <summary>
        /// Set of predicates in the state.
        /// </summary>
        public HashSet<IAtom> Predicates { set; get; }

        /// <summary>
        /// Collection of numeric function values in the state.
        /// </summary>
        public Dictionary<IAtom, double> NumericFunctions { set; get; }

        /// <summary>
        /// Collection of object function values in the state.
        /// </summary>
        public Dictionary<IAtom, int> ObjectFunctions { set; get; }

        /// <summary>
        /// ID manager.
        /// </summary>
        protected IdManager IdManager { set; get; }

        /// <summary>
        /// Constructs an empty state.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public State(IdManager idManager)
        {
            IdManager = idManager;
        }

        /// <summary>
        /// Constructs the state from the specified data.
        /// </summary>
        /// <param name="predicates">Predicates of the state.</param>
        /// <param name="numericFunctions">Numeric function values.</param>
        /// <param name="objectFunctions">Object function values.</param>
        /// <param name="idManager">ID manager.</param>
        public State(HashSet<IAtom> predicates, Dictionary<IAtom, double> numericFunctions, Dictionary<IAtom, int> objectFunctions, IdManager idManager) : this(idManager)
        {
            Predicates = predicates;
            NumericFunctions = numericFunctions;
            ObjectFunctions = objectFunctions;
        }

        /// <summary>
        /// Constructs the state from the input data.
        /// </summary>
        /// <param name="init">Input data.</param>
        /// <param name="idManager">ID manager.</param>
        public State(InputData.PDDL.Init init, IdManager idManager) : this(idManager)
        {
            InitialStateDataBuilder builder = new InitialStateDataBuilder(idManager);
            builder.Build(init);

            Predicates = (builder.Predicates.Count != 0) ? builder.Predicates : null;
            NumericFunctions = (builder.NumericFunctions.Count != 0) ? builder.NumericFunctions : null;
            ObjectFunctions = (builder.ObjectFunctions.Count != 0) ? builder.ObjectFunctions : null;
        }

        /// <summary>
        /// Adds the predicate to the state.
        /// </summary>
        /// <param name="predicate">Predicate to be added.</param>
        public void AddPredicate(IAtom predicate)
        {
            if (Predicates == null)
            {
                Predicates = new HashSet<IAtom>();
            }
            Predicates.Add(predicate);
        }

        /// <summary>
        /// Removes the predicate from the state.
        /// </summary>
        /// <param name="predicate">Predicate to be removed.</param>
        public void RemovePredicate(IAtom predicate)
        {
            Predicates?.Remove(predicate);
        }

        /// <summary>
        /// Checks whether the state contains the given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to be checked.</param>
        /// <returns>True if the state contains the predicate, false otherwise.</returns>
        public bool HasPredicate(IAtom predicate)
        {
            if (Predicates == null)
            {
                return false;
            }
            return Predicates.Contains(predicate);
        }

        /// <summary>
        /// Returns the value of the given object function.
        /// </summary>
        /// <param name="function">Grounded function atom.</param>
        /// <returns>Object value, i.e. constant name ID.</returns>
        public int GetObjectFunctionValue(IAtom function)
        {
            if (ObjectFunctions == null || !ObjectFunctions.ContainsKey(function))
            {
                return ObjectFunctionTerm.UndefinedValue;
            }
            return ObjectFunctions[function];
        }

        /// <summary>
        /// Defines a new value for the requested function in the state.
        /// </summary>
        /// <param name="function">Requested function.</param>
        /// <param name="assignment">Value to be assigned.</param>
        public void AssignObjectFunction(IAtom function, int assignment)
        {
            if (assignment == ObjectFunctionTerm.UndefinedValue)
            {
                if (ObjectFunctions != null && ObjectFunctions.ContainsKey(function))
                {
                    ObjectFunctions.Remove(function);
                }
            }
            else
            {
                CheckObjectFunctionsInitialized();
                ObjectFunctions[function] = assignment;
            }
        }

        /// <summary>
        /// Returns the value of the given numeric function.
        /// </summary>
        /// <param name="function">Grounded function atom.</param>
        /// <returns>Numeric value.</returns>
        public double GetNumericFunctionValue(IAtom function)
        {
            if (NumericFunctions == null || !NumericFunctions.ContainsKey(function))
            {
                return NumericFunction.UndefinedValue;
            }
            return NumericFunctions[function];
        }

        /// <summary>
        /// Defines a new value for the requested function in the state.
        /// </summary>
        /// <param name="function">Requested function.</param>
        /// <param name="assignment">Value to be assigned.</param>
        public void AssignNumericFunction(IAtom function, double assignment)
        {
            if (NumericFunction.IsValueUndefined(assignment))
            {
                if (NumericFunctions != null && NumericFunctions.ContainsKey(function))
                {
                    NumericFunctions.Remove(function);
                }
            }
            else
            {
                CheckNumericFunctionsInitialized();
                NumericFunctions[function] = assignment;
            }
        }

        /// <summary>
        /// Checks whether the numeric functions collection is initialized. If not, the collection is initialized.
        /// </summary>
        private void CheckNumericFunctionsInitialized()
        {
            if (NumericFunctions == null)
            {
                NumericFunctions = new Dictionary<IAtom, double>();
            }
        }

        /// <summary>
        /// Checks whether the object functions collection is initialized. If not, the collection is initialized.
        /// </summary>
        private void CheckObjectFunctionsInitialized()
        {
            if (ObjectFunctions == null)
            {
                ObjectFunctions = new Dictionary<IAtom, int>();
            }
        }

        /// <summary>
        /// Increase the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be increased by.</param>
        public void IncreaseNumericFunction(IAtom function, double value)
        {
            CheckNumericFunctionsInitialized();

            if (!NumericFunctions.ContainsKey(function))
            {
                NumericFunctions[function] = value;
            }
            else
            {
                NumericFunctions[function] += value;
            }
        }

        /// <summary>
        /// Decrease the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be decreased by.</param>
        public void DecreaseNumericFunction(IAtom function, double value)
        {
            CheckNumericFunctionsInitialized();

            if (!NumericFunctions.ContainsKey(function))
            {
                NumericFunctions[function] = -value;
            }
            else
            {
                NumericFunctions[function] -= value;
            }
        }

        /// <summary>
        /// Scale-up the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be scaled-up by.</param>
        public void ScaleUpNumericFunction(IAtom function, double value)
        {
            CheckNumericFunctionsInitialized();

            if (!NumericFunctions.ContainsKey(function))
            {
                NumericFunctions[function] = 0.0;
            }
            else
            {
                NumericFunctions[function] *= value;
            }
        }

        /// <summary>
        /// Scale-down the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be scaled-down by.</param>
        public void ScaleDownNumericFunction(IAtom function, double value)
        {
            CheckNumericFunctionsInitialized();

            if (!NumericFunctions.ContainsKey(function))
            {
                NumericFunctions[function] = 0.0;
            }
            else
            {
                NumericFunctions[function] /= value;
            }
        }

        /// <summary>
        /// Enumerates the contained predicates.
        /// </summary>
        /// <returns>Enumeration of contained predicates.</returns>
        public IEnumerable<IAtom> GetPredicates()
        {
            if (Predicates == null)
            {
                yield break;
            }

            foreach (var predicate in Predicates)
            {
                yield return predicate;
            }
        }

        /// <summary>
        /// Enumerates the contained object functions with their assigned values. If a function is not contained, its value is undefined.
        /// </summary>
        /// <returns>Enumeration of object functions with their values.</returns>
        public IEnumerable<KeyValuePair<IAtom, int>> GetObjectFunctions()
        {
            if (ObjectFunctions == null)
            {
                yield break;
            }

            foreach (var objectFunctionWithValue in ObjectFunctions)
            {
                yield return objectFunctionWithValue;
            }
        }

        /// <summary>
        /// Enumerates the contained numeric functions with their assigned values. If a function is not contained, its value is undefined.
        /// </summary>
        /// <returns>Enumeration of numeric functions with their values.</returns>
        public IEnumerable<KeyValuePair<IAtom, double>> GetNumericFunctions()
        {
            if (NumericFunctions == null)
            {
                yield break;
            }

            foreach (var numericFunctionWithValue in NumericFunctions)
            {
                yield return numericFunctionWithValue;
            }
        }

        /// <summary>
        /// Gets the relaxed variant of this state.
        /// </summary>
        /// <returns>Relaxed state.</returns>
        public Planner.IState GetRelaxedState()
        {
            return Clone();
        }

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        public virtual Planner.IState Clone()
        {
            return new State(
                (Predicates == null) ? null : new HashSet<IAtom>(Predicates),
                (NumericFunctions == null) ? null : new Dictionary<IAtom, double>(NumericFunctions),
                (ObjectFunctions == null) ? null : new Dictionary<IAtom, int>(ObjectFunctions),
                IdManager);
        }

        /// <summary>
        /// Clears the content of the state.
        /// </summary>
        public virtual void ClearContent()
        {
            Predicates = null;
            NumericFunctions = null;
            ObjectFunctions = null;
        }

        /// <summary>
        /// Gets the state size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>State size.</returns>
        public int GetSize()
        {
            int result = 0;
            result += Predicates?.Count ?? 0;
            result += NumericFunctions?.Count ?? 0;
            result += ObjectFunctions?.Count ?? 0;
            return result;
        }

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        public virtual Planner.IConditions GetDescribingConditions(IProblem problem)
        {
            Conditions newConditions = new Conditions(((Problem)problem).EvaluationManager);

            if (Predicates != null)
            {
                foreach (var predicate in Predicates)
                {
                    newConditions.Add(new PredicateExpression(predicate.Clone(), IdManager));
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
        /// Is the node a goal node of the search, in the given planning problem?
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>True if the node is a goal node of the search. False otherwise.</returns>
        public bool DetermineGoalNode(IProblem problem)
        {
            return problem.IsGoalState(this);
        }

        /// <summary>
        /// Gets the heuristic value of the search node, for the given heuristic.
        /// </summary>
        /// <param name="heuristic">Heuristic.</param>
        /// <returns>Heuristic value of the search node.</returns>
        public double DetermineHeuristicValue(Heuristics.IHeuristic heuristic)
        {
            return heuristic.GetValue(this);
        }

        /// <summary>
        /// Gets the transitions from the search node, in the given planning problem (i.e. successors/predecessors).
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <returns>Transitions from the search node.</returns>
        public IEnumerable<ITransition> DetermineTransitions(IProblem problem)
        {
            return problem.GetSuccessors(this);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> itemsList = new List<string>();
            if (Predicates != null)
            {
                foreach (var predicateAtom in Predicates)
                {
                    itemsList.Add(predicateAtom.ToString(IdManager.Predicates));
                }
            }
            if (NumericFunctions != null)
            {
                foreach (var numericFunction in NumericFunctions)
                {
                    string functionAtom = numericFunction.Key.ToString(IdManager.Functions);
                    string numericValue = (NumericFunction.IsValueUndefined(numericFunction.Value)) ? "undefined" : numericFunction.Value.ToString(CultureInfo.InvariantCulture);
                    itemsList.Add($"(= {functionAtom} {numericValue})");
                }
            }
            if (ObjectFunctions != null)
            {
                foreach (var objectFunction in ObjectFunctions)
                {
                    string functionAtom = objectFunction.Key.ToString(IdManager.Functions);
                    string constValue = IdManager.Constants.GetNameFromId(objectFunction.Value);
                    itemsList.Add($"(= {functionAtom} {constValue})");
                }
            }
            return string.Join(", ", itemsList);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCodeForOrderNoMatterCollection(Predicates)
                .CombineHashCode(HashHelper.GetHashCodeForOrderNoMatterCollection(NumericFunctions))
                .CombineHashCode(HashHelper.GetHashCodeForOrderNoMatterCollection(ObjectFunctions));
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

            State other = obj as State;
            if (other == null)
            {
                return false;
            }

            if (!CollectionsEquality.Equals(Predicates, other.Predicates) ||
                !CollectionsEquality.Equals(NumericFunctions, other.NumericFunctions) ||
                !CollectionsEquality.Equals(ObjectFunctions, other.ObjectFunctions))
            {
                return false;
            }

            return true;
        }
    }
}
