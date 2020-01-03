using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Grounder object returning (partially) grounded conditions for the given substitution. All variable instances specified in the given
    /// variable substitution are replaced by the substituted values (i.e. constants), others remain intact in the conditions parameters.
    /// </summary>
    public class ConditionsGrounder
    {
        /// <summary>
        /// Expressions grounder.
        /// </summary>
        private Lazy<ExpressionsGrounder> ExpressionsGrounder { get; }

        /// <summary>
        /// Constructs the conditions grounder.
        /// </summary>
        /// <param name="expressionsGrounder">Expressions grounder.</param>
        public ConditionsGrounder(Lazy<ExpressionsGrounder> expressionsGrounder)
        {
            ExpressionsGrounder = expressionsGrounder;
        }

        /// <summary>
        /// Grounds the conditions by the specified substitution.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <returns>Grounded conditions.</returns>
        public Conditions Ground(Conditions conditions, ISubstitution substitution)
        {
            // (partially) ground the whole conditions by the specified substitution

            Conditions newConditions = conditions.CloneEmpty();
            foreach (var expression in conditions)
            {
                newConditions.Add(ExpressionsGrounder.Value.Ground(expression, substitution));
            }

            // for partially lifted conditions - remove parameters that were grounded by the substitution

            if (conditions.Parameters != null)
            {
                newConditions.Parameters.Clear();
                foreach (var parameter in conditions.Parameters)
                {
                    if (!substitution.Contains(parameter.ParameterNameId))
                    {
                        newConditions.Parameters.Add(parameter.Clone());
                    }
                }

                // if the conditions are fully grounded, set the parameters as null

                if (newConditions.Parameters.Count == 0)
                {
                    newConditions.Parameters = null;
                }
            }

            return newConditions;
        }
    }
}
