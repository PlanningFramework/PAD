using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a GD expression (general logical condition).
    /// </summary>
    public class Gd : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public Gd(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var gd = new NonTerminal("GD", typeof(TransientAstNode));
            var gdBase = new NonTerminal("GD base", typeof(TransientAstNode));
            var gdStarList = new NonTerminal("GD star-list", typeof(TransientAstNode));

            var andGdBase = new NonTerminal("AND expression (GDs)", typeof(AndGdAstNode));
            var orGdBase = new NonTerminal("OR expression (GD)", typeof(OrGdAstNode));
            var notGdBase = new NonTerminal("NOT expression (GD)", typeof(NotGdAstNode));
            var implyGdBase = new NonTerminal("IMPLY expression (GD)", typeof(ImplyGdAstNode));
            var existsGdBase = new NonTerminal("EXISTS expression (GD)", typeof(ExistsGdAstNode));
            var forallGdBase = new NonTerminal("FORALL expression (GD)", typeof(ForallGdAstNode));
            var equalsOpGdBase = new NonTerminal("Equals operator (GD)", typeof(EqualsOpGdAstNode));
            var numCompGdBase = new NonTerminal("Numeric comparison expression (GD)", typeof(NumCompGdAstNode));
            var binaryComparer = new NonTerminal("Binary comparer", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var numericExpr = new NumericExpr(p);
            var valueOrTerm = new ValueOrTerm(p);
            var predicateGdBase = new PredicateGd(p, BForm.BASE);

            // RULES

            gd.Rule = p.ToTerm("(") + gdBase + ")";
            gdBase.Rule = andGdBase | orGdBase | notGdBase | implyGdBase | existsGdBase | forallGdBase | predicateGdBase | equalsOpGdBase | numCompGdBase;

            andGdBase.Rule    = p.ToTerm("and") + gdStarList;
            orGdBase.Rule     = p.ToTerm("or") + gdStarList;
            notGdBase.Rule    = p.ToTerm("not") + gd;
            implyGdBase.Rule  = p.ToTerm("imply") + gd + gd;
            existsGdBase.Rule = p.ToTerm("exists") + "(" + typedList + ")" + gd;
            forallGdBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + gd;
            equalsOpGdBase.Rule = p.ToTerm("=") + valueOrTerm + valueOrTerm;
            numCompGdBase.Rule = binaryComparer + numericExpr + numericExpr;
            binaryComparer.Rule = p.ToTerm(">") | "<" | ">=" | "<=";

            gdStarList.Rule = p.MakeStarRule(gdStarList, gd);

            p.MarkTransient(gd, gdBase, binaryComparer);

            Rule = gd;
        }
    }
}
