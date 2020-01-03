using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Logical 'imply' expression.
    /// </summary>
    public class ImplyExpression : IExpression
    {
        /// <summary>
        /// Left child expression.
        /// </summary>
        public IExpression LeftChild { set; get; }

        /// <summary>
        /// Right child expression.
        /// </summary>
        public IExpression RightChild { set; get; }

        /// <summary>
        /// Constructs the IMPLY expression.
        /// </summary>
        /// <param name="leftChild">Left argument of the expression.</param>
        /// <param name="rightChild">Right argument of the expression.</param>
        public ImplyExpression(IExpression leftChild, IExpression rightChild)
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
            return $"(imply {LeftChild} {RightChild})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("imply", LeftChild, RightChild);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            ImplyExpression other = obj as ImplyExpression;
            if (other == null)
            {
                return false;
            }
            return LeftChild.Equals(other.LeftChild) && RightChild.Equals(other.RightChild);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            return new ImplyExpression(LeftChild.Clone(), RightChild.Clone());
        }

        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        public bool Accept(IExpressionEvalVisitor visitor)
        {
            // (a imply b) ~ (not(a) or b)
            return !LeftChild.Accept(visitor) || RightChild.Accept(visitor);
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
