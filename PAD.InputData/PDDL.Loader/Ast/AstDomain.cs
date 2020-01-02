using System.Collections.Generic;
using Irony.Parsing;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a domain. Aggregates the domain name and all the definition sections.
    /// </summary>
    public class DomainAstNode : BaseAstNode
    {
        /// <summary>
        /// Domain name.
        /// </summary>
        public string DomainName { get; private set; } = "";

        /// <summary>
        /// Domain sections.
        /// </summary>
        public List<DomainSectionAstNode> DomainSections { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            DomainName = treeNode.GetChildString(2);
            DomainSections = treeNode.GetChildAstList<DomainSectionAstNode>(3);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAcceptList(DomainSections, visitor);
        }
    }

    /// <summary>
    /// AST node representing a domain definition section. Can be one of the specific definition sections (requirements, types, constants,
    /// predicates, functions, constraints, action, durative-actions, derived-predicates).
    /// </summary>
    public abstract class DomainSectionAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a domain requirements section. Aggregates the list of requirements.
    /// </summary>
    public class DomainRequirementsAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// List of requirements.
        /// </summary>
        public List<Requirement> RequirementsList { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            RequirementsList = new List<Requirement>();

            List<string> reqList = treeNode.GetChildStringList(1);
            foreach (var req in reqList)
            {
                RequirementsList.Add(EnumMapper.ToRequirement(req));
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

    /// <summary>
    /// AST node representing a domain types section. Aggregates the list of types and their predecessors (base types).
    /// </summary>
    public class DomainTypesAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// List of types, in the form of pairs (type, baseType).
        /// </summary>
        public TypedListAstNode TypesList { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            TypesList = treeNode.GetChildAst<TypedListAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(TypesList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a domain constants section. Aggregates the list of typed constants.
    /// </summary>
    public class DomainConstantsAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// List of typed constants.
        /// </summary>
        public TypedListAstNode ConstantsList { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            ConstantsList = treeNode.GetChildAst<TypedListAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(ConstantsList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a domain predicates section. Aggregates the list of predicate specifications.
    /// </summary>
    public class DomainPredicatesAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// List of predicate specifications (predicate name, list of typed arguments).
        /// </summary>
        public List<PredicateSkeletonAstNode> PredicatesList { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            PredicatesList = treeNode.GetChildAstList<PredicateSkeletonAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAcceptList(PredicatesList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a domain functions section. Aggregates the list of function specifications.
    /// </summary>
    public class DomainFunctionsAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// List of function specifications (function name, list of typed arguments, return type).
        /// </summary>
        public FunctionTypedListAstNode FunctionTypedList { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            FunctionTypedList = treeNode.GetChildAst<FunctionTypedListAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(FunctionTypedList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a domain constraint section. Aggregates the constraint expression for the domain.
    /// </summary>
    public class DomainConstraintsAstNode : DomainSectionAstNode
    {
        /// <summary>
        /// Constraint expression for the domain.
        /// </summary>
        public ConGDAstNode Expression { get; private set; } = null;

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<ConGDAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Expression, visitor);
        }
    }
}
