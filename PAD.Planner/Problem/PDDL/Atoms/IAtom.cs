using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Interface of an atom structure. Atom is a common identifier for predicates and functions and can be potentially lifted,
    /// i.e. not all of the arguments are grounded - argument terms can be either a constant, a variable, or an object function.
    /// </summary>
    public interface IAtom
    {
        /// <summary>
        /// Gets the name of the atom.
        /// </summary>
        /// <returns>Name ID of the atom.</returns>
        int GetNameID();

        /// <summary>
        /// Gets the atom arity (number of arguments).
        /// </summary>
        /// <returns>Atom arity.</returns>
        int GetArity();

        /// <summary>
        /// Gets the grounded term of the atom (i.e. constant/object ID), if grounded.
        /// </summary>
        /// <param name="index">Term index.</param>
        /// <returns>Grounded term (i.e. constant/object ID), or -1 if not grounded.</returns>
        int GetGroundedTerm(int index);

        /// <summary>
        /// Get full argument terms (potentially lifted).
        /// </summary>
        /// <returns>Argument terms.</returns>
        List<ITerm> GetTerms();

        /// <summary>
        /// Gets the unification with the specified atom, i.e. calculates the variable substitution required to ground variable terms of the
        /// current atom with the grounded terms (i.e. constants) of the other atom.
        /// </summary>
        /// <param name="referenceAtom">Reference atom.</param>
        /// <returns>Unification in the form of variable substitution.</returns>
        ISubstitution GetUnificationWith(IAtom referenceAtom);

        /// <summary>
        /// Creates a deep copy of the atom.
        /// </summary>
        /// <returns>A copy of the atom.</returns>
        IAtom Clone();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        /// <returns>String representation.</returns>
        string ToString(EntityIDManager idManager);
    }
}
