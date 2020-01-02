using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node derived from the general typed list, with constants as the items being typed.
    /// </summary>
    public class TypedListC : TypedList
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public TypedListC(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Template method specifying which type of items are used within the typed list.
        /// </summary>
        /// <returns>Identifier type for the typed list item.</returns>
        protected override IdentifierType getItemIdentifierType()
        {
            return IdentifierType.CONSTANT;
        }
    }
}
