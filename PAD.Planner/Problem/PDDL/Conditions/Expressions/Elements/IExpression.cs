using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface representing PDDL logical expressions.
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        bool Accept(IExpressionEvalVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        Tuple<int, int> Accept(IExpressionPropCountVisitor visitor);

        /// <summary>
        /// Accepts a visitor counting some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        Tuple<double, double> Accept(IExpressionPropEvalVisitor visitor);

        /// <summary>
        /// Accepts an expression transformation visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        IExpression Accept(IExpressionTransformVisitor visitor);

        /// <summary>
        /// Accepts a generic expression visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        void Accept(IExpressionVisitor visitor);

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        IExpression Clone();
    }
}
