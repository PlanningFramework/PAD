
namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Specific master grammar for the PDDL problem file.
    /// </summary>
    public class MasterGrammarProblem : MasterGrammar
    {
        /// <summary>
        /// Constructor for the problem master grammar class.
        /// </summary>
        public MasterGrammarProblem()
        {
            Root = new Problem(this);
        }
    }
}
