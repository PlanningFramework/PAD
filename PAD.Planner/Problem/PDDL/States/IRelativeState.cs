using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for a relative state in the PDDL planning problem. Relative state is an extension of a standard state, representing
    /// a whole class of states. It is an alternative way to express conditions in the backwards planning (an alternative to the more general
    /// IConditions). Relative states in PDDL contain only the predicates and function values that are common for a group of states and
    /// additionally allow to explicitly express that something cannot be true, via "negated" predicates. Note the difference between a
    /// standard state and a relative state: everything not expressed in the state is implicitly not true, while everything not expressed
    /// in the relative state is implicitly both true and false - that's why the negated predicates have a good purpose here.
    /// </summary>
    public interface IRelativeState : Planner.IRelativeState, IState
    {
        /// <summary>
        /// Adds the negated predicate to the relative state.
        /// </summary>
        /// <param name="predicate">Predicate to be added.</param>
        void AddNegatedPredicate(IAtom predicate);

        /// <summary>
        /// Removes the negated predicate from the relative state.
        /// </summary>
        /// <param name="predicate">Predicate to be removed.</param>
        void RemoveNegatedPredicate(IAtom predicate);

        /// <summary>
        /// Checks whether the relative state contains the given negated predicate.
        /// </summary>
        /// <param name="predicate">Predicate to be checked.</param>
        /// <returns>True if the relative state contains the negated predicate, false otherwise.</returns>
        bool HasNegatedPredicate(IAtom predicate);

        /// <summary>
        /// Enumerates the contained negated predicates.
        /// </summary>
        /// <returns>Enumeration of negated predicates.</returns>
        IEnumerable<IAtom> GetNegatedPredicates();
    }
}
