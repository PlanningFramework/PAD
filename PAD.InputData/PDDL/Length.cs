using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a PDDL length specification.
    /// </summary>
    public class Length : List<LengthSpecElement>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":length");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(lengthElem => lengthElem.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL length specification element.
    /// </summary>
    public class LengthSpecElement : IVisitable
    {
        /// <summary>
        /// Length specifier.
        /// </summary>
        public LengthSpecifier LengthSpecifier { get; set; } = LengthSpecifier.SERIAL;

        /// <summary>
        /// Length parameter.
        /// </summary>
        public int Parameter { get; set; } = 0;

        /// <summary>
        /// Constructs a length specifier.
        /// </summary>
        /// <param name="lengthSpecifier">Length specifier.</param>
        /// <param name="parameter">Length parameter.</param>
        public LengthSpecElement(LengthSpecifier lengthSpecifier, int parameter)
        {
            LengthSpecifier = lengthSpecifier;
            Parameter = parameter;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({LengthSpecifier.EnumToString()} {Parameter})";
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
