
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a 'not' effect.
    /// </summary>
    public class NotEffect : PrimitiveEffect
    {
        /// <summary>
        /// Argument effect.
        /// </summary>
        public AtomicFormulaEffect Argument { get; set; } = null;

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="argument">Argument effect.</param>
        public NotEffect(AtomicFormulaEffect effect)
        {
            Argument = effect;
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
            return HashHelper.GetHashCode("not", Argument);
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

            NotEffect other = obj as NotEffect;
            if (other == null)
            {
                return false;
            }

            return Argument.Equals(other.Argument);
        }
    }
}
