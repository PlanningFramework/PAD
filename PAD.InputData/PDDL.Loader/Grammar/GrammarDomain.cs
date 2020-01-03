using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a domain. Root node for the PDDL domain file.
    /// </summary>
    public class Domain : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Domain(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var domain = new NonTerminal("Domain", typeof(DomainAstNode));
            var domainSections = new NonTerminal("Domain sections", typeof(TransientAstNode));
            var domainSection = new NonTerminal("Domain section", typeof(TransientAstNode));
            var domainSectionBase = new NonTerminal("Domain section base", typeof(TransientAstNode));

            var requireDef = new NonTerminal("Requirements section", typeof(DomainRequirementsAstNode));
            var typesDef = new NonTerminal("Types section", typeof(DomainTypesAstNode));
            var constantsDef = new NonTerminal("Constants section", typeof(DomainConstantsAstNode));
            var predicatesDef = new NonTerminal("Predicates section", typeof(DomainPredicatesAstNode));
            var functionsDef = new NonTerminal("Functions section", typeof(DomainFunctionsAstNode));
            var constraintsDef = new NonTerminal("Constraints section", typeof(DomainConstraintsAstNode));
            var structureDef = new NonTerminal("Structure section", typeof(TransientAstNode));

            var requireList = new NonTerminal("Requirements", typeof(TransientAstNode));

            var predicatesList = new NonTerminal("Predicates list", typeof(TransientAstNode));
            var predicateSkeleton = new NonTerminal("Predicate skeleton", typeof(TransientAstNode));
            var predicateSkeletonBase = new NonTerminal("Predicate skeleton base", typeof(PredicateSkeletonAstNode));

            var domainName = new IdentifierTerminal("Domain name", IdentifierType.CONSTANT);
            var requireName = new IdentifierTerminal("Requirement identifier", IdentifierType.REQUIREMENT);
            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var typedListC = new TypedListC(p);
            var typedListV = new TypedList(p);
            var funcTypedList = new FunctionTypedList(p);
            var conGd = new ConGd(p);
            
            var action = new Action(p, BForm.BASE);
            var durAction = new DurAction(p, BForm.BASE);
            var derivedPred = new DerivedPred(p, BForm.BASE);

            // RULES

            domain.Rule = p.ToTerm("(") + p.ToTerm("define") + "(" + p.ToTerm("domain") + domainName + ")" + domainSections + ")";
            domainSections.Rule = p.MakeStarRule(domainSections, domainSection);

            domainSection.Rule = p.ToTerm("(") + domainSectionBase + ")";
            domainSectionBase.Rule = requireDef | typesDef | constantsDef | predicatesDef | functionsDef | constraintsDef | structureDef;

            requireDef.Rule = p.ToTerm(":requirements") + requireList;
            requireList.Rule = p.MakePlusRule(requireList, requireName);

            typesDef.Rule = p.ToTerm(":types") + typedListC;
            constantsDef.Rule = p.ToTerm(":constants") + typedListC;

            predicatesDef.Rule = p.ToTerm(":predicates") + predicatesList;
            predicatesList.Rule = p.MakePlusRule(predicatesList, predicateSkeleton);
            predicateSkeleton.Rule = p.ToTerm("(") + predicateSkeletonBase + ")";
            predicateSkeletonBase.Rule = predicateIdentifier + typedListV;

            functionsDef.Rule = p.ToTerm(":functions") + funcTypedList;

            constraintsDef.Rule = p.ToTerm(":constraints") + conGd;

            structureDef.Rule = action | durAction | derivedPred;

            p.MarkTransient(domainSection, domainSectionBase, predicateSkeleton, structureDef);

            Rule = domain;
        }
    }
}
