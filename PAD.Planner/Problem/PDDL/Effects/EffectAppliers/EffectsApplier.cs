using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Effects applier, applying the operator effects to the specified state (making the successor state).
    /// </summary>
    public class EffectsApplier : IEffectVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Evaluation manager.
        /// </summary>
        private EvaluationManager EvaluationManager { get; }

        /// <summary>
        /// Numeric evaluator.
        /// </summary>
        private Lazy<NumericExpressionEvaluator> NumericEvaluator { get; }

        /// <summary>
        /// Effects applier mode.
        /// </summary>
        private EffectsApplierMode EffectsApplierMode { set; get; } = EffectsApplierMode.STANDARD;

        /// <summary>
        /// State on which the effects are applied on.
        /// </summary>
        private IState State { set; get; }

        /// <summary>
        /// Currently used variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; }

        /// <summary>
        /// Constructs the effects applier.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public EffectsApplier(EvaluationManager evaluationManager)
        {
            GroundingManager = evaluationManager.GroundingManager;
            EvaluationManager = evaluationManager;
            NumericEvaluator = new Lazy<NumericExpressionEvaluator>(() => new NumericExpressionEvaluator(GroundingManager));
        }

        /// <summary>
        /// Sets the delete relaxation for the operator effects.
        /// </summary>
        /// <param name="relaxed">Relaxed mode?</param>
        public void SetDeleteRelaxation(bool relaxed)
        {
            EffectsApplierMode = (relaxed) ? EffectsApplierMode.DELETE_RELAXATION : EffectsApplierMode.STANDARD;
        }

        /// <summary>
        /// Applies the specified effect on the given state (modifies him!).
        /// </summary>
        /// <param name="effect">Effect for the application.</param>
        /// <param name="state">State.</param>
        /// <param name="substitution">Variables substitution.</param>
        public void Apply(IEffect effect, IState state, ISubstitution substitution)
        {
            State = state;
            Substitution = substitution;

            effect.Accept(this);

            Substitution = null;
            State = null;
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
            if (EvaluationManager.Evaluate(effect.Expression, Substitution, State))
            {
                foreach (var localEffect in effect.Effects)
                {
                    localEffect.Accept(this);
                }
            }
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(PredicateEffect effect)
        {
            IAtom groundedPredicateAtom = GroundingManager.GroundAtomDeep(effect.PredicateAtom, Substitution, State);
            State.AddPredicate(groundedPredicateAtom);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(EqualsEffect effect)
        {
            // note: it is unclear what this type of effect should do (it's valid by PDDL grammar, though); ignored for now
            Debug.Assert(false, "Unsupported type of effect");
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(NotEffect effect)
        {
            if (EffectsApplierMode == EffectsApplierMode.DELETE_RELAXATION)
            {
                return;
            }

            PredicateEffect argumentEffect = effect.Argument as PredicateEffect;
            if (argumentEffect != null)
            {
                IAtom groundedPredicateAtom = GroundingManager.GroundAtomDeep(argumentEffect.PredicateAtom, Substitution, State);
                State.RemovePredicate(groundedPredicateAtom);
            }
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(NumericAssignEffect effect)
        {
            IAtom groundedFunctionAtom = GroundingManager.GroundAtomDeep(effect.FunctionAtom, Substitution, State);
            double value = NumericEvaluator.Value.Evaluate(effect.Value, Substitution, State);
            effect.ApplyAssignOperation(State, groundedFunctionAtom, value);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ObjectAssignEffect effect)
        {
            IAtom groundedFunctionAtom = GroundingManager.GroundAtomDeep(effect.FunctionAtom, Substitution, State);
            ITerm value = GroundingManager.GroundTermDeep(effect.Value, Substitution, State);
            ConstantTerm constantTermValue = (ConstantTerm)value;
            State.AssignObjectFunction(groundedFunctionAtom, constantTermValue.NameId);
        }
    }
}
