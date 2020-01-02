using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a durative constraints structure.
    /// </summary>
    public class ToDurativeConstraintsConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public DurativeConstraints ConstraintsData { get; private set; } = new DurativeConstraints();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndSimpleDurationConstraintsAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => ConstraintsData.AddRange(MasterExporter.ToDurativeConstraints(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtSimpleDurationConstraintAstNode astNode)
        {
            ConstraintsData.Add(new AtDurativeConstraint(astNode.TimeSpecifier, MasterExporter.ToDurativeConstraints(astNode.DurationConstraint)[0]));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(CompOpSimpleDurationConstraintAstNode astNode)
        {
            ConstraintsData.Add(new CompareDurativeConstraint(astNode.DurationComparer, MasterExporter.ToNumericExpression(astNode.DurationArgument)));
        }
    }
}
