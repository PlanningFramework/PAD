using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Evaluator/collector of satisfying atoms for specified conditions.
    /// </summary>
    public class SatisfyingAtomsEvaluator : ConditionsCNFEvaluator
    {
        /// <summary>
        /// List of the atoms satisfying the evaluated conditions.
        /// </summary>
        public List<IAtom> Atoms { set; get; } = new List<IAtom>();

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="rigidRelations">Rigid relations.</param>
        public SatisfyingAtomsEvaluator(GroundingManager groundingManager, RigidRelations rigidRelations) : base(groundingManager, rigidRelations)
        {
        }

        /// <summary>
        /// Gets a list of atoms from the specified state that are necessary to make these conditions true.
        /// </summary>
        /// <param name="conditions">Conditions to evaluate.</param>
        /// <param name="substitution">Variable substitution.</param>
        /// <param name="predecessorState">Preceding state.</param>
        /// <returns>List of satisfying atoms.</returns>
        public List<IAtom> Evaluate(IConditions conditions, ISubstitution substitution, IState predecessorState)
        {
            ConditionsCNF conditionsCNF = (ConditionsCNF)conditions.GetCNF();
            Atoms = new List<IAtom>();
            Substitution = substitution;
            ReferenceState = predecessorState;

            conditionsCNF.Accept(this);

            return Atoms;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public override bool Visit(PredicateLiteralCNF expression)
        {
            IAtom groundedPredicateAtom = GroundingManager.GroundAtomDeep(expression.PredicateAtom, Substitution, ReferenceState);

            if (RigidRelations.Contains(groundedPredicateAtom))
            {
                // satisfied or failed by rigid relation
                return !expression.IsNegated;
            }

            bool hasPredicate = ReferenceState.HasPredicate(groundedPredicateAtom);

            if (hasPredicate == expression.IsNegated)
            {
                // failed by state predicate
                return false;
            }

            // satisfied by state predicate -> store this atom
            Atoms.Add(groundedPredicateAtom);
            return true;
        }
    }
}
