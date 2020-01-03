using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general duration-constraint. Can be either an and-expression of simple-duration-constraints,
    /// or a simple-duration-constraint.
    /// </summary>
    public abstract class DurationConstraintAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing an and-expression of simple-duration-constraints.
    /// </summary>
    public class AndSimpleDurationConstraintsAstNode : DurationConstraintAstNode
    {
        /// <summary>
        /// List of simple-duration-constraints.
        /// </summary>
        public List<SimpleDurationConstraintAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<SimpleDurationConstraintAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// AST node representing a simple-duration-constraint. Can be either an at-expression, or a comparison-expression.
    /// </summary>
    public abstract class SimpleDurationConstraintAstNode : DurationConstraintAstNode
    {
    }

    /// <summary>
    /// AST node representing an at-expression for simple-duration-constraint.
    /// </summary>
    public class AtSimpleDurationConstraintAstNode : SimpleDurationConstraintAstNode
    {
        /// <summary>
        /// Time specifier of the expression.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; private set; } = TimeSpecifier.START;

        /// <summary>
        /// Argument duration constraint of the expression.
        /// </summary>
        public SimpleDurationConstraintAstNode DurationConstraint { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            TimeSpecifier = EnumMapper.ToTimeSpecifier(treeNode.GetChildString(1));
            DurationConstraint = treeNode.GetChildAst<SimpleDurationConstraintAstNode>(2);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// AST node representing a comparison-expression for simple-duration-constraint.
    /// </summary>
    public class CompOpSimpleDurationConstraintAstNode : SimpleDurationConstraintAstNode
    {
        /// <summary>
        /// Duration comparer for the expression.
        /// </summary>
        public DurationComparer DurationComparer { get; private set; } = DurationComparer.EQ;

        /// <summary>
        /// Duration argument of the expression.
        /// </summary>
        public TermOrNumericAstNode DurationArgument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            DurationComparer = EnumMapper.ToDurationComparer(treeNode.GetChildString(0));
            DurationArgument = treeNode.GetChildAst<TermOrNumericAstNode>(2);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
