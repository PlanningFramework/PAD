using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL goal conditions.
    /// </summary>
    public class Goal : List<Expression>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":goal", true, true);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(goal => goal.Accept(visitor));
        }
    }
}
