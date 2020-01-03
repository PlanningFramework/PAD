using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Logical 'equals' expression, i.e. (= termA termB).
    /// </summary>
    public class EqualsExpression : IExpression
    {
        /// <summary>
        /// Left argument of the expression.
        /// </summary>
        public ITerm LeftArgument { set; get; }

        /// <summary>
        /// Right argument of the expression.
        /// </summary>
        public ITerm RightArgument { set; get; }

        /// <summary>
        /// Constructs the EQUALS expression.
        /// </summary>
        /// <param name="leftArgument">Left argument.</param>
        /// <param name="rightArgument">Right argument.</param>
        public EqualsExpression(ITerm leftArgument, ITerm rightArgument)
        {
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(= {LeftArgument} {RightArgument})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("equals", LeftArgument, RightArgument);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            EqualsExpression other = obj as EqualsExpression;
            if (other == null)
            {
                return false;
            }
            return LeftArgument.Equals(other.LeftArgument) && RightArgument.Equals(other.RightArgument);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            return new EqualsExpression(LeftArgument.Clone(), RightArgument.Clone());
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
    }
}
