using Irony.Parsing;
using System;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Abstract base master grammar class. Specifies general properties for the base Irony grammar, punctuation, comments and grammar rules.
    /// Derived variants are used for parsing concrete PDDL inputs.
    /// </summary>
    public abstract class MasterGrammar : Irony.Interpreter.InterpretedLanguageGrammar
    {
        /// <summary>
        /// Constructor for the master grammar class.
        /// </summary>
        public MasterGrammar() : base(false)
        {
            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst;
            this.MarkPunctuation("(", ")");
            this.NonGrammarTerminals.Add(new CommentTerminal("Comment specification", ";", "\r", "\n", Environment.NewLine));
            this.Root = MakeGrammarRules();
        }

        /// <summary>
        /// Factory method for the definition of root grammar rules.
        /// </summary>
        /// <returns>Grammar rules for this grammar.</returns>
        protected abstract NonTerminal MakeGrammarRules();
    }
}
