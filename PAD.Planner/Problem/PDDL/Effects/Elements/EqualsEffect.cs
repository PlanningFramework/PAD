
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing an equals effect, i.e. (= termA termB).
    /// </summary>
    public class EqualsEffect : AtomicFormulaEffect
    {
        /// <summary>
        /// First argument term.
        /// </summary>
        public ITerm LeftArgument { get; set; }

        /// <summary>
        /// Second argument term.
        /// </summary>
        public ITerm RightArgument { get; set; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="leftArgument">First argument term.</param>
        /// <param name="rightArgument">Second argument term.</param>
        public EqualsEffect(ITerm leftArgument, ITerm rightArgument)
        {
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }

        /// <summary>
        /// Accepts a visitor evaluating the effect.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IEffectVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("equals", LeftArgument, RightArgument);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            EqualsEffect other = obj as EqualsEffect;
            if (other == null)
            {
                return false;
            }

            return LeftArgument.Equals(other.LeftArgument)
                && RightArgument.Equals(other.RightArgument);
        }
    }
}
