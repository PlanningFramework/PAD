using System.Collections.Generic;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a 'forall' effect.
    /// </summary>
    public class ForallEffect : IEffect
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; }

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public List<IEffect> Effects { get; set; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="effects">List of effects.</param>
        public ForallEffect(Parameters parameters, List<IEffect> effects)
        {
            Parameters = parameters;
            Effects = effects;
        }

        /// <summary>
        /// Accepts a visitor evaluating the effect.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IEffectVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("forall", Parameters, Effects);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            ForallEffect other = obj as ForallEffect;
            if (other == null)
            {
                return false;
            }

            return Parameters.Equals(other.Parameters)
                && CollectionsEquality.Equals(Effects, other.Effects);
        }
    }
}
