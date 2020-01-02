using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing timed variant of effect.
    /// </summary>
    public class TimedEffect : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public TimedEffect(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public TimedEffect(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var timedEffect = new NonTerminal("Timed effect", typeof(TransientAstNode));
            var timedEffectBase = new NonTerminal("Timed effect base", typeof(TransientAstNode));

            var atTimedEffectBase = new NonTerminal("AT expression (timed effect)", typeof(AtTimedEffectAstNode));
            var assignTimedEffectBase = new NonTerminal("Assign expression (timed effect)", typeof(AssignTimedEffectAstNode));

            var timeSpecifier = new NonTerminal("Time specifier", typeof(TransientAstNode));
            var timedCondEffect = new NonTerminal("Timed cond-Effect", typeof(TransientAstNode));
            var timedCondEffectBase = new NonTerminal("Timed Cond-Effect base", typeof(TransientAstNode));

            var andTimedPEffectsBase = new NonTerminal("AND expression (timed p-effects)", typeof(AndPEffectsAstNode));
            var timedPEffectStarList = new NonTerminal("Timed p-effects star-list", typeof(TransientAstNode));

            var assignTimedEffectOp = new NonTerminal("Assign expression operator (timed effect)", typeof(TransientAstNode));
            var assignTimedEffectFuncHead = new NonTerminal("Assign expression function head (timed effect)", typeof(TransientAstNode));
            var assignFuncIdentifier = new NonTerminal("Function identifier for ASSIGN operation", typeof(FunctionTermAstNode));

            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var assignFunctionTerm = new FunctionTerm(p);
            var numericExprT = new NumericExprT(p);
            var timedPEffect = new PEffectT(p, BForm.FULL);
            var timedPEffectBase = new PEffectT(p, BForm.BASE);

            // RULES

            // timed effect
            timedEffect.Rule = p.ToTerm("(") + timedEffectBase + ")";
            timedEffectBase.Rule = atTimedEffectBase | assignTimedEffectBase;

            atTimedEffectBase.Rule = p.ToTerm("at") + timeSpecifier + timedCondEffect;
            timeSpecifier.Rule = p.ToTerm("start") | p.ToTerm("end");

            // timed cond-effect ("cond-effect" with assign operation being extended with the use of "f-exp-da")
            timedCondEffect.Rule = p.ToTerm("(") + timedCondEffectBase + ")";
            timedCondEffectBase.Rule = timedPEffectBase | andTimedPEffectsBase;

            andTimedPEffectsBase.Rule = p.ToTerm("and") + timedPEffectStarList;
            timedPEffectStarList.Rule = p.MakeStarRule(timedPEffectStarList, timedPEffect);

            // timed p-effect
            timedPEffect.Rule = p.ToTerm("(") + timedPEffectBase + ")";

            assignTimedEffectBase.Rule = assignTimedEffectOp + assignTimedEffectFuncHead + numericExprT;
            assignTimedEffectOp.Rule = p.ToTerm("increase") | "decrease";
            assignTimedEffectFuncHead.Rule = assignFuncIdentifier | assignFunctionTerm;
            assignFuncIdentifier.Rule = functionIdentifier;

            p.MarkTransient(timedEffect, timedEffectBase, timedCondEffect, timedCondEffectBase, assignTimedEffectFuncHead);

            return (bForm == BForm.BASE) ? timedEffectBase : timedEffect;
        }
    }
}
