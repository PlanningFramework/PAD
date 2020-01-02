using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluator of the operator label in the relaxed planning graph (for preconditions in CNF).
    /// </summary>
    public class PlanningGraphOperatorLabelEvaluatorCNF : IConditionsCNFPropEvalVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// State labels.
        /// </summary>
        private StateLabels StateLabels { set; get; } = null;

        /// <summary>
        /// Evaluation strategy.
        /// </summary>
        private ForwardCostEvaluationStrategy EvaluationStrategy { set; get; } = ForwardCostEvaluationStrategy.MAX_VALUE;

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        public PlanningGraphOperatorLabelEvaluatorCNF(GroundingManager groundingManager)
        {
            GroundingManager = groundingManager;
        }

        /// <summary>
        /// Evaluates the label of the operator with the specified preconditions and substitution.
        /// </summary>
        /// <param name="conditions">Operator conditions.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="stateLabels">Atom labels in the predecessor layer.</param>
        /// <param name="evaluationStrategy">Evaluation strategy.</param>
        /// <returns>Operator label value in the relaxed planning graph.</returns>
        public double Evaluate(ConditionsCNF conditions, ISubstitution substitution, StateLabels stateLabels, Planner.ForwardCostEvaluationStrategy evaluationStrategy)
        {
            Substitution = substitution;
            StateLabels = stateLabels;
            EvaluationStrategy = evaluationStrategy;

            return conditions.Accept(this).Item1;
        }

        /// <summary>
        /// Visits and performs a property count on CNF conjunction.
        /// </summary>
        /// <param name="expression">CNF conjunction.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(ConditionsCNF expression)
        {
            double positiveValue = 0;
            double negativeValue = 0;

            foreach (var child in expression)
            {
                var childPropertyCounts = child.Accept(this);

                if (EvaluationStrategy == ForwardCostEvaluationStrategy.ADDITIVE_VALUE)
                {
                    positiveValue += childPropertyCounts.Item1;
                    negativeValue += childPropertyCounts.Item2;
                }
                else
                {
                    positiveValue = Math.Max(positiveValue, childPropertyCounts.Item1);
                    negativeValue = Math.Max(negativeValue, childPropertyCounts.Item2);
                }
            }

            return Tuple.Create(positiveValue, negativeValue);
        }

        /// <summary>
        /// Visits and performs a property count on CNF clause.
        /// </summary>
        /// <param name="expression">CNF clause.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(ClauseCNF expression)
        {
            double positiveValue = 0;
            double negativeValue = 0;

            foreach (var child in expression)
            {
                var childPropertyCounts = child.Accept(this);
                positiveValue = Math.Max(positiveValue, childPropertyCounts.Item1);
                negativeValue = Math.Max(negativeValue, childPropertyCounts.Item2);
            }

            return Tuple.Create(positiveValue, negativeValue);
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(PredicateLiteralCNF expression)
        {
            var groundedAtom = GroundingManager.GroundAtom(expression.PredicateAtom, Substitution);

            double value = -1;
            if (StateLabels.TryGetValue(groundedAtom, out value))
            {
                return (expression.IsNegated) ? Tuple.Create(0.0, value) : Tuple.Create(value, 0.0);
            }

            return Tuple.Create(0.0, 0.0);
        }

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(EqualsLiteralCNF expression)
        {
            // this property is not taken into account
            return Tuple.Create(0.0, 0.0);
        }

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(NumericCompareLiteralCNF expression)
        {
            // this property is not taken into account
            return Tuple.Create(0.0, 0.0);
        }
    }
}
