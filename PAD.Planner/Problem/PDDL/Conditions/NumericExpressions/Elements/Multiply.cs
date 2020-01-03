using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric 'multiply' expression.
    /// </summary>
    public class Multiply : INumericExpression
    {
        /// <summary>
        /// Children numeric expressions.
        /// </summary>
        public List<INumericExpression> Children { set; get; }

        /// <summary>
        /// Constructs the "multiply" numeric expression.
        /// </summary>
        /// <param name="leftChild">Left numeric expression.</param>
        /// <param name="rightChild">Right numeric expression.</param>
        public Multiply(INumericExpression leftChild, INumericExpression rightChild)
        {
            Children = new List<INumericExpression> {leftChild, rightChild};
        }

        /// <summary>
        /// Constructs the "multiply" numeric expression.
        /// </summary>
        /// <param name="children">Children numeric expressions.</param>
        public Multiply(List<INumericExpression> children)
        {
            Children = children;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> childrenStrings = new List<string>();
            Children.ForEach(child => childrenStrings.Add(child.ToString()));
            return $"(* {string.Join(" ", childrenStrings)})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Children).CombineHashCode("*");
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Multiply other = obj as Multiply;
            if (other == null)
            {
                return false;
            }
            return CollectionsEquality.Equals(Children, other.Children);
        }

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            List<INumericExpression> newChildren = new List<INumericExpression>();
            foreach (var child in Children)
            {
                newChildren.Add(child.Clone());
            }
            return new Multiply(newChildren);
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            double product = 1.0;
            foreach (var child in Children)
            {
                product *= child.Accept(visitor);
            }
            return product;
        }

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(INumericExpressionVisitor visitor)
        {
            foreach (var child in Children)
            {
                child.Accept(visitor);
            }
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
