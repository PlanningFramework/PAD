using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing con-GD expression (time constrained GD expression).
    /// </summary>
    public class ConGd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public ConGd(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var conGd = new NonTerminal("Con-GD", typeof(TransientAstNode));
            var conGdBase = new NonTerminal("Con-GD base", typeof(TransientAstNode));
            var conGdStarList = new NonTerminal("Con-GD star-list", typeof(TransientAstNode));

            var andConGdBase = new NonTerminal("AND expression (con-GDs)", typeof(AndConGdAstNode));
            var forallConGdBase = new NonTerminal("FORALL expression (con-GDs)", typeof(ForallConGdAstNode));

            var atEndConGdBase = new NonTerminal("AT-END expression (con-GD)", typeof(AtEndConGdAstNode));
            var alwaysConGdBase = new NonTerminal("ALWAYS expression (con-GD)", typeof(AlwaysConGdAstNode));
            var sometimeConGdBase = new NonTerminal("SOMETIME expression (con-GD)", typeof(SometimeConGdAstNode));
            var withinConGdBase = new NonTerminal("WITHIN expression (con-GD)", typeof(WithinConGdAstNode));
            var atMostOnceConGdBase = new NonTerminal("AT-MOST-ONCE expression (con-GD)", typeof(AtMostOnceConGdAstNode));
            var sometimeAfterConGdBase = new NonTerminal("SOMETIME-AFTER expression (con-GD)", typeof(SometimeAfterConGdAstNode));
            var sometimeBeforeConGdBase = new NonTerminal("SOMETIME-BEFORE expression (con-GD)", typeof(SometimeBeforeConGdAstNode));
            var alwaysWithinConGdBase = new NonTerminal("ALWAYS-WITHIN expression (con-GD)", typeof(AlwaysWithinConGdAstNode));
            var holdDuringConGdBase = new NonTerminal("HOLD-DURING expression (con-GD)", typeof(HoldDuringConGdAstNode));
            var holdAfterConGdBase = new NonTerminal("HOLD-AFTER expression (con-GD)", typeof(HoldAfterConGdAstNode));

            var number = new NumberLiteral("Number");

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var gd = new Gd(p);

            // RULES

            conGd.Rule = p.ToTerm("(") + conGdBase + ")";
            conGdBase.Rule = andConGdBase | forallConGdBase | atEndConGdBase | alwaysConGdBase | sometimeConGdBase | withinConGdBase | atMostOnceConGdBase
                | sometimeAfterConGdBase | sometimeBeforeConGdBase | alwaysWithinConGdBase | holdDuringConGdBase | holdAfterConGdBase;

            andConGdBase.Rule = p.ToTerm("and") + conGdStarList;
            forallConGdBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + conGd;

            atEndConGdBase.Rule = p.ToTerm("at") + p.ToTerm("end") + gd;
            alwaysConGdBase.Rule = p.ToTerm("always") + gd;
            sometimeConGdBase.Rule = p.ToTerm("sometime") + gd;
            withinConGdBase.Rule = p.ToTerm("within") + number + gd;
            atMostOnceConGdBase.Rule = p.ToTerm("at-most-once") + gd;
            sometimeAfterConGdBase.Rule = p.ToTerm("sometime-after") + gd + gd;
            sometimeBeforeConGdBase.Rule = p.ToTerm("sometime-before") + gd + gd;
            alwaysWithinConGdBase.Rule = p.ToTerm("always-within") + number + gd + gd;
            holdDuringConGdBase.Rule = p.ToTerm("hold-during") + number + number + gd;
            holdAfterConGdBase.Rule = p.ToTerm("hold-after") + number + gd;

            conGdStarList.Rule = p.MakeStarRule(conGdStarList, conGd);

            p.MarkTransient(conGd, conGdBase);

            Rule = conGd;
        }
    }
}
