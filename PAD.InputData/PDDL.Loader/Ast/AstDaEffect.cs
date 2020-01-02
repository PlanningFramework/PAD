using System.Collections.Generic;
using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general da-effect (effect for durative actions). Can be one of the specific da-effect (and, forall,
    /// when, timed-effect).
    /// </summary>
    public abstract class DaEffectAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing an and-expression of da-effects.
    /// </summary>
    public class AndDaEffectsAstNode : DaEffectAstNode
    {
        /// <summary>
        /// List of da-effects in the expression.
        /// </summary>
        public List<DaEffectAstNode> Arguments { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<DaEffectAstNode>(1);
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
    /// AST node representing a forall-expression for da-effect.
    /// </summary>
    public class ForallDaEffectAstNode : DaEffectAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; } = null;

        /// <summary>
        /// Argument da-effect.
        /// </summary>
        public DaEffectAstNode Effect { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
            Effect = treeNode.GetChildAst<DaEffectAstNode>(2);
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
    /// AST node representing a when-expression for da-effect.
    /// </summary>
    public class WhenDaEffectAstNode : DaEffectAstNode
    {
        /// <summary>
        /// Condition of the expression.
        /// </summary>
        public DaGDAstNode Condition { get; private set; } = null;

        /// <summary>
        /// Effect of the expression.
        /// </summary>
        public TimedEffectAstNode Effect { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Condition = treeNode.GetChildAst<DaGDAstNode>(1);
            Effect = treeNode.GetChildAst<TimedEffectAstNode>(2);
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
