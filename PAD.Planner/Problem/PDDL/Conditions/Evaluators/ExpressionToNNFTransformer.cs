using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Transformer converting the given expression into the negation-normal-form (NNF). An expression in NNF has no imply expressions, preferences
    /// are omitted, and all negations are applied only to primitive elements (i.e. literals). These primitive elements are predicate expressions,
    /// equals expressions and numeric compare expressions. Quantified expressions (forall, exists) are grounded and unfolded into simple and/or
    /// expressions and recursively transformed as well, even though the resulting expression can grow significantly.
    /// </summary>
    public class ExpressionToNNFTransformer : BaseExpressionTransformVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager = null;

        /// <summary>
        /// Is currently negating the processed expression?
        /// </summary>
        private bool IsNegating { set; get; } = false;

        /// <summary>
        /// Creates the expression NNF transformer.
        /// </summary>
        /// <param name="evaluationManager">Evalution manager.</param>
        public ExpressionToNNFTransformer(EvaluationManager evaluationManager)
        {
            GroundingManager = evaluationManager.GroundingManager;
        }

        /// <summary>
        /// Transforms the expression into the negation-normal-form from the given expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression in negation-normal form.</returns>
        public IExpression Transform(IExpression expression)
        {
            IsNegating = false;
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(PredicateExpression expression)
        {
            if (IsNegating)
            {
                return new NotExpression(expression);
            }
            return expression;
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(PreferenceExpression expression)
        {
            return expression.Child.Accept(this);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(EqualsExpression expression)
        {
            if (IsNegating)
            {
                return new NotExpression(expression);
            }
            return expression;
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(AndExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();
            foreach (var child in expression.Children)
            {
                arguments.Add(child.Accept(this));
            }

            if (IsNegating)
            {
                return new OrExpression(arguments);
            }
            return new AndExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(OrExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();
            foreach (var child in expression.Children)
            {
                arguments.Add(child.Accept(this));
            }

            if (IsNegating)
            {
                return new AndExpression(arguments);
            }
            return new OrExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(NotExpression expression)
        {
            IsNegating = !IsNegating;
            IExpression tranformedExpression = expression.Child.Accept(this);
            IsNegating = !IsNegating;

            return tranformedExpression;
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(ImplyExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();

            IsNegating = !IsNegating;
            arguments.Add(expression.LeftChild.Accept(this));
            IsNegating = !IsNegating;

            arguments.Add(expression.RightChild.Accept(this));

            if (IsNegating)
            {
                return new AndExpression(arguments);
            }
            return new OrExpression(arguments);
        }

        /// <summary>
        /// Auxiliary function for transforming the argument sub-expression of a quantified expression. The expression is grounded
        /// by all possible substitutions and each of the resulting expression is recursively transformed.
        /// </summary>
        /// <param name="parameters">Quantified expression parameters.</param>
        /// <param name="expression">Argument sub-expression of the quantified expression.</param>
        /// <returns>List of all possible grounded and transformed sub-expressions.</returns>
        private List<IExpression> TransformQuantifiedSubExpression(Parameters parameters, IExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();

            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                var groundedSubExpression = GroundingManager.GroundExpression(expression, localSubstitution);
                arguments.Add(groundedSubExpression.Accept(this));
            }

            return arguments;
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(ExistsExpression expression)
        {
            List<IExpression> arguments = TransformQuantifiedSubExpression(expression.Parameters, expression.Child);
            if (IsNegating)
            {
                return new AndExpression(arguments);
            }
            return new OrExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(ForallExpression expression)
        {
            List<IExpression> arguments = TransformQuantifiedSubExpression(expression.Parameters, expression.Child);
            if (IsNegating)
            {
                return new OrExpression(arguments);
            }
            return new AndExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(NumericCompareExpression expression)
        {
            if (IsNegating)
            {
                return new NotExpression(expression);
            }
            return expression;
        }
    }
}
