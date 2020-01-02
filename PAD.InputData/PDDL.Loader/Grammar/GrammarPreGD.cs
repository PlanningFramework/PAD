using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing pre-GD block (standard GD expression with preferences).
    /// </summary>
    public class PreGD : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PreGD(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var preGD = new NonTerminal("Pre-GD", typeof(TransientAstNode));
            var preGDBase = new NonTerminal("Pre-GD base", typeof(TransientAstNode));
            var preGDStarList = new NonTerminal("Pre-GD star-list", typeof(TransientAstNode));

            var prefPreGDBase = new NonTerminal("Preference expression (pre-GD)", typeof(PreferenceGDAstNode));
            var prefNameOrEmpty = new NonTerminal("Optional preference name", typeof(TransientAstNode));
            var prefName = new IdentifierTerminal("Preference name", IdentifierType.CONSTANT);

            var andPreGDBase = new NonTerminal("AND expression (pre-GDs)", typeof(AndGDAstNode));
            var forallPreGDBase = new NonTerminal("FORALL expression (pre-GD)", typeof(ForallGDAstNode));
            var orGDBase = new NonTerminal("OR expression (pre-GD)", typeof(OrGDAstNode));
            var notGDBase = new NonTerminal("NOT expression (pre-GD)", typeof(NotGDAstNode));
            var implyGDBase = new NonTerminal("IMPLY expression (pre-GD)", typeof(ImplyGDAstNode));
            var existsGDBase = new NonTerminal("EXISTS expression (pre-GD)", typeof(ExistsGDAstNode));
            var equalsOpGDBase = new NonTerminal("Equals operator (pre-GD)", typeof(EqualsOpGDAstNode));

            var numCompGDBase = new NonTerminal("Numeric comparison expression (pre-GD)", typeof(NumCompGDAstNode));
            var binaryComparer = new NonTerminal("Binary comparer", typeof(TransientAstNode));

            var GDStarList = new NonTerminal("GD star-list", typeof(TransientAstNode));

            // USED SUB-TREES

            var typedList = new TypedList(p);
            var GD = new GD(p);
            var predicateGDBase = new PredicateGD(p, BForm.BASE);
            var numericExpr = new NumericExpr(p);
            var valueOrTerm = new ValueOrTerm(p);

            // RULES

            preGD.Rule = p.ToTerm("(") + preGDBase + ")";
            preGDBase.Rule = prefPreGDBase | andPreGDBase | forallPreGDBase | orGDBase | notGDBase | implyGDBase | existsGDBase | predicateGDBase | equalsOpGDBase | numCompGDBase;

            prefPreGDBase.Rule = p.ToTerm("preference") + prefNameOrEmpty + GD;
            prefNameOrEmpty.Rule = prefName | p.Empty;

            andPreGDBase.Rule = p.ToTerm("and") + preGDStarList;
            forallPreGDBase.Rule = p.ToTerm("forall") + "(" + typedList + ")" + preGD;
            orGDBase.Rule = p.ToTerm("or") + GDStarList;
            notGDBase.Rule = p.ToTerm("not") + GD;
            implyGDBase.Rule = p.ToTerm("imply") + GD + GD;
            existsGDBase.Rule = p.ToTerm("exists") + "(" + typedList + ")" + GD;
            equalsOpGDBase.Rule = p.ToTerm("=") + valueOrTerm + valueOrTerm;

            numCompGDBase.Rule = binaryComparer + numericExpr + numericExpr;
            binaryComparer.Rule = p.ToTerm(">") | "<" | ">=" | "<=";

            preGDStarList.Rule = p.MakeStarRule(preGDStarList, preGD);
            GDStarList.Rule = p.MakeStarRule(GDStarList, GD);

            p.MarkTransient(preGD, preGDBase, GD, numericExpr, binaryComparer);

            return preGD;
        }
    }
}
