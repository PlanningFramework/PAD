using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing an action.
    /// </summary>
    public class Action : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Action(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public Action(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var actionDef = new NonTerminal("Action definition", typeof(TransientAstNode));
            var actionDefBase = new NonTerminal("Action definition", typeof(DomainActionAstNode));
            var actionParameters = new NonTerminal("Action parameters", typeof(TransientAstNode));
            var actionPreconditions = new NonTerminal("Action preconditions", typeof(TransientAstNode));
            var actionEffects = new NonTerminal("Action effects", typeof(TransientAstNode));

            var emptyOrPreGd = new NonTerminal("Empty or pre-GD", typeof(TransientAstNode));
            var emptyOrEffect = new NonTerminal("Empty or effect", typeof(TransientAstNode));
            var emptyBlock = new NonTerminal("Empty block", typeof(TransientAstNode));

            var actionName = new IdentifierTerminal("Action name", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var preGd = new PreGd(p);
            var effect = new Effect(p);

            // RULES

            actionDef.Rule = p.ToTerm("(") + actionDefBase + ")";
            actionDefBase.Rule = p.ToTerm(":action") + actionName + actionParameters + actionPreconditions + actionEffects;
            actionParameters.Rule = p.ToTerm(":parameters") + "(" + typedList + ")";
            actionPreconditions.Rule = (p.ToTerm(":precondition") + emptyOrPreGd) | p.Empty;
            actionEffects.Rule = (p.ToTerm(":effect") + emptyOrEffect) | p.Empty;

            emptyOrPreGd.Rule = emptyBlock | preGd;
            emptyOrEffect.Rule = emptyBlock | effect;
            emptyBlock.Rule = p.ToTerm("(") + p.ToTerm(")");

            p.MarkTransient(actionDef, emptyOrPreGd, emptyOrEffect);

            Rule = (bForm == BForm.BASE) ? actionDefBase : actionDef;
        }
    }
}
