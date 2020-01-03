using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into an effects structure.
    /// </summary>
    public class ToEffectsConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Effects EffectsData { get; } = new Effects();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndCEffectsAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => EffectsData.AddRange(MasterExporter.ToEffects(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallCEffectAstNode astNode)
        {
            EffectsData.Add(new ForallEffect(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToEffects(astNode.Effect)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(WhenCEffectAstNode astNode)
        {
            EffectsData.Add(new WhenEffect(MasterExporter.ToExpression(astNode.Condition), ToPrimitiveEffects(MasterExporter.ToEffects(astNode.Effect))));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndPEffectsAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => EffectsData.AddRange(MasterExporter.ToEffects(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(PredicatePEffectAstNode astNode)
        {
            PredicateEffect predicate = new PredicateEffect(astNode.Name);
            astNode.Terms.ForEach(term => predicate.Terms.Add(MasterExporter.ToTerm(term)));
            EffectsData.Add(predicate);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(EqualsOpPEffectAstNode astNode)
        {
            EffectsData.Add(new EqualsEffect(MasterExporter.ToTerm(astNode.Term1), MasterExporter.ToTerm(astNode.Term2)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(NotPEffectAstNode astNode)
        {
            EffectsData.Add(new NotEffect((AtomicFormulaEffect)MasterExporter.ToEffects(astNode.Argument)[0]));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AssignPEffectAstNode astNode)
        {
            if (astNode.AssignOperator == Traits.AssignOperator.ASSIGN && MasterExporter.IsTerm(astNode.Argument2))
            {
                EffectsData.Add(new ObjectAssignEffect((ObjectFunctionTerm)MasterExporter.ToTerm(astNode.Argument1), MasterExporter.ToTerm(astNode.Argument2)));
            }
            else
            {
                EffectsData.Add(new NumericAssignEffect(astNode.AssignOperator, (NumericFunction)MasterExporter.ToNumericExpression(astNode.Argument1), MasterExporter.ToNumericExpression(astNode.Argument2)));
            }
        }

        /// <summary>
        /// Converts a list of effects into a list of primitive effects.
        /// </summary>
        /// <param name="effects">List of effects.</param>
        /// <returns>List of primitive effects.</returns>
        private static PrimitiveEffects ToPrimitiveEffects(Effects effects)
        {
            PrimitiveEffects primitiveEffects = new PrimitiveEffects();
            effects.ForEach(x => primitiveEffects.Add((PrimitiveEffect)x));
            return primitiveEffects;
        }
    }
}
