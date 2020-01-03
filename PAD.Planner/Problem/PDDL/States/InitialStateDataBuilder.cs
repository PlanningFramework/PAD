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
        public HashSet<IAtom> Predicates { set; get; }

        /// <summary>
        /// Collection of numeric function values in the initial state.
        /// </summary>
        public Dictionary<IAtom, double> NumericFunctions { set; get; }

        /// <summary>
        /// Collection of object function values in the initial state.
        /// </summary>
        public Dictionary<IAtom, int> ObjectFunctions { set; get; }

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Constructs the initial state data builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public InitialStateDataBuilder(IdManager idManager)
        {
            IdManager = idManager;
        }

        /// <summary>
        /// Builds PDDL initial state data.
        /// </summary>
        /// <param name="init">Initial state input data.</param>
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
            int predicateNameId = IdManager.Predicates.GetId(data.Name, data.Terms.Count);
            List<ITerm> terms = GetTerms(data.Terms);

            Predicates.Add(new Atom(predicateNameId, terms));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.EqualsNumericFunctionInitElement data)
        {
            int functionNameId = IdManager.Functions.GetId(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = GetTerms(data.Function.Terms);

            NumericFunctions.Add(new Atom(functionNameId, terms), data.Number);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.EqualsObjectFunctionInitElement data)
        {
            int functionNameId = IdManager.Functions.GetId(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = GetTerms(data.Function.Terms);
            int valueTermId = IdManager.Constants.GetId(data.Term.Name);

            ObjectFunctions.Add(new Atom(functionNameId, terms), valueTermId);
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
                terms.Add(new ConstantTerm(IdManager.Constants.GetId(term.Name), IdManager));
            }
            return terms;
        }
    }
}
