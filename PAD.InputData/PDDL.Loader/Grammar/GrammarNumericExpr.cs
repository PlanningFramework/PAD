using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a general numeric expression.
    /// </summary>
    public class NumericExpr : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public NumericExpr(MasterGrammar p) : base(p)
        {
            Rule = ConstructNumericExprRule(p);
        }

        /// <summary>
        /// Constructs the numeric expression rule from the specified parameters.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="durationVariable">Duration variable.</param>
        /// <returns>Numeric expression grammar rule.</returns>
        public static NonTerminal ConstructNumericExprRule(MasterGrammar p, NonTerminal durationVariable = null)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var numericExpr = new NonTerminal("Numeric expression", typeof(TransientAstNode));
            var numericExprNumber = new NonTerminal("Number term for numeric expression", typeof(NumberTermAstNode));
            var numericExprFuncIdentifier = new NonTerminal("Function identifier term for numeric expression", typeof(FunctionTermAstNode));
            var numericExprComplex = new NonTerminal("Complex numeric expression", typeof(TransientAstNode));
            var numericExprComplexBase = new NonTerminal("Complex numeric expression base", typeof(TransientAstNode));

            var number = new NumberLiteral("Number");
            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var termFunctionBase = new FunctionTerm(p, BForm.BASE);
            var numericOpBase = new NumericOp(p, numericExpr, BForm.BASE);

            // RULES

            numericExpr.Rule = numericExprNumber | numericExprFuncIdentifier | numericExprComplex;
            numericExprNumber.Rule = number;
            numericExprFuncIdentifier.Rule = functionIdentifier;
            numericExprComplex.Rule = p.ToTerm("(") + numericExprComplexBase + ")";
            numericExprComplexBase.Rule = termFunctionBase | numericOpBase;

            if (durationVariable != null)
            {
                numericExpr.Rule = durationVariable | numericExpr.Rule;
            }

            p.MarkTransient(numericExpr, numericExprComplex, numericExprComplexBase, numericOpBase);

            return numericExpr;
        }
    }
}
