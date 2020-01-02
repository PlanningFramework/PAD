
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a SAS+ state proposition (i.e. an encapsulated assignment).
    /// </summary>
    public class Proposition : IProposition
    {
        /// <summary>
        /// Assignment.
        /// </summary>
        public IAssignment Assignment { set; get; } = null;

        /// <summary>
        /// Creates the proposition.
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        public Proposition(IAssignment assignment)
        {
            Assignment = assignment;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Assignment.GetHashCode();
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

            return Assignment.Equals(other.Assignment);
        }
    }
}
