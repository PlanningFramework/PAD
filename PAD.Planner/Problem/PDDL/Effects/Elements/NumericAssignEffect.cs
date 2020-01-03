
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing a numeric function assignment effect.
    /// </summary>
    public class NumericAssignEffect : PrimitiveEffect
    {
        /// <summary>
        /// Function atom.
        /// </summary>
        public IAtom FunctionAtom { set; get; }

        /// <summary>
        /// Numeric value to be assigned.
        /// </summary>
        public INumericExpression Value { get; set; }

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        protected IdManager IdManager { set; get; }

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="functionAtom">Numeric function atom.</param>
        /// <param name="value">Numeric value to be assigned.</param>
        /// <param name="idManager">ID manager.</param>
        public NumericAssignEffect(IAtom functionAtom, INumericExpression value, IdManager idManager)
        {
            FunctionAtom = functionAtom;
            Value = value;
            IdManager = idManager;
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
        /// Applies the assign operation effect.
        /// </summary>
        /// <param name="state">State to be modified.</param>
        /// <param name="functionAtom">Numeric function atom.</param>
        /// <param name="value">Value to be assigned.</param>
        public virtual void ApplyAssignOperation(IState state, IAtom functionAtom, double value)
        {
            state.AssignNumericFunction(functionAtom, value);
        }

        /// <summary>
        /// Gets the substituted value from the backwards application of the assignment.
        /// </summary>
        /// <returns>Backwards applied assignment.</returns>
        public virtual INumericExpression GetBackwardsSubstitutedValue()
        {
            return Value;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("numAssign", FunctionAtom, Value);
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

            NumericAssignEffect other = obj as NumericAssignEffect;
            if (other == null)
            {
                return false;
            }

            return FunctionAtom.Equals(other.FunctionAtom) && Value.Equals(other.Value);
        }
    }
}
