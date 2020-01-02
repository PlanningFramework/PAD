using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    public static class Grounder
    {
        private static ExpressionTermGrounder TermGrounder { set; get; } = new ExpressionTermGrounder();

        public static IGroundedAtom GroundAtom(IAtom atom, ISubstitution substitution, State referenceState)
        {
            List<int> argumentsList = new List<int>();
            foreach (var term in atom.GetTerms())
            {
                argumentsList.Add(TermGrounder.Ground(term, substitution, referenceState));
            }
            return new GroundedAtom(atom.GetNameID(), argumentsList);
        }

        public static int GroundTerm(IExpressionTerm term, ISubstitution substitution, State referenceState)
        {
            return TermGrounder.Ground(term, substitution, referenceState);
        }
    }

    /// <summary>
    /// Expression terms grounder returning grounded constant term ID for the given substitution.
    /// </summary>
    public class ExpressionTermGrounder : IExpressionTermVisitor
    {
        /// <summary>
        /// Stack of grounded term parts.
        /// </summary>
        private Stack<int> TermStack { set; get; } = new Stack<int>();

        /// <summary>
        /// Reference state.
        /// </summary>
        private State ReferenceState { set; get; } = null;

        /// <summary>
        /// Variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Grounds the PDDL term.
        /// </summary>
        /// <param name="term">Input data term.</param>
        /// <param name="substitution">Variables substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>Grounded term, i.e. constant name ID.</returns>
        public int Ground(IExpressionTerm term, ISubstitution substitution, State referenceState)
        {
            Debug.Assert(TermStack.Count == 0);
            TermStack.Clear();
            Substitution = substitution;
            ReferenceState = referenceState;

            term.Accept(this);

            Substitution = null;
            ReferenceState = null;
            Debug.Assert(TermStack.Count == 1);
            return TermStack.Pop();
        }

        /// <summary>
        /// Visits expression term.
        /// </summary>
        /// <param name="term">Expression term.</param>
        public void Visit(ConstantTerm term)
        {
            TermStack.Push(term.NameID);
        }

        /// <summary>
        /// Visits expression term.
        /// </summary>
        /// <param name="term">Expression term.</param>
        public void Visit(VariableTerm term)
        {
            TermStack.Push(Substitution.GetValue(term.NameID));
        }

        /// <summary>
        /// Visits expression term.
        /// </summary>
        /// <param name="term">Expression term.</param>
        public void Visit(ObjectFunctionTerm term)
        {
        }

        /// <summary>
        /// Visits expression term.
        /// </summary>
        /// <param name="term">Expression term.</param>
        public void PostVisit(ObjectFunctionTerm term)
        {
            List<int> argumentsList = new List<int>();
            for (int i = 0; i < term.Terms.Count; ++i)
            {
                argumentsList.Add(TermStack.Pop());
            }
            argumentsList.Reverse();

            IGroundedAtom groundedFunctionAtom = new GroundedAtom(term.NameID, argumentsList);

            int groundedConstantValue = ReferenceState.GetObjectFunctionValue(groundedFunctionAtom);
            TermStack.Push(groundedConstantValue);
        }
    }
}
