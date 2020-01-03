using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing da-GD expression (duration action GD).
    /// </summary>
    public class DaGd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public DaGd(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var daGd = new NonTerminal("Da-GD", typeof(TransientAstNode));
            var daGdBase = new NonTerminal("Da-GD base", typeof(TransientAstNode));
            var daGdStarList = new NonTerminal("Da-GD star-list", typeof(TransientAstNode));

            var andDaGdBase = new NonTerminal("AND expression (da-GDs)", typeof(AndDaGdAstNode));
            var forallDaGdBase = new NonTerminal("FORALL expression (da-GD)", typeof(ForallDaGdAstNode));
            var prefDaGdBase = new NonTerminal("Preference (da-GD)", typeof(PreferenceDaGdAstNode));

            var prefNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var prefName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var timedDaGd = new NonTerminal("Timed-da-GD", typeof(TransientAstNode));
            var timedDaGdBase = new NonTerminal("Timed-da-GD base", typeof(TransientAstNode));
            var atTimedDaGd = new NonTerminal("AT expression (timed-da-GD)", typeof(AtTimedDaGdAstNode));
            var overTimedDaGd = new NonTerminal("OVER expression (timed-da-GD)", typeof(OverTimedDaGdAstNode));

            var timeSpecifier = new NonTerminal("Time specifier", typeof(TransientAstNode));
            var intervalSpecifier = new NonTerminal("Interval specifier", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var gd = new Gd(p);

            // RULES

            daGd.Rule = p.ToTerm("(") + daGdBase + ")";
            daGdBase.Rule = andDaGdBase | forallDaGdBase | prefDaGdBase | timedDaGdBase;

            andDaGdBase.Rule = p.ToTerm("and") + daGdStarList;
            forallDaGdBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + daGd;

            prefDaGdBase.Rule = p.ToTerm("preference") + prefNameOrEmpty + timedDaGd;
            prefNameOrEmpty.Rule = prefName | p.Empty;

            timedDaGd.Rule = p.ToTerm("(") + timedDaGdBase + ")";
            timedDaGdBase.Rule = atTimedDaGd | overTimedDaGd;
            atTimedDaGd.Rule = p.ToTerm("at") + timeSpecifier + gd;
            overTimedDaGd.Rule = p.ToTerm("over") + intervalSpecifier + gd;

            timeSpecifier.Rule = p.ToTerm("start") | p.ToTerm("end");
            intervalSpecifier.Rule = p.ToTerm("all");

            daGdStarList.Rule = p.MakeStarRule(daGdStarList, daGd);

            p.MarkTransient(daGd, daGdBase, timedDaGd, timedDaGdBase);

            Rule = daGd;
        }
    }
}
