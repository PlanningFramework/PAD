
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a object function assignment effect.
    /// </summary>
    public class ObjectAssignEffect : PrimitiveEffect
    {
        /// <summary>
        /// Function atom.
        /// </summary>
        public IAtom FunctionAtom { set; get; }

        /// <summary>
        /// Object value to be assigned.
        /// </summary>
        public ITerm Value { get; set; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="functionAtom">Object function atom.</param>
        /// <param name="value">Object value to be assigned.</param>
        public ObjectAssignEffect(IAtom functionAtom, ITerm value)
        {
            FunctionAtom = functionAtom;
            Value = value;
        }

        /// <summary>
        /// Accepts a visitor evaluating the effect.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IEffectVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("objAssign", FunctionAtom, Value);
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

            ObjectAssignEffect other = obj as ObjectAssignEffect;
            if (other == null)
            {
                return false;
            }

            return FunctionAtom.Equals(other.FunctionAtom) && Value.Equals(other.Value);
        }
    }
}
