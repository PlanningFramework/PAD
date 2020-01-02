using PAD.InputData.PDDL.Loader.Ast;

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
        public DurativeExpression ExpressionData { get; private set; } = null;

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndDaGDAstNode astNode)
        {
            AndDurativeExpression andExpression = new AndDurativeExpression();
            astNode.Arguments.ForEach(arg => andExpression.Arguments.Add(MasterExporter.ToDurativeExpression(arg)));
            ExpressionData = andExpression;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallDaGDAstNode astNode)
        {
            ExpressionData = new ForallDurativeExpression(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToDurativeExpression(astNode.Expression));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtTimedDaGDAstNode astNode)
        {
            ExpressionData = new AtTimedExpression(astNode.TimeSpecifier, MasterExporter.ToExpression(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(OverTimedDaGDAstNode astNode)
        {
            ExpressionData = new OverTimedExpression(astNode.IntervalSpecifier, MasterExporter.ToExpression(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PreferenceDaGDAstNode astNode)
        {
            ExpressionData = new PreferencedTimedExpression(astNode.Name, (TimedExpression)MasterExporter.ToDurativeExpression(astNode.Argument));
        }
    }
}
