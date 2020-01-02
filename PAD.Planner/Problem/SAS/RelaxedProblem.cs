
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a relaxed SAS+ planning problem.
    /// </summary>
    public class RelaxedProblem : Problem, IRelaxedProblem
    {
        /// <summary>
        /// Goal conditions for the relaxed planning problem.
        /// </summary>
        public IConditions ComplexGoalConditions { set; get; } = null;

        /// <summary>
        /// Constructs the relaxed SAS+ planning problem.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="initialStateInit">Should the initial state be inited?</param>
        public RelaxedProblem(InputData.SASInputData inputData, bool initialStateInit = true) : base(inputData)
        {
            if (initialStateInit)
            {
                InitialState = new RelaxedState((State)InitialState);
            }
            else
            {
                InitialState = null;
            }

            MutexGroups.Clear();
        }

        /// <summary>
        /// Sets the initial state of the planning problem.
        /// </summary>
        /// <param name="state">State.</param>
        public override void SetInitialState(Planner.IState state)
        {
            InitialState = new RelaxedState((State)state);
        }

        /// <summary>
        /// Sets the goal conditions of the planning problem.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        public override void SetGoalConditions(Planner.IConditions conditions)
        {
            ComplexGoalConditions = (IConditions)conditions;
        }

        /// <summary>
        /// Gets the goal conditions of the planning problem.
        /// </summary>
        /// <returns>Goal conditions.</returns>
        public override Planner.IConditions GetGoalConditions()
        {
            return (ComplexGoalConditions == null) ? GoalConditions : ComplexGoalConditions;
        }

        /// <summary>
        /// Gets the corresponding relaxed planning graph.
        /// </summary>
        /// <returns>Relaxed planning graph.</returns>
        public IRelaxedPlanningGraph GetRelaxedPlanningGraph()
        {
            return new RelaxedPlanningGraph(this);
        }
    }
}
