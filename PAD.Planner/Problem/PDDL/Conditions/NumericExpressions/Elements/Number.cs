
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expression - numeric constant.
    /// </summary>
    public class Number : INumericExpression
    {
        /// <summary>
        /// Numeric constant value.
        /// </summary>
        public double Value { set; get; } = 0.0;

        /// <summary>
        /// Constructs the numeric constant numeric expression.
        /// </summary>
        /// <param name="constVal">Numeric constant value.</param>
        public Number(double value)
        {
            Value = value;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Number other = obj as Number;
            if (other == null)
            {
                return false;
            }
            return (Value == other.Value);
        }

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            return new Number(Value);
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            return Value;
        }

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(INumericExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor transforming numeric expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public INumericExpression Accept(INumericExpressionTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
