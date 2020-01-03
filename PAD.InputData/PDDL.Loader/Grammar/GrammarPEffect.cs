using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

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
        /// <param name="bForm">Block form.</param>
        public PEffect(MasterGrammar p, BForm bForm) : base(p, bForm)
        {
            Rule = ConstructPEffectRule(p, bForm, new ValueOrTerm(p), new NumericExpr(p));
        }

        /// <summary>
        /// Constructs the primitive effect rule from the specified parameters.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="bForm">Block form.</param>
        /// <param name="valueOrTermForAssign">Value or term for an assignment.</param>
        /// <param name="numericExprForAssign">Numeric expression for an assignment.</param>
        /// <returns>Primitive effect grammar rule.</returns>
        public static NonTerminal ConstructPEffectRule(MasterGrammar p, BForm bForm, NonTerminal valueOrTermForAssign, NonTerminal numericExprForAssign)
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
    }
}
