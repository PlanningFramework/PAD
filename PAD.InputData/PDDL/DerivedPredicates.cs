using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL derived predicates.
    /// </summary>
    public class DerivedPredicates : List<DerivedPredicate>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString();
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(derivedPredicate => derivedPredicate.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL derived predicate.
    /// </summary>
    public class DerivedPredicate : IVisitable
    {
        /// <summary>
        /// Derived predicate signature.
        /// </summary>
        public Predicate Predicate { get; set; } = null;

        /// <summary>
        /// Derived predicate expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(:derived {Predicate} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Predicate.Accept(visitor);
            Expression.Accept(visitor);
            visitor.PostVisit(this);
        }
    }
}
