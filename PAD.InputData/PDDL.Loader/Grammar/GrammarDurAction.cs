using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a duration action.
    /// </summary>
    public class DurAction : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DurAction(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public DurAction(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var durActionDef = new NonTerminal("Durative action definition", typeof(TransientAstNode));
            var durActionDefBase = new NonTerminal("Durative action definition", typeof(DomainDurActionAstNode));
            var daParameters = new NonTerminal("Durative action parameters", typeof(TransientAstNode));
            var daDuration = new NonTerminal("Durative action duration", typeof(TransientAstNode));
            var daConditions = new NonTerminal("Durative action conditions", typeof(TransientAstNode));
            var daEffects = new NonTerminal("Durative action effects", typeof(TransientAstNode));

            var emptyOrDurConstr = new NonTerminal("Empty or duration-constraint", typeof(TransientAstNode));
            var emptyOrDaGd = new NonTerminal("Empty or da-GD", typeof(TransientAstNode));
            var emptyOrDaEffect = new NonTerminal("Empty or da-effect", typeof(TransientAstNode));
            var emptyBlock = new NonTerminal("Empty block", typeof(TransientAstNode));

            var daName = new IdentifierTerminal("Durative action name", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var durationConstr = new DurConstr(p);
            var daGd = new DaGd(p);
            var daEffect = new DaEffect(p);

            // RULES

            durActionDef.Rule = p.ToTerm("(") + durActionDefBase + ")";
            durActionDefBase.Rule = p.ToTerm(":durative-action") + daName + daParameters + daDuration + daConditions + daEffects;
            daParameters.Rule = p.ToTerm(":parameters") + "(" + typedList + ")";
            daDuration.Rule = p.ToTerm(":duration") + emptyOrDurConstr;
            daConditions.Rule = p.ToTerm(":condition") + emptyOrDaGd;
            daEffects.Rule = p.ToTerm(":effect") + emptyOrDaEffect;

            emptyOrDurConstr.Rule = emptyBlock | durationConstr;
            emptyOrDaGd.Rule = emptyBlock | daGd;
            emptyOrDaEffect.Rule = emptyBlock | daEffect;
            emptyBlock.Rule = p.ToTerm("(") + p.ToTerm(")");

            p.MarkTransient(durActionDef, emptyOrDurConstr, emptyOrDaGd, emptyOrDaEffect);

            Rule = (bForm == BForm.BASE) ? durActionDefBase : durActionDef;
        }
    }
}
