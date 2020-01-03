using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general GD expression. Can be one of the specific GD expressions (and, or, not, imply, exists, forall,
    /// numeric comparisons, preference, predicate or equals-expression).
    /// </summary>
    public abstract class GdAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a preference-expression for GD.
    /// </summary>
    public class PreferenceGdAstNode : GdAstNode
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Preference argument.
        /// </summary>
        public GdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
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
    /// AST node representing a predicate expression for GD.
    /// </summary>
    public class PredicateGdAstNode : GdAstNode
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Predicate terms.
        /// </summary>
        public List<TermAstNode> Terms { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(0);
            Terms = treeNode.GetChildAstList<TermAstNode>(1);
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
    /// AST node representing an equals-expression for GD.
    /// </summary>
    public class EqualsOpGdAstNode : GdAstNode
    {
        /// <summary>
        /// First argument of the expression.
        /// </summary>
        public TermOrNumericAstNode Argument1 { get; private set; }

        /// <summary>
        /// Second argument of the expression.
        /// </summary>
        public TermOrNumericAstNode Argument2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Argument1 = treeNode.GetChildAst<TermOrNumericAstNode>(1);
            Argument2 = treeNode.GetChildAst<TermOrNumericAstNode>(2);
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
    /// AST node representing an and-expression for GD.
    /// </summary>
    public class AndGdAstNode : GdAstNode
    {
        /// <summary>
        /// List of arguments for the expression.
        /// </summary>
        public List<GdAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<GdAstNode>(1);
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
    /// AST node representing an or-expression for GD.
    /// </summary>
    public class OrGdAstNode : GdAstNode
    {
        /// <summary>
        /// List of arguments for the expression.
        /// </summary>
        public List<GdAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<GdAstNode>(1);
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
    /// AST node representing a not-expression for GD.
    /// </summary>
    public class NotGdAstNode : GdAstNode
    {
        /// <summary>
        /// Argument for the expression.
        /// </summary>
        public GdAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Argument = treeNode.GetChildAst<GdAstNode>(1);
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
    /// AST node representing an imply-expression for GD.
    /// </summary>
    public class ImplyGdAstNode : GdAstNode
    {
        /// <summary>
        /// First argument of the expression.
        /// </summary>
        public GdAstNode Argument1 { get; private set; }

        /// <summary>
        /// Second argument of the expression.
        /// </summary>
        public GdAstNode Argument2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Argument1 = treeNode.GetChildAst<GdAstNode>(1);
            Argument2 = treeNode.GetChildAst<GdAstNode>(2);
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
    /// AST node representing an exists-expression for GD.
    /// </summary>
    public class ExistsGdAstNode : GdAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; }

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
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
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
    /// AST node representing a forall-expression for GD.
    /// </summary>
    public class ForallGdAstNode : GdAstNode
    {
        /// <summary>
        /// List of typed parameters.
        /// </summary>
        public TypedListAstNode Parameters { get; private set; }

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
            Parameters = treeNode.GetChildAst<TypedListAstNode>(1);
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
    /// AST node representing a numeric comparison expression for GD.
    /// </summary>
    public class NumCompGdAstNode : GdAstNode
    {
        /// <summary>
        /// Numeric comparison operator.
        /// </summary>
        public NumericComparer Operator { get; private set; } = NumericComparer.LT;

        /// <summary>
        /// First argument of the expression.
        /// </summary>
        public TermOrNumericAstNode Argument1 { get; private set; }

        /// <summary>
        /// Second argument of the expression.
        /// </summary>
        public TermOrNumericAstNode Argument2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Operator = EnumMapper.ToNumericComparer(treeNode.GetChildString(0));
            Argument1 = treeNode.GetChildAst<TermOrNumericAstNode>(1);
            Argument2 = treeNode.GetChildAst<TermOrNumericAstNode>(2);
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
