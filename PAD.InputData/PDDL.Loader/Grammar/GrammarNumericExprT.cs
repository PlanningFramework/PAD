using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Timed variant of the numeric expression grammar node.
    /// </summary>
    public class NumericExprT : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public NumericExprT(MasterGrammar p) : base(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var numericExprT = new NonTerminal("Timed numeric expression", typeof(TransientAstNode));
            var numericExprTSimple = new NonTerminal("Simple timed numeric expression", typeof(TimedNumericExpressionAstNode));
            var numericExprTComplex = new NonTerminal("Product of timed numeric expression", typeof(TransientAstNode));

            var numericExprTProduct = new NonTerminal("Product variant of timed numeric expression", typeof(TimedNumericExpressionAstNode));
            var numericExprTProductVariant = new NonTerminal("Either product variant of timed numeric expression", typeof(TransientAstNode));
            var numericExprTProductVariant1 = new NonTerminal("Product variant 1 of timed numeric expression", typeof(TransientAstNode));
            var numericExprTProductVariant2 = new NonTerminal("Product variant 2 of timed numeric expression", typeof(TransientAstNode));

            // USED SUB-TREES

            var numericExpr = new NumericExpr(p);

            // RULES

            numericExprT.Rule = numericExprTSimple | numericExprTComplex;
            numericExprTSimple.Rule = p.ToTerm("#t");
            numericExprTComplex.Rule = p.ToTerm("(") + numericExprTProduct + ")";

            numericExprTProduct.Rule = p.ToTerm("*") + numericExprTProductVariant;
            numericExprTProductVariant.Rule = numericExprTProductVariant1 | numericExprTProductVariant2;
            numericExprTProductVariant1.Rule = p.ToTerm("#t") + numericExpr;
            numericExprTProductVariant2.Rule = numericExpr + "#t";

            p.MarkTransient(numericExprT, numericExprTComplex, numericExprTProductVariant);

            return numericExprT;
        }
    }
}
