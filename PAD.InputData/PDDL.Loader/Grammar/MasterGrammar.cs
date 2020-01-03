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
        protected MasterGrammar() : base(false)
        {
            LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst;
            MarkPunctuation("(", ")");
            NonGrammarTerminals.Add(new CommentTerminal("Comment specification", ";", "\r", "\n", Environment.NewLine));
            Root = null;
        }
    }
}
