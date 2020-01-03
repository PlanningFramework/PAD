using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Structure for grounding of lifted atoms, terms, expressions and conditions.
    /// </summary>
    public class Grounder
    {
        /// <summary>
        /// Terms and atoms grounder.
        /// </summary>
        private Lazy<TermsGrounder> TermsAndAtomsGrounder { get; }

        /// <summary>
        /// Expressions grounder.
        /// </summary>
        private Lazy<ExpressionsGrounder> ExpressionsGrounder { get; }

        /// <summary>
        /// Numeric expression grounder.
        /// </summary>
        private Lazy<NumericExpressionsGrounder> NumericExpressionsGrounder { get; }

        /// <summary>
        /// Conditions grounder.
        /// </summary>
        private Lazy<ConditionsGrounder> ConditionsGrounder { get; }

        /// <summary>
        /// Constructs the grounder object.
        /// </summary>
        public Grounder(IdManager idManager)
        {
            TermsAndAtomsGrounder = new Lazy<TermsGrounder>(() => new TermsGrounder(idManager));
            NumericExpressionsGrounder = new Lazy<NumericExpressionsGrounder>(() => new NumericExpressionsGrounder(TermsAndAtomsGrounder, idManager));
            ExpressionsGrounder = new Lazy<ExpressionsGrounder>(() => new ExpressionsGrounder(TermsAndAtomsGrounder, NumericExpressionsGrounder, idManager));
            ConditionsGrounder = new Lazy<ConditionsGrounder>(() => new ConditionsGrounder(ExpressionsGrounder));
        }

        /// <summary>
        /// Grounds the specified atom by the given substitution and returns it.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtom(IAtom atom, ISubstitution substitution)
        {
            return TermsAndAtomsGrounder.Value.GroundAtom(atom, substitution);
        }

        /// <summary>
        /// Grounds the specified atom by the given substitution and returns it. The "deep" version of terms grounding is used.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="state">Reference state.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtomDeep(IAtom atom, ISubstitution substitution, IState state)
        {
            return TermsAndAtomsGrounder.Value.GroundAtomDeep(atom, substitution, state);
        }

        /// <summary>
        /// Grounds the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTerm(ITerm term, ISubstitution substitution)
        {
            return TermsAndAtomsGrounder.Value.GroundTerm(term, substitution);
        }

        /// <summary>
        /// Grounds the term. This version does "deep" grounding, in the sense that even object function terms are 
        /// grounded into constant terms (the value of the object function term is gotten from the given reference state).
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTermDeep(ITerm term, ISubstitution substitution, IState referenceState)
        {
            return TermsAndAtomsGrounder.Value.GroundTermDeep(term, substitution, referenceState);
        }

        /// <summary>
        /// Grounds the expression by the given substitution and returns it.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>Grounded expression.</returns>
        public IExpression GroundExpression(IExpression expression, ISubstitution substitution)
        {
            return ExpressionsGrounder.Value.Ground(expression, substitution);
        }

        /// <summary>
        /// Grounds the numeric expression by the given substitution and returns it.
        /// </summary>
        /// <param name="numericExpression">Numeric expression.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>Grounded numeric expression.</returns>
        public INumericExpression GroundNumericExpression(INumericExpression numericExpression, ISubstitution substitution)
        {
            return NumericExpressionsGrounder.Value.Ground(numericExpression, substitution);
        }

        /// <summary>
        /// Grounds the given conditions by the substitution. Variables in the conditions that are not contained in the substitution remain lifted and are added to the
        /// conditions parameters. The result is partially grounded/lifted conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="substitution">Substitution.</param>
        /// <returns>(Partially) grounded conditions.</returns>
        public Conditions GroundConditions(Conditions conditions, ISubstitution substitution)
        {
            return ConditionsGrounder.Value.Ground(conditions, substitution);
        }
    }
}
