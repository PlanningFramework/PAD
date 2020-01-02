using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

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
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var derivedDef = new NonTerminal("Derived predicate definition", typeof(TransientAstNode));
            var derivedDefBase = new NonTerminal("Derived predicate definition", typeof(DomainDerivedPredAstNode));

            var predicateSkeleton = new NonTerminal("Derived predicate skeleton", typeof(TransientAstNode));
            var predicateSkeletonBase = new NonTerminal("Derived predicate skeleton", typeof(PredicateSkeletonAstNode));

            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var GD = new GD(p);
            var typedList = new TypedList(p);

            // RULES

            derivedDef.Rule = p.ToTerm("(") + derivedDefBase + ")";
            derivedDefBase.Rule = p.ToTerm(":derived") + predicateSkeleton + GD;

            predicateSkeleton.Rule = p.ToTerm("(") + predicateSkeletonBase + ")";
            predicateSkeletonBase.Rule = predicateIdentifier + typedList;

            p.MarkTransient(derivedDef, predicateSkeleton);

            return (bForm == BForm.BASE) ? derivedDefBase : derivedDef;
        }
    }
}
