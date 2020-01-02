using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for PDDL logical conditions, typically used as goal conditions or operator conditions.
    /// </summary>
    public interface IConditions : Planner.IConditions
    {
        /// <summary>
        /// Gets the parameters of the conditions specifying of the lifted arguments.
        /// </summary>
        /// <returns>Parameters of the conditions. Can be null, if conditions fully grounded.</returns>
        Parameters GetParameters();

        /// <summary>
        /// Evaluates the conditions with the given reference state and variable substitution.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        bool Evaluate(IState state, ISubstitution substitution = null);

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        int GetNotAccomplishedConstraintsCount(IState state);

        /// <summary>
        /// Returns a collection of all used predicates within this conditions. Can be used for some preprocessing.
        /// </summary>
        /// <returns>Collection of used predicates.</returns>
        HashSet<IAtom> GetUsedPredicates();

        /// <summary>
        /// Returns the CNF (conjunctive-normal-form) of the condition expressions.
        /// </summary>
        /// <returns>CNF of the conditions.</returns>
        IConditions GetCNF();
    }
}
