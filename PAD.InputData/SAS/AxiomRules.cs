using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for SAS+ axiom rules.
    /// </summary>
    public class AxiomRules : List<AxiomRule>, IVisitable
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
            ForEach(axiomRule => axiomRule.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single SAS+ variable.
    /// </summary>
    public class AxiomRule : IVisitable
    {
        /// <summary>
        /// Axiom conditions.
        /// </summary>
        public Conditions Conditions { set; get; } = new Conditions();

        /// <summary>
        /// Axiom primitive effect (variable-value assignment).
        /// </summary>
        public Assignment PrimitiveEffect { set; get; } = new Assignment();

        /// <summary>
        /// Gets the affected variable by this axiom rule.
        /// </summary>
        /// <returns>Affected variable index.</returns>
        public int GetAffectedVariable() { return PrimitiveEffect.Variable; }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines(
                "begin_rule",
                Conditions,
                $"{PrimitiveEffect.Variable} {-1} {PrimitiveEffect.Value}",
                "end_rule");
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
