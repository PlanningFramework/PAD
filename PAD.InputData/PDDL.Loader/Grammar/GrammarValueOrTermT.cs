using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Timed variant of ValueOrTerm grammar node, allowing the usage of duration variant of numeric expressions (NumericExprDa).
    /// </summary>
    public class ValueOrTermT : ValueOrTerm
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public ValueOrTermT(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for specifying the type of numeric expression that is being used.
        /// </summary>
        /// <returns>Grammar node of the specific numeric expression.</returns>
        protected override NonTerminal getNumericExpr()
        {
            return new NumericExprDa(p);
        }

        /// <summary>
        /// Factory method for an optional duration variable to be added into grammar rules.
        /// </summary>
        /// <returns>Duration variable rule.</returns>
        protected override NonTerminal getDurationVarExpr()
        {
            var durationVar = new NonTerminal("Duration variable term (duration numeric expression)", typeof(DurationVariableTermAstNode));
            durationVar.Rule = p.ToTerm("?duration");
            return durationVar;
        }
    }
}
