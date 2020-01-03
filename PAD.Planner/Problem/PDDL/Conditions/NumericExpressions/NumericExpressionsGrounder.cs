using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expressions grounder returning grounded expression for the given substitution. All variable instances specified in the given
    /// variable substitution are replaced by the constants.
    /// </summary>
    public class NumericExpressionsGrounder : INumericExpressionTransformVisitor
    {
        /// <summary>
        /// Terms grounder.
        /// </summary>
        private Lazy<TermsGrounder> TermsGrounder { get; }

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
        /// <param name="idManager">ID manager.</param>
        public NumericExpressionsGrounder(Lazy<TermsGrounder> termsGrounder, IdManager idManager)
        {
            TermsGrounder = termsGrounder;
            IdManager = idManager;
        }

        /// <summary>
        /// Grounds the expression.
        /// </summary>
        /// <param name="numericExpression">Numeric expression.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded expression.</returns>
        public INumericExpression Ground(INumericExpression numericExpression, ISubstitution substitution)
        {
            Substitution = substitution;
            return numericExpression.Accept(this);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Plus expression)
        {
            List<INumericExpression> arguments = new List<INumericExpression>();
            expression.Children.ForEach(child => arguments.Add(child.Accept(this)));
            return new Plus(arguments);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Minus expression)
        {
            return new Minus(expression.LeftChild.Accept(this), expression.RightChild.Accept(this));
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(UnaryMinus expression)
        {
            return new UnaryMinus(expression.Child.Accept(this));
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Multiply expression)
        {
            List<INumericExpression> arguments = new List<INumericExpression>();
            expression.Children.ForEach(child => arguments.Add(child.Accept(this)));
            return new Multiply(arguments);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Divide expression)
        {
            return new Divide(expression.LeftChild.Accept(this), expression.RightChild.Accept(this));
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Number expression)
        {
            return new Number(expression.Value);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(DurationVariable expression)
        {
            return expression;
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(NumericFunction expression)
        {
            return new NumericFunction(TermsGrounder.Value.GroundAtom(expression.FunctionAtom, Substitution), IdManager);
        }
    }
}
