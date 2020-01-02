
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Common interface for a relative state in the SAS+ planning problem. Relative state is an extension of a standard state, representing
    /// a whole class of states. It is an alternative way to express conditions in the backwards planning (an alternative to the more general
    /// IConditions). Relative states in SAS+ allow variables to have another value: a wild card (-1), which indicates that the value might be
    /// arbitrary.
    /// </summary>
    public interface IRelativeState : Planner.IRelativeState, IState
    {
    }
}
