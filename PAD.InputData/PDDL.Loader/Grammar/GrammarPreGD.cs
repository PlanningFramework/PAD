using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing pre-GD block (standard GD expression with preferences).
    /// </summary>
    public class PreGd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PreGd(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var preGd = new NonTerminal("Pre-GD", typeof(TransientAstNode));
            var preGdBase = new NonTerminal("Pre-GD base", typeof(TransientAstNode));
            var preGdStarList = new NonTerminal("Pre-GD star-list", typeof(TransientAstNode));

            var prefPreGdBase = new NonTerminal("Preference expression (pre-GD)", typeof(PreferenceGdAstNode));
            var prefNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var prefName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var andPreGdBase = new NonTerminal("AND expression (pre-GDs)", typeof(AndGdAstNode));
            var forallPreGdBase = new NonTerminal("FORALL expression (pre-GD)", typeof(ForallGdAstNode));
            var orGdBase = new NonTerminal("OR expression (pre-GD)", typeof(OrGdAstNode));
            var notGdBase = new NonTerminal("NOT expression (pre-GD)", typeof(NotGdAstNode));
            var implyGdBase = new NonTerminal("IMPLY expression (pre-GD)", typeof(ImplyGdAstNode));
            var existsGdBase = new NonTerminal("EXISTS expression (pre-GD)", typeof(ExistsGdAstNode));
            var equalsOpGdBase = new NonTerminal("Equals operator (pre-GD)", typeof(EqualsOpGdAstNode));

            var numCompGdBase = new NonTerminal("Numeric comparison expression (pre-GD)", typeof(NumCompGdAstNode));
            var binaryComparer = new NonTerminal("Binary comparer", typeof(TransientAstNode));

            var gdStarList = new NonTerminal("GD star-list", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var gd = new Gd(p);
            var predicateGdBase = new PredicateGd(p, BForm.BASE);
            var numericExpr = new NumericExpr(p);
            var valueOrTerm = new ValueOrTerm(p);

            // RULES

            preGd.Rule = p.ToTerm("(") + preGdBase + ")";
            preGdBase.Rule = prefPreGdBase | andPreGdBase | forallPreGdBase | orGdBase | notGdBase | implyGdBase | existsGdBase | predicateGdBase | equalsOpGdBase | numCompGdBase;

            prefPreGdBase.Rule = p.ToTerm("preference") + prefNameOrEmpty + gd;
            prefNameOrEmpty.Rule = prefName | p.Empty;

            andPreGdBase.Rule = p.ToTerm("and") + preGdStarList;
            forallPreGdBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + preGd;
            orGdBase.Rule = p.ToTerm("or") + gdStarList;
            notGdBase.Rule = p.ToTerm("not") + gd;
            implyGdBase.Rule = p.ToTerm("imply") + gd + gd;
            existsGdBase.Rule = p.ToTerm("exists") + "(" + typedList + ")" + gd;
            equalsOpGdBase.Rule = p.ToTerm("=") + valueOrTerm + valueOrTerm;

            numCompGdBase.Rule = binaryComparer + numericExpr + numericExpr;
            binaryComparer.Rule = p.ToTerm(">") | "<" | ">=" | "<=";

            preGdStarList.Rule = p.MakeStarRule(preGdStarList, preGd);
            gdStarList.Rule = p.MakeStarRule(gdStarList, gd);

            p.MarkTransient(preGd, preGdBase, gd, numericExpr, binaryComparer);

            Rule = preGd;
        }
    }
}
