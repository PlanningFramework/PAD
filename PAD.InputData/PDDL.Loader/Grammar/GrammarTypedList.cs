using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Grammar node representing general typed list, with variables as the items being typed.
    /// </summary>
    public class TypedList : BaseGrammarNode
    {
        /// <summary>
        /// Constructor of the grammar node.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        public TypedList(MasterGrammar p) : base(p)
        {
            Rule = ConstructTypedListRule(p, IdentifierType.VARIABLE);
        }

        /// <summary>
        /// Constructs the typed list rule from the specified parameters.
        /// </summary>
        /// <param name="p">Parent master grammar.</param>
        /// <param name="itemIdentifierType">Item identifier type.</param>
        /// <returns>Typed list grammar rule.</returns>
        public static NonTerminal ConstructTypedListRule(MasterGrammar p, IdentifierType itemIdentifierType)
        {
            // NON-TERMINAL AND TERMINAL SYMBOLS

            var typedList = new NonTerminal("Typed list", typeof(TypedListAstNode));
            var singleTypedList = new NonTerminal("Single typed list", typeof(TransientAstNode));
            var typeDeclaration = new NonTerminal("Type declaration", typeof(TransientAstNode));

            var type = new NonTerminal("Type", typeof(TransientAstNode));
            var typePlusList = new NonTerminal("Type list", typeof(TransientAstNode));
            var identifiersList = new NonTerminal("Typed list identifiers", typeof(TransientAstNode));

            var itemIdentifier = new IdentifierTerminal("Item identifier", itemIdentifierType);
            var typeIdentifier = new IdentifierTerminal("Type identifier", IdentifierType.CONSTANT);

            // RULES

            typedList.Rule = p.MakeStarRule(typedList, singleTypedList);
            singleTypedList.Rule = identifiersList + typeDeclaration;
            identifiersList.Rule = p.MakeStarRule(identifiersList, itemIdentifier);
            typeDeclaration.Rule = p.Empty | ("-" + type);

            type.Rule = typeIdentifier | ("(" + p.ToTerm("either") + typePlusList + ")");
            typePlusList.Rule = p.MakePlusRule(typePlusList, typeIdentifier);

            return typedList;
        }
    }
}
