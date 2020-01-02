using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a metric expression (for metric definition of the problem).
    /// </summary>
    public class MetricExpr : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public MetricExpr(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var metricExpr = new NonTerminal("Metric expression", typeof(TransientAstNode));
            var metricExprTimeConst = new NonTerminal("Total-time const (metric expression)", typeof(FunctionTermAstNode));
            var metricExprNumber = new NonTerminal("Number term (metric expression)", typeof(NumberTermAstNode));
            var metricExprFuncIdentifier = new NonTerminal("Function term (metric expression)", typeof(FunctionTermAstNode));
            var metricExprComplex = new NonTerminal("Complex metric expression", typeof(TransientAstNode));
            var metricExprComplexBase = new NonTerminal("Complex metric expression base", typeof(TransientAstNode));

            var metricExprViolatedBase = new NonTerminal("Preference violation expression (metric expression)", typeof(MetricPreferenceViolationAstNode));
            var metricExprFunctionBase = new NonTerminal("Function term (metric expression)", typeof(FunctionTermAstNode));
            var metricOpBase = new NonTerminal("Numeric operation base (metric expression)", typeof(TransientAstNode));

            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);
            var number = new NumberLiteral("Number");
            var preferenceName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var functionTerm = new FunctionTermC(p, BForm.BASE);
            var numericOp = new NumericOp(p, metricExpr, BForm.BASE);

            // RULES

            metricExpr.Rule = metricExprTimeConst | metricExprNumber | metricExprFuncIdentifier | metricExprComplex;

            metricExprTimeConst.Rule = p.ToTerm("total-time");
            metricExprNumber.Rule = number;
            metricExprFuncIdentifier.Rule = functionIdentifier;
            metricExprComplex.Rule = p.ToTerm("(") + metricExprComplexBase + ")";
            metricExprComplexBase.Rule = metricExprViolatedBase | metricExprFunctionBase | metricOpBase;

            metricExprViolatedBase.Rule = p.ToTerm("is-violated") + preferenceName;
            metricExprFunctionBase.Rule = functionTerm;
            metricOpBase.Rule = numericOp;

            p.MarkTransient(metricExpr, metricExprComplex, metricExprComplexBase, metricExprFunctionBase, metricOpBase);

            return metricExpr;
        }
    }
}
