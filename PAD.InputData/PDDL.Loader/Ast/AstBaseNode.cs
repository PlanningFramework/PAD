using System.Collections.Generic;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter.Ast;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// Base AST node used by all our AST nodes. Encapsulates the Irony's AstNode and provides methods for safe acceptation of the visitors.
    /// </summary>
    public abstract class BaseAstNode : AstNode, IAstVisitableNode
    {
        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="context">Corresponding context.</param>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Init(treeNode);
        }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public abstract void Init(ParseTreeNode treeNode);

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public abstract void Accept(IAstVisitor visitor);

        /// <summary>
        /// Method for a safe node acceptation of the visitor, checking the validity of the given astNode.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        /// <param name="visitor">AST visitor.</param>
        public static void SafeAccept(BaseAstNode astNode, IAstVisitor visitor)
        {
            astNode?.Accept(visitor);
        }

        /// <summary>
        /// Method for a safe nodes acceptation of the visitor, checking the validity of the given list of nodes.
        /// </summary>
        /// <typeparam name="T">Type of the given AST nodes.</typeparam>
        /// <param name="astNodesList">List of AST nodes.</param>
        /// <param name="visitor">AST visitor.</param>
        public static void SafeAcceptList<T>(List<T> astNodesList, IAstVisitor visitor) where T : BaseAstNode
        {
            if (astNodesList != null)
            {
                foreach (var astNode in astNodesList)
                {
                    SafeAccept(astNode, visitor);
                }
            }
        }
    }

    /// <summary>
    /// AST node representing a transient AST node (temporary node, which won't be used in the resulting AST tree).
    /// Needed for the consistent parsing and correct conversions of Irony framework.
    /// </summary>
    public class TransientAstNode : BaseAstNode
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
        }
    }
}
