using Irony.Parsing;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST representing a general timed-effect. Can be either at-timed-effect, or assign-timed-effect.
    /// </summary>
    public abstract class TimedEffectAstNode : DaEffectAstNode
    {
    }

    /// <summary>
    /// AST representing at-timed-effect. Aggregates time specifier and the actual effect.
    /// </summary>
    public class AtTimedEffectAstNode : TimedEffectAstNode
    {
        /// <summary>
        /// Time specifier.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; private set; } = TimeSpecifier.START;

        /// <summary>
        /// Effect.
        /// </summary>
        public CondEffectAstNode Effect { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            TimeSpecifier = EnumMapper.ToTimeSpecifier(treeNode.GetChildString(1));
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
    /// AST node representing assign-timed-effect. Aggregates a concrete assignment operator, a function and an expression to be assigned.
    /// </summary>
    public class AssignTimedEffectAstNode : TimedEffectAstNode
    {
        /// <summary>
        /// Assign operator for timed effect.
        /// </summary>
        public TimedEffectAssignOperator AssignOperator { get; private set; } = TimedEffectAssignOperator.INCREASE;

        /// <summary>
        /// Function term for the assignment.
        /// </summary>
        public FunctionTermAstNode Function { get; private set; }

        /// <summary>
        /// Timed numeric expression to be assigned.
        /// </summary>
        public TimedNumericExpressionAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            AssignOperator = EnumMapper.ToTimedEffectAssignOperator(treeNode.GetChildString(0));
            Function = treeNode.GetChildAst<FunctionTermAstNode>(1);
            Expression = treeNode.GetChildAst<TimedNumericExpressionAstNode>(2);
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
