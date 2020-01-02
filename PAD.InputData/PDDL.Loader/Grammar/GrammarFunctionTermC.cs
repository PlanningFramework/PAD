using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node derived from the general function term, where only constants can be the function arguments.
    /// </summary>
    public class FunctionTermC : FunctionTerm
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public FunctionTermC(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public FunctionTermC(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Template method specifying which type of items can be used as the function arguments.
        /// </summary>
        /// <returns>Identifier type of the function arguments.</returns>
        protected override IdentifierType getItemIdentifierType()
        {
            return IdentifierType.CONSTANT;
        }
    }
}
