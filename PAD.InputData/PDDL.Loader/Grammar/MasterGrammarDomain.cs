
namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Specific master grammar for the PDDL domain file.
    /// </summary>
    public class MasterGrammarDomain : MasterGrammar
    {
        /// <summary>
        /// Constructor for the domain master grammar class.
        /// </summary>
        public MasterGrammarDomain()
        {
            Root = new Domain(this);
        }
    }
}
