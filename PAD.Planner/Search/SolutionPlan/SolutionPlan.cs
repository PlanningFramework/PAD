using System.Collections.Generic;
using System.Linq;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Solution plan, i.e. a sequence of applied grounded operators from the initial state of the planning problem
    /// to a goal state.
    /// </summary>
    public class SolutionPlan : List<IOperator>, ISolutionPlan
    {
        /// <summary>
        /// Initial state of the plan.
        /// </summary>
        public ISearchNode InitialState { set; get; } = null;

        /// <summary>
        /// Constructs an empty solution plan.
        /// </summary>
        /// <param name="initialState">Initial state of the plan.</param>
        public SolutionPlan(ISearchNode initialState)
        {
            if (initialState != null)
            {
                InitialState = initialState;
            }
        }

        /// <summary>
        /// Constructs the solution plan from the given sequence of operators.
        /// </summary>
        /// <param name="initialState">Initial state of the plan.</param>
        /// <param name="operators">Sequence of grounded operators.</param>
        public SolutionPlan(ISearchNode initialState, IEnumerable<IOperator> operators) : base(operators)
        {
            if (initialState != null)
            {
                InitialState = initialState;
            }
        }

        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISearchNode> GetStatesSequence()
        {
            ISearchNode currentState = InitialState;
            yield return currentState;

            foreach (var op in this)
            {
                currentState = op.Apply((IState)currentState);
                yield return currentState;
            }
        }

        /// <summary>
        /// Gets a sequence of states corresponding to successive application of the operators in the solution plan.
        /// The first state should be always initial state of the problem, the last state should be a goal state.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IOperator> GetOperatorsSequence()
        {
            return this;
        }

        /// <summary>
        /// Gets the solution cost (i.e. solution length).
        /// </summary>
        /// <returns>Solution cost.</returns>
        public double GetCost()
        {
            return this.Sum(oper => oper.GetCost());
        }

        /// <summary>
        /// Gets the full string description of the solution plan, i.e. the full path of operartor applications
        /// including all the intermediate states.
        /// </summary>
        /// <returns>Full description of the solution plan.</returns>
        public string GetFullDescription()
        {
            List<string> description = new List<string>();
            ISearchNode currentState = InitialState;

            description.Add(currentState.ToString());
            foreach (var oper in this)
            {
                description.Add("+");
                description.Add(oper.GetName());
                description.Add("=>");

                currentState = oper.Apply((IState)currentState);
                description.Add(currentState.ToString());
            }

            return string.Join(" ", description);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"<{string.Join(", ", ConvertAll(oper => oper.GetName()))}>";
        }
    }
}
