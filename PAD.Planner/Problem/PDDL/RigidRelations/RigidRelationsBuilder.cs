using System.Collections.Generic;
// ReSharper disable IdentifierTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Rigid relations builder. Finds out which relations from the initial state are always true.
    /// </summary>
    public class RigidRelationsBuilder : IEffectVisitor
    {
        /// <summary>
        /// Collection of predicate names that can be influenced by PDDL operator effects.
        /// </summary>
        private HashSet<int> PredicatesInfluencedByOperators { set; get; }

        /// <summary>
        /// Builds the collection of rigid relations in the PDDL planning problem.
        /// </summary>
        /// <param name="initialState">Initial state of the planning problem.</param>
        /// <param name="operators">Operators of the planning problem.</param>
        /// <returns>Set of relations of the initial state that are always true.</returns>
        public HashSet<IAtom> Build(IState initialState, LiftedOperators operators)
        {
            PredicatesInfluencedByOperators = new HashSet<int>();
            foreach (var oper in operators)
            {
                foreach (var effect in oper.Effects)
                {
                    effect.Accept(this);
                }
            }

            HashSet<IAtom> rigidRelations = new HashSet<IAtom>();
            foreach (var predicate in initialState.GetPredicates())
            {
                if (!PredicatesInfluencedByOperators.Contains(predicate.GetNameId()))
                {
                    rigidRelations.Add(predicate);
                }
            }

            return rigidRelations;
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ForallEffect effect)
        {
            effect.Effects.ForEach(localEffect => localEffect.Accept(this));
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(WhenEffect effect)
        {
            effect.Effects.ForEach(localEffect => localEffect.Accept(this));
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(PredicateEffect effect)
        {
            PredicatesInfluencedByOperators.Add(effect.PredicateAtom.GetNameId());
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(EqualsEffect effect)
        {
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(NotEffect effect)
        {
            effect.Argument.Accept(this);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(NumericAssignEffect effect)
        {
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ObjectAssignEffect effect)
        {
        }
    }
}
