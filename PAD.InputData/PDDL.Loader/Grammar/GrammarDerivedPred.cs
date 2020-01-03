using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a derived predicate.
    /// </summary>
    public class DerivedPred : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DerivedPred(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public DerivedPred(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var derivedDef = new NonTerminal("Derived predicate definition", typeof(TransientAstNode));
            var derivedDefBase = new NonTerminal("Derived predicate definition", typeof(DomainDerivedPredAstNode));

            var predicateSkeleton = new NonTerminal("Derived predicate skeleton", typeof(TransientAstNode));
            var predicateSkeletonBase = new NonTerminal("Derived predicate skeleton", typeof(PredicateSkeletonAstNode));

            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var gd = new Gd(p);
            var typedList = new TypedList(p);

            // RULES

            derivedDef.Rule = p.ToTerm("(") + derivedDefBase + ")";
            derivedDefBase.Rule = p.ToTerm(":derived") + predicateSkeleton + gd;

            predicateSkeleton.Rule = p.ToTerm("(") + predicateSkeletonBase + ")";
            predicateSkeletonBase.Rule = predicateIdentifier + typedList;

            p.MarkTransient(derivedDef, predicateSkeleton);

            Rule = (bForm == BForm.BASE) ? derivedDefBase : derivedDef;
        }
    }
}
