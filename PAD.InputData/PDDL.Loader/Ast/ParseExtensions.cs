using System;
using System.Collections.Generic;
using Irony.Parsing;
using Irony.Ast;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// Extensions for Irony's ParseTreeNode - implementation of convenient methods for conversion from parse-tree to AST.
    /// </summary>
    public static class ParseTreeNodeExtensions
    {
        /// <summary>
        /// Gets a parse-tree node from the list, checking index boundary.
        /// </summary>
        /// <param name="nodeList">List of parse-tree nodes.</param>
        /// <param name="index">Item position.</param>
        /// <returns>Required parse-tree node, if exists. Null otherwise.</returns>
        public static ParseTreeNode GetSafeItem(this ParseTreeNodeList nodeList, int index)
        {
            return (index < nodeList.Count) ? nodeList[index] : null;
        }

        /// <summary>
        /// Finds a mapped child of the parse-tree node and returns its AST node, if the expected AST type matches.
        /// </summary>
        /// <typeparam name="TargetAst">Expected AST type.</typeparam>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="index">Item position.</param>
        /// <returns>Child AST node, if it matches the expected type. Null otherwise.</returns>
        public static TargetAst GetChildAst<TargetAst>(this ParseTreeNode parseNode, int index) where TargetAst : BaseAstNode
        {
            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(index);
            return childNode?.AstNode as TargetAst;
        }

        /// <summary>
        /// Finds a mapped list of child parse-tree nodes and returns their AST nodes, if the expected AST type matches.
        /// </summary>
        /// <typeparam name="TargetAst">Expected AST type.</typeparam>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="index">Item position.</param>
        /// <returns>Child AST nodes list, if it matches the expected type. Null otherwise.</returns>
        public static List<TargetAst> GetChildAstList<TargetAst>(this ParseTreeNode parseNode, int index) where TargetAst : BaseAstNode
        {
            List<TargetAst> retList = new List<TargetAst>();

            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(index);
            if (childNode != null)
            {
                foreach (var listItem in childNode.GetMappedChildNodes())
                {
                    var listItemAst = listItem.AstNode as TargetAst;
                    if (listItemAst != null)
                    {
                        retList.Add(listItemAst);
                    }
                }
            }

            return retList;
        }

        /// <summary>
        /// Finds a mapped grand-child of the parse-tree node and returns its AST node, if the expected AST type matches.
        /// </summary>
        /// <typeparam name="TargetAst">Expected AST type.</typeparam>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="childIndex">Child item position.</param>
        /// <param name="grandChildIndex">Grand-child item position.</param>
        /// <returns>Grand-child AST node, if it matches the expected type. Null otherwise.</returns>
        public static TargetAst GetGrandChildAst<TargetAst>(this ParseTreeNode parseNode, int childIndex, int grandChildIndex) where TargetAst : BaseAstNode
        {
            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(childIndex);
            var grandChildNode = childNode?.GetMappedChildNodes().GetSafeItem(grandChildIndex);
            return grandChildNode?.AstNode as TargetAst;
        }

        /// <summary>
        /// Finds a mapped child of the parse-tree node and returns its string token, if it has any.
        /// </summary>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="index">Item position.</param>
        /// <returns>String token of the child parse-node, if it has any. Empty string otherwise.</returns>
        public static string GetChildString(this ParseTreeNode parseNode, int index)
        {
            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(index);
            string childNodeStr = childNode?.FindTokenAndGetText();
            return childNodeStr ?? "";
        }

        /// <summary>
        /// Finds a mapped child list of the parse-tree node and returns their string token, if they have any.
        /// </summary>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="index">Item position.</param>
        /// <returns>String tokens of the child parse-nodes, if they have any. Null otherwise.</returns>
        public static List<string> GetChildStringList(this ParseTreeNode parseNode, int index)
        {
            List<string> retList = new List<string>();

            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(index);
            if (childNode != null)
            {
                foreach (var listItem in childNode.GetMappedChildNodes())
                {
                    string childNodeStr = listItem.FindTokenAndGetText();
                    if (childNodeStr != null)
                    {
                        retList.Add(childNodeStr);
                    }
                }
            }

            return retList;
        }

        /// <summary>
        /// Finds a mapped grand-child of the parse-tree node and returns its string token, if it has any.
        /// </summary>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="childIndex">Child item position.</param>
        /// <param name="grandChildIndex">Grand-child item position.</param>
        /// <returns>String token of the grand-child parse-node, if it has any. Empty string otherwise.</returns>
        public static string GetGrandChildString(this ParseTreeNode parseNode, int childIndex, int grandChildIndex)
        {
            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(childIndex);
            var grandChildNode = childNode?.GetMappedChildNodes().GetSafeItem(grandChildIndex);
            string grandChildNodeStr = grandChildNode?.FindTokenAndGetText();
            return grandChildNodeStr ?? "";
        }

        /// <summary>
        /// Finds a mapped child of the parse-tree node and returns its float value token, if it has any.
        /// </summary>
        /// <param name="parseNode">Original parse-tree node.</param>
        /// <param name="index">Item position.</param>
        /// <returns>Float value token of the child parse-node, if it has any. Zero value otherwise.</returns>
        public static double GetChildNumberVal(this ParseTreeNode parseNode, int index)
        {
            var childNode = parseNode?.GetMappedChildNodes().GetSafeItem(index);
            return childNode != null ? Convert.ToDouble(childNode.Token.Value) : 0.0;
        }
    }
}