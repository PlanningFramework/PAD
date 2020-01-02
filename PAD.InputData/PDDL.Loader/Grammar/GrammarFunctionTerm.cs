using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a general function term. Can act both as a term inside a predicate, or a numeric/object value.
    /// </summary>
    public class FunctionTerm : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public FunctionTerm(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public FunctionTerm(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var functionTerm = new NonTerminal("Functional term", typeof(TransientAstNode));
            var functionTermBase = new NonTerminal("Functional term base", typeof(FunctionTermAstNode));
            var functionArguments = new NonTerminal("Functional term arguments", typeof(TransientAstNode));

            var argTerm = new NonTerminal("Term", typeof(TransientAstNode));
            var argTermIdentifier = new NonTerminal("Identifier term", typeof(IdentifierTermAstNode));

            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);
            var varOrConstIdentifier = new IdentifierTerminal("Variable or constant identifier", getItemIdentifierType());

            // RULES

            functionTerm.Rule = p.ToTerm("(") + functionTermBase + ")";
            functionTermBase.Rule = functionIdentifier + functionArguments;
            functionArguments.Rule = p.MakeStarRule(functionArguments, argTerm);

            argTerm.Rule = argTermIdentifier | functionTerm;
            argTermIdentifier.Rule = varOrConstIdentifier;

            p.MarkTransient(functionTerm, argTerm);

            return (bForm == BForm.BASE) ? functionTermBase : functionTerm;
        }

        /// <summary>
        /// Template method specifying which type of items can be used as the function arguments.
        /// </summary>
        /// <returns>Identifier type of the function arguments.</returns>
        protected virtual IdentifierType getItemIdentifierType()
        {
            return IdentifierType.VARIABLE_OR_CONSTANT;
        }
    }
}
