using System.Diagnostics;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a metric expression structure.
    /// </summary>
    public class ToMetricExpressionConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public MetricExpression ExpressionData { get; private set; }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(IdentifierTermAstNode astNode)
        {
            if (astNode.Name.Equals("total-time"))
            {
                ExpressionData = new MetricTotalTime();
            }
            else
            {
                ExpressionData = new MetricNumericFunction(astNode.Name);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(FunctionTermAstNode astNode)
        {
            if (astNode.Name.Equals("total-time"))
            {
                ExpressionData = new MetricTotalTime();
            }
            else
            {
                MetricNumericFunction function = new MetricNumericFunction(astNode.Name);
                astNode.Terms.ForEach(term => function.Terms.Add(MasterExporter.ToConstantTerm(term)));
                ExpressionData = function;
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NumberTermAstNode astNode)
        {
            ExpressionData = new MetricNumber(astNode.Number);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(MetricPreferenceViolationAstNode astNode)
        {
            ExpressionData = new MetricPreferenceViolation(astNode.Name);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NumericOpAstNode astNode)
        {
            MetricExpressions arguments = new MetricExpressions();
            astNode.Arguments.ForEach(arg => arguments.Add(MasterExporter.ToMetricExpression(arg)));

            switch (astNode.Operator)
            {
                case Traits.NumericOperator.PLUS:
                {
                    ExpressionData = new MetricPlus(arguments);
                    break;
                }
                case Traits.NumericOperator.MINUS:
                {
                    if (arguments.Count == 1)
                    {
                        ExpressionData = new MetricUnaryMinus(arguments[0]);
                    }
                    else
                    {
                        ExpressionData = new MetricMinus(arguments[0], arguments[1]);
                    }
                    break;
                }
                case Traits.NumericOperator.MUL:
                {
                    ExpressionData = new MetricMultiply(arguments);
                    break;
                }
                case Traits.NumericOperator.DIV:
                {
                    ExpressionData = new MetricDivide(arguments[0], arguments[1]);
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }
        }
    }
}
