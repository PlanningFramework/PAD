using System.Collections.Generic;
using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general effect. Can be either an and-expression of c-effects, or a c-effect.
    /// </summary>
    public abstract class EffectAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing an and-expression of c-effects.
    /// </summary>
    public class AndCEffectsAstNode : EffectAstNode
    {
        /// <summary>
        /// List of c-effects in the expression.
        /// </summary>
        public List<CEffectAstNode> Arguments { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<CEffectAstNode>(1);
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
    /// AST node representing a c-effect. Can be one of the specific c-effect expressions (forall, when, cond-effect).
    /// </summary>
    public abstract class CEffectAstNode : EffectAstNode
    {
    }

    /// <summary>
    /// AST node representing a forall-expression for c-effect.
    /// </summary>
    public class ForallCEffectAstNode : CEffectAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; } = null;

        /// <summary>
        /// Argument effect.
        /// </summary>
        public EffectAstNode Effect { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
            Effect = treeNode.GetChildAst<EffectAstNode>(2);
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
    /// AST node representing a when-expression for c-effect.
    /// </summary>
    public class WhenCEffectAstNode : CEffectAstNode
    {
        /// <summary>
        /// Condition of the expression.
        /// </summary>
        public GDAstNode Condition { get; private set; } = null;

        /// <summary>
        /// Effect of the expression.
        /// </summary>
        public CondEffectAstNode Effect { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Condition = treeNode.GetChildAst<GDAstNode>(1);
            Effect = treeNode.GetChildAst<CondEffectAstNode>(2);  
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
    /// AST node representing a cond-effect. Can be either an and-expression of p-effects, or a p-effect.
    /// </summary>
    public abstract class CondEffectAstNode : CEffectAstNode
    {
    }
}
