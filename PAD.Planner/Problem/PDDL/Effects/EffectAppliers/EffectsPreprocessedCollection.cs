using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure for collecting and preprocessing of used operator effects.
    /// </summary>
    public class EffectsPreprocessedCollection : IEffectVisitor
    {
        /// <summary>
        /// Positive predicate effects.
        /// </summary>
        public List<IAtom> PositivePredicateEffects { set; get; } = new List<IAtom>();

        /// <summary>
        /// Negative predicate effects.
        /// </summary>
        public List<IAtom> NegativePredicateEffects { set; get; } = new List<IAtom>();

        /// <summary>
        /// Numeric function assignments.
        /// </summary>
        public List<NumericAssignEffect> NumericFunctionAssignmentEffects { set; get; } = new List<NumericAssignEffect>();

        /// <summary>
        /// Object function assignments.
        /// </summary>
        public List<ObjectAssignEffect> ObjectFunctionAssignmentEffects { set; get; } = new List<ObjectAssignEffect>();

        /// <summary>
        /// Forall effects.
        /// </summary>
        public List<ForallEffect> ForallEffects { set; get; } = new List<ForallEffect>();

        /// <summary>
        /// When effects.
        /// </summary>
        public List<WhenEffect> WhenEffects { set; get; } = new List<WhenEffect>();

        /// <summary>
        /// Grounded positive predicate effects by the current variable substitution.
        /// </summary>
        public HashSet<IAtom> GroundedPositivePredicateEffects { set; get; } = new HashSet<IAtom>();

        /// <summary>
        /// Grounded negative predicate effects by the current variable substitution.
        /// </summary>
        public HashSet<IAtom> GroundedNegativePredicateEffects { set; get; } = new HashSet<IAtom>();

        /// <summary>
        /// Mapping of the grounded predicates in the effects to the original lifted ones.
        /// </summary>
        public Dictionary<IAtom, IAtom> OriginalLiftedPredicates { set; get; } = new Dictionary<IAtom, IAtom>();

        /// <summary>
        /// Grounded numeric function assignement effects.
        /// </summary>
        public Dictionary<IAtom, INumericExpression> GroundedNumericFunctionAssignmentEffects { set; get; } = new Dictionary<IAtom, INumericExpression>();

        /// <summary>
        /// Grounded object function assignments effects.
        /// </summary>
        public Dictionary<IAtom, ITerm> GroundedObjectFunctionAssignmentEffects { set; get; } = new Dictionary<IAtom, ITerm>();

        /// <summary>
        /// Mapping of the grounded functions in the effects to the original lifted ones.
        /// </summary>
        public Dictionary<IAtom, IAtom> OriginalLiftedFunctions { set; get; } = new Dictionary<IAtom, IAtom>();

        /// <summary>
        /// Is the current effect being negated?
        /// </summary>
        private bool IsNegated { set; get; } = false;

        /// <summary>
        /// Collects primitive effects used in the effects list into the containers.
        /// </summary>
        /// <param name="effects">Effects list.</param>
        public void CollectEffects(List<IEffect> effects)
        {
            foreach (var effect in effects)
            {
                effect.Accept(this);
            }
        }

        /// <summary>
        /// Grounds all the cached effects with the currently use operator substitution.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="operatorSubstitution">Variables substitution.</param>
        public void GroundEffectsByCurrentOperatorSubstitution(GroundingManager groundingManager, ISubstitution operatorSubstitution)
        {
            OriginalLiftedPredicates.Clear();
            OriginalLiftedFunctions.Clear();

            GroundedPositivePredicateEffects.Clear();
            foreach (var predicate in PositivePredicateEffects)
            {
                var groundedPredicate = operatorSubstitution.IsEmpty() ? predicate : groundingManager.GroundAtom(predicate, operatorSubstitution);
                GroundedPositivePredicateEffects.Add(groundedPredicate);
                if (!OriginalLiftedPredicates.ContainsKey(groundedPredicate))
                {
                    OriginalLiftedPredicates.Add(groundedPredicate, predicate);
                }
            }

            GroundedNegativePredicateEffects.Clear();
            foreach (var predicate in NegativePredicateEffects)
            {
                var groundedPredicate = operatorSubstitution.IsEmpty() ? predicate : groundingManager.GroundAtom(predicate, operatorSubstitution);
                GroundedNegativePredicateEffects.Add(groundedPredicate);
                if (!OriginalLiftedPredicates.ContainsKey(groundedPredicate))
                {
                    OriginalLiftedPredicates.Add(groundedPredicate, predicate);
                }
            }

            GroundedNumericFunctionAssignmentEffects.Clear();
            foreach (var numericFunc in NumericFunctionAssignmentEffects)
            {
                var groundedFunction = operatorSubstitution.IsEmpty() ? numericFunc.FunctionAtom : groundingManager.GroundAtom(numericFunc.FunctionAtom, operatorSubstitution);
                var groundedAssignment = operatorSubstitution.IsEmpty() ? numericFunc.GetBackwardsSubstituedValue() : groundingManager.GroundNumericExpression(numericFunc.GetBackwardsSubstituedValue(), operatorSubstitution);
                GroundedNumericFunctionAssignmentEffects.Add(groundedFunction, groundedAssignment);
                if (!OriginalLiftedFunctions.ContainsKey(groundedFunction))
                {
                    OriginalLiftedFunctions.Add(groundedFunction, numericFunc.FunctionAtom);
                }
            }

            GroundedObjectFunctionAssignmentEffects.Clear();
            foreach (var objectFunc in ObjectFunctionAssignmentEffects)
            {
                var groundedFunction = operatorSubstitution.IsEmpty() ? objectFunc.FunctionAtom : groundingManager.GroundAtom(objectFunc.FunctionAtom, operatorSubstitution);
                var groundedAssignment = operatorSubstitution.IsEmpty() ? objectFunc.Value : groundingManager.GroundTerm(objectFunc.Value, operatorSubstitution);
                GroundedObjectFunctionAssignmentEffects.Add(groundedFunction, groundedAssignment);
                if (!OriginalLiftedFunctions.ContainsKey(groundedFunction))
                {
                    OriginalLiftedFunctions.Add(groundedFunction, objectFunc.FunctionAtom);
                }
            }
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ForallEffect effect)
        {
            ForallEffects.Add(effect);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(WhenEffect effect)
        {
            WhenEffects.Add(effect);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(PredicateEffect effect)
        {
            if (IsNegated)
            {
                NegativePredicateEffects.Add(effect.PredicateAtom);
            }
            else
            {
                PositivePredicateEffects.Add(effect.PredicateAtom);
            }
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
            NumericFunctionAssignmentEffects.Add(effect);
        }

        /// <summary>
        /// Visits and handles the effect.
        /// </summary>
        /// <param name="effect">Effect.</param>
        public void Visit(ObjectAssignEffect effect)
        {
            ObjectFunctionAssignmentEffects.Add(effect);
        }
    }
}
