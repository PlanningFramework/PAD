using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general da-GD expression (GD for durative actions). Can be one of the specific da-GD expressions (and,
    /// forall, preference, at-timed-expression or over-timed-expression)
    /// </summary>
    public abstract class DaGdAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing an and-expression for da-GD.
    /// </summary>
    public class AndDaGdAstNode : DaGdAstNode
    {
        /// <summary>
        /// List of arguments for the expression.
        /// </summary>
        public List<DaGdAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<DaGdAstNode>(1);
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
    /// AST node representing a forall-expression for da-GD.
    /// </summary>
    public class ForallDaGdAstNode : DaGdAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public DaGdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
            Expression = treeNode.GetChildAst<DaGdAstNode>(2);
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
    /// AST node representing a preference-expression for da-GD.
    /// </summary>
    public class PreferenceDaGdAstNode : DaGdAstNode
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Preference argument.
        /// </summary>
        public DaGdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
            Argument = treeNode.GetChildAst<DaGdAstNode>(2);
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
    /// AST node representing a at-timed-expression for da-GD.
    /// </summary>
    public class AtTimedDaGdAstNode : DaGdAstNode
    {
        /// <summary>
        /// Time specifier of the expression.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; private set; } = TimeSpecifier.START;

        /// <summary>
        /// Argument of the expression.
        /// </summary>
        public GdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            TimeSpecifier = EnumMapper.ToTimeSpecifier(treeNode.GetChildString(1));
            Argument = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing a over-timed-expression for da-GD.
    /// </summary>
    public class OverTimedDaGdAstNode : DaGdAstNode
    {
        /// <summary>
        /// Interval specifier of the expression.
        /// </summary>
        public IntervalSpecifier IntervalSpecifier { get; private set; } = IntervalSpecifier.ALL;

        /// <summary>
        /// Argument of the expression.
        /// </summary>
        public GdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            IntervalSpecifier = EnumMapper.ToIntervalSpecifier(treeNode.GetChildString(1));
            Argument = treeNode.GetChildAst<GdAstNode>(2);
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
