using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing pref-con-GD expression (time constrained GD expression with preferences).
    /// </summary>
    public class PrefConGd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PrefConGd(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var prefConGd = new NonTerminal("Pref-con-GD", typeof(TransientAstNode));
            var prefConGdBase = new NonTerminal("Pref-con-GD base", typeof(TransientAstNode));
            var prefConGdStarList = new NonTerminal("Pref-con-GD star-list", typeof(TransientAstNode));

            var preferencePrefConGdBase = new NonTerminal("Preference expression (pref-con-GD)", typeof(PreferenceConGdAstNode));
            var preferenceNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var preferenceName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var andPrefConGdBase = new NonTerminal("AND expression (pref-con-GDs)", typeof(AndConGdAstNode));
            var forallPrefConGdBase = new NonTerminal("FORALL expression (pref-con-GDs)", typeof(ForallConGdAstNode));

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
            var conGd = new ConGd(p);
            var gd = new Gd(p);

            // RULES

            prefConGd.Rule = p.ToTerm("(") + prefConGdBase + ")";
            prefConGdBase.Rule = preferencePrefConGdBase | andPrefConGdBase | forallPrefConGdBase | atEndConGdBase | alwaysConGdBase | sometimeConGdBase | withinConGdBase
                | atMostOnceConGdBase | sometimeAfterConGdBase | sometimeBeforeConGdBase | alwaysWithinConGdBase | holdDuringConGdBase | holdAfterConGdBase;

            preferencePrefConGdBase.Rule = p.ToTerm("preference") + preferenceNameOrEmpty + conGd;
            preferenceNameOrEmpty.Rule = preferenceName | p.Empty;

            andPrefConGdBase.Rule = p.ToTerm("and") + prefConGdStarList;
            forallPrefConGdBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + prefConGd;

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

            prefConGdStarList.Rule = p.MakeStarRule(prefConGdStarList, prefConGd);

            p.MarkTransient(prefConGd, prefConGdBase);

            Rule = prefConGd;
        }
    }
}
