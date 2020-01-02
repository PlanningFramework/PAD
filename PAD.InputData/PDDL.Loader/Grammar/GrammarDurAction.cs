using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

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
        /// <param name="bForm">Block form.</param>
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
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var durActionDef = new NonTerminal("Durative action definition", typeof(TransientAstNode));
            var durActionDefBase = new NonTerminal("Durative action definition", typeof(DomainDurActionAstNode));
            var daParameters = new NonTerminal("Durative action parameters", typeof(TransientAstNode));
            var daDuration = new NonTerminal("Durative action duration", typeof(TransientAstNode));
            var daConditions = new NonTerminal("Durative action conditions", typeof(TransientAstNode));
            var daEffects = new NonTerminal("Durative action effects", typeof(TransientAstNode));

            var emptyOrDurConstr = new NonTerminal("Empty or duration-constraint", typeof(TransientAstNode));
            var emptyOrDaGD = new NonTerminal("Empty or da-GD", typeof(TransientAstNode));
            var emptyOrDaEffect = new NonTerminal("Empty or da-effect", typeof(TransientAstNode));
            var emptyBlock = new NonTerminal("Empty block", typeof(TransientAstNode));

            var daName = new IdentifierTerminal("Durative action name", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var durationConstr = new DurConstr(p);
            var daGD = new DaGD(p);
            var daEffect = new DaEffect(p);

            // RULES

            durActionDef.Rule = p.ToTerm("(") + durActionDefBase + ")";
            durActionDefBase.Rule = p.ToTerm(":durative-action") + daName + daParameters + daDuration + daConditions + daEffects;
            daParameters.Rule = p.ToTerm(":parameters") + "(" + typedList + ")";
            daDuration.Rule = p.ToTerm(":duration") + emptyOrDurConstr;
            daConditions.Rule = p.ToTerm(":condition") + emptyOrDaGD;
            daEffects.Rule = p.ToTerm(":effect") + emptyOrDaEffect;

            emptyOrDurConstr.Rule = emptyBlock | durationConstr;
            emptyOrDaGD.Rule = emptyBlock | daGD;
            emptyOrDaEffect.Rule = emptyBlock | daEffect;
            emptyBlock.Rule = p.ToTerm("(") + p.ToTerm(")");

            p.MarkTransient(durActionDef, emptyOrDurConstr, emptyOrDaGD, emptyOrDaEffect);

            return (bForm == BForm.BASE) ? durActionDefBase : durActionDef;
        }
    }
}
