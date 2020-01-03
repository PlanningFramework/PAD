using System.Collections.Generic;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL durative action conditions.
    /// </summary>
    public class DurativeConditions : List<DurativeExpression>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return (Count == 0) ? "()" : this.ToBlockString(null, true, true);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(condition => condition.Accept(visitor));
        }
    }
}
