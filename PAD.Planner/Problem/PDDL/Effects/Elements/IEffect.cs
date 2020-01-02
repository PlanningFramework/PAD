
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface representing a PDDL effect.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Accepts a visitor evaluating the effect.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Accept(IEffectVisitor visitor);
    }
}
