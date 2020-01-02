using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Logical 'or' expression.
    /// </summary>
    public class OrExpression : IExpression
    {
        /// <summary>
        /// Children expressions.
        /// </summary>
        public List<IExpression> Children { set; get; } = null;

        /// <summary>
        /// Constructs the OR expression.
        /// </summary>
        /// <param name="children">Arguments of the expression.</param>
        public OrExpression(List<IExpression> children)
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
            return $"(or {string.Join(" ", childrenStrings)})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Children).CombineHashCode("or");
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            OrExpression other = obj as OrExpression;
            if (other == null)
            {
                return false;
            }
            return CollectionsEquality.Equals(Children, other.Children);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            List<IExpression> newChildren = new List<IExpression>();
            foreach (var child in Children)
            {
                newChildren.Add(child.Clone());
            }
            return new OrExpression(newChildren);
        }

        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        public bool Accept(IExpressionEvalVisitor visitor)
        {
            foreach (var child in Children)
            {
                if (child.Accept(visitor))
                {
                    return true;
                }
            }
            return false;
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
