
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a numeric function decrease effect.
    /// </summary>
    public class NumericDecreaseEffect : NumericAssignEffect
    {
        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="functionAtom">Numeric function atom.</param>
        /// <param name="value">Numeric value to be assigned.</param>
        /// <param name="idManager">ID manager.</param>
        public NumericDecreaseEffect(IAtom functionAtom, INumericExpression value, IDManager idManager) : base(functionAtom, value, idManager)
        {
        }

        /// <summary>
        /// Applies the assign operation effect.
        /// </summary>
        /// <param name="state">State to be modified.</param>
        /// <param name="functionAtom">Numeric function atom.</param>
        /// <param name="value">Value to be assigned.</param>
        public override void ApplyAssignOperation(IState state, IAtom functionAtom, double value)
        {
            state.DecreaseNumericFunction(functionAtom, value);
        }

        /// <summary>
        /// Gets the substitued value from the backwards application of the assignment.
        /// </summary>
        /// <returns>Backwards applied assignment.</returns>
        public override INumericExpression GetBackwardsSubstituedValue()
        {
            return new Plus(new NumericFunction(FunctionAtom, IDManager), Value);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("decrease", FunctionAtom, Value);
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

            NumericDecreaseEffect other = obj as NumericDecreaseEffect;
            if (other == null)
            {
                return false;
            }

            return FunctionAtom.Equals(other.FunctionAtom) && Value.Equals(other.Value);
        }
    }
}
