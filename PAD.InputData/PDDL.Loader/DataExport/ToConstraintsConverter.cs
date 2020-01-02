using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a constraints structure.
    /// </summary>
    public class ToConstraintsConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Constraints ConstraintsData { get; private set; } = new Constraints();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndConGDAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => ConstraintsData.AddRange(MasterExporter.ToConstraints(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallConGDAstNode astNode)
        {
            ConstraintsData.Add(new ForallConstraint(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToConstraints(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtEndConGDAstNode astNode)
        {
            ConstraintsData.Add(new AtEndConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AlwaysConGDAstNode astNode)
        {
            ConstraintsData.Add(new AlwaysConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeConGDAstNode astNode)
        {
            ConstraintsData.Add(new SometimeConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(WithinConGDAstNode astNode)
        {
            ConstraintsData.Add(new WithinConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtMostOnceConGDAstNode astNode)
        {
            ConstraintsData.Add(new AtMostOnceConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeAfterConGDAstNode astNode)
        {
            ConstraintsData.Add(new SometimeAfterConstraint(MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeBeforeConGDAstNode astNode)
        {
            ConstraintsData.Add(new SometimeBeforeConstraint(MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AlwaysWithinConGDAstNode astNode)
        {
            ConstraintsData.Add(new AlwaysWithinConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(HoldDuringConGDAstNode astNode)
        {
            ConstraintsData.Add(new HoldDuringConstraint(astNode.Number1, astNode.Number2, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(HoldAfterConGDAstNode astNode)
        {
            ConstraintsData.Add(new HoldAfterConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PreferenceConGDAstNode astNode)
        {
            ConstraintsData.Add(new PreferenceConstraint(astNode.Name, MasterExporter.ToConstraints(astNode.Argument)));
        }
    }
}
