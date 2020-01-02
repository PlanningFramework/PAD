using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Form specification of the block represented by the grammar node.
    /// </summary>
    public enum BForm
    {
        /// <summary>
        /// Full form of the block (if that makes sense for the particular grammar node), e.g. "(predicateName ?a ?b)"
        /// </summary>
        FULL,
        /// <summary>
        /// Base form of the block without the parentheses, e.g. "predicateName ?a ?b"
        /// </summary>
        BASE
    }

    /// <summary>
    /// Abstract base node for grammar non-terminals. Every non-terminal needs to derive from it.
    /// </summary>
    public abstract class BaseGrammarNode : NonTerminal
    {
        /// <summary>
        /// Reference to parent master grammar.
        /// </summary>
        protected MasterGrammar p;

        /// <summary>
        /// Requested form of the grammar node.
        /// </summary>
        protected BForm bForm;

        /// <summary>
        /// Sub-expression for optional parametrizing of the generic grammar node.
        /// </summary>
        protected NonTerminal subExpression;

        /// <summary>
        /// Constructor of the base grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Form specification.</param>
        /// <param name="subExpression">Sub-expression specification.</param>
        public BaseGrammarNode(MasterGrammar p, BForm bForm = BForm.FULL, NonTerminal subExpression = null) : base("Base grammar node", typeof(TransientAstNode))
        {
            this.p = p;
            this.bForm = bForm;
            this.subExpression = subExpression;

            p.MarkTransient(this);
            Rule = Make();
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected abstract NonTerminal Make();
    }
}
