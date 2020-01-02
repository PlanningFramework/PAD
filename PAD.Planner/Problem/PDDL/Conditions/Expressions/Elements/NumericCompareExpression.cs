using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common class for the relational operator expressions.
    /// </summary>
    public class NumericCompareExpression : IExpression
    {
        /// <summary>
        /// Standard relational operators.
        /// </summary>
        public enum RelationalOperator { LT, LTE, GT, GTE, EQ }

        /// <summary>
        /// Compare operator.
        /// </summary>
        public RelationalOperator Operator { set; get; } = RelationalOperator.EQ;

        /// <summary>
        /// Left numeric expression.
        /// </summary>
        public INumericExpression LeftArgument { set; get; } = null;

        /// <summary>
        /// Right numeric expression.
        /// </summary>
        public INumericExpression RightArgument { set; get; } = null;

        /// <summary>
        /// Constructs the relation operator expression.
        /// </summary>
        /// <param name="relationalOperator">Relational operator.</param>
        /// <param name="leftArgument">Left numeric expression.</param>
        /// <param name="rightArgument">Right numeric expression.</param>
        public NumericCompareExpression(RelationalOperator relationalOperator, INumericExpression leftArgument, INumericExpression rightArgument)
        {
            Operator = relationalOperator;
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }

        /// <summary>
        /// Gets the string representation of the relational operator.
        /// </summary>
        /// <param name="relOperator">Relational operator.</param>
        /// <returns>String representation of the operator.</returns>
        public static string ToString(RelationalOperator relOperator)
        {
            switch (relOperator)
            {
                case RelationalOperator.LT:
                    return "<";
                case RelationalOperator.LTE:
                    return "<=";
                case RelationalOperator.GT:
                    return ">";
                case RelationalOperator.GTE:
                    return ">=";
                case RelationalOperator.EQ:
                    return "=";
                default:
                    Debug.Assert(false, "Unknown operator!");
                    return "?";
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({ToString(Operator)} {LeftArgument} {RightArgument})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Operator, LeftArgument, RightArgument);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            NumericCompareExpression other = obj as NumericCompareExpression;
            if (other == null)
            {
                return false;
            }
            return (Operator == other.Operator) && LeftArgument.Equals(other.LeftArgument) && RightArgument.Equals(other.RightArgument);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            return new NumericCompareExpression(Operator, LeftArgument.Clone(), RightArgument.Clone());
        }

        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        public bool Accept(IExpressionEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<int, int> Accept(IExpressionPropCountVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor evaluating some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property evaluation visitor.</param>
        /// <returns>Result value of expression evaluation (fulfilling and non-fulfilling specified condition).</returns>
        public Tuple<double, double> Accept(IExpressionPropEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts an expression transformation visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        public IExpression Accept(IExpressionTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic expression visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Applies the concrete relational operator to evaluate logical value.
        /// </summary>
        /// <param name="relOperator">Relational operator.</param>
        /// <param name="leftValue">Left value.</param>
        /// <param name="rightValue">Right value.</param>
        /// <returns>True if the concrete operation evaluates as true, false otherwise.</returns>
        public static bool ApplyCompare(RelationalOperator relOperator, double leftValue, double rightValue)
        {
            switch (relOperator)
            {
                case RelationalOperator.LT:
                    return (leftValue < rightValue);
                case RelationalOperator.LTE:
                    return (leftValue <= rightValue);
                case RelationalOperator.GT:
                    return (leftValue > rightValue);
                case RelationalOperator.GTE:
                    return (leftValue >= rightValue);
                case RelationalOperator.EQ:
                    return (leftValue == rightValue);
                default:
                    Debug.Assert(false, "Unknown operator!");
                    return false;
            }
        }
    }
}
