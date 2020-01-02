using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for operator conditions.
    /// </summary>
    public class Conditions : List<Assignment>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines(Count, Utilities.JoinLines(this));
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(assignment => assignment.Accept(visitor));
            visitor.Visit(this);
        }
    }
}
