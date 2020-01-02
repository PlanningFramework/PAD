using System.Collections.Generic;
using System;
using Irony.Parsing;
using Irony.Ast;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing general typed list. Contains string pairs of (item, type).
    /// In case of "either" clause, the types in the string are separated by ';' character, e.g. ("item1", "typeA;typeB").
    /// </summary>
    public class TypedListAstNode : BaseAstNode
    {
        /// <summary>
        /// Pairs of identifiers (item,type)
        /// </summary>
        public List<Tuple<string, string>> TypedIdentifiers { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            TypedIdentifiers = new List<Tuple<string, string>>();

            var childNodes = treeNode.GetMappedChildNodes();
            var listOfTypeListsN = (childNodes.Count > 0) ? childNodes[0] : null;

            if (listOfTypeListsN != null)
            {
                foreach (var singleTypedList in listOfTypeListsN.GetMappedChildNodes())
                {
                    var typedListBlocks = singleTypedList.GetMappedChildNodes();

                    var identifiersList = (typedListBlocks.Count > 0) ? typedListBlocks[0] : null;
                    var typeDeclaration = (typedListBlocks.Count > 1) ? typedListBlocks[1] : null;

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

                    if (identifiersList != null)
                    {
                        foreach (var identifierItem in identifiersList.GetMappedChildNodes())
                        {
                            string identifier = identifierItem.FindTokenAndGetText();
                            TypedIdentifiers.Add(Tuple.Create(identifier, type));
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
