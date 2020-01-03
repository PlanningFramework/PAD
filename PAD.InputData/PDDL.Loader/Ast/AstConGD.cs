using System.Collections.Generic;
using Irony.Parsing;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general con-GD expression. Can be one of the specific con-GD expressions (preference, and, forall, at-end,
    /// always, sometimes, within, at-most-once, sometime-after, sometime-before, always-within, hold-during, hold-after).
    /// </summary>
    public abstract class ConGdAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a preference-expression for con-GD.
    /// </summary>
    public class PreferenceConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Preference argument.
        /// </summary>
        public ConGdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
            Argument = treeNode.GetChildAst<ConGdAstNode>(2);
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
    /// AST node representing an and-expression for con-GD.
    /// </summary>
    public class AndConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// List of arguments for the expression.
        /// </summary>
        public List<ConGdAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<ConGdAstNode>(1);
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
    /// AST node representing a forall-expression for con-GD.
    /// </summary>
    public class ForallConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public ConGdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
            Expression = treeNode.GetChildAst<ConGdAstNode>(2);
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
    /// AST node representing a at-end-expression for con-GD.
    /// </summary>
    public class AtEndConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing a always-expression for con-GD.
    /// </summary>
    public class AlwaysConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GdAstNode>(1);
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
    /// AST node representing a sometime-expression for con-GD.
    /// </summary>
    public class SometimeConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GdAstNode>(1);
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
    /// AST node representing a within-expression for con-GD.
    /// </summary>
    public class WithinConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing a at-most-once-expression for con-GD.
    /// </summary>
    public class AtMostOnceConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GdAstNode>(1);
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
    /// AST node representing a sometime-after-expression for con-GD.
    /// </summary>
    public class SometimeAfterConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public GdAstNode Expression1 { get; private set; }

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GdAstNode Expression2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression1 = treeNode.GetChildAst<GdAstNode>(1);
            Expression2 = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing a sometime-before-expression for con-GD.
    /// </summary>
    public class SometimeBeforeConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public GdAstNode Expression1 { get; private set; }

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GdAstNode Expression2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression1 = treeNode.GetChildAst<GdAstNode>(1);
            Expression2 = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing a always-within-expression for con-GD.
    /// </summary>
    public class AlwaysWithinConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// First argument expression.
        /// </summary>
        public GdAstNode Expression1 { get; private set; }

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GdAstNode Expression2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression1 = treeNode.GetChildAst<GdAstNode>(2);
            Expression2 = treeNode.GetChildAst<GdAstNode>(3);
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
    /// AST node representing a hold-during-expression for con-GD.
    /// </summary>
    public class HoldDuringConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// First argument number.
        /// </summary>
        public double Number1 { get; private set; }

        /// <summary>
        /// Second argument number.
        /// </summary>
        public double Number2 { get; private set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number1 = treeNode.GetChildNumberVal(1);
            Number2 = treeNode.GetChildNumberVal(2);
            Expression = treeNode.GetChildAst<GdAstNode>(3);
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
    /// AST node representing a hold-after-expression for con-GD.
    /// </summary>
    public class HoldAfterConGdAstNode : ConGdAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression = treeNode.GetChildAst<GdAstNode>(2);
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
