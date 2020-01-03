
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a numeric function scale-down effect.
    /// </summary>
    public class NumericScaleDownEffect : NumericAssignEffect
    {
        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="functionAtom">Numeric function atom.</param>
        /// <param name="value">Numeric value to be assigned.</param>
        /// <param name="idManager">ID manager.</param>
        public NumericScaleDownEffect(IAtom functionAtom, INumericExpression value, IdManager idManager) : base(functionAtom, value, idManager)
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
            state.ScaleDownNumericFunction(functionAtom, value);
        }

        /// <summary>
        /// Gets the substituted value from the backwards application of the assignment.
        /// </summary>
        /// <returns>Backwards applied assignment.</returns>
        public override INumericExpression GetBackwardsSubstitutedValue()
        {
            return new Multiply(new NumericFunction(FunctionAtom, IdManager), Value);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("scale-down", FunctionAtom, Value);
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

            NumericScaleDownEffect other = obj as NumericScaleDownEffect;
            if (other == null)
            {
                return false;
            }

            return FunctionAtom.Equals(other.FunctionAtom) && Value.Equals(other.Value);
        }
    }
}
