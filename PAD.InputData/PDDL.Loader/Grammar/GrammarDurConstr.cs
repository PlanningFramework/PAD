using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a duration constraint.
    /// </summary>
    public class DurConstr : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DurConstr(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var durationConstr = new NonTerminal("Duration-constraint", typeof(TransientAstNode));
            var durationConstrBase = new NonTerminal("Duration-constraint base", typeof(TransientAstNode));

            var simpleDurationConstr = new NonTerminal("Simple duration-constraint", typeof(TransientAstNode));
            var simpleDurationConstrBase = new NonTerminal("Simple duration-constraint base", typeof(TransientAstNode));

            var andSimpleDurationConstrsBase = new NonTerminal("AND expression (duration-constraints)", typeof(AndSimpleDurationConstraintsAstNode));
            var simpleDurationConstrsPlusList = new NonTerminal("Simple duration constraints plus-list", typeof(TransientAstNode));

            var atSimpleDurationConstr = new NonTerminal("AT expression (duration-constraint)", typeof(AtSimpleDurationConstraintAstNode));
            var compOpSimpleDurationConstr = new NonTerminal("CompareOp (duration-constraint)", typeof(CompOpSimpleDurationConstraintAstNode));
            var timeSpecifier = new NonTerminal("Time specifier", typeof(TransientAstNode));

            var durCompareOp = new NonTerminal("Compare operator for duration-constraint", typeof(TransientAstNode));
            var durCompareVal = new NonTerminal("Value for duration-constraint", typeof(TransientAstNode));

            // USED SUB-TREES

            var numericExpr = new NumericExpr(p);

            // RULES

            durationConstr.Rule = p.ToTerm("(") + durationConstrBase + ")";
            durationConstrBase.Rule = andSimpleDurationConstrsBase | simpleDurationConstrBase;

            andSimpleDurationConstrsBase.Rule = p.ToTerm("and") + simpleDurationConstrsPlusList;
            simpleDurationConstrsPlusList.Rule = p.MakeStarRule(simpleDurationConstrsPlusList, simpleDurationConstr);

            simpleDurationConstr.Rule = p.ToTerm("(") + simpleDurationConstrBase + ")";
            simpleDurationConstrBase.Rule = atSimpleDurationConstr | compOpSimpleDurationConstr;

            atSimpleDurationConstr.Rule = p.ToTerm("at") + timeSpecifier + simpleDurationConstr;
            compOpSimpleDurationConstr.Rule = durCompareOp + "?duration" + durCompareVal;
            timeSpecifier.Rule = p.ToTerm("start") | p.ToTerm("end");

            durCompareOp.Rule = p.ToTerm("<=") | ">=" | "=";
            durCompareVal.Rule = numericExpr;

            p.MarkTransient(durationConstr, durationConstrBase, simpleDurationConstr, simpleDurationConstrBase, durCompareVal);

            Rule = durationConstr;
        }
        
    }
}
