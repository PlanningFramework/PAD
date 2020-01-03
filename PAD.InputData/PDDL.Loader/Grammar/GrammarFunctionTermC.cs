// ReSharper disable UnusedMember.Global

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node derived from the general function term, where only constants can be the function arguments.
    /// </summary>
    public class FunctionTermC : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public FunctionTermC(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public FunctionTermC(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            Rule = FunctionTerm.ConstructFunctionTermRule(p, bForm, IdentifierType.CONSTANT);
        }
    }
}
