using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Counter of not accomplished condition constraints for given states. Used in heuristics.
    /// E.g. expression (and (true) (true) (false)) has 2 fulfilled constraints and 1 not fulfilled).
    /// </summary>
    public class NotAccomplishedConstraintsCounter : IExpressionPropCountVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Expression evaluator.
        /// </summary>
        private Lazy<ExpressionEvaluator> ExpressionEvaluator { set; get; } = null;

        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Reference state for the evaluation.
        /// </summary>
        private IState ReferenceState { set; get; } = null;

        /// <summary>
        /// Constructs the counter.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="expressionEvaluator">Expression evaluator.</param>
        public NotAccomplishedConstraintsCounter(GroundingManager groundingManager, Lazy<ExpressionEvaluator> expressionEvaluator)
        {
            GroundingManager = groundingManager;
            ExpressionEvaluator = expressionEvaluator;
            Substitution = new Substitution();
        }

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        public int Evaluate(Conditions conditions, IState referenceState)
        {
            ReferenceState = referenceState;
            return conditions.GetWrappedConditions().Accept(this).Item2;
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(AndExpression expression)
        {
            int fulfilled = 0;
            int notFulfilled = 0;

            foreach (var child in expression.Children)
            {
                var childPropertyCounts = child.Accept(this);
                fulfilled += childPropertyCounts.Item1;
                notFulfilled += childPropertyCounts.Item2;
            }

            return Tuple.Create(fulfilled, notFulfilled);
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(OrExpression expression)
        {
            int minFulfilled = int.MaxValue;
            int minNotFulfilled = int.MaxValue;

            foreach (var child in expression.Children)
            {
                var childPropertyCounts = child.Accept(this);
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
        public Tuple<int, int> Visit(ImplyExpression expression)
        {
            var leftChildPropCounts = expression.LeftChild.Accept(this);
            var rightChildPropCounts = expression.RightChild.Accept(this);

            // fulfilled: min(notFulfilled(a),fulfilled(b)); notFulfilled: min(fulfilled(a),notFulfilled(b))
            return Tuple.Create(Math.Min(leftChildPropCounts.Item2, rightChildPropCounts.Item1), Math.Min(leftChildPropCounts.Item1, rightChildPropCounts.Item2));
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(PredicateExpression expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(EqualsExpression expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(NumericCompareExpression expression)
        {
            return ProcessPrimitiveExpression(expression);
        }

        /// <summary>
        /// Visits and performs a property count on a primitive logical expression (predicate, equals, numeric compare).
        /// </summary>
        /// <param name="expression">Logical expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        private Tuple<int, int> ProcessPrimitiveExpression(IExpression expression)
        {
            bool isFulfilled = ExpressionEvaluator.Value.Evaluate(expression, null, ReferenceState);
            return isFulfilled ? Tuple.Create(1, 0) : Tuple.Create(0, 1);
        }

        /// <summary>
        /// Visits and performs a property count on exists expression.
        /// </summary>
        /// <param name="expression">Exists expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(ExistsExpression expression)
        {
            int minFulfilled = int.MaxValue;
            int minNotFulfilled = int.MaxValue;

            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                var childPropertyCounts = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

                minFulfilled = Math.Min(minFulfilled, childPropertyCounts.Item1);
                minNotFulfilled = Math.Min(minNotFulfilled, childPropertyCounts.Item2);
            }

            return Tuple.Create(minFulfilled, minNotFulfilled);
        }

        /// <summary>
        /// Visits and performs a property count on forall expression.
        /// </summary>
        /// <param name="expression">Forall expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<int, int> Visit(ForallExpression expression)
        {
            int fulfilled = 0;
            int notFulfilled = 0;

            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                var childPropertyCounts = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

                fulfilled += childPropertyCounts.Item1;
                notFulfilled += childPropertyCounts.Item2;
            }

            return Tuple.Create(fulfilled, notFulfilled);
        }
    }
}
