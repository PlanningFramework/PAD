using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Common interface for a state in the PDDL planning problem.
    /// </summary>
    public interface IState : Planner.IState
    {
        /// <summary>
        /// Adds the predicate to the state.
        /// </summary>
        /// <param name="predicate">Predicate to be added.</param>
        void AddPredicate(IAtom predicate);

        /// <summary>
        /// Removes the predicate from the state.
        /// </summary>
        /// <param name="predicate">Predicate to be removed.</param>
        void RemovePredicate(IAtom predicate);

        /// <summary>
        /// Checks whether the state contains the given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to be checked.</param>
        /// <returns>True if the state contains the predicate, false otherwise.</returns>
        bool HasPredicate(IAtom predicate);

        /// <summary>
        /// Returns the value of the given object function.
        /// </summary>
        /// <param name="function">Grounded function atom.</param>
        /// <returns>Object value, i.e. constant name ID.</returns>
        int GetObjectFunctionValue(IAtom function);

        /// <summary>
        /// Defines a new value for the requested function in the state.
        /// </summary>
        /// <param name="function">Requested function.</param>
        /// <param name="assignment">Value to be assigned.</param>
        void AssignObjectFunction(IAtom function, int assignment);

        /// <summary>
        /// Returns the value of the given numeric function.
        /// </summary>
        /// <param name="function">Grounded function atom.</param>
        /// <returns>Numeric value.</returns>
        double GetNumericFunctionValue(IAtom function);

        /// <summary>
        /// Defines a new value for the requested function in the state.
        /// </summary>
        /// <param name="function">Requested function.</param>
        /// <param name="assignment">Value to be assigned.</param>
        void AssignNumericFunction(IAtom function, double assignment);

        /// <summary>
        /// Increase the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be increased by.</param>
        void IncreaseNumericFunction(IAtom function, double value);

        /// <summary>
        /// Decrease the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be decreased by.</param>
        void DecreaseNumericFunction(IAtom function, double value);

        /// <summary>
        /// Scale-up the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be scaled-up by.</param>
        void ScaleUpNumericFunction(IAtom function, double value);

        /// <summary>
        /// Scale-down the value of the requested numeric function by the specified value.
        /// </summary>
        /// <param name="function">Requested numeric function.</param>
        /// <param name="value">Value to be scaled-down by.</param>
        void ScaleDownNumericFunction(IAtom function, double value);

        /// <summary>
        /// Enumerates the contained predicates.
        /// </summary>
        /// <returns>Enumeration of contained predicates.</returns>
        IEnumerable<IAtom> GetPredicates();

        /// <summary>
        /// Enumerates the contained object functions with their assigned values. If a function is not contained, its value is undefined.
        /// </summary>
        /// <returns>Enumeration of object functions with their values.</returns>
        IEnumerable<KeyValuePair<IAtom, int>> GetObjectFunctions();

        /// <summary>
        /// Enumerates the contained numeric functions with their assigned values. If a function is not contained, its value is undefined.
        /// </summary>
        /// <returns>Enumeration of numeric functions with their values.</returns>
        IEnumerable<KeyValuePair<IAtom, double>> GetNumericFunctions();

        /// <summary>
        /// Clears the content of the state.
        /// </summary>
        void ClearContent();
    }
}
