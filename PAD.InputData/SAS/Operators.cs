using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for SAS+ operators.
    /// </summary>
    public class Operators : List<Operator>, IVisitable
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
            ForEach(operatorItem => operatorItem.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single operator.
    /// </summary>
    public class Operator : IVisitable
    {
        /// <summary>
        /// Name of the operator.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Conditions of the operator.
        /// </summary>
        public Conditions Conditions { set; get; } = new Conditions();

        /// <summary>
        /// Effects of the operator.
        /// </summary>
        public Effects Effects { set; get; } = new Effects();

        /// <summary>
        /// Cost of the operator.
        /// </summary>
        public int Cost { set; get; } = 0;

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_operator", Name, Conditions, Effects, Cost, "end_operator");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Conditions.Accept(visitor);
            Effects.Accept(visitor);
        }
    }
}
