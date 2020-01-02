
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing primitive effect (i.e. without conditional effects).
    /// </summary>
    public abstract class PrimitiveEffect : IEffect
    {
        /// <summary>
        /// Accepts a visitor evaluating the effect.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IEffectVisitor visitor);
    }
}
