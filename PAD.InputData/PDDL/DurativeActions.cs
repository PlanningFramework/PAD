using System.Collections.Generic;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL durative-actions.
    /// </summary>
    public class DurativeActions : List<DurativeAction>, IVisitable
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
            ForEach(durativeAction => durativeAction.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL durative-action.
    /// </summary>
    public class DurativeAction : IVisitable
    {
        /// <summary>
        /// Durative-action name.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// List of durative-action parameters.
        /// </summary>
        public Parameters Parameters { get; set; } = new Parameters();

        /// <summary>
        /// Durative-action duration constraints.
        /// </summary>
        public DurativeConstraints Durations { get; set; } = new DurativeConstraints();

        /// <summary>
        /// Durative-action conditions.
        /// </summary>
        public DurativeConditions Conditions { get; set; } = new DurativeConditions();

        /// <summary>
        /// Durative-action effects.
        /// </summary>
        public DurativeEffects Effects { get; set; } = new DurativeEffects();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(:durative-action {Name} :parameters {Parameters} :duration {Durations} :condition {Conditions} :effect {Effects})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Parameters.Accept(visitor);
            Durations.Accept(visitor);
            Conditions.Accept(visitor);
            Effects.Accept(visitor);
            visitor.PostVisit(this);
        }
    }
}
