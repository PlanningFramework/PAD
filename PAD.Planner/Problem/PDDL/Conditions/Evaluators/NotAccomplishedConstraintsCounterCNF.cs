using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Counter of not accomplished condition constraints for given states. Used in heuristics.
    /// E.g. expression (and (true) (true) (false)) has 2 fulfilled constraints and 1 not fulfilled).
    /// </summary>
    public class NotAccomplishedConstraintsCounterCNF : IConditionsCNFPropCountVisitor
    {
        /// <summary>
        /// CNF evaluator.
        /// </summary>
        private Lazy<ConditionsCNFEvaluator> ConditionsCNFEvaluator { get; }

        /// <summary>
        /// Reference state for the evaluation.
        /// </summary>
        private IState ReferenceState { set; get; }

        /// <summary>
        /// Constructs the counter.
        /// </summary>
        /// <param name="conditionsCNFEvaluator">Conditions evaluator.</param>
        public NotAccomplishedConstraintsCounterCNF(Lazy<ConditionsCNFEvaluator> conditionsCNFEvaluator)
        {
            ConditionsCNFEvaluator = conditionsCNFEvaluator;
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int Evaluate(ConditionsCNF conditions, IState referenceState)
        {
            ReferenceState = referenceState;
            return conditions.Accept(this).Item2;
        }

        /// <summary>
        /// Visits and performs a property count on CNF conjunction.
        /// </summary>
        /// <param name="expression">CNF conjunction.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(ConditionsCNF expression)
        {
            int fulfilled = 0;
            int notFulfilled = 0;

            foreach (var conjunct in expression)
            {
                var childPropertyCounts = conjunct.Accept(this);
                fulfilled += childPropertyCounts.Item1;
                notFulfilled += childPropertyCounts.Item2;
            }

            return Tuple.Create(fulfilled, notFulfilled);
        }

        /// <summary>
        /// Visits and performs a property count on CNF clause.
        /// </summary>
        /// <param name="expression">CNF clause.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(ClauseCNF expression)
        {
            int minFulfilled = int.MaxValue;
            int minNotFulfilled = int.MaxValue;

            foreach (var literal in expression)
            {
                var childPropertyCounts = literal.Accept(this);
                minFulfilled = Math.Min(minFulfilled, childPropertyCounts.Item1);
                minNotFulfilled = Math.Min(minNotFulfilled, childPropertyCounts.Item2);
            }

            return Tuple.Create(minFulfilled, minNotFulfilled);
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(PredicateLiteralCNF expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(EqualsLiteralCNF expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(NumericCompareLiteralCNF expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on a primitive logical expression (predicate, equals, numeric compare).
        /// </summary>
        /// <param name="expression">Logical expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        private Tuple<int, int> ProcessPrimitiveExpression(LiteralCNF expression)
        {
            bool isFulfilled = ConditionsCNFEvaluator.Value.Evaluate(expression, null, ReferenceState);
            return isFulfilled ? Tuple.Create(1, 0) : Tuple.Create(0, 1);
        }
    }
}
