using System.Collections.Generic;
using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a general initial element inside init block. Can be either literal element or at-expression.
    /// </summary>
    public abstract class InitElemAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a literal element inside init block. Can be either atomic formula or not-expression.
    /// </summary>
    public abstract class LiteralInitElemAstNode : InitElemAstNode
    {
    }

    /// <summary>
    /// AST node representing an atomic formula element inside init block. Can be either predicate or equals-operator.
    /// </summary>
    public abstract class AtomicFormulaInitElemAstNode : LiteralInitElemAstNode
    {
    }

    /// <summary>
    /// AST node representing a predicate element inside init block. Aggregates the predicate name and its terms.
    /// </summary>
    public class PredicateInitElemAstNode : AtomicFormulaInitElemAstNode
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Predicate terms (constants).
        /// </summary>
        public List<string> Terms { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Name = treeNode.GetChildString(0);
            Terms = treeNode.GetChildStringList(1);
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
    /// AST node representing an equals-operator (=) element inside init block.
    /// </summary>
    public class EqualsOpInitElemAstNode : AtomicFormulaInitElemAstNode
    {
        /// <summary>
        /// First argument of the equals-operator,
        /// </summary>
        public TermOrNumericAstNode Term1 { get; private set; }

        /// <summary>
        /// Second argument of the equals-operator,
        /// </summary>
        public TermOrNumericAstNode Term2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Term1 = treeNode.GetChildAst<TermOrNumericAstNode>(1);
            Term2 = treeNode.GetChildAst<TermOrNumericAstNode>(2);
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
    /// AST node representing a not-expression element inside init block.
    /// </summary>
    public class NotInitElemAstNode : LiteralInitElemAstNode
    {
        /// <summary>
        /// Argument of the not-expression.
        /// </summary>
        public AtomicFormulaInitElemAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Argument = treeNode.GetChildAst<AtomicFormulaInitElemAstNode>(1);
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
    /// AST node representing a at-expression element inside init block.
    /// </summary>
    public class AtInitElemAstNode : InitElemAstNode
    {
        /// <summary>
        /// Number of the expression.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// Literal argument of the expression.
        /// </summary>
        public LiteralInitElemAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Number = treeNode.GetChildNumberVal(1);
            Argument = treeNode.GetChildAst<LiteralInitElemAstNode>(2);
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
