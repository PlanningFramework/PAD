
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a relaxed PDDL planning problem.
    /// </summary>
    public class RelaxedProblem : Problem, IRelaxedProblem
    {
        /// <summary>
        /// Constructs the relaxed PDDL planning problem.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public RelaxedProblem(InputData.PDDLInputData inputData) : base(inputData)
        {
            foreach (var liftedOperator in Operators)
            {
                liftedOperator.Effects.SetDeleteRelaxation();
            }
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
