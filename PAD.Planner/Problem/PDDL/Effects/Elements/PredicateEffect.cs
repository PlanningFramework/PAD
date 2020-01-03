
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a predicate effect.
    /// </summary>
    public class PredicateEffect : AtomicFormulaEffect
    {
        /// <summary>
        /// Predicate atom.
        /// </summary>
        public IAtom PredicateAtom { set; get; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="predicateAtom">Predicate atom.</param>
        public PredicateEffect(IAtom predicateAtom)
        {
            PredicateAtom = predicateAtom;
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
            return HashHelper.GetHashCode("predicate", PredicateAtom);
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

            PredicateEffect other = obj as PredicateEffect;
            if (other == null)
            {
                return false;
            }

            return PredicateAtom.Equals(other.PredicateAtom);
        }
    }
}
