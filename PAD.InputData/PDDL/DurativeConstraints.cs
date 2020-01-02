using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL durative-action duration constraints.
    /// </summary>
    public class DurativeConstraints : List<DurativeConstraint>, IVisitable
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
            ForEach(constraint => constraint.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL durative-action duration constraint.
    /// </summary>
    public abstract class DurativeConstraint : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a comparison duration constraint.
    /// </summary>
    public class CompareDurativeConstraint : DurativeConstraint
    {
        /// <summary>
        /// Duration comparer specifier.
        /// </summary>
        public DurationComparer DurationComparer { get; set; } = DurationComparer.LTE;

        /// <summary>
        /// Numeric expression value to which the duration is compared.
        /// </summary>
        public NumericExpression Value { get; set; } = null;

        /// <summary>
        /// Constructs the durative constraint.
        /// </summary>
        /// <param name="durationComparer">Duration comparer specifier.</param>
        /// <param name="value">Numeric expression to compare.</param>
        public CompareDurativeConstraint(DurationComparer durationComparer, NumericExpression value)
        {
            DurationComparer = durationComparer;
            Value = value;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({DurationComparer.EnumToString()} ?duration {Value})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Value.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for an "at" duration constraint.
    /// </summary>
    public class AtDurativeConstraint : DurativeConstraint
    {
        /// <summary>
        /// Time specifier.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; set; } = TimeSpecifier.START;

        /// <summary>
        /// Argument durative constraint to which the expression is applied.
        /// </summary>
        public DurativeConstraint DurativeConstraint { get; set; } = null;

        /// <summary>
        /// Constructs the durative constraint.
        /// </summary>
        /// <param name="timeSpecifier">Time specifier.</param>
        /// <param name="durativeConstraint">Argument durative constraint.</param>
        public AtDurativeConstraint(TimeSpecifier timeSpecifier, DurativeConstraint durativeConstraint)
        {
            TimeSpecifier = timeSpecifier;
            DurativeConstraint = durativeConstraint;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at {TimeSpecifier.EnumToString()} {DurativeConstraint})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            DurativeConstraint.Accept(visitor);
        }
    }
}
