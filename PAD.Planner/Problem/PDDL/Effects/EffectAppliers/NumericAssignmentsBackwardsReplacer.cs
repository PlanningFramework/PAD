using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric assignments backwards replacer. In the given numeric expression, all instances of numeric functions are replaced
    /// by the available assigned values from the numeric function assignments effects. The whole expression is partially or fully
    /// reduced (evaluated) - i.e. in the best case, only the result number is returned.
    /// </summary>
    public class NumericAssignmentsBackwardsReplacer : INumericExpressionTransformVisitor
    {
        /// <summary>
        /// Numeric function assignments from the effects.
        /// </summary>
        private Dictionary<IAtom, INumericExpression> NumericFunctionAssignments { set; get; } = null;

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Variables substitution of the effects' parent operator.
        /// </summary>
        private ISubstitution OperatorSubstitution { set; get; } = null;

        /// <summary>
        /// Variables substitution of the expression (expression can be lifted e.g. in forall subexpressions).
        /// </summary>
        private ISubstitution ExpressionSubstitution { set; get; } = null;

        /// <summary>
        /// Register of already replaced function atoms in the current sub-expression. Needs to be checked to avoid chaining of the same
        /// assignments - e.g. with the effect of form "(assign (func) (+ (func) 12))".
        /// </summary>
        public HashSet<IAtom> ReplacedFunctionAtomsInSubExpression = new HashSet<IAtom>();

        /// <summary>
        /// Constructs the numeric assignment backwards replacer.
        /// </summary>
        /// <param name="numericFunctionAssignments">Numeric function assignments.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="operatorSubstitution">Operator substitution.</param>
        /// <param name="expressionSubstitution">Expression substitution.</param>
        public NumericAssignmentsBackwardsReplacer(Dictionary<IAtom, INumericExpression> numericFunctionAssignments, GroundingManager groundingManager, ISubstitution operatorSubstitution, ISubstitution expressionSubstitution)
        {
            NumericFunctionAssignments = numericFunctionAssignments;
            GroundingManager = groundingManager;
            OperatorSubstitution = operatorSubstitution;
            ExpressionSubstitution = expressionSubstitution;
        }

        /// <summary>
        /// Replaces numeric function instances in the given numeric expression and the whole expression is partially or fully reduced.
        /// </summary>
        /// <param name="numericExpression">Numeric expression to be evaluated.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Replace(INumericExpression numericExpression)
        {
            return numericExpression.Accept(this);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Plus expression)
        {
            double argumentsSum = 0.0;
            List<INumericExpression> newArguments = new List<INumericExpression>();

            foreach (var argument in expression.Children)
            {
                var transformedArgument = argument.Accept(this);

                Number numberArgument = transformedArgument as Number;
                if (numberArgument != null)
                {
                    argumentsSum += numberArgument.Value;
                }
                else
                {
                    newArguments.Add(transformedArgument);
                }
            }

            if (newArguments.Count == 0)
            {
                return new Number(argumentsSum);
            }
            else if (argumentsSum != 0.0)
            {
                newArguments.Add(new Number(argumentsSum));
            }

            return new Plus(newArguments);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Minus expression)
        {
            var transformedLeftArgument = expression.LeftChild.Accept(this);
            var transformedRightArgument = expression.RightChild.Accept(this);

            Number numberLeftArgument = transformedLeftArgument as Number;
            Number numberRightArgument = transformedRightArgument as Number;

            if (numberLeftArgument != null && numberRightArgument != null)
            {
                return new Number(numberLeftArgument.Value - numberRightArgument.Value);
            }

            return new Minus(transformedLeftArgument, transformedRightArgument);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(UnaryMinus expression)
        {
            var transformedArgument = expression.Child.Accept(this);

            Number numberArgument = transformedArgument as Number;
            if (numberArgument != null)
            {
                return new Number(-numberArgument.Value);
            }

            return new UnaryMinus(transformedArgument);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Multiply expression)
        {
            double argumentsProduct = 1.0;
            List<INumericExpression> newArguments = new List<INumericExpression>();

            foreach (var argument in expression.Children)
            {
                var transformedArgument = argument.Accept(this);

                Number numberArgument = transformedArgument as Number;
                if (numberArgument != null)
                {
                    argumentsProduct *= numberArgument.Value;
                }
                else
                {
                    newArguments.Add(transformedArgument);
                }
            }

            if (newArguments.Count == 0)
            {
                return new Number(argumentsProduct);
            }
            else if (argumentsProduct != 1.0)
            {
                newArguments.Add(new Number(argumentsProduct));
            }

            return new Multiply(newArguments);
        }

        /// <summary>
        /// Transforms the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Transformed numeric expression.</returns>
        public INumericExpression Visit(Divide expression)
        {
            var transformedLeftArgument = expression.LeftChild.Accept(this);
            var transformedRightArgument = expression.RightChild.Accept(this);

            Number numberLeftArgument = transformedLeftArgument as Number;
            Number numberRightArgument = transformedRightArgument as Number;

            if (numberLeftArgument != null && numberRightArgument != null)
            {
                return new Number(numberLeftArgument.Value / numberRightArgument.Value);
            }

            return new Divide(transformedLeftArgument, transformedRightArgument);
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
            IAtom functionAtom = GroundingManager.GroundAtom(expression.FunctionAtom, ExpressionSubstitution);

            if (!ReplacedFunctionAtomsInSubExpression.Contains(functionAtom))
            {
                INumericExpression substituedValue = null;
                if (NumericFunctionAssignments.TryGetValue(functionAtom, out substituedValue))
                {
                    ReplacedFunctionAtomsInSubExpression.Add(functionAtom);
                    ExpressionSubstitution.AddLocalSubstitution(OperatorSubstitution);

                    INumericExpression transformedValue = substituedValue.Accept(this);

                    ExpressionSubstitution.RemoveLocalSubstitution(OperatorSubstitution);
                    ReplacedFunctionAtomsInSubExpression.Remove(functionAtom);

                    return transformedValue;
                }
            }

            return expression.Clone();
        }
    }
}
