
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Representation of a single SAS+ axiom rule.
    /// </summary>
    public class AxiomRule
    {
        /// <summary>
        /// Conditions to be met for the axiom rule to be applied ("axiom rule body").
        /// </summary>
        public IConditions Conditions { set; get; }

        /// <summary>
        /// Actual effect of the axiom rule ("axiom rule head").
        /// </summary>
        public IAssignment Assignment { set; get; }

        /// <summary>
        /// Constructs a single SAS+ axiom rule from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public AxiomRule(InputData.SAS.AxiomRule inputData)
        {
            Conditions = new Conditions(inputData.Conditions);
            Assignment = new Assignment(inputData.PrimitiveEffect);
        }

        /// <summary>
        /// Checks whether the axiom rule is applicable to the specified state.
        /// </summary>
        /// <param name="state">State to be checked.</param>
        /// <returns>True if the axiom rule is applicable, false otherwise.</returns>
        public bool IsApplicable(IState state)
        {
            return Conditions.Evaluate(state);
        }

        /// <summary>
        /// Applies the rule to the specified state, by directly modifying it.
        /// </summary>
        /// <param name="state">State to be modified.</param>
        /// <returns>True if the state was actually modified by the axiom rule, false otherwise.</returns>
        public bool Apply(IState state)
        {
            if (!state.HasValue(Assignment))
            {
                state.SetValue(Assignment);
                return true;
            }
            return false;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({Conditions}) -> ({Assignment})";
        }
    }
}
