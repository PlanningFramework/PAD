using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable UnusedMember.Global

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a predicate used within GD expression..
    /// </summary>
    public class PredicateGd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PredicateGd(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public PredicateGd(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var predicate = new NonTerminal("Predicate GD", typeof(TransientAstNode));
            var predicateBase = new NonTerminal("Predicate GD base", typeof(PredicateGdAstNode));
            var predicateArguments = new NonTerminal("Predicate arguments", typeof(TransientAstNode));
            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var term = new Term(p);

            // RULES

            predicate.Rule = p.ToTerm("(") + predicateBase + ")";
            predicateBase.Rule = predicateIdentifier + predicateArguments;
            predicateArguments.Rule = p.MakeStarRule(predicateArguments, term);

            p.MarkTransient(predicate, term);

            Rule = (bForm == BForm.BASE) ? predicateBase : predicate;
        }
    }
}