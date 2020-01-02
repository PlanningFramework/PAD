﻿using System.Collections.Generic;
using ConstantID = System.Int32;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Terms grounder returning grounded terms and atoms for the given substitution. All variable instances specified in the given
    /// variable substitution are replaced by the constants. Object function terms remain intact, if the standard ground methods are called.
    /// When "deep" grounding methods are called, the grounding is performed replacing even object function terms with the values from the
    /// specified reference state.
    /// </summary>
    public class TermsGrounder : ITermTransformVisitor
    {
        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Reference state.
        /// </summary>
        private IState ReferenceState { set; get; } = null;

        /// <summary>
        /// ID manager.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the grounding object.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public TermsGrounder(IDManager idManager)
        {
            IDManager = idManager;
        }

        /// <summary>
        /// Grounds the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTerm(ITerm term, ISubstitution substitution)
        {
            Substitution = substitution;
            ReferenceState = null;
            return term.Accept(this);
        }

        /// <summary>
        /// Grounds the term. This version does "deep" grounding, in the sense that even object function terms are 
        /// grounded into constant terms (the value of the object function term is getted from the given reference state).
        /// </summary>
        /// <param name="term">Term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Grounded term.</returns>
        public ITerm GroundTermDeep(ITerm term, ISubstitution substitution, IState referenceState)
        {
            Substitution = substitution;
            ReferenceState = referenceState;
            return term.Accept(this);
        }

        /// <summary>
        /// Grounds the atom.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtom(IAtom atom, ISubstitution substitution)
        {
            List<ITerm> groundedTerms = new List<ITerm>();
            atom.GetTerms().ForEach(term => groundedTerms.Add(GroundTerm(term, substitution)));
            return new Atom(atom.GetNameID(), groundedTerms);
        }

        /// <summary>
        /// Grounds the atom. The "deep" version of terms grouding is used.
        /// </summary>
        /// <param name="atom">Function or predicate atom.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Grounded atom.</returns>
        public IAtom GroundAtomDeep(IAtom atom, ISubstitution substitution, IState referenceState)
        {
            List<ITerm> groundedTerms = new List<ITerm>();
            atom.GetTerms().ForEach(term => groundedTerms.Add(GroundTermDeep(term, substitution, referenceState)));
            return new Atom(atom.GetNameID(), groundedTerms);
        }

        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        public ITerm Visit(ConstantTerm term)
        {
            return term.Clone();
        }

        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        public ITerm Visit(VariableTerm term)
        {
            ConstantID substituedValue = IDManager.INVALID_ID;
            if (Substitution.TryGetValue(term.NameID, out substituedValue))
            {
                return new ConstantTerm(substituedValue, IDManager);
            }
            return term.Clone();
        }

        /// <summary>
        /// Transforms the term.
        /// </summary>
        /// <param name="term">Term.</param>
        /// <returns>Transformed term.</returns>
        public ITerm Visit(ObjectFunctionTerm term)
        {
            if (ReferenceState != null)
            {
                IAtom groundedFunctionAtom = GroundAtomDeep(term.FunctionAtom, Substitution, ReferenceState);
                return new ConstantTerm(ReferenceState.GetObjectFunctionValue(groundedFunctionAtom), IDManager);
            }

            return new ObjectFunctionTerm(GroundAtom(term.FunctionAtom, Substitution), IDManager);
        }
    }
}
