using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL predicates.
    /// </summary>
    public class Predicates : List<Predicate>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":predicates");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(predicate => predicate.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL predicate.
    /// </summary>
    public class Predicate : IVisitable
    {
        /// <summary>
        /// Name of the predicate.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Terms of the predicate.
        /// </summary>
        public DefinitionTerms Terms { set; get; } = new DefinitionTerms();

        /// <summary>
        /// Constructs a predicate.
        /// </summary>
        /// <param name="name">Predicate name.</param>
        public Predicate(string name)
        {
            Name = name;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return (Terms.Count > 0) ? $"({Name} {Terms})" : $"({Name})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Terms.Accept(visitor);
        }
    }
}
