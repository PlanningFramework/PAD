using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing an action in the domain.
    /// </summary>
    public class DomainActionAstNode : DomainSectionAstNode
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
        /// Action preconditions.
        /// </summary>
        public GDAstNode Preconditions { get; private set; } = null;

        /// <summary>
        /// Action effects.
        /// </summary>
        public EffectAstNode Effects { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
            Parameters = treeNode.GetGrandChildAst<TypedListAstNode>(2, 1);
            Preconditions = treeNode.GetGrandChildAst<GDAstNode>(3, 1);
            Effects = treeNode.GetGrandChildAst<EffectAstNode>(4, 1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Parameters, visitor);
            SafeAccept(Preconditions, visitor);
            SafeAccept(Effects, visitor);
        }
    }
}
