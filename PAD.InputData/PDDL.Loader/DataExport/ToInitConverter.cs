using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into an init element structure.
    /// </summary>
    public class ToInitElementConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public InitElement InitElementData { get; private set; }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PredicateInitElemAstNode astNode)
        {
            var predicate = new PredicateInitElement(astNode.Name);
            astNode.Terms.ForEach(term => predicate.Terms.Add(new ConstantTerm(term)));
            InitElementData = predicate;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(EqualsOpInitElemAstNode astNode)
        {
            var numericFunction = MasterExporter.ToBasicNumericFunctionTerm(astNode.Term1);
            var numberAstNode = astNode.Term2 as NumberTermAstNode;
            if (numericFunction != null && numberAstNode != null)
            {
                InitElementData = new EqualsNumericFunctionInitElement(numericFunction, numberAstNode.Number);
                return;
            }

            var objectFunction = MasterExporter.ToBasicObjectFunctionTerm(astNode.Term1);
            var term2IdentifierAstNode = astNode.Term2 as IdentifierTermAstNode;
            if (objectFunction != null && term2IdentifierAstNode != null)
            {
                InitElementData = new EqualsObjectFunctionInitElement(objectFunction, MasterExporter.ToConstantTerm(term2IdentifierAstNode));
                return;
            }

            var term1IdentifierAstNode = astNode.Term1 as IdentifierTermAstNode;
            if (term1IdentifierAstNode != null && term2IdentifierAstNode != null)
            {
                InitElementData = new EqualsInitElement(MasterExporter.ToConstantTerm(astNode.Term1), MasterExporter.ToConstantTerm(astNode.Term2));
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NotInitElemAstNode astNode)
        {
            InitElementData = new NotInitElement((AtomicFormulaInitElement)MasterExporter.ToInitElement(astNode.Argument));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtInitElemAstNode astNode)
        {
            InitElementData = new AtInitElement(astNode.Number, (LiteralInitElement)MasterExporter.ToInitElement(astNode.Argument));
        }
    }
}
