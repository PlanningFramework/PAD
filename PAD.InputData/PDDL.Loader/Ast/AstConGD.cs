using System.Collections.Generic;
using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general con-GD expression. Can be one of the specific con-GD expressions (preference, and, forall, at-end,
    /// always, sometimes, within, at-most-once, sometime-after, sometime-before, always-within, hold-during, hold-after).
    /// </summary>
    public abstract class ConGDAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a preference-expression for con-GD.
    /// </summary>
    public class PreferenceConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Preference argument.
        /// </summary>
        public ConGDAstNode Argument { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
            Argument = treeNode.GetChildAst<ConGDAstNode>(2);
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
    public class AndConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// List of arguments for the expression.
        /// </summary>
        public List<ConGDAstNode> Arguments { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<ConGDAstNode>(1);
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
    public class ForallConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; } = null;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public ConGDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
            Expression = treeNode.GetChildAst<ConGDAstNode>(2);
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
    public class AtEndConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GDAstNode>(2);
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
    public class AlwaysConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GDAstNode>(1);
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
    public class SometimeConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GDAstNode>(1);
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
    public class WithinConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression = treeNode.GetChildAst<GDAstNode>(2);
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
    public class AtMostOnceConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<GDAstNode>(1);
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
    public class SometimeAfterConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public GDAstNode Expression1 { get; private set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GDAstNode Expression2 { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression1 = treeNode.GetChildAst<GDAstNode>(1);
            Expression2 = treeNode.GetChildAst<GDAstNode>(2);
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
    public class SometimeBeforeConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public GDAstNode Expression1 { get; private set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GDAstNode Expression2 { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression1 = treeNode.GetChildAst<GDAstNode>(1);
            Expression2 = treeNode.GetChildAst<GDAstNode>(2);
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
    public class AlwaysWithinConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; } = 0.0;

        /// <summary>
        /// First argument expression.
        /// </summary>
        public GDAstNode Expression1 { get; private set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public GDAstNode Expression2 { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression1 = treeNode.GetChildAst<GDAstNode>(2);
            Expression2 = treeNode.GetChildAst<GDAstNode>(3);
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
    public class HoldDuringConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// First argument number.
        /// </summary>
        public double Number1 { get; private set; } = 0.0;

        /// <summary>
        /// Second argument number.
        /// </summary>
        public double Number2 { get; private set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number1 = treeNode.GetChildNumberVal(1);
            Number2 = treeNode.GetChildNumberVal(2);
            Expression = treeNode.GetChildAst<GDAstNode>(3);
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
    public class HoldAfterConGDAstNode : ConGDAstNode
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; private set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public GDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Expression = treeNode.GetChildAst<GDAstNode>(2);
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
