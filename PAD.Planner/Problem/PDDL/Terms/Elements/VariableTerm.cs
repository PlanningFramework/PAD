
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a variable term.
    /// </summary>
    public class VariableTerm : ITerm
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public int NameId { get; set; }

        /// <summary>
        /// Constructs the term.
        /// </summary>
        /// <param name="nameId">Variable name.</param>
        public VariableTerm(int nameId)
        {
            NameId = nameId;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{IdManager.GenericVariablePrefix}{NameId.ToString()}";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("var", NameId);
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

            VariableTerm other = obj as VariableTerm;
            if (other == null)
            {
                return false;
            }

            return (NameId == other.NameId);
        }

        /// <summary>
        /// Creates a deep copy of the term.
        /// </summary>
        /// <returns>A copy of the term.</returns>
        public ITerm Clone()
        {
            return new VariableTerm(NameId);
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
