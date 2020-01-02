using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of PDDL operator effects.
    /// </summary>
    public class Effects : List<IEffect>
    {
        /// <summary>
        /// Effects applier, applying the operator effects to the specified state (making the successor state).
        /// </summary>
        private Lazy<EffectsApplier> EffectsApplier { set; get; } = null;

        /// <summary>
        /// Effects relevance evaluator, checking whether the operator effects are relevant to the specified state.
        /// </summary>
        private Lazy<EffectsRelevanceConditionsEvaluator> EffectsRelevanceConditionsEvaluator { set; get; } = null;

        /// <summary>
        /// Effects relevance evaluator, checking whether the operator effects are relevant to the specified relative state.
        /// </summary>
        private Lazy<EffectsRelevanceRelativeStateEvaluator> EffectsRelevanceRelativeStateEvaluator { set; get; } = null;

        /// <summary>
        /// Effects backwards applier, applying the relevant operators backwards to the given conditions.
        /// </summary>
        private Lazy<EffectsBackwardsConditionsApplier> EffectsBackwardsConditionsApplier { set; get; } = null;

        /// <summary>
        /// Effects backwards applier, applying the relevant operators backwards to the given relative state.
        /// </summary>
        private Lazy<EffectsBackwardsRelativeStateApplier> EffectsBackwardsRelativeStateApplier { set; get; } = null;

        /// <summary>
        /// Collector structure gathering positive result atoms of the operator effects application.
        /// </summary>
        private Lazy<EffectsResultAtomsCollector> EffectsResultAtomsCollector { set; get; } = null;

        /// <summary>
        /// Constructs the object from the input data.
        /// </summary>
        /// <param name="inputData">Operator effects input data.</param>
        /// <param name="operatorPreconditions">Operator precoditions.</param>
        /// <param name="idManager">ID manager.</param>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public Effects(InputData.PDDL.Effects inputData, Conditions operatorPreconditions, IDManager idManager, EvaluationManager evaluationManager)
        {
            EffectsBuilder effectsBuilder = new EffectsBuilder(idManager);
            inputData.ForEach(inputEffect => Add(effectsBuilder.Build(inputEffect)));

            EffectsApplier = new Lazy<EffectsApplier>(() => new EffectsApplier(evaluationManager));
            EffectsRelevanceConditionsEvaluator = new Lazy<EffectsRelevanceConditionsEvaluator>(() => new EffectsRelevanceConditionsEvaluator(this, evaluationManager.GroundingManager));
            EffectsRelevanceRelativeStateEvaluator = new Lazy<EffectsRelevanceRelativeStateEvaluator>(() => new EffectsRelevanceRelativeStateEvaluator(this, evaluationManager.GroundingManager));
            EffectsBackwardsConditionsApplier = new Lazy<EffectsBackwardsConditionsApplier>(() => new EffectsBackwardsConditionsApplier(operatorPreconditions, this, evaluationManager));
            EffectsBackwardsRelativeStateApplier = new Lazy<EffectsBackwardsRelativeStateApplier>(() => new EffectsBackwardsRelativeStateApplier(operatorPreconditions, this, evaluationManager));
            EffectsResultAtomsCollector = new Lazy<EffectsResultAtomsCollector>(() => new EffectsResultAtomsCollector(this, evaluationManager.GroundingManager));
        }

        /// <summary>
        /// Sets the delete relaxation mode for the operator effects, i.e. negative effects are not applied.
        /// </summary>
        /// <param name="relaxed">Relaxed mode?</param>
        public void SetDeleteRelaxation(bool relaxed = true)
        {
            EffectsApplier.Value.SetDeleteRelaxation(relaxed);
        }

        /// <summary>
        /// Applies the operator effects to the given state. The result is a new state - successor.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="directlyModify">Should the input state be directly modified? (otherwise a new node is created)</param>
        /// <returns>Successor state to the given state.</returns>
        public IState Apply(IState state, ISubstitution substitution, bool directlyModify = false)
        {
            IState newState = (directlyModify) ? state : (IState)state.Clone();

            foreach (var effect in this)
            {
                EffectsApplier.Value.Apply(effect, newState, substitution);
            }

            return newState;
        }

        /// <summary>
        /// Checks whether the operator effects are relevant to the given target conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True if the operator effects are relevant to the given condititons, false otherwise.</returns>
        public bool IsRelevant(IConditions conditions, ISubstitution substitution, IList<int> relevantContionalEffects = null)
        {
            return EffectsRelevanceConditionsEvaluator.Value.Evaluate(conditions, substitution, new Substitution(), relevantContionalEffects);
        }

        /// <summary>
        /// Checks whether the operator is relevant to the given target relative state.
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="relevantContionalEffects">Output indices of relevant conditional effects (can be null).</param>
        /// <returns>True if the operator is relevant to the given relative state, false otherwise.</returns>
        public bool IsRelevant(IRelativeState relativeState, ISubstitution substitution, IList<int> relevantContionalEffects = null)
        {
            return EffectsRelevanceRelativeStateEvaluator.Value.Evaluate(relativeState, substitution, relevantContionalEffects);
        }

        /// <summary>
        /// Applies the operator backwards to the given target conditions. The result is a new set of conditions.
        /// </summary>
        /// <param name="conditions">Target conditions.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Preceding conditions.</returns>
        public IConditions ApplyBackwards(IConditions conditions, ISubstitution substitution)
        {
            return EffectsBackwardsConditionsApplier.Value.ApplyBackwards(conditions, substitution);
        }

        /// <summary>
        /// Applies the operator backwards to the given target relative state. The result is a new relative state (or more relative states, if conditional effects are present).
        /// </summary>
        /// <param name="relativeState">Target relative state.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Preceding relative states.</returns>
        public IEnumerable<Planner.IRelativeState> ApplyBackwards(IRelativeState relativeState, ISubstitution substitution)
        {
            return EffectsBackwardsRelativeStateApplier.Value.ApplyBackwards(relativeState, substitution);
        }

        /// <summary>
        /// Gets a list of atoms that are made true by the application of these effects, i.e. only simple positive effects are wanted.
        /// </summary>
        /// <param name="substitution">Variable substitution.</param>
        /// <returns>List of result effect atoms.</returns>
        public List<IAtom> GetResultAtoms(ISubstitution substitution)
        {
            return EffectsResultAtomsCollector.Value.Collect(substitution);
        }
    }
}
