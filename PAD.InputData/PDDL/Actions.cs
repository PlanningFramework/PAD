using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL actions.
    /// </summary>
    public class Actions : List<Action>, IVisitable
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
            ForEach(action => action.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL action.
    /// </summary>
    public class Action : IVisitable
    {
        /// <summary>
        /// Action name.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// List of action parameters.
        /// </summary>
        public Parameters Parameters { get; set; } = new Parameters();

        /// <summary>
        /// Action preconditions.
        /// </summary>
        public Preconditions Preconditions { get; set; } = new Preconditions();

        /// <summary>
        /// Action effects.
        /// </summary>
        public Effects Effects { get; set; } = new Effects();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(:action {Name} :parameters {Parameters} :precondition {Preconditions} :effect {Effects})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Parameters.Accept(visitor);
            Preconditions.Accept(visitor);
            Effects.Accept(visitor);
            visitor.PostVisit(this);
        }
    }
}
