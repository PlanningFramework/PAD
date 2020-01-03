using System.Collections.Generic;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluator of the operator label in the relaxed planning graph.
    /// </summary>
    public class PlanningGraphOperatorLabelEvaluator : IExpressionPropEvalVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; }

        /// <summary>
        /// State labels.
        /// </summary>
        private StateLabels StateLabels { set; get; }

        /// <summary>
        /// Evaluation strategy.
        /// </summary>
        private ForwardCostEvaluationStrategy EvaluationStrategy { set; get; } = ForwardCostEvaluationStrategy.MAX_VALUE;

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        public PlanningGraphOperatorLabelEvaluator(GroundingManager groundingManager)
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
        public double Evaluate(Conditions conditions, ISubstitution substitution, StateLabels stateLabels, ForwardCostEvaluationStrategy evaluationStrategy)
        {
            Substitution = substitution;
            StateLabels = stateLabels;
            EvaluationStrategy = evaluationStrategy;

            return conditions.GetWrappedConditions().Accept(this).Item1;
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(PredicateExpression expression)
        {
            var groundedAtom = GroundingManager.GroundAtom(expression.PredicateAtom, Substitution);

            double value;
            if (StateLabels.TryGetValue(groundedAtom, out value))
            {
                return Tuple.Create(value, 0.0);
            }

            return Tuple.Create(0.0, 0.0);
        }

        /// <summary>
        /// Visits and performs a property count on equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(EqualsExpression expression)
        {
            // this property is not taken into account
            return Tuple.Create(0.0, 0.0);
        }

        /// <summary>
        /// Visits and performs a property count on relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(NumericCompareExpression expression)
        {
            // this property is not taken into account
            return Tuple.Create(0.0, 0.0);
        }

        /// <summary>
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(AndExpression expression)
        {
            double positiveValue = 0.0;
            double negativeValue = 0.0;

            foreach (var child in expression.Children)
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
        /// Visits and performs a property count on predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(OrExpression expression)
        {
            double positiveValue = 0.0;
            double negativeValue = 0.0;

            foreach (var child in expression.Children)
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
        public Tuple<double, double> Visit(ImplyExpression expression)
        {
            var leftChildPropCounts = expression.LeftChild.Accept(this);
            var rightChildPropCounts = expression.RightChild.Accept(this);

            return Tuple.Create(Math.Max(leftChildPropCounts.Item2, rightChildPropCounts.Item1), Math.Max(leftChildPropCounts.Item1, rightChildPropCounts.Item2));
        }

        /// <summary>
        /// Visits and performs a property count on exists expression.
        /// </summary>
        /// <param name="expression">Exists expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(ExistsExpression expression)
        {
            double positiveValue = 0.0;
            double negativeValue = 0.0;

            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                var childPropertyCounts = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

                positiveValue = Math.Max(positiveValue, childPropertyCounts.Item1);
                negativeValue = Math.Max(negativeValue, childPropertyCounts.Item2);
            }

            return Tuple.Create(positiveValue, negativeValue);
        }

        /// <summary>
        /// Visits and performs a property count on forall expression.
        /// </summary>
        /// <param name="expression">Forall expression.</param>
        /// <returns>Tuple (property satisfied count, property not satisfied count).</returns>
        public Tuple<double, double> Visit(ForallExpression expression)
        {
            double positiveValue = 0;
            double negativeValue = 0;

            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                var childPropertyCounts = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

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
    }
}
