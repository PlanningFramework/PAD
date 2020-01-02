using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Duration action variant of a numeric expression node.
    /// </summary>
    public class NumericExprDa : NumericExpr
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public NumericExprDa(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for an optional duration variable to be added into grammar rules.
        /// </summary>
        /// <returns>Duration variable rule.</returns>
        protected override NonTerminal getDurationVariable()
        {
            var durationVar = new NonTerminal("Duration variable term (duration numeric expression)", typeof(DurationVariableTermAstNode));
            durationVar.Rule = p.ToTerm("?duration");
            return durationVar;
        }
    }
}
