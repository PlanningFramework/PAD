using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a duration-action in the domain.
    /// </summary>
    public class DomainDurActionAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// Action name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Action parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; } = null;

        /// <summary>
        /// Action duration constraints.
        /// </summary>
        public DurationConstraintAstNode DurationConstraint { get; private set; } = null;

        /// <summary>
        /// Action condition.
        /// </summary>
        public DaGDAstNode Condition { get; private set; } = null;

        /// <summary>
        /// Action effect.
        /// </summary>
        public DaEffectAstNode Effect { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
            Parameters = treeNode.GetGrandChildAst<TypedListAstNode>(2, 1);
            DurationConstraint = treeNode.GetGrandChildAst<DurationConstraintAstNode>(3, 1);
            Condition = treeNode.GetGrandChildAst<DaGDAstNode>(4, 1);
            Effect = treeNode.GetGrandChildAst<DaEffectAstNode>(5, 1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Parameters, visitor);
            SafeAccept(DurationConstraint, visitor);
            SafeAccept(Condition, visitor);
            SafeAccept(Effect, visitor);
        }
    }
}
