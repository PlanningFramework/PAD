using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Collector structure gathering positive result atoms of the operator effects application.
    /// </summary>
    public class EffectsResultAtomsCollector : IEffectVisitor
    {
        /// <summary>
        /// Collected result atoms.
        /// </summary>
        public List<IAtom> ResultAtoms { set; get; } = new List<IAtom>();

        /// <summary>
        /// Effects list.
        /// </summary>
        private List<IEffect> Effects { set; get; } = null;

        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Variable substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Is the current effect being negated?
        /// </summary>
        private bool IsNegated { set; get; } = false;

        /// <summary>
        /// Constructs the collector.
        /// </summary>
        /// <param name="effects">List of used effects.</param>
        /// <param name="groundingManager">Grounding manager.</param>
        public EffectsResultAtomsCollector(List<IEffect> effects, GroundingManager groundingManager)
        {
            Effects = effects;
            GroundingManager = groundingManager;
        }

        /// <summary>
        /// Collects result atoms.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <returns>Collection of result atoms.</returns>
        public List<IAtom> Collect(ISubstitution substitution)
        {
            ResultAtoms = new List<IAtom>();
            Substitution = substitution;

            foreach (var effect in Effects)
            {
                effect.Accept(this);
            }

            Substitution = null;
            return ResultAtoms;
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ForallEffect effect)
        {
            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(effect.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                foreach (var localEffect in effect.Effects)
                {
                    localEffect.Accept(this);
                }
                Substitution.RemoveLocalSubstitution(localSubstitution);
            }
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(WhenEffect effect)
        {
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(PredicateEffect effect)
        {
            if (!IsNegated)
            {
                ResultAtoms.Add(GroundingManager.GroundAtom(effect.PredicateAtom, Substitution));
            }
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
            IsNegated = true;
            effect.Argument.Accept(this);
            IsNegated = false;
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
