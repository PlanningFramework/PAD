using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a GD expression (general logical condition).
    /// </summary>
    public class GD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public GD(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var GD = new NonTerminal("GD", typeof(TransientAstNode));
            var GDBase = new NonTerminal("GD base", typeof(TransientAstNode));
            var GDStarList = new NonTerminal("GD star-list", typeof(TransientAstNode));

            var andGDBase = new NonTerminal("AND expression (GDs)", typeof(AndGDAstNode));
            var orGDBase = new NonTerminal("OR expression (GD)", typeof(OrGDAstNode));
            var notGDBase = new NonTerminal("NOT expression (GD)", typeof(NotGDAstNode));
            var implyGDBase = new NonTerminal("IMPLY expression (GD)", typeof(ImplyGDAstNode));
            var existsGDBase = new NonTerminal("EXISTS expression (GD)", typeof(ExistsGDAstNode));
            var forallGDBase = new NonTerminal("FORALL expression (GD)", typeof(ForallGDAstNode));
            var equalsOpGDBase = new NonTerminal("Equals operator (GD)", typeof(EqualsOpGDAstNode));
            var numCompGDBase = new NonTerminal("Numeric comparison expression (GD)", typeof(NumCompGDAstNode));
            var binaryComparer = new NonTerminal("Binary comparer", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var numericExpr = new NumericExpr(p);
            var valueOrTerm = new ValueOrTerm(p);
            var predicateGDBase = new PredicateGD(p, BForm.BASE);

            // RULES

            GD.Rule = p.ToTerm("(") + GDBase + ")";
            GDBase.Rule = andGDBase | orGDBase | notGDBase | implyGDBase | existsGDBase | forallGDBase | predicateGDBase | equalsOpGDBase | numCompGDBase;

            andGDBase.Rule    = p.ToTerm("and") + GDStarList;
            orGDBase.Rule     = p.ToTerm("or") + GDStarList;
            notGDBase.Rule    = p.ToTerm("not") + GD;
            implyGDBase.Rule  = p.ToTerm("imply") + GD + GD;
            existsGDBase.Rule = p.ToTerm("exists") + "(" + typedList + ")" + GD;
            forallGDBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + GD;
            equalsOpGDBase.Rule = p.ToTerm("=") + valueOrTerm + valueOrTerm;
            numCompGDBase.Rule = binaryComparer + numericExpr + numericExpr;
            binaryComparer.Rule = p.ToTerm(">") | "<" | ">=" | "<=";

            GDStarList.Rule = p.MakeStarRule(GDStarList, GD);

            p.MarkTransient(GD, GDBase, binaryComparer);

            return GD;
        }
    }
}
