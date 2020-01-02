using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for SAS+ goal conditions.
    /// </summary>
    public class GoalConditions : List<Assignment>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_goal", Count, Utilities.JoinLines(this), "end_goal");
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
