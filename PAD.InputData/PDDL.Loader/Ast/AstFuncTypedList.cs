using System.Collections.Generic;
using System;
using Irony.Parsing;
using Irony.Ast;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing function typed list. Contains tuples of (function-name, function-args, function-return-type).
    /// In case of "either" clause, the types in the string are separated by ';' character, e.g. "typeA;typeB".
    /// </summary>
    public class FunctionTypedListAstNode : BaseAstNode
    {
        /// <summary>
        /// List of function definitions, in the form of tuples (function-name, function-args, function-return-type).
        /// </summary>
        public List<Tuple<string, TypedListAstNode, string>> FunctionsList { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            FunctionsList = new List<Tuple<string, TypedListAstNode, string>>();

            var typedFunctionsBlocks = treeNode.GetMappedChildNodes();
            foreach (var typedFunctionsBlock in typedFunctionsBlocks)
            {
                if (typedFunctionsBlock != null)
                {
                    var typedBlockParts = typedFunctionsBlock.GetMappedChildNodes();

                    var functionsBlock = (typedBlockParts.Count > 0) ? typedBlockParts[0] : null;
                    var typeDeclaration = (typedBlockParts.Count > 1) ? typedBlockParts[1] : null;

                    string type = "";
                    if (typeDeclaration != null)
                    {
                        var typeDeclStruct = typeDeclaration.GetMappedChildNodes();
                        bool typeDeclared = (typeDeclStruct.Count > 1);

                        if (typeDeclared)
                        {
                            var typeDeclItemList = typeDeclStruct[1].GetMappedChildNodes();
                            var typeOrEitherTypeList = (typeDeclItemList.Count > 1) ? typeDeclItemList[1] : null;

                            if (typeOrEitherTypeList != null)
                            {
                                var eitherTypeList = typeOrEitherTypeList.GetMappedChildNodes();
                                foreach (var eitherType in eitherTypeList)
                                {
                                    type += (type.Length > 0) ? ";" : "";
                                    type += eitherType.FindTokenAndGetText();
                                }
                            }
                            else
                            {
                                type = typeDeclItemList[0].FindTokenAndGetText();
                            }
                        }
                    }

                    if (functionsBlock != null)
                    {
                        foreach (var singleFunction in functionsBlock.GetMappedChildNodes())
                        {
                            var singleFuncParts = singleFunction.GetMappedChildNodes();

                            var funcName = (singleFuncParts.Count > 0) ? singleFuncParts[0] : null;
                            var paramsTypedList = (singleFuncParts.Count > 1) ? singleFuncParts[1] : null;

                            string funcNameStr = (funcName != null) ? funcName.FindTokenAndGetText() : "";
                            var paramsTypedListAstNode = paramsTypedList?.AstNode as TypedListAstNode;

                            FunctionsList.Add(Tuple.Create(funcNameStr, paramsTypedListAstNode, type));
                        }
                    }

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
