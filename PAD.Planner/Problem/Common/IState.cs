// ReSharper disable CommentTypo

namespace PAD.Planner
{
    /// <summary>
    /// Common interface for a PDDL or SAS+ state in the planning problem.
    /// </summary>
    public interface IState : IStateOrConditions
    {
        /// <summary>
        /// Gets the state size (i.e. number of grounded fluents).
        /// </summary>
        /// <returns>State size.</returns>
        int GetSize();

        /// <summary>
        /// Gets the relaxed variant of this state.
        /// </summary>
        /// <returns>Relaxed state.</returns>
        IState GetRelaxedState();

        /// <summary>
        /// Gets the corresponding conditions describing this state in the given planning problem.
        /// </summary>
        /// <param name="problem">Parent planning problem.</param>
        /// <returns>Conditions describing the state.</returns>
        IConditions GetDescribingConditions(IProblem problem);

        /// <summary>
        /// Makes a deep copy of the state.
        /// </summary>
        /// <returns>Deep copy of the state.</returns>
        IState Clone();
    }
}
