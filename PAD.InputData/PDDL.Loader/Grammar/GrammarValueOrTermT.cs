using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Timed variant of ValueOrTerm grammar node, allowing the usage of duration variant of numeric expressions (NumericExprDa).
    /// </summary>
    public class ValueOrTermT : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public ValueOrTermT(MasterGrammar p) : base(p)
        {
            var durationVar = new NonTerminal("Duration variable term (duration numeric expression)", typeof(DurationVariableTermAstNode)) {Rule = p.ToTerm("?duration")};
            Rule = ValueOrTerm.ConstructValueOrTermRule(p, new NumericExprDa(p), durationVar);
        }
    }
}
