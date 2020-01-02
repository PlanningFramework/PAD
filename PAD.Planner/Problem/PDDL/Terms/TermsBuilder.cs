using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Terms builder converting input data terms into PDDL terms.
    /// </summary>
    public class TermsBuilder : InputData.PDDL.BaseVisitor
    {
        /// <summary>
        /// Stack of term parts.
        /// </summary>
        private Stack<ITerm> TermStack { set; get; } = new Stack<ITerm>();

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the terms builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public TermsBuilder(IDManager idManager)
        {
            IDManager = idManager;
        }

        /// <summary>
        /// Builds PDDL term from the input data.
        /// </summary>
        /// <param name="term">Input data term.</param>
        /// <returns>Built term.</returns>
        public ITerm Build(InputData.PDDL.Term term)
        {
            Debug.Assert(TermStack.Count == 0);
            TermStack.Clear();

            term.Accept(this);

            Debug.Assert(TermStack.Count == 1);
            return TermStack.Pop();
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.ConstantTerm data)
        {
            int constantNameID = IDManager.Constants.GetID(data.Name);
            TermStack.Push(new ConstantTerm(constantNameID, IDManager));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.VariableTerm data)
        {
            int variableNameID = IDManager.Variables.GetID(data.Name);
            TermStack.Push(new VariableTerm(variableNameID));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ObjectFunctionTerm data)
        {
            int functionNameID = IDManager.Functions.GetID(data.Name, data.Terms.Count);

            List<ITerm> argumentTerms = new List<ITerm>();
            for (int i = 0; i < data.Terms.Count; ++i)
            {
                argumentTerms.Add(TermStack.Pop());
            }

            TermStack.Push(new ObjectFunctionTerm(new Atom(functionNameID, argumentTerms), IDManager));
        }
    }
}
