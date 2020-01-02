using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for a SAS+ initial state.
    /// </summary>
    public class InitialState : List<int>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_state", Utilities.JoinLines(this), "end_state");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
