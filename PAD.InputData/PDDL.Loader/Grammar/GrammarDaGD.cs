using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing da-GD expression (duration action GD).
    /// </summary>
    public class DaGD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DaGD(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var daGD = new NonTerminal("Da-GD", typeof(TransientAstNode));
            var daGDBase = new NonTerminal("Da-GD base", typeof(TransientAstNode));
            var daGDStarList = new NonTerminal("Da-GD star-list", typeof(TransientAstNode));

            var andDaGDBase = new NonTerminal("AND expression (da-GDs)", typeof(AndDaGDAstNode));
            var forallDaGDBase = new NonTerminal("FORALL expression (da-GD)", typeof(ForallDaGDAstNode));
            var prefDaGDBase = new NonTerminal("Preference (da-GD)", typeof(PreferenceDaGDAstNode));

            var prefNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var prefName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var timedDaGD = new NonTerminal("Timed-da-GD", typeof(TransientAstNode));
            var timedDaGDBase = new NonTerminal("Timed-da-GD base", typeof(TransientAstNode));
            var atTimedDaGD = new NonTerminal("AT expression (timed-da-GD)", typeof(AtTimedDaGDAstNode));
            var overTimedDaGD = new NonTerminal("OVER expression (timed-da-GD)", typeof(OverTimedDaGDAstNode));

            var timeSpecifier = new NonTerminal("Time specifier", typeof(TransientAstNode));
            var intervalSpecifier = new NonTerminal("Interval specifier", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var GD = new GD(p);

            // RULES

            daGD.Rule = p.ToTerm("(") + daGDBase + ")";
            daGDBase.Rule = andDaGDBase | forallDaGDBase | prefDaGDBase | timedDaGDBase;

            andDaGDBase.Rule = p.ToTerm("and") + daGDStarList;
            forallDaGDBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + daGD;

            prefDaGDBase.Rule = p.ToTerm("preference") + prefNameOrEmpty + timedDaGD;
            prefNameOrEmpty.Rule = prefName | p.Empty;

            timedDaGD.Rule = p.ToTerm("(") + timedDaGDBase + ")";
            timedDaGDBase.Rule = atTimedDaGD | overTimedDaGD;
            atTimedDaGD.Rule = p.ToTerm("at") + timeSpecifier + GD;
            overTimedDaGD.Rule = p.ToTerm("over") + intervalSpecifier + GD;

            timeSpecifier.Rule = p.ToTerm("start") | p.ToTerm("end");
            intervalSpecifier.Rule = p.ToTerm("all");

            daGDStarList.Rule = p.MakeStarRule(daGDStarList, daGD);

            p.MarkTransient(daGD, daGDBase, timedDaGD, timedDaGDBase);

            return daGD;
        }
    }
}
