using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Standard implementation of an atom structure (identification of predicates/functions).
    /// </summary>
    public class Atom : IAtom
    {
        /// <summary>
        /// Atom name ID.
        /// </summary>
        public int NameId { set; get; }

        /// <summary>
        /// Argument terms.
        /// </summary>
        public List<ITerm> Terms { set; get; }

        /// <summary>
        /// Value for the term that is not grounded (see Atom.GetGroundedTerm(int)).
        /// </summary>
        public const int NotGroundedTerm = -1;

        /// <summary>
        /// Constructs the atom.
        /// </summary>
        /// <param name="nameId">Name ID.</param>
        public Atom(int nameId)
        {
            NameId = nameId;
            Terms = new List<ITerm>();
        }

        /// <summary>
        /// Constructs the atom.
        /// </summary>
        /// <param name="nameId">Name ID.</param>
        /// <param name="terms">Argument terms</param>
        public Atom(int nameId, List<ITerm> terms)
        {
            NameId = nameId;
            Terms = terms;
        }

        /// <summary>
        /// Gets the name of the atom.
        /// </summary>
        /// <returns>Name ID of the atom.</returns>
        public int GetNameId()
        {
            return NameId;
        }

        /// <summary>
        /// Gets the atom arity (number of arguments).
        /// </summary>
        /// <returns>Atom arity.</returns>
        public int GetArity()
        {
            return Terms.Count;
        }

        /// <summary>
        /// Gets the grounded term of the atom (i.e. constant/object ID), if grounded.
        /// </summary>
        /// <param name="index">Term index.</param>
        /// <returns>Grounded term (i.e. constant/object ID), or -1 if not grounded.</returns>
        public int GetGroundedTerm(int index)
        {
            ConstantTerm constantTerm = Terms[index] as ConstantTerm;
            return constantTerm?.NameId ?? NotGroundedTerm;
        }

        /// <summary>
        /// Get argument terms.
        /// </summary>
        /// <returns>Argument terms.</returns>
        public List<ITerm> GetTerms()
        {
            return Terms;
        }

        /// <summary>
        /// Gets the unification with the specified atom, i.e. calculates the variable substitution required to ground variable terms of the
        /// current atom with the grounded terms (i.e. constants) of the other atom.
        /// </summary>
        /// <param name="referenceAtom">Reference atom.</param>
        /// <returns>Unification in the form of variable substitution.</returns>
        public ISubstitution GetUnificationWith(IAtom referenceAtom)
        {
            Debug.Assert(GetNameId() == referenceAtom.GetNameId() && GetTerms().Count == referenceAtom.GetTerms().Count, "Unification of incompatible atoms!");

            ISubstitution unification = new Substitution();

            int termIndex = 0;
            foreach (var term in GetTerms())
            {
                VariableTerm variableTerm = term as VariableTerm;
                if (variableTerm != null)
                {
                    ConstantTerm valueTerm = referenceAtom.GetTerms()[termIndex] as ConstantTerm;
                    if (valueTerm != null)
                    {
                        unification.Add(variableTerm.NameId, valueTerm.NameId);
                    }
                }
                ++termIndex;
            }

            return unification;
        }

        /// <summary>
        /// Creates a deep copy of the atom.
        /// </summary>
        /// <returns>A copy of the atom.</returns>
        public IAtom Clone()
        {
            return new Atom(NameId, new List<ITerm>(Terms));
        }

        /// <summary>
        /// Gets the full name of the atom from its ID.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        /// <returns>Full atom name.</returns>
        public string GetFullName(EntityIdManager idManager)
        {
            return idManager.GetNameFromId(GetNameId());
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        /// <returns>String representation.</returns>
        public string ToString(EntityIdManager idManager)
        {
            string atomName = GetFullName(idManager);

            IList<ITerm> terms = GetTerms();
            if (terms.Count == 0)
            {
                return $"({atomName})";
            }

            List<string> termsNames = new List<string>();
            foreach (var term in terms)
            {
                termsNames.Add(term.ToString());
            }

            return $"({atomName} {string.Join(" ", termsNames)})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Terms).CombineHashCode(NameId);
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

            Atom other = obj as Atom;
            if (other == null)
            {
                return false;
            }

            return (NameId == other.NameId) && CollectionsEquality.Equals(Terms, other.Terms);
        }
    }
}
