using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Data structure for PDDL lifted operators (i.e. lifted 'actions').
    /// </summary>
    public class LiftedOperators : List<LiftedOperator>
    {
        /// <summary>
        /// Constructs the object from the input data.
        /// </summary>
        /// <param name="actions">PDDL actions definition.</param>
        /// <param name="idManager">ID manager.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public LiftedOperators(InputData.PDDL.Actions actions, IdManager idManager, EvaluationManager evaluationManager)
        {
            actions.ForEach(action => Add(new LiftedOperator(action, idManager, evaluationManager)));
        }
    }
}