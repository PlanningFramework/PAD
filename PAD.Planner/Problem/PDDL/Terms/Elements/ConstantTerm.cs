
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a constant term.
    /// </summary>
    public class ConstantTerm : ITerm
    {
        /// <summary>
        /// Constant name.
        /// </summary>
        public int NameID { get; set; } = IDManager.INVALID_ID;

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the term.
        /// </summary>
        /// <param name="nameID">Constant name.</param>
        /// <param name="idManager">ID manager.</param>
        public ConstantTerm(int nameID, IDManager idManager = null)
        {
            NameID = nameID;
            IDManager = idManager;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return (IDManager != null) ? IDManager.Constants.GetNameFromID(NameID) : NameID.ToString();
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(NameID, "const");
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

            ConstantTerm other = obj as ConstantTerm;
            if (other == null)
            {
                return false;
            }

            return (NameID == other.NameID);
        }

        /// <summary>
        /// Creates a deep copy of the term.
        /// </summary>
        /// <returns>A copy of the term.</returns>
        public ITerm Clone()
        {
            return new ConstantTerm(NameID, IDManager);
        }

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">Term visitor.</param>
        public ITerm Accept(ITermTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
