
namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Timed variant of p-effect grammar node, allowing the use of timed numeric expressions and values.
    /// </summary>
    public class PEffectT : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public PEffectT(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            Rule = PEffect.ConstructPEffectRule(p, bForm, new ValueOrTermT(p), new NumericExprDa(p));
        }
    }
}
