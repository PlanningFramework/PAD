using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure representing rigid relations in a PDDL planning problem, i.e. the predicates that always hold.
    /// </summary>
    public class RigidRelations : HashSet<IAtom>
    {
        /// <summary>
        /// Constructs the rigid relations from the initial state and the defined operators.
        /// </summary>
        /// <param name="initialState">Initial state in the planning problem.</param>
        /// <param name="operators">Operators in the planning problem.</param>
        public RigidRelations(IState initialState, LiftedOperators operators)
        {
            SetInitialState(initialState, operators);
        }

        /// <summary>
        /// Constructs an empty rigid relations collection.
        /// </summary>
        public RigidRelations()
        {
        }

        /// <summary>
        /// Sets the rigid relations from the initial state and the defined operators. The relations in the initial state that are
        /// identified as rigid are moved to this structure and they are removed from the initial state.
        /// </summary>
        /// <param name="initialState">Initial state in the planning problem.</param>
        /// <param name="operators">Operators in the planning problem.</param>
        public void SetInitialState(IState initialState, LiftedOperators operators)
        {
            Clear();

            RigidRelationsBuilder builder = new RigidRelationsBuilder();
            HashSet<IAtom> rigidRelations = builder.Build(initialState, operators);

            foreach (var rigidRelation in rigidRelations)
            {
                initialState.RemovePredicate(rigidRelation);
                Add(rigidRelation);
            }
        }

        /// <summary>
        /// Checks whether the specified predicate is a rigid relation of the planning problem.
        /// </summary>
        /// <param name="predicateAtom">Predicate atom.</param>
        /// <returns>Treu if the predicate is of rigid relation, false otherwise.</returns>
        public bool IsPredicateRigidRelation(IAtom predicateAtom)
        {
            foreach (var item in this)
            {
                if (item.GetNameID() == predicateAtom.GetNameID())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
