using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Expressions grounder returning grounded expression for the given substitution. All variable instances specified in the given
    /// variable substitution are replaced by the constants. Object function terms remain intact.
    /// </summary>
    public class ExpressionsGrounder : IExpressionTransformVisitor
    {
        /// <summary>
        /// Terms grounder.
        /// </summary>
        private Lazy<TermsGrounder> TermsGrounder { get; }

        /// <summary>
        /// Numeric expressions grounder.
        /// </summary>
        private Lazy<NumericExpressionsGrounder> NumericExpressionsGrounder { get; }

        /// <summary>
        /// ID manager.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; }

        /// <summary>
        /// Constructs the expressions grounder.
        /// </summary>
        /// <param name="termsGrounder">Terms grounder.</param>
        /// <param name="numericExpressionsGrounder">Numeric expressions grounder.</param>
        /// <param name="idManager">ID manager.</param>
        public ExpressionsGrounder(Lazy<TermsGrounder> termsGrounder, Lazy<NumericExpressionsGrounder> numericExpressionsGrounder, IdManager idManager)
        {
            TermsGrounder = termsGrounder;
            NumericExpressionsGrounder = numericExpressionsGrounder;
            IdManager = idManager;
        }

        /// <summary>
        /// Grounds the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <returns>Grounded expression.</returns>
        public IExpression Ground(IExpression expression, ISubstitution substitution)
        {
            Substitution = substitution;
            return expression.Accept(this);
        }

        /// <summary>
        /// Grounds the specified atom.
        /// </summary>
        /// <param name="atom">Atom.</param>
        /// <returns>Grounded atom.</returns>
        private IAtom GroundAtom(IAtom atom)
        {
            return TermsGrounder.Value.GroundAtom(atom, Substitution);
        }

        /// <summary>
        /// Grounds the specified term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Grounded term.</returns>
        private ITerm GroundTerm(ITerm term)
        {
            return TermsGrounder.Value.GroundTerm(term, Substitution);
        }

        /// <summary>
        /// Grounds the specified numeric expression.
        /// </summary>
        /// <param name="numericExpression">Numeric expression.</param>
        /// <returns>Grounded numeric expression.</returns>
        private INumericExpression GroundNumericExpression(INumericExpression numericExpression)
        {
            return NumericExpressionsGrounder.Value.Ground(numericExpression, Substitution);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(PredicateExpression expression)
        {
            return new PredicateExpression(GroundAtom(expression.PredicateAtom), IdManager);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(PreferenceExpression expression)
        {
            return new PreferenceExpression(expression.PreferenceNameId, expression.Accept(this), IdManager);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(EqualsExpression expression)
        {
            return new EqualsExpression(GroundTerm(expression.LeftArgument), GroundTerm(expression.RightArgument));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(AndExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();
            expression.Children.ForEach(child => arguments.Add(child.Accept(this)));
            return new AndExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(OrExpression expression)
        {
            List<IExpression> arguments = new List<IExpression>();
            expression.Children.ForEach(child => arguments.Add(child.Accept(this)));
            return new OrExpression(arguments);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(NotExpression expression)
        {
            return new NotExpression(expression.Child.Accept(this));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(ImplyExpression expression)
        {
            return new ImplyExpression(expression.LeftChild.Accept(this), expression.RightChild.Accept(this));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(ExistsExpression expression)
        {
            return new ExistsExpression(expression.Parameters, expression.Child.Accept(this));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(ForallExpression expression)
        {
            return new ForallExpression(expression.Parameters, expression.Child.Accept(this));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public IExpression Visit(NumericCompareExpression expression)
        {
            return new NumericCompareExpression(expression.Operator, GroundNumericExpression(expression.LeftArgument), GroundNumericExpression(expression.RightArgument));
        }
    }
}
