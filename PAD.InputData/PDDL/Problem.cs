using System.Collections.Generic;
using System;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a PDDL problem.
    /// </summary>
    public class Problem : IVisitable
    {
        /// <summary>
        /// Problem name.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Corresponding domain name.
        /// </summary>
        public string DomainName { set; get; } = "";

        /// <summary>
        /// Problem requirements.
        /// </summary>
        public Requirements Requirements { set; get; } = new Requirements();

        /// <summary>
        /// Problem objects.
        /// </summary>
        public Objects Objects { set; get; } = new Objects();

        /// <summary>
        /// Problem init elements.
        /// </summary>
        public Init Init { set; get; } = new Init();

        /// <summary>
        /// Problem goal condition.
        /// </summary>
        public Goal Goal { set; get; } = new Goal();

        /// <summary>
        /// Problem constraints.
        /// </summary>
        public Constraints Constraints { set; get; } = new Constraints();

        /// <summary>
        /// Problem metric specification.
        /// </summary>
        public Metric Metric { set; get; } = new Metric();

        /// <summary>
        /// Problem length specification.
        /// </summary>
        public Length Length { set; get; } = new Length();

        /// <summary>
        /// Path to the source PDDL problem file.
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
                Requirements.ToString(), Objects.ToString(), Init.ToString(), Goal.ToString(), Constraints.ToString(),
                Metric.ToString(), Length.ToString()
            };
            sections.RemoveAll(string.IsNullOrEmpty);

            return string.Join(Environment.NewLine,
                $"(define(problem {Name})",
                $" (:domain {DomainName})",
                $" {string.Join($"{Environment.NewLine} ", sections)}",
                ")");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Requirements.Accept(visitor);
            Objects.Accept(visitor);
            Init.Accept(visitor);
            Goal.Accept(visitor);
            Constraints.Accept(visitor);
            Metric.Accept(visitor);
            Length.Accept(visitor);
        }
    }
}
