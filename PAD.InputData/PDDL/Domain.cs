using System.Collections.Generic;
using System;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a PDDL domain.
    /// </summary>
    public class Domain : IVisitable
    {
        /// <summary>
        /// Domain name.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Domain requirements.
        /// </summary>
        public Requirements Requirements { set; get; } = new Requirements();

        /// <summary>
        /// Domain types.
        /// </summary>
        public Types Types { set; get; } = new Types();

        /// <summary>
        /// Domain constants.
        /// </summary>
        public Constants Constants { set; get; } = new Constants();

        /// <summary>
        /// Domain predicates.
        /// </summary>
        public Predicates Predicates { set; get; } = new Predicates();

        /// <summary>
        /// Domain functions.
        /// </summary>
        public Functions Functions { set; get; } = new Functions();

        /// <summary>
        /// Domain constraints.
        /// </summary>
        public Constraints Constraints { set; get; } = new Constraints();

        /// <summary>
        /// Domain actions.
        /// </summary>
        public Actions Actions { set; get; } = new Actions();

        /// <summary>
        /// Domain durative actions.
        /// </summary>
        public DurativeActions DurativeActions { set; get; } = new DurativeActions();

        /// <summary>
        /// Domain derived predicates.
        /// </summary>
        public DerivedPredicates DerivedPredicates { set; get; } = new DerivedPredicates();

        /// <summary>
        /// Path to the source PDDL domain file.
        /// </summary>
        public string FilePath { set; get; } = "";

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> sections = new List<string>
            {
                Requirements.ToString(), Types.ToString(), Constants.ToString(), Predicates.ToString(), Functions.ToString(),
                Constraints.ToString(), Actions.ToString(), DurativeActions.ToString(), DerivedPredicates.ToString()
            };
            sections.RemoveAll(x => string.IsNullOrEmpty(x));

            return string.Join(Environment.NewLine,
                $"(define(domain {Name})",
                $" {string.Join($"{Environment.NewLine} ", sections)}",
                $")");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Requirements.Accept(visitor);
            Types.Accept(visitor);
            Constants.Accept(visitor);
            Predicates.Accept(visitor);
            Functions.Accept(visitor);
            Constraints.Accept(visitor);
            Actions.Accept(visitor);
            DurativeActions.Accept(visitor);
            DerivedPredicates.Accept(visitor);
        }
    }
}
