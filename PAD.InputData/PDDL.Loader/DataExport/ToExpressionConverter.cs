using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into an expression structure.
    /// </summary>
    public class ToExpressionConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Expression ExpressionData { get; private set; } = null;

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndGDAstNode astNode)
        {
            AndExpression andExpression = new AndExpression();
            astNode.Arguments.ForEach(arg => andExpression.Arguments.Add(MasterExporter.ToExpression(arg)));
            ExpressionData = andExpression;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(OrGDAstNode astNode)
        {
            OrExpression orExpression = new OrExpression();
            astNode.Arguments.ForEach(arg => orExpression.Arguments.Add(MasterExporter.ToExpression(arg)));
            ExpressionData = orExpression;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NotGDAstNode astNode)
        {
            ExpressionData = new NotExpression(MasterExporter.ToExpression(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ImplyGDAstNode astNode)
        {
            ExpressionData = new ImplyExpression(MasterExporter.ToExpression(astNode.Argument1), MasterExporter.ToExpression(astNode.Argument2));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ExistsGDAstNode astNode)
        {
            ExpressionData = new ExistsExpression(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToExpression(astNode.Expression));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallGDAstNode astNode)
        {
            ExpressionData = new ForallExpression(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToExpression(astNode.Expression));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PredicateGDAstNode astNode)
        {
            PredicateExpression predicate = new PredicateExpression(astNode.Name);
            astNode.Terms.ForEach(term => predicate.Terms.Add(MasterExporter.ToTerm(term)));
            ExpressionData = predicate;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(EqualsOpGDAstNode astNode)
        {
            if (MasterExporter.IsNumericExpression(astNode.Argument1) && MasterExporter.IsNumericExpression(astNode.Argument2))
            {
                ExpressionData = new NumericCompareExpression(Traits.NumericComparer.EQ, MasterExporter.ToNumericExpression(astNode.Argument1), MasterExporter.ToNumericExpression(astNode.Argument2));
            }
            else
            {
                ExpressionData = new EqualsExpression(MasterExporter.ToTerm(astNode.Argument1), MasterExporter.ToTerm(astNode.Argument2));
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NumCompGDAstNode astNode)
        {
            ExpressionData = new NumericCompareExpression(astNode.Operator, MasterExporter.ToNumericExpression(astNode.Argument1), MasterExporter.ToNumericExpression(astNode.Argument2));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PreferenceGDAstNode astNode)
        {
            ExpressionData = new PreferenceExpression(astNode.Name, MasterExporter.ToExpression(astNode.Argument));
        }
    }
}
