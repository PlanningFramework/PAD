using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Initial state data builder converting input data into PDDL state data.
    /// </summary>
    public class InitialStateDataBuilder : InputData.PDDL.BaseVisitor
    {
        /// <summary>
        /// Set of predicates in the initial state.
        /// </summary>
        public HashSet<IAtom> Predicates { set; get; } = null;

        /// <summary>
        /// Collection of numeric function values in the initial state.
        /// </summary>
        public Dictionary<IAtom, double> NumericFunctions { set; get; } = null;

        /// <summary>
        /// Collection of object function values in the intitial state.
        /// </summary>
        public Dictionary<IAtom, int> ObjectFunctions { set; get; } = null;

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the initial state data builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public InitialStateDataBuilder(IDManager idManager)
        {
            IDManager = idManager;
        }

        /// <summary>
        /// Builds PDDL inital state data.
        /// </summary>
        /// <param name="init">Inital state input data.</param>
        public void Build(InputData.PDDL.Init init)
        {
            Predicates = new HashSet<IAtom>();
            NumericFunctions = new Dictionary<IAtom, double>();
            ObjectFunctions = new Dictionary<IAtom, int>();

            foreach (var initElement in init)
            {
                initElement.Accept(this);
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.PredicateInitElement data)
        {
            int predicateNameID = IDManager.Predicates.GetID(data.Name, data.Terms.Count);
            List<ITerm> terms = GetTerms(data.Terms);

            Predicates.Add(new Atom(predicateNameID, terms));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.EqualsNumericFunctionInitElement data)
        {
            int functionNameID = IDManager.Functions.GetID(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = GetTerms(data.Function.Terms);

            NumericFunctions.Add(new Atom(functionNameID, terms), data.Number);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.EqualsObjectFunctionInitElement data)
        {
            int functionNameID = IDManager.Functions.GetID(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = GetTerms(data.Function.Terms);
            int valueTermID = IDManager.Constants.GetID(data.Term.Name);

            ObjectFunctions.Add(new Atom(functionNameID, terms), valueTermID);
        }

        /// <summary>
        /// Converts input constant terms into a corresponding list of terms.
        /// </summary>
        /// <param name="inputTerms">Input data terms.</param>
        /// <returns>List of terms.</returns>
        private List<ITerm> GetTerms(InputData.PDDL.ConstantTerms inputTerms)
        {
            List<ITerm> terms = new List<ITerm>();
            foreach (var term in inputTerms)
            {
                terms.Add(new ConstantTerm(IDManager.Constants.GetID(term.Name), IDManager));
            }
            return terms;
        }
    }
}
