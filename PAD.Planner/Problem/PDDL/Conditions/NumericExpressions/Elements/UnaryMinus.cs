
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric 'umary minus' expression.
    /// </summary>
    public class UnaryMinus : INumericExpression
    {
        /// <summary>
        /// Child numeric expression.
        /// </summary>
        public INumericExpression Child { set; get; } = null;

        /// <summary>
        /// Constructs the "unary minus" numeric expression.
        /// </summary>
        /// <param name="child">Child numeric expression.</param>
        public UnaryMinus(INumericExpression child)
        {
            Child = child;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(- {Child})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("-", Child);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            UnaryMinus other = obj as UnaryMinus;
            if (other == null)
            {
                return false;
            }
            return Child.Equals(other.Child);
        }

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            return new UnaryMinus(Child.Clone());
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            return -(Child.Accept(visitor));
        }

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(INumericExpressionVisitor visitor)
        {
            Child.Accept(visitor);
            visitor.PostVisit(this);
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
