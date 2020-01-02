using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing da-effect (duration action effect).
    /// </summary>
    public class DaEffect : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DaEffect(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public DaEffect(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var daEffect = new NonTerminal("Da-effect", typeof(TransientAstNode));
            var daEffectBase = new NonTerminal("Da-effect base", typeof(TransientAstNode));

            var andDaEffects = new NonTerminal("AND expression (da-effects)", typeof(AndDaEffectsAstNode));
            var daEffectsStarList = new NonTerminal("Da-effects star list", typeof(TransientAstNode));

            var forallDaEffect = new NonTerminal("FORALL expression (da-effect)", typeof(ForallDaEffectAstNode));
            var whenTimeEffect = new NonTerminal("WHEN expression (da-effect)", typeof(WhenDaEffectAstNode));

            var timedEffect = new NonTerminal("Timed-effect", typeof(TransientAstNode));

            // USED SUB-TREES

            var timedEffectBase = new TimedEffect(p, BForm.BASE);
            var typedList = new TypedList(p);
            var daGD = new DaGD(p);

            // RULES

            daEffect.Rule = p.ToTerm("(") + daEffectBase + ")";
            daEffectBase.Rule = timedEffectBase | andDaEffects | forallDaEffect | whenTimeEffect;

            andDaEffects.Rule = p.ToTerm("and") + daEffectsStarList;
            daEffectsStarList.Rule = p.MakeStarRule(daEffectsStarList, daEffect);

            forallDaEffect.Rule = p.ToTerm("forall") + "(" + typedList + ")" + daEffect;
            whenTimeEffect.Rule = p.ToTerm("when") + daGD + timedEffect;

            timedEffect.Rule = p.ToTerm("(") + timedEffectBase + ")";

            p.MarkTransient(daEffect, daEffectBase, timedEffect);

            return (bForm == BForm.BASE) ? daEffectBase : daEffect;
        }
        
    }
}
