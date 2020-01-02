using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing con-GD expression (time constrained GD expression).
    /// </summary>
    public class ConGD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public ConGD(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var conGD = new NonTerminal("Con-GD", typeof(TransientAstNode));
            var conGDBase = new NonTerminal("Con-GD base", typeof(TransientAstNode));
            var conGDStarList = new NonTerminal("Con-GD star-list", typeof(TransientAstNode));

            var andConGDBase = new NonTerminal("AND expression (con-GDs)", typeof(AndConGDAstNode));
            var forallConGDBase = new NonTerminal("FORALL expression (con-GDs)", typeof(ForallConGDAstNode));

            var atEndConGDBase = new NonTerminal("AT-END expression (con-GD)", typeof(AtEndConGDAstNode));
            var alwaysConGDBase = new NonTerminal("ALWAYS expression (con-GD)", typeof(AlwaysConGDAstNode));
            var sometimeConGDBase = new NonTerminal("SOMETIME expression (con-GD)", typeof(SometimeConGDAstNode));
            var withinConGDBase = new NonTerminal("WITHIN expression (con-GD)", typeof(WithinConGDAstNode));
            var atMostOnceConGDBase = new NonTerminal("AT-MOST-ONCE expression (con-GD)", typeof(AtMostOnceConGDAstNode));
            var sometimeAfterConGDBase = new NonTerminal("SOMETIME-AFTER expression (con-GD)", typeof(SometimeAfterConGDAstNode));
            var sometimeBeforeConGDBase = new NonTerminal("SOMETIME-BEFORE expression (con-GD)", typeof(SometimeBeforeConGDAstNode));
            var alwaysWithinConGDBase = new NonTerminal("ALWAYS-WITHIN expression (con-GD)", typeof(AlwaysWithinConGDAstNode));
            var holdDuringConGDBase = new NonTerminal("HOLD-DURING expression (con-GD)", typeof(HoldDuringConGDAstNode));
            var holdAfterConGDBase = new NonTerminal("HOLD-AFTER expression (con-GD)", typeof(HoldAfterConGDAstNode));

            var number = new NumberLiteral("Number");

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var GD = new GD(p);

            // RULES

            conGD.Rule = p.ToTerm("(") + conGDBase + ")";
            conGDBase.Rule = andConGDBase | forallConGDBase | atEndConGDBase | alwaysConGDBase | sometimeConGDBase | withinConGDBase | atMostOnceConGDBase
                | sometimeAfterConGDBase | sometimeBeforeConGDBase | alwaysWithinConGDBase | holdDuringConGDBase | holdAfterConGDBase;

            andConGDBase.Rule = p.ToTerm("and") + conGDStarList;
            forallConGDBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + conGD;

            atEndConGDBase.Rule = p.ToTerm("at") + p.ToTerm("end") + GD;
            alwaysConGDBase.Rule = p.ToTerm("always") + GD;
            sometimeConGDBase.Rule = p.ToTerm("sometime") + GD;
            withinConGDBase.Rule = p.ToTerm("within") + number + GD;
            atMostOnceConGDBase.Rule = p.ToTerm("at-most-once") + GD;
            sometimeAfterConGDBase.Rule = p.ToTerm("sometime-after") + GD + GD;
            sometimeBeforeConGDBase.Rule = p.ToTerm("sometime-before") + GD + GD;
            alwaysWithinConGDBase.Rule = p.ToTerm("always-within") + number + GD + GD;
            holdDuringConGDBase.Rule = p.ToTerm("hold-during") + number + number + GD;
            holdAfterConGDBase.Rule = p.ToTerm("hold-after") + number + GD;

            conGDStarList.Rule = p.MakeStarRule(conGDStarList, conGD);

            p.MarkTransient(conGD, conGDBase);

            return conGD;
        }
    }
}
