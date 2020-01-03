using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a durative effects structure.
    /// </summary>
    public class ToDurativeEffectsConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public DurativeEffects EffectsData { get; } = new DurativeEffects();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AndDaEffectsAstNode astNode)
        {
            astNode.Arguments.ForEach(arg => EffectsData.AddRange(MasterExporter.ToDurativeEffects(arg)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ForallDaEffectAstNode astNode)
        {
            EffectsData.Add(new ForallDurativeEffect(MasterExporter.ToParameters(astNode.Parameters), MasterExporter.ToDurativeEffects(astNode.Effect)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(WhenDaEffectAstNode astNode)
        {
            EffectsData.Add(new WhenDurativeEffect(MasterExporter.ToDurativeExpression(astNode.Condition), (TimedEffect)MasterExporter.ToDurativeEffects(astNode.Effect)[0]));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AtTimedEffectAstNode astNode)
        {
            EffectsData.Add(new AtTimedEffect(astNode.TimeSpecifier, ToPrimitiveEffects(MasterExporter.ToEffects(astNode.Effect))));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(AssignTimedEffectAstNode astNode)
        {
            EffectsData.Add(new AssignTimedEffect(astNode.AssignOperator, (NumericFunction)MasterExporter.ToNumericExpression(astNode.Function), MasterExporter.ToTimedNumericExpression(astNode.Expression)));
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
