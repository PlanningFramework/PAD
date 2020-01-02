using Irony.Parsing;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Specific master grammar for the PDDL domain file.
    /// </summary>
    public class MasterGrammarDomain : MasterGrammar
    {
        /// <summary>
        /// Factory method for the definition of root grammar rules.
        /// </summary>
        /// <returns>Grammar rules for this grammar.</returns>
        protected override NonTerminal MakeGrammarRules()
        {
            return new Domain(this);
        }
    }
}
