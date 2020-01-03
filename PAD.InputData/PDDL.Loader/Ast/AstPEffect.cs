using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing and-expression of p-effects. Aggregates the list of p-effects.
    /// </summary>
    public class AndPEffectsAstNode : CondEffectAstNode
    {
        /// <summary>
        /// List of p-effects.
        /// </summary>
        public List<PEffectAstNode> Arguments { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Arguments = treeNode.GetChildAstList<PEffectAstNode>(1);
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
    /// AST node representing p-effect (primitive effect). Can be either atomic formula, not-expression, or assign-expression.
    /// </summary>
    public abstract class PEffectAstNode : CondEffectAstNode
    {
    }

    /// <summary>
    /// AST node representing atomic formula p-effect. Can be either predicate or equals-expression.
    /// </summary>
    public abstract class AtomicFormulaPEffectAstNode : PEffectAstNode
    {
    }

    /// <summary>
    /// AST node representing predicate p-effect. Aggregates the predicate name and the list of terms.
    /// </summary>
    public class PredicatePEffectAstNode : AtomicFormulaPEffectAstNode
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// List of terms.
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
    /// AST node representing equals-expression p-effect. Aggregates the argument terms.
    /// </summary>
    public class EqualsOpPEffectAstNode : AtomicFormulaPEffectAstNode
    {
        /// <summary>
        /// First operand of the expression.
        /// </summary>
        public TermOrNumericAstNode Term1 { get; private set; }

        /// <summary>
        /// Second operand of the expression.
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
    /// AST node representing not-expression. Aggregates the atomic formula to be negated.
    /// </summary>
    public class NotPEffectAstNode : PEffectAstNode
    {
        /// <summary>
        /// Atomic formula to be negated.
        /// </summary>
        public AtomicFormulaPEffectAstNode Argument { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Argument = treeNode.GetChildAst<AtomicFormulaPEffectAstNode>(1);
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
    /// AST node representing assign-expression p-effect. Aggregates the assign operator, the function term and the term to be assigned.
    /// </summary>
    public class AssignPEffectAstNode : PEffectAstNode
    {
        /// <summary>
        /// Assignment operator (assign, increase, decrease etc.).
        /// </summary>
        public AssignOperator AssignOperator { get; private set; } = AssignOperator.ASSIGN;

        /// <summary>
        /// Function term for the assignment.
        /// </summary>
        public FunctionTermAstNode Argument1 { get; private set; }

        /// <summary>
        /// Term or expression to be assigned.
        /// </summary>
        public TermOrNumericAstNode Argument2 { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            AssignOperator = EnumMapper.ToAssignOperator(treeNode.GetChildString(0));
            Argument1 = treeNode.GetChildAst<FunctionTermAstNode>(1);
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
