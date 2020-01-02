using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for operator effects.
    /// </summary>
    public class Effects : List<Effect>, IVisitable
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
            ForEach(effect => effect.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single operator effect.
    /// </summary>
    public class Effect : IVisitable
    {
        /// <summary>
        /// Effect conditions, in case of conditional effect.
        /// </summary>
        public Conditions Conditions { set; get; } = new Conditions();

        /// <summary>
        /// Primitive effect (variable-value assignment).
        /// </summary>
        public Assignment PrimitiveEffect { set; get; } = new Assignment();

        /// <summary>
        /// Gets the affected variable by this effect.
        /// </summary>
        /// <returns>Affected variable index.</returns>
        public int GetAffectedVariable() { return PrimitiveEffect.Variable; }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{Conditions.Count} {string.Join(" ", Conditions)} {PrimitiveEffect.Variable} {-1} {PrimitiveEffect.Value}";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            Conditions.Accept(visitor);
            PrimitiveEffect.Accept(visitor);
            visitor.Visit(this);
        }
    }
}
