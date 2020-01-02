
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric 'divide' expression.
    /// </summary>
    public class Divide : INumericExpression
    {
        /// <summary>
        /// Left numeric expression.
        /// </summary>
        public INumericExpression LeftChild { set; get; } = null;

        /// <summary>
        /// Right numeric expression.
        /// </summary>
        public INumericExpression RightChild { set; get; } = null;

        /// <summary>
        /// Constructs the "divide" numeric expression.
        /// </summary>
        /// <param name="leftChild">Left numeric expression.</param>
        /// <param name="rightChild">Right numeric expression.</param>
        public Divide(INumericExpression leftChild, INumericExpression rightChild)
        {
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(/ {LeftChild} {RightChild})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("/", LeftChild, RightChild);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Divide other = obj as Divide;
            if (other == null)
            {
                return false;
            }
            return LeftChild.Equals(other.LeftChild) && RightChild.Equals(other.RightChild);
        }

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            return new Divide(LeftChild.Clone(), RightChild.Clone());
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            return LeftChild.Accept(visitor) / RightChild.Accept(visitor);
        }

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(INumericExpressionVisitor visitor)
        {
            LeftChild.Accept(visitor);
            RightChild.Accept(visitor);
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
