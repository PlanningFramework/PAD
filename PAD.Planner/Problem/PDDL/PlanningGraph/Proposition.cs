
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a PDDL state proposition (i.e. an encapsulated predicate atom).
    /// </summary>
    public class Proposition : IProposition
    {
        /// <summary>
        /// Predicate atom.
        /// </summary>
        public IAtom Atom { set; get; }

        /// <summary>
        /// Creates the proposition.
        /// </summary>
        /// <param name="atom">Predicate atom.</param>
        public Proposition(IAtom atom)
        {
            Atom = atom;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Atom.GetHashCode();
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

            Proposition other = obj as Proposition;
            if (other == null)
            {
                return false;
            }

            return Atom.Equals(other.Atom);
        }
    }
}
