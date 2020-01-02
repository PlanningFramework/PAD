using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a predicate used within GD expression..
    /// </summary>
    public class PredicateGD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PredicateGD(MasterGrammar p) : base(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public PredicateGD(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var predicate = new NonTerminal("Predicate GD", typeof(TransientAstNode));
            var predicateBase = new NonTerminal("Predicate GD base", typeof(PredicateGDAstNode));
            var predicateArguments = new NonTerminal("Predicate arguments", typeof(TransientAstNode));
            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var term = new Term(p);

            // RULES

            predicate.Rule = p.ToTerm("(") + predicateBase + ")";
            predicateBase.Rule = predicateIdentifier + predicateArguments;
            predicateArguments.Rule = p.MakeStarRule(predicateArguments, term);

            p.MarkTransient(predicate, term);

            return (bForm == BForm.BASE) ? predicateBase : predicate;
        }
    }
}