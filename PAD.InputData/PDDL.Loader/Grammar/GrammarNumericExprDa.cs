using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Duration action variant of a numeric expression node.
    /// </summary>
    public class NumericExprDa : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public NumericExprDa(MasterGrammar p) : base(p)
        {
            var durationVar = new NonTerminal("Duration variable term (duration numeric expression)", typeof(DurationVariableTermAstNode)) {Rule = p.ToTerm("?duration")};
            Rule = NumericExpr.ConstructNumericExprRule(p, durationVar);
        }
    }
}
