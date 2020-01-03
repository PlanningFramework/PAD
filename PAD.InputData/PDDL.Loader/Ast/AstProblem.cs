using System;
using System.Collections.Generic;
using Irony.Parsing;
using Irony.Ast;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// AST node representing a problem. Aggregates the problem name, corresponding domain name and all the definition sections.
    /// </summary>
    public class ProblemAstNode : BaseAstNode
    {
        /// <summary>
        /// Problem name.
        /// </summary>
        public string ProblemName { get; private set; } = "";

        /// <summary>
        /// Corresponding domain name.
        /// </summary>
        public string CorrespondingDomainName { get; private set; } = "";

        /// <summary>
        /// Problem definition sections.
        /// </summary>
        public List<ProblemSectionAstNode> ProblemSections { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            ProblemName = treeNode.GetGrandChildString(1, 1);
            CorrespondingDomainName = treeNode.GetGrandChildString(2, 1);
            ProblemSections = treeNode.GetChildAstList<ProblemSectionAstNode>(3);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAcceptList(ProblemSections, visitor);
        }
    }

    /// <summary>
    /// AST node representing a problem definition section. Can be one of the specific definition sections (requirements, objects, init,
    /// goal, constraints, metric, length spec).
    /// </summary>
    public abstract class ProblemSectionAstNode : BaseAstNode
    {
    }

    /// <summary>
    /// AST node representing a problem requirements section. Aggregates the list of requirements.
    /// </summary>
    public class ProblemRequirementsAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// List of PDDL requirements.
        /// </summary>
        public List<Requirement> RequirementsList { get; private set; }

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
    /// AST node representing a problem objects section. Aggregates the list of typed objects.
    /// </summary>
    public class ProblemObjectsAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// List of typed objects.
        /// </summary>
        public TypedListAstNode ObjectsList { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            ObjectsList = treeNode.GetChildAst<TypedListAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(ObjectsList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a problem init section. Aggregates the list of initialization elements.
    /// </summary>
    public class ProblemInitAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// List of init elements.
        /// </summary>
        public List<InitElemAstNode> InitElemList { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            InitElemList = treeNode.GetChildAstList<InitElemAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAcceptList(InitElemList, visitor);
        }
    }

    /// <summary>
    /// AST node representing a problem goal section. Aggregates the goal condition expression.
    /// </summary>
    public class ProblemGoalAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// Goal condition of the problem.
        /// </summary>
        public GdAstNode Condition { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Condition = treeNode.GetChildAst<GdAstNode>(1);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(Condition, visitor);
        }
    }

    /// <summary>
    /// AST node representing a problem constraint section. Aggregates the constraint expression for the problem.
    /// </summary>
    public class ProblemConstraintsAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// Constraint expression for the problem.
        /// </summary>
        public ConGdAstNode Expression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            Expression = treeNode.GetChildAst<ConGdAstNode>(1);
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

    /// <summary>
    /// AST node representing a problem metric section. Aggregates the optimization specifier (minimize/maximize) and the expression
    /// to be optimized.
    /// </summary>
    public class ProblemMetricAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// Optimization specifier.
        /// </summary>
        public OptimizationSpecifier OptimizationSpecifier { get; private set; } = OptimizationSpecifier.MINIMIZE;

        /// <summary>
        /// Expression to be optimized.
        /// </summary>
        public TermOrNumericAstNode MetricExpression { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            OptimizationSpecifier = EnumMapper.ToOptimizationSpecifier(treeNode.GetChildString(1));
            MetricExpression = treeNode.GetChildAst<TermOrNumericAstNode>(2);
        }

        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            SafeAccept(MetricExpression, visitor);
        }
    }

    /// <summary>
    /// AST node representing a problem length section. Aggregates the length specifications.
    /// </summary>
    public class ProblemLengthAstNode : ProblemSectionAstNode
    {
        /// <summary>
        /// List of length specifications.
        /// </summary>
        public List<Tuple<LengthSpecifier, int>> LengthSpecifications { get; private set; }

        /// <summary>
        /// Initialization of the AST node. Specifies conversion from parse-tree node to AST node.
        /// </summary>
        /// <param name="treeNode">Parse-tree node.</param>
        public override void Init(ParseTreeNode treeNode)
        {
            if (treeNode != null)
            {
                LengthSpecifications = new List<Tuple<LengthSpecifier, int>>();
                for (int i = 1; i <= 2; ++i)
                {
                    var childNode = treeNode.GetMappedChildNodes().GetSafeItem(i);
                    if (childNode != null)
                    {
                        if (childNode.GetMappedChildNodes().Count == 0)
                            continue;
                        LengthSpecifier lengthSpecif = EnumMapper.ToLengthSpecifier(childNode.GetChildString(0));
                        int numVal = (int)childNode.GetChildNumberVal(1);
                        LengthSpecifications.Add(Tuple.Create(lengthSpecif, numVal));
                    }
                }
                if (LengthSpecifications.Count == 0)
                    LengthSpecifications = null;
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
