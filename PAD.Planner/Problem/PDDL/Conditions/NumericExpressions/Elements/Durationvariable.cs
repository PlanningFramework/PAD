
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expression - duration variable.
    /// </summary>
    public class DurationVariable : INumericExpression
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "?duration";
        }
        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("duration");
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            DurationVariable other = obj as DurationVariable;
            if (other == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            return new DurationVariable();
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            return visitor.Visit(this);
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
