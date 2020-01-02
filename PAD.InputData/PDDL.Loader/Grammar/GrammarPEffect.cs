using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a p-effect (primitive effect).
    /// </summary>
    public class PEffect : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public PEffect(MasterGrammar p) : this(p, BForm.FULL)
        {
        }

        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        public PEffect(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var pEffect = new NonTerminal("P-effect", typeof(TransientAstNode));
            var pEffectBase = new NonTerminal("P-effect base", typeof(TransientAstNode));

            var atomicFormulaPEffect = new NonTerminal("Atomic formula (p-effect)", typeof(TransientAstNode));
            var atomicFormulaPEffectBase = new NonTerminal("Atomic formula base (p-effect)", typeof(TransientAstNode));

            var predicatePEffectBase = new NonTerminal("Predicate (p-effect)", typeof(PredicatePEffectAstNode));
            var predicateArguments = new NonTerminal("Predicate arguments", typeof(TransientAstNode));

            var equalsOpPEffectBase = new NonTerminal("Equals-op (p-effect)", typeof(EqualsOpPEffectAstNode));
            var notPEffectBase = new NonTerminal("NOT expression (p-effect)", typeof(NotPEffectAstNode));

            var assignPEffectBase = new NonTerminal("General assign expression (p-effect)", typeof(TransientAstNode));
            var objOrNumAssign = new NonTerminal("Numeric or object assign operator (p-effect)", typeof(AssignPEffectAstNode));
            var onlyNumAssign = new NonTerminal("Numeric assign operator (p-effect)", typeof(AssignPEffectAstNode));

            var numAssignOp = new NonTerminal("Assign operation", typeof(TransientAstNode));
            var assignOpArg1 = new NonTerminal("Assign operation first argument", typeof(TransientAstNode));
            var assignOpArg2 = new NonTerminal("Assign operation second argument", typeof(TransientAstNode));
            var assignOpArg2Num = new NonTerminal("Assign operation second numeric argument", typeof(TransientAstNode));
            var assignOpFuncIdentif = new NonTerminal("Function identifier for ASSIGN operation", typeof(FunctionTermAstNode));
            var undefinedFuncVal = new NonTerminal("Undefined function value", typeof(UndefinedFuncValAstNode));

            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);
            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var valueOrTerm = new ValueOrTerm(p);
            var valueOrTermForAssign = getValueOrTerm();
            var numericExprForAssign = getNumericExpr();
            var predFuncTermFunction = new FunctionTerm(p);
            var term = new Term(p);

            // RULES

            pEffect.Rule = p.ToTerm("(") + pEffectBase + ")";
            pEffectBase.Rule = atomicFormulaPEffectBase | notPEffectBase | assignPEffectBase;

            atomicFormulaPEffect.Rule = p.ToTerm("(") + atomicFormulaPEffectBase + ")";
            atomicFormulaPEffectBase.Rule = predicatePEffectBase | equalsOpPEffectBase;

            predicatePEffectBase.Rule = predicateIdentifier + predicateArguments;
            predicateArguments.Rule = p.MakeStarRule(predicateArguments, term);

            equalsOpPEffectBase.Rule = p.ToTerm("=") + valueOrTerm + valueOrTerm;
            notPEffectBase.Rule = p.ToTerm("not") + atomicFormulaPEffect;

            assignPEffectBase.Rule = objOrNumAssign | onlyNumAssign;
            objOrNumAssign.Rule = p.ToTerm("assign") + assignOpArg1 + assignOpArg2;
            onlyNumAssign.Rule = numAssignOp + assignOpArg1 + assignOpArg2Num;

            assignOpArg1.Rule = assignOpFuncIdentif | predFuncTermFunction;
            assignOpArg2.Rule = undefinedFuncVal | valueOrTermForAssign;
            assignOpFuncIdentif.Rule = functionIdentifier;
            assignOpArg2Num.Rule = numericExprForAssign;

            numAssignOp.Rule = p.ToTerm("scale-up") | "scale-down" | "increase" | "decrease";
            undefinedFuncVal.Rule = p.ToTerm("undefined");

            p.MarkTransient(pEffect, pEffectBase, atomicFormulaPEffect, atomicFormulaPEffectBase, assignPEffectBase, assignOpArg1, assignOpArg2, assignOpArg2Num);

            return (bForm == BForm.BASE) ? pEffectBase : pEffect;
        }

        /// <summary>
        /// Factory method for specifying a numeric expression type being used within this grammar node.
        /// </summary>
        /// <returns>Numeric expression type.</returns>
        protected virtual NonTerminal getNumericExpr()
        {
            return new NumericExpr(p);
        }

        /// <summary>
        /// Factory method specifying a value/term entity type being used within this grammar node.
        /// </summary>
        /// <returns>Value/term type.</returns>
        protected virtual NonTerminal getValueOrTerm()
        {
            return new ValueOrTerm(p);
        }
    }
}
