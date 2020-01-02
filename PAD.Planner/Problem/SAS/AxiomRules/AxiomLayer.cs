using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Representation of a single axiom layer with SAS+ axiom rules. Evaluation of a axiom layer means that all axiom rules on this
    /// layer are repeteadly applied, until no further changes occur. Then the next axiom layer is evaluated etc. We assume there are
    /// no possibly cyclic dependencies in the axiom rules definitions that could cause an endless evaluation (cases like that should
    /// be marked as invalid input in the validation of input data).
    /// </summary>
    public class AxiomLayer : List<AxiomRule>
    {
        /// <summary>
        /// Constructs an empty axiom layer.
        /// </summary>
        public AxiomLayer()
        {
        }

        /// <summary>
        /// Applies all the axiom rules on the current axiom layer to the given state, by directly modifying it.
        /// </summary>
        /// <param name="state">State to be modified.</param>
        public void Apply(IState state)
        {
            bool anyApplied = false;

            do
            {
                // repeatedly apply the rules on the current layer, until there is no change
                anyApplied = false;
                foreach (var axiomRule in this)
                {
                    if (axiomRule.IsApplicable(state) && axiomRule.Apply(state))
                    {
                        anyApplied = true;
                    }
                }
            }
            while (anyApplied);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}
