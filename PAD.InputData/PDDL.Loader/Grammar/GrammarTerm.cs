using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a term.
    /// </summary>
    public class Term : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Term(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var term = new NonTerminal("Term", typeof(TransientAstNode));
            var termIdentifier = new NonTerminal("Identifier term", typeof(IdentifierTermAstNode));
            var termFunction = new NonTerminal("Functional term", typeof(TransientAstNode));

            var termFunctionBase = new NonTerminal("Functional term base", typeof(FunctionTermAstNode));
            var functionArguments = new NonTerminal("Functional term arguments", typeof(TransientAstNode));

            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);
            var varOrConstIdentifier = new IdentifierTerminal("Variable or constant identifier", IdentifierType.VARIABLE_OR_CONSTANT);

            // RULES

            term.Rule = termIdentifier | termFunction;
            termIdentifier.Rule = varOrConstIdentifier;
            termFunction.Rule = p.ToTerm("(") + termFunctionBase + ")";

            termFunctionBase.Rule = functionIdentifier + functionArguments;
            functionArguments.Rule = p.MakeStarRule(functionArguments, term);

            p.MarkTransient(term, termFunction);

            Rule = term;
        }
    }
}
