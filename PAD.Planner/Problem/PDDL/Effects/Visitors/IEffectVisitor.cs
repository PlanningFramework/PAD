
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for visitors processing PDDL effects.
    /// </summary>
    public interface IEffectVisitor
    {
        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(ForallEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(WhenEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(PredicateEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(EqualsEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(NotEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(NumericAssignEffect effect);

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        void Visit(ObjectAssignEffect effect);
    }
}
