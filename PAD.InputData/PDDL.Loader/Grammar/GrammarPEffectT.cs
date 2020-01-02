using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Timed variant of p-effect grammar node, allowing the use of timed numeric expressions and values.
    /// </summary>
    public class PEffectT : PEffect
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PEffectT(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public PEffectT(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for specifying a numeric expression type being used within this grammar node.
        /// </summary>
        /// <returns>Numeric expression type.</returns>
        protected override NonTerminal getNumericExpr()
        {
            return new NumericExprDa(p);
        }

        /// <summary>
        /// Factory method specifying a value/term entity type being used within this grammar node.
        /// </summary>
        /// <returns>Value/term type.</returns>
        protected override NonTerminal getValueOrTerm()
        {
            return new ValueOrTermT(p);
        }
    }
}
