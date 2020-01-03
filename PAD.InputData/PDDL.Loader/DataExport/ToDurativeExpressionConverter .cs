using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a durative expression structure.
    /// </summary>
    public class ToDurativeExpressionConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public DurativeExpression ExpressionData { get; private set; }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndDaGdAstNode astNode)
        {
            AndDurativeExpression andExpression = new AndDurativeExpression();
            astNode.Arguments.ForEach(arg => andExpression.Arguments.Add(MasterExporter.ToDurativeExpression(arg)));
            ExpressionData = andExpression;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallDaGdAstNode astNode)
        {
            ExpressionData = new ForallDurativeExpression(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToDurativeExpression(astNode.Expression));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtTimedDaGdAstNode astNode)
        {
            ExpressionData = new AtTimedExpression(astNode.TimeSpecifier, MasterExporter.ToExpression(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(OverTimedDaGdAstNode astNode)
        {
            ExpressionData = new OverTimedExpression(astNode.IntervalSpecifier, MasterExporter.ToExpression(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PreferenceDaGdAstNode astNode)
        {
            ExpressionData = new PreferencedTimedExpression(astNode.Name, (TimedExpression)MasterExporter.ToDurativeExpression(astNode.Argument));
        }
    }
}
