using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing an initial element (for defining initial state of the problem). 
    /// </summary>
    public class InitElem : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public InitElem(MasterGrammar p) : base(p)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var initElem = new NonTerminal("Init element", typeof(TransientAstNode));
            var initElemBase = new NonTerminal("Init element", typeof(TransientAstNode));

            var predicateInitElemBase = new NonTerminal("Predicate (init elem)", typeof(PredicateInitElemAstNode));
            var predicateArguments = new NonTerminal("Predicate arguments", typeof(TransientAstNode));

            var equalsInitElemBase = new NonTerminal("EQUALS expression (init elem)", typeof(EqualsOpInitElemAstNode));
            var equalsFirstArg = new NonTerminal("EQUALS expression first argument", typeof(TransientAstNode));
            var equalsSecondArg = new NonTerminal("EQUALS expression second argument", typeof(TransientAstNode));

            var functionEqualsArg = new NonTerminal("EQUALS expression function argument", typeof(TransientAstNode));
            var functionBase = new NonTerminal("Function term", typeof(FunctionTermAstNode));
            var functionArguments = new NonTerminal("Function arguments", typeof(TransientAstNode));

            var constOrFuncTerm = new NonTerminal("Const/func identifier term", typeof(IdentifierTermAstNode));
            var constTerm = new NonTerminal("Const identifier term", typeof(IdentifierTermAstNode));
            var numberTerm = new NonTerminal("Number term", typeof(NumberTermAstNode));

            var notInitElemBase = new NonTerminal("NOT expression (init elem)", typeof(NotInitElemAstNode));
            var notArgument = new NonTerminal("NOT expression argument", typeof(TransientAstNode));
            var notArgumentBase = new NonTerminal("NOT expression argument", typeof(TransientAstNode));
            var equalsSimpleInitElemBase = new NonTerminal("EQUALS expression (init elem)", typeof(EqualsOpInitElemAstNode));
            
            var atPredicateInitElemBase = new NonTerminal("Predicate (init elem)", typeof(PredicateInitElemAstNode));
            var atExpressionInitElemBase = new NonTerminal("AT expression (init elem)", typeof(AtInitElemAstNode));
            var atArgument = new NonTerminal("AT expression argument", typeof(TransientAstNode));
            var atArgumentBase = new NonTerminal("AT expression argument", typeof(TransientAstNode));

            var predicateIdentifier = new IdentifierTerminal("Predicate identifier", IdentifierType.CONSTANT);
            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);
            var constOrFuncIdentifier = new IdentifierTerminal("Constant or function identifier", IdentifierType.CONSTANT);
            var constIdentifier = new IdentifierTerminal("Constant identifier", IdentifierType.CONSTANT);
            var number = new NumberLiteral("Number");

            // RULES

            initElem.Rule = p.ToTerm("(") + initElemBase + ")";
            initElemBase.Rule = predicateInitElemBase | equalsInitElemBase | notInitElemBase | atPredicateInitElemBase | atExpressionInitElemBase;

            predicateInitElemBase.Rule = predicateIdentifier + predicateArguments;
            predicateArguments.Rule = p.MakeStarRule(predicateArguments, constIdentifier);

            equalsInitElemBase.Rule = p.ToTerm("=") + equalsFirstArg + equalsSecondArg;
            equalsFirstArg.Rule = constOrFuncTerm | functionEqualsArg;
            equalsSecondArg.Rule = constTerm | numberTerm;

            functionEqualsArg.Rule = p.ToTerm("(") + functionBase + ")";
            functionBase.Rule = functionIdentifier + functionArguments;
            functionArguments.Rule = p.MakeStarRule(functionArguments, constTerm);

            constOrFuncTerm.Rule = constOrFuncIdentifier;
            constTerm.Rule = constIdentifier;
            numberTerm.Rule = number;

            notInitElemBase.Rule = p.ToTerm("not") + notArgument;
            notArgument.Rule = p.ToTerm("(") + notArgumentBase + ")";
            notArgumentBase.Rule = predicateInitElemBase | equalsSimpleInitElemBase;
            equalsSimpleInitElemBase.Rule = p.ToTerm("=") + constTerm + constTerm;

            atPredicateInitElemBase.Rule = p.ToTerm("at") + predicateArguments;
            atExpressionInitElemBase.Rule = p.ToTerm("at") + number + atArgument;
            atArgument.Rule = p.ToTerm("(") + atArgumentBase + ")";
            atArgumentBase.Rule = predicateInitElemBase | equalsSimpleInitElemBase | notInitElemBase;

            p.MarkTransient(initElem, initElemBase, equalsFirstArg, equalsSecondArg, functionEqualsArg, notArgument, notArgumentBase, atArgument, atArgumentBase);

            Rule = initElem;
        }
    }
}