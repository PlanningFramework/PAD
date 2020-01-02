using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing pref-con-GD expression (time constrained GD expression with preferences).
    /// </summary>
    public class PrefConGD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PrefConGD(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var prefConGD = new NonTerminal("Pref-con-GD", typeof(TransientAstNode));
            var prefConGDBase = new NonTerminal("Pref-con-GD base", typeof(TransientAstNode));
            var prefConGDStarList = new NonTerminal("Pref-con-GD star-list", typeof(TransientAstNode));

            var preferencePrefConGDBase = new NonTerminal("Preference expression (pref-con-GD)", typeof(PreferenceConGDAstNode));
            var preferenceNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var preferenceName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var andPrefConGDBase = new NonTerminal("AND expression (pref-con-GDs)", typeof(AndConGDAstNode));
            var forallPrefConGDBase = new NonTerminal("FORALL expression (pref-con-GDs)", typeof(ForallConGDAstNode));

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
            var conGD = new ConGD(p);
            var GD = new GD(p);

            // RULES

            prefConGD.Rule = p.ToTerm("(") + prefConGDBase + ")";
            prefConGDBase.Rule = preferencePrefConGDBase | andPrefConGDBase | forallPrefConGDBase | atEndConGDBase | alwaysConGDBase | sometimeConGDBase | withinConGDBase
                | atMostOnceConGDBase | sometimeAfterConGDBase | sometimeBeforeConGDBase | alwaysWithinConGDBase | holdDuringConGDBase | holdAfterConGDBase;

            preferencePrefConGDBase.Rule = p.ToTerm("preference") + preferenceNameOrEmpty + conGD;
            preferenceNameOrEmpty.Rule = preferenceName | p.Empty;

            andPrefConGDBase.Rule = p.ToTerm("and") + prefConGDStarList;
            forallPrefConGDBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + prefConGD;

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

            prefConGDStarList.Rule = p.MakeStarRule(prefConGDStarList, prefConGD);

            p.MarkTransient(prefConGD, prefConGDBase);

            return prefConGD;
        }
    }
}
