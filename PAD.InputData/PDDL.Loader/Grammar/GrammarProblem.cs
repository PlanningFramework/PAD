using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a problem. Root node for the PDDL problem file.
    /// </summary>
    public class Problem : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Problem(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var problem = new NonTerminal("Problem", typeof(ProblemAstNode));
            var definedProblem = new NonTerminal("Defined problem", typeof(TransientAstNode));
            var correspondingDomain = new NonTerminal("Corresponding domain", typeof(TransientAstNode));
            
            var problemSections = new NonTerminal("Problem sections", typeof(TransientAstNode));
            var problemSection = new NonTerminal("Problem section", typeof(TransientAstNode));
            var problemSectionBase = new NonTerminal("Problem section base", typeof(TransientAstNode));

            var requireDef = new NonTerminal("Requirements section", typeof(ProblemRequirementsAstNode));
            var objectsDef = new NonTerminal("Objects section", typeof(ProblemObjectsAstNode));
            var initDef = new NonTerminal("Init section", typeof(ProblemInitAstNode));
            var goalDef = new NonTerminal("Goal section", typeof(ProblemGoalAstNode));
            var constrDef = new NonTerminal("Constraints section", typeof(ProblemConstraintsAstNode));
            var metricDef = new NonTerminal("Metric section", typeof(ProblemMetricAstNode));
            var lengthDef = new NonTerminal("Length section", typeof(ProblemLengthAstNode));

            var requireList = new NonTerminal("Requirements list", typeof(TransientAstNode));
            var initElemList = new NonTerminal("Init elements list", typeof(TransientAstNode));
            var optimizationSpecifier = new NonTerminal("Optimization specifier", typeof(TransientAstNode));
            var lengthSpec = new NonTerminal("Length definition specification", typeof(TransientAstNode));
            var lengthSpecifier = new NonTerminal("Length specifier", typeof(TransientAstNode));

            var problemName = new IdentifierTerminal("Problem name", IdentifierType.CONSTANT);
            var domainName = new IdentifierTerminal("Domain name", IdentifierType.CONSTANT);
            var requireName = new IdentifierTerminal("Requirement identifier", IdentifierType.REQUIREMENT);
            var number = new NumberLiteral("Number");

            // USED SUB-TREES

            var typedListC = new TypedListC(p);
            var initElem = new InitElem(p);
            var preGd = new PreGd(p);
            var prefConGd = new PrefConGd(p);
            var metricExpr = new MetricExpr(p);

            // RULES

            problem.Rule = p.ToTerm("(") + p.ToTerm("define") + definedProblem + correspondingDomain + problemSections + ")";

            definedProblem.Rule = "(" + p.ToTerm("problem") + problemName + ")";
            correspondingDomain.Rule = "(" + p.ToTerm(":domain") + domainName + ")";
            problemSections.Rule = p.MakeStarRule(problemSections, problemSection);

            problemSection.Rule = p.ToTerm("(") + problemSectionBase + ")";
            problemSectionBase.Rule = requireDef | objectsDef | initDef | goalDef | constrDef | metricDef | lengthDef;

            requireDef.Rule = p.ToTerm(":requirements") + requireList;
            objectsDef.Rule = p.ToTerm(":objects") + typedListC;
            initDef.Rule    = p.ToTerm(":init") + initElemList;
            goalDef.Rule    = p.ToTerm(":goal") + preGd;
            constrDef.Rule  = p.ToTerm(":constraints") + prefConGd;
            metricDef.Rule  = p.ToTerm(":metric") + optimizationSpecifier + metricExpr;
            lengthDef.Rule  = p.ToTerm(":length") + lengthSpec + lengthSpec;

            requireList.Rule = p.MakePlusRule(requireList, requireName);
            initElemList.Rule = p.MakeStarRule(initElemList, initElem);

            optimizationSpecifier.Rule = p.ToTerm("minimize") | p.ToTerm("maximize");

            lengthSpec.Rule = (p.ToTerm("(") + lengthSpecifier + number + ")") | p.Empty;
            lengthSpecifier.Rule = p.ToTerm(":serial") | p.ToTerm(":parallel");

            p.MarkTransient(problemSection, problemSectionBase, optimizationSpecifier, lengthSpecifier);

            Rule = problem;
        }
    }
}
