using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing either value or term. Typically used as an argument of equals operator (=) or assignment operation.
    /// </summary>
    public class ValueOrTerm : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public ValueOrTerm(MasterGrammar p) : base(p)
        {
            Rule = ConstructValueOrTermRule(p, new NumericExpr(p));
        }

        /// <summary>
        /// Constructs the value or term rule from the specified parameters.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="numericExpr">Numeric expression.</param>
        /// <param name="durationVarExpr">Duration variable..</param>
        /// <returns>Value or term grammar rule.</returns>
        public static NonTerminal ConstructValueOrTermRule(MasterGrammar p, NonTerminal numericExpr, NonTerminal durationVarExpr = null)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var valueOrTerm = new NonTerminal("Value/term", typeof(TransientAstNode));
            var valueOrTermComplex = new NonTerminal("Complex value/term", typeof(TransientAstNode));
            var valueOrTermComplexBase = new NonTerminal("Complex value/term base", typeof(TransientAstNode));

            var valueOrTermIdentifier = new NonTerminal("Identifier value/term", typeof(IdentifierTermAstNode));
            var valueOrTermNumber = new NonTerminal("Number value/term", typeof(NumberTermAstNode));
            var valueOrTermFunction = new NonTerminal("Functional value/term", typeof(TransientAstNode));
            var valueOrTermNumericOp = new NonTerminal("Numeric-op value/term", typeof(TransientAstNode));

            var varOrConstOrFuncIdentifier = new IdentifierTerminal("Variable or constant or function identifier", IdentifierType.VARIABLE_OR_CONSTANT);
            var number = new NumberLiteral("Number");

            // USED SUB-TREES

            var numericOpBase = new NumericOp(p, numericExpr, BForm.BASE);
            var termFunctionBase = new FunctionTerm(p, BForm.BASE);

            // RULES

            valueOrTerm.Rule = valueOrTermIdentifier | valueOrTermNumber | valueOrTermComplex;
            valueOrTermComplex.Rule = p.ToTerm("(") + valueOrTermComplexBase + ")";
            valueOrTermComplexBase.Rule = valueOrTermFunction | valueOrTermNumericOp;
            valueOrTermIdentifier.Rule = varOrConstOrFuncIdentifier;
            valueOrTermNumber.Rule = number;
            valueOrTermFunction.Rule = termFunctionBase;
            valueOrTermNumericOp.Rule = numericOpBase;

            if (durationVarExpr != null)
            {
                valueOrTerm.Rule |= durationVarExpr;
            }

            p.MarkTransient(valueOrTerm, valueOrTermComplex, valueOrTermComplexBase, valueOrTermFunction, valueOrTermNumericOp, numericOpBase);

            return valueOrTerm;
        }
    }
}
