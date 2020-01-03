using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a term structure.
    /// </summary>
    public class ToTermConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Term TermData { get; private set; }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(IdentifierTermAstNode astNode)
        {
            if (MasterExporter.IsIdentifierTermObjectFunction(astNode))
            {
                TermData = new ObjectFunctionTerm(astNode.Name);
            }
            else if (MasterExporter.IsIdentifierTermVariable(astNode))
            {
                TermData = new VariableTerm(astNode.Name);
            }
            else
            {
                TermData = new ConstantTerm(astNode.Name);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(FunctionTermAstNode astNode)
        {
            ObjectFunctionTerm functionTerm = new ObjectFunctionTerm(astNode.Name);
            astNode.Terms.ForEach(argTerm => functionTerm.Terms.Add(MasterExporter.ToTerm(argTerm)));
            TermData = functionTerm;
        }
    }
}
