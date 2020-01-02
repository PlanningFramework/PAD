using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Interface representing common SAS+ conditions, specifying basic operations on such conditions. SAS+ conditions give constraints for
    /// SAS+ states - we can also say that single conditions represent a class of states fulfilling the constraints given by these conditions.
    /// </summary>
    public interface IConditions : Planner.IConditions
    {
        /// <summary>
        /// Evaluates the conditions with the given reference state.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <returns>True if all conditions are met in the given state, false otherwise.</returns>
        bool Evaluate(IState state);

        /// <summary>
        /// Creates a conjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Conjunction of the current conditions and the given other conditions.</returns>
        IConditions ConjunctionWith(IConditions other);

        /// <summary>
        /// Creates a disjunction of the current conditions and the specified other conditions.
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>Disjunction of the current conditions and the given other conditions.</returns>
        IConditions DisjunctionWith(IConditions other);

        /// <summary>
        /// Checks whether the conditions are in conflict with the other conditions (i.e. different constraints on the same variables).
        /// </summary>
        /// <param name="other">Other conditions.</param>
        /// <returns>True if the conditions are conflicted with the other conditions, false otherwise.</returns>
        bool IsConflictedWith(IConditions other);

        /// <summary>
        /// Removes the specified assignment constraint from the current conditions.
        /// </summary>
        /// <param name="assignment">Constraint assignment.</param>
        /// <returns>True if the constraint has been successfully removed, false otherwise.</returns>
        bool RemoveConstraint(IAssignment assignment);

        /// <summary>
        /// Checks whether the specified assignment is actually constrained by the conditions.
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the given assignment is constrained by the conditions, false otherwise.</returns>
        bool IsConstrained(IAssignment assignment);

        /// <summary>
        /// Checks whether the conditions are in conflict with the specified assignment (i.e. different constraints on the same variable).
        /// </summary>
        /// <param name="assignment">Assignment.</param>
        /// <returns>True if the conditions are conflicted with the given assignment, false otherwise.</returns>
        bool IsConflictedWith(IAssignment assignment);

        /// <summary>
        /// Evaluates the relavance of a single effect assignment.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        /// <returns>Effect relevance of the specified assignment.</returns>
        EffectRelevance IsEffectAssignmentRelevant(IAssignment assignment);

        /// <summary>
        /// Checks whether the conditions are compatible with the given mutually exclusion constraints.
        /// </summary>
        /// <param name="mutexConstraints">Mutex constraints.</param>
        /// <returns>True if the conditions is compatible with the specified mutex constraints, false otherwise.</returns>
        bool IsCompatibleWithMutexContraints(IList<IAssignment> mutexConstraints);

        /// <summary>
        /// Gets the collection of simple conditions, i.e. individual conjuncts in case of the clause conditions, or itself
        /// if the current conditions is already simple conditions variant.
        /// </summary>
        /// <returns>Collection of simple conditions.</returns>
        IEnumerable<ISimpleConditions> GetSimpleConditions();

        /// <summary>
        /// Gets the number of not accomplished condition constraints for the specified state.
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished condition constraints.</returns>
        int GetNotAccomplishedConstraintsCount(IState state);
    }
}
