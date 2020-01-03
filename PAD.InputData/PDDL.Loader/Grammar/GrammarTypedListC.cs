
namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node derived from the general typed list, with constants as the items being typed.
    /// </summary>
    public class TypedListC : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public TypedListC(MasterGrammar p) : base(p)
        {
            Rule = TypedList.ConstructTypedListRule(p, IdentifierType.CONSTANT);
        }
    }
}
