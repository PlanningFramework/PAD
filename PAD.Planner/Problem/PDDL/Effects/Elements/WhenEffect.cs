using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a 'when' effect.
    /// </summary>
    public class WhenEffect : IEffect
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public IExpression Expression { get; set; }

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public List<PrimitiveEffect> Effects { get; set; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        /// <param name="effects">List of primitive effects.</param>
        public WhenEffect(IExpression expression, List<PrimitiveEffect> effects)
        {
            Expression = expression;
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
            return HashHelper.GetHashCode("when", Expression, Effects);
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

            WhenEffect other = obj as WhenEffect;
            if (other == null)
            {
                return false;
            }

            return Expression.Equals(other.Expression)
                && CollectionsEquality.Equals(Effects, other.Effects);
        }
    }
}
