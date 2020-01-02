using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a general numeric operation. Can be parametrized with a specific numeric expression type.
    /// </summary>
    public class NumericOp : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="numericExpr">Numeric expression type used within the numeric operation</param>
        public NumericOp(MasterGrammar p, NonTerminal numericExpr) : this(p, numericExpr, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="numericExpr">Numeric expression type used within the numeric operation</param>
        /// <param name="bForm">Block form.</param>
        public NumericOp(MasterGrammar p, NonTerminal numericExpr, BForm bForm) : base(p, bForm, numericExpr)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var numericOp = new NonTerminal("Numeric operation", typeof(TransientAstNode));
            var numericOpBase = new NonTerminal("Numeric operation base", typeof(TransientAstNode));

            var multiOp = new NonTerminal("Multiary operation", typeof(NumericOpAstNode));
            var multiOperator = new NonTerminal("Multiary operator", typeof(TransientAstNode));
            var multiOpArguments = new NonTerminal("Multiary operation arguments", typeof(TransientAstNode));

            var binaryOp = new NonTerminal("Binary operation", typeof(NumericOpAstNode));
            var binaryUnaryOp = new NonTerminal("Binary or unary operation", typeof(NumericOpAstNode));
            var binaryOpSecondArg = new NonTerminal("Second argument of binary operation", typeof(TransientAstNode));

            // USED SUB-TREES

            var numericExpr = subExpression; // passed as a parameter

            // RULES

            numericOp.Rule = p.ToTerm("(") + numericOpBase + ")";
            numericOpBase.Rule = multiOp | binaryOp | binaryUnaryOp;

            multiOp.Rule = multiOperator + numericExpr + multiOpArguments; // at least two numeric args
            multiOperator.Rule = p.ToTerm("*") | "+";
            multiOpArguments.Rule = p.MakePlusRule(multiOpArguments, numericExpr);

            binaryOp.Rule = p.ToTerm("/") + numericExpr + numericExpr; // the only binary is division
            binaryUnaryOp.Rule = p.ToTerm("-") + numericExpr + binaryOpSecondArg; // unary/binary minus
            binaryOpSecondArg.Rule = numericExpr | p.Empty;

            p.MarkTransient(numericOp, numericOpBase, multiOperator, binaryOpSecondArg);

            return (bForm == BForm.BASE) ? numericOpBase : numericOp;
        }
    }
}
