using System.Collections.Generic;
using Irony.Parsing;
using Irony.Ast;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general term or numeric expression (there are some elements that can act as both of these).
    /// </summary>
    public abstract class TermOrNumericAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a term.
    /// </summary>
    public abstract class TermAstNode : TermOrNumericAstNode
    {
    }

    /// <summary>
    /// AST node representing identifier term. Aggregates the name of the term and a check if it's variable.
    /// </summary>
    public class IdentifierTermAstNode : TermAstNode
    {
        /// <summary>
        /// Identifier name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Is the identifier variable?
        /// </summary>
        /// <returns>True if the identifier is a variable, false otherwise.</returns>
        public bool IsVariable() { return Name.StartsWith("?"); }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(0);
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
    /// AST node representing function term. Aggregates the name and the list of terms.
    /// </summary>
    public class FunctionTermAstNode : TermAstNode
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// List of terms.
        /// </summary>
        public List<TermAstNode> Terms { get; private set; } = new List<TermAstNode>();

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
    /// AST node representing number term. Aggregates the correspondent float value.
    /// </summary>
    public class NumberTermAstNode : TermOrNumericAstNode
    {
        /// <summary>
        /// Float value of the number term.
        /// </summary>
        public double Number { get; private set; } = 0.0;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(0);
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
    /// AST node representing undefined function value.
    /// </summary>
    public class UndefinedFuncValAstNode : TermOrNumericAstNode
    {
        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
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
    /// AST node representing preference violation within the metric expression. Agreggates the name of the preference.
    /// </summary>
    public class MetricPreferenceViolationAstNode : TermOrNumericAstNode
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(1);
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
    /// AST node representing duration variable ("?duration") within timed numeric expressions.
    /// </summary>
    public class DurationVariableTermAstNode : TermOrNumericAstNode
    {
        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
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
    /// AST node representing a timed numeric expression.
    /// </summary>
    public class TimedNumericExpressionAstNode : BaseAstNode
    {
        /// <summary>
        /// Is the expression in product form (i.e. "* #t numExpr" or "* numExpr #t")? Otherwise only "#t".
        /// </summary>
        public bool IsProductExpression { get; private set; } = false;

        /// <summary>
        /// The numeric factor in case of product expression.
        /// </summary>
        public TermOrNumericAstNode ProductExprNumericFactor { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            if (treeNode.GetMappedChildNodes().Count == 1)
            {
                IsProductExpression = false;
                ProductExprNumericFactor = null;
            }
            else
            {
                IsProductExpression = true;
                var productArg1 = treeNode.GetGrandChildAst<TermOrNumericAstNode>(1, 0);
                var productArg2 = treeNode.GetGrandChildAst<TermOrNumericAstNode>(1, 1);

                if (productArg1 != null)
                {
                    ProductExprNumericFactor = productArg1;
                }
                else if (productArg2 != null)
                {
                    ProductExprNumericFactor = productArg2;
                }
            }
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
