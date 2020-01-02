using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing a typed list of functions.
    /// </summary>
    public class FunctionTypedList : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public FunctionTypedList(MasterGrammar p) : base(p)
        {
        }

        /// <summary>
        /// Factory method for defining grammar rules of the grammar node.
        /// </summary>
        /// <returns>Grammar rules for this node.</returns>
        protected override NonTerminal Make()
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var functionsTypedList = new NonTerminal("Functions typed list", typeof(FunctionTypedListAstNode));

            var typedFuncBlock = new NonTerminal("Typed functions block", typeof(TransientAstNode));
            var funcBlock = new NonTerminal("Functions block", typeof(TransientAstNode));

            var singleFunction = new NonTerminal("Single function", typeof(TransientAstNode));
            var singleFunctionBase = new NonTerminal("Single function base", typeof(TransientAstNode));
            var functionIdentifier = new IdentifierTerminal("Function identifier", IdentifierType.CONSTANT);

            var singleTypedList = new NonTerminal("Single typed list", typeof(TransientAstNode));
            var typeDeclaration = new NonTerminal("Type declaration", typeof(TransientAstNode));

            var type = new NonTerminal("Type", typeof(TransientAstNode));
            var typePlusList = new NonTerminal("Type list", typeof(TransientAstNode));
            var typeIdentifier = new IdentifierTerminal("Type identifier", IdentifierType.CONSTANT);

            // USED SUB-TREES

            var typedList = new TypedList(p);

            // RULES

            functionsTypedList.Rule = p.MakeStarRule(functionsTypedList, typedFuncBlock);
            typedFuncBlock.Rule = funcBlock + typeDeclaration;

            funcBlock.Rule = p.MakePlusRule(funcBlock, singleFunction);
            singleFunction.Rule = p.ToTerm("(") + singleFunctionBase + ")";
            singleFunctionBase.Rule = functionIdentifier + typedList;

            typeDeclaration.Rule = p.Empty | ("-" + type);
            type.Rule = typeIdentifier | ("(" + p.ToTerm("either") + typePlusList + ")");
            typePlusList.Rule = p.MakePlusRule(typePlusList, typeIdentifier);

            p.MarkTransient(singleFunction);

            return functionsTypedList;
        }
    }
}
