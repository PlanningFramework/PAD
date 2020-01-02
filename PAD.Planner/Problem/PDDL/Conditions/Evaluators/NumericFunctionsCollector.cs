using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Collector of numeric functions used in the specified numeric expression.
    /// </summary>
    public class NumericFunctionsCollector : BaseNumericExpressionVisitor
    {
        /// <summary>
        /// Collected numeric functions used in the expression.
        /// </summary>
        private HashSet<NumericFunction> NumericFunctions { set; get; } = new HashSet<NumericFunction>();

        /// <summary>
        /// Collects used numeric functions in the given numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        /// <returns>Collected numeric functions.</returns>
        public HashSet<NumericFunction> Collect(INumericExpression expression)
        {
            Debug.Assert(NumericFunctions.Count == 0);
            NumericFunctions.Clear();

            expression.Accept(this);

            HashSet<NumericFunction> result = NumericFunctions;
            NumericFunctions = new HashSet<NumericFunction>();

            return result;
        }

        /// <summary>
        /// Visits numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        public override void Visit(NumericFunction expression)
        {
            NumericFunctions.Add(expression);
        }
    }
}
