using System.Diagnostics;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a numeric expression structure.
    /// </summary>
    public class ToNumericExpressionConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public NumericExpression ExpressionData { get; private set; }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(IdentifierTermAstNode astNode)
        {
            ExpressionData = new NumericFunction(astNode.Name);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(FunctionTermAstNode astNode)
        {
            NumericFunction function = new NumericFunction(astNode.Name);
            astNode.Terms.ForEach(term => function.Terms.Add(MasterExporter.ToTerm(term)));
            ExpressionData = function;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NumberTermAstNode astNode)
        {
            ExpressionData = new Number(astNode.Number);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DurationVariableTermAstNode astNode)
        {
            ExpressionData = new DurationVariable();
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NumericOpAstNode astNode)
        {
            NumericExpressions arguments = new NumericExpressions();
            astNode.Arguments.ForEach(arg => arguments.Add(MasterExporter.ToNumericExpression(arg)));

            switch (astNode.Operator)
            {
                case Traits.NumericOperator.PLUS:
                {
                    ExpressionData = new Plus(arguments);
                    break;
                }
                case Traits.NumericOperator.MINUS:
                {
                    if (arguments.Count == 1)
                    {
                        ExpressionData = new UnaryMinus(arguments[0]);
                    }
                    else
                    {
                        ExpressionData = new Minus(arguments[0], arguments[1]);
                    }
                    break;
                }
                case Traits.NumericOperator.MUL:
                {
                    ExpressionData = new Multiply(arguments);
                    break;
                }
                case Traits.NumericOperator.DIV:
                {
                    ExpressionData = new Divide(arguments[0], arguments[1]);
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
