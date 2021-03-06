﻿using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Logical 'not' expression.
    /// </summary>
    public class NotExpression : IExpression
    {
        /// <summary>
        /// Child expression to be negated.
        /// </summary>
        public IExpression Child { set; get; }

        /// <summary>
        /// Constructs the NOT expression.
        /// </summary>
        /// <param name="child">An argument of the expression.</param>
        public NotExpression(IExpression child)
        {
            Child = child;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(not {Child})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("not", Child);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            NotExpression other = obj as NotExpression;
            if (other == null)
            {
                return false;
            }
            return Child.Equals(other.Child);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            return new NotExpression(Child.Clone());
        }

        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        public bool Accept(IExpressionEvalVisitor visitor)
        {
            return !Child.Accept(visitor);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<int, int> Accept(IExpressionPropCountVisitor visitor)
        {
            var childPropCounts = Child.Accept(visitor);
            return Tuple.Create(childPropCounts.Item2, childPropCounts.Item1);
        }

        /// <summary>
        /// Accepts a visitor evaluating some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property evaluation visitor.</param>
        /// <returns>Result value of expression evaluation (fulfilling and non-fulfilling specified condition).</returns>
        public Tuple<double, double> Accept(IExpressionPropEvalVisitor visitor)
        {
            var childPropCounts = Child.Accept(visitor);
            return Tuple.Create(childPropCounts.Item2, childPropCounts.Item1);
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
