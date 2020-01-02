using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a numeric operation. Aggregates the numeric operator and the list of arguments.
    /// </summary>
    public class NumericOpAstNode : TermOrNumericAstNode
    {
        /// <summary>
        /// Numeric operator.
        /// </summary>
        public NumericOperator Operator { get; private set; } = NumericOperator.PLUS;

        /// <summary>
        /// List of numeric arguments.
        /// </summary>
        public List<TermOrNumericAstNode> Arguments { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Operator = EnumMapper.ToNumericOperator(treeNode.GetChildString(0));
            Arguments = new List<TermOrNumericAstNode>();

            var arg1 = treeNode.GetChildAst<TermOrNumericAstNode>(1);
            var arg2 = treeNode.GetChildAst<TermOrNumericAstNode>(2);
            
            if (arg1 != null)
            {
                Arguments.Add(arg1);
            }

            if (arg2 != null)
            {
                Arguments.Add(arg2);
            }
            else
            {
                // multiary operation
                var arg2List = treeNode.GetChildAstList<TermOrNumericAstNode>(2);
                if (arg2List != null)
                    Arguments.AddRange(arg2List);
            }


            if (Arguments.Count == 0)
                Arguments = null;
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
