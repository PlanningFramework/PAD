using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing an effect (primitive effect(s) with a possibility of conditional effect(s)).
    /// </summary>
    public class Effect : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Effect(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public Effect(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var effect = new NonTerminal("Effect", typeof(TransientAstNode));
            var effectBase = new NonTerminal("Effect base", typeof(TransientAstNode));

            var andCEffectsBase = new NonTerminal("AND expression (c-effects)", typeof(AndCEffectsAstNode));
            var cEffectStarList = new NonTerminal("C-effects list", typeof(TransientAstNode));

            var cEffect = new NonTerminal("C-effect", typeof(TransientAstNode));
            var cEffectBase = new NonTerminal("C-effect base", typeof(TransientAstNode));

            var forallCEffectBase = new NonTerminal("FORALL expression (c-effect)", typeof(ForallCEffectAstNode));
            var whenCEffectBase = new NonTerminal("WHEN expression (c-effect)", typeof(WhenCEffectAstNode));

            var condEffect = new NonTerminal("Cond-Effect", typeof(TransientAstNode));
            var condEffectBase = new NonTerminal("Cond-Effect base", typeof(TransientAstNode));

            var andPEffectsBase = new NonTerminal("AND expression (p-effects)", typeof(AndPEffectsAstNode));
            var pEffectStarList = new NonTerminal("P-effects star-list", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var GD = new GD(p);
            var pEffect = new PEffect(p, BForm.FULL);
            var pEffectBase = new PEffect(p, BForm.BASE);

            // RULES

            // (general) effect - c-effect or a list of c-effects
            effect.Rule = p.ToTerm("(") + effectBase + ")";
            effectBase.Rule = cEffectBase | andCEffectsBase;

            andCEffectsBase.Rule = p.ToTerm("and") + cEffectStarList;
            cEffectStarList.Rule = p.MakeStarRule(cEffectStarList, cEffect);

            // c-effect (allowing conditional effects)
            cEffect.Rule = p.ToTerm("(") + cEffectBase + ")";
            cEffectBase.Rule = forallCEffectBase | whenCEffectBase | pEffectBase;

            forallCEffectBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + effect;
            whenCEffectBase.Rule = p.ToTerm("when") + GD + condEffect;

            // cond-effect (not allowing inner conditional effects)
            condEffect.Rule = p.ToTerm("(") + condEffectBase + ")";
            condEffectBase.Rule = pEffectBase | andPEffectsBase;

            andPEffectsBase.Rule = p.ToTerm("and") + pEffectStarList;
            pEffectStarList.Rule = p.MakeStarRule(pEffectStarList, pEffect);

            p.MarkTransient(effect, effectBase, cEffect, cEffectBase, condEffect, condEffectBase);

            return (bForm == BForm.BASE) ? effectBase : effect;
        }
    }
}
