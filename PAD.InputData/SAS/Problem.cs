
namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for a SAS+ problem.
    /// </summary>
    public class Problem : IVisitable
    {
        /// <summary>
        /// Problem name (extracted from the problem fileName).
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Problem version.
        /// </summary>
        public Version Version { set; get; } = new Version();

        /// <summary>
        /// Problem metric (action costs).
        /// </summary>
        public Metric Metric { set; get; } = new Metric();

        /// <summary>
        /// Problem variables.
        /// </summary>
        public Variables Variables { set; get; } = new Variables();

        /// <summary>
        /// Problem mutex groups.
        /// </summary>
        public MutexGroups MutexGroups { set; get; } = new MutexGroups();

        /// <summary>
        /// Problem initial state.
        /// </summary>
        public InitialState InitialState { set; get; } = new InitialState();

        /// <summary>
        /// Problem goal conditions.
        /// </summary>
        public GoalConditions GoalConditions { set; get; } = new GoalConditions();

        /// <summary>
        /// Problem operators.
        /// </summary>
        public Operators Operators { set; get; } = new Operators();

        /// <summary>
        /// Problem axiom rules.
        /// </summary>
        public AxiomRules AxiomRules { set; get; } = new AxiomRules();

        /// <summary>
        /// Path to the source SAS+ problem file.
        /// </summary>
        public string FilePath { set; get; } = "";

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines(Version, Metric, Variables, MutexGroups, InitialState, GoalConditions, Operators, AxiomRules);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Version.Accept(visitor);
            Metric.Accept(visitor);
            Variables.Accept(visitor);
            MutexGroups.Accept(visitor);
            InitialState.Accept(visitor);
            GoalConditions.Accept(visitor);
            Operators.Accept(visitor);
            AxiomRules.Accept(visitor);
        }
    }
}
