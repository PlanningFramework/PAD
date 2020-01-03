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
        public Constraints ConstraintsData { get; } = new Constraints();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndConGdAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => ConstraintsData.AddRange(MasterExporter.ToConstraints(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallConGdAstNode astNode)
        {
            ConstraintsData.Add(new ForallConstraint(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToConstraints(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtEndConGdAstNode astNode)
        {
            ConstraintsData.Add(new AtEndConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AlwaysConGdAstNode astNode)
        {
            ConstraintsData.Add(new AlwaysConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeConGdAstNode astNode)
        {
            ConstraintsData.Add(new SometimeConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(WithinConGdAstNode astNode)
        {
            ConstraintsData.Add(new WithinConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtMostOnceConGdAstNode astNode)
        {
            ConstraintsData.Add(new AtMostOnceConstraint(MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeAfterConGdAstNode astNode)
        {
            ConstraintsData.Add(new SometimeAfterConstraint(MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(SometimeBeforeConGdAstNode astNode)
        {
            ConstraintsData.Add(new SometimeBeforeConstraint(MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AlwaysWithinConGdAstNode astNode)
        {
            ConstraintsData.Add(new AlwaysWithinConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression1), MasterExporter.ToExpression(astNode.Expression2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(HoldDuringConGdAstNode astNode)
        {
            ConstraintsData.Add(new HoldDuringConstraint(astNode.Number1, astNode.Number2, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(HoldAfterConGdAstNode astNode)
        {
            ConstraintsData.Add(new HoldAfterConstraint(astNode.Number, MasterExporter.ToExpression(astNode.Expression)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PreferenceConGdAstNode astNode)
        {
            ConstraintsData.Add(new PreferenceConstraint(astNode.Name, MasterExporter.ToConstraints(astNode.Argument)));
        }
    }
}
