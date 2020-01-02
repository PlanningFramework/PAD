using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a derived predicate. Aggregates the predicate specification and the corresponding expression.
    /// </summary>
    public class DomainDerivedPredAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// Predicate specification (predicate name, list of typed arguments).
        /// </summary>
        public PredicateSkeletonAstNode Predicate { get; private set; } = null;

        /// <summary>
        /// Expression for the derived predicate.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Predicate = treeNode.GetChildAst<PredicateSkeletonAstNode>(1);
            Expression = treeNode.GetChildAst<GDAstNode>(2);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Predicate, visitor);
            SafeAccept(Expression, visitor);
        }
    }

    /// <summary>
    /// AST node representing a predicate specification. Aggregates the predicate name and the list of typed arguments.
    /// </summary>
    public class PredicateSkeletonAstNode : BaseAstNode
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// List of typed arguments.
        /// </summary>
        public TypedListAstNode Arguments { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(0);
            Arguments = treeNode.GetChildAst<TypedListAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Arguments, visitor);
        }
    }
}
