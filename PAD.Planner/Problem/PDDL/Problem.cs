using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of the PDDL model of the planning problem.
    /// </summary>
    public class Problem : IProblem
    {
        /// <summary>
        /// Name of the PDDL domain.
        /// </summary>
        public string DomainName { set; get; } = "";

        /// <summary>
        /// Name of the PDDL problem.
        /// </summary>
        public string ProblemName { set; get; } = "";

        /// <summary>
        /// List of operators in the PDDL planning problem.
        /// </summary>
        public LiftedOperators Operators { set; get; } = null;

        /// <summary>
        /// Initial state of the PDDL problem.
        /// </summary>
        public IState InitialState { set; get; } = null;

        /// <summary>
        /// Goal conditions of the PDDL problem.
        /// </summary>
        public IConditions GoalConditions { set; get; } = null;

        /// <summary>
        /// Rigid relations (true in all states of the state space).
        /// </summary>
        public RigidRelations RigidRelations { set; get; } = null;

        /// <summary>
        /// Generator of successors and predecessors (forward and backward transitions) in the PDDL planning problem.
        /// </summary>
        private Lazy<TransitionsGenerator> TransitionsGenerator { set; get; } = null;

        /// <summary>
        /// Backup of the original input data, used e.g. for creation of a relaxed version of this planning problem.
        /// </summary>
        private InputData.PDDLInputData OriginalInputData { set; get; } = null;

        /// <summary>
        /// ID manager of the planning problem.
        /// </summary>
        public IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Evaluation manager of the planning problem.
        /// </summary>
        public EvaluationManager EvaluationManager { set; get; } = null;

        /// <summary>
        /// Constructs the PDDL planning problem from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public Problem(InputData.PDDLInputData inputData)
        {
            DomainName = inputData.Domain.Name;
            ProblemName = inputData.Problem.Name;
            OriginalInputData = inputData;

            IDManager = new IDManager(inputData);
            EvaluationManager = new EvaluationManager(new GroundingManager(inputData, IDManager));

            Operators = new LiftedOperators(inputData.Domain.Actions, IDManager, EvaluationManager);
            InitialState = new State(inputData.Problem.Init, IDManager);
            GoalConditions = new Conditions(inputData.Problem.Goal, null, IDManager, EvaluationManager);
            RigidRelations = new RigidRelations(InitialState, Operators);
            TransitionsGenerator = new Lazy<TransitionsGenerator>(() => new TransitionsGenerator(this));

            EvaluationManager.SetRigidRelations(RigidRelations);
        }

        /// <summary>
        /// Constructs the PDDL planning problem from the specified input files.
        /// </summary>
        /// <param name="domainFilePath">Input domain file.</param>
        /// <param name="problemFilePath">Input problem file.</param>
        /// <param name="validateData">Should the loaded data be validated?</param>
        /// <returns></returns>
        public Problem(string domainFilePath, string problemFilePath, bool validateData = true)
            : this(new InputData.PDDLInputData(domainFilePath, problemFilePath, validateData))
        {
        }

        /// <summary>
        /// Gets the planning problem name.
        /// </summary>
        /// <returns>The name of the planning problem.</returns>
        public string GetDomainName()
        {
            return DomainName;
        }

        /// <summary>
        /// Gets the planning problem name.
        /// </summary>
        /// <returns>The name of the planning problem.</returns>
        public string GetProblemName()
        {
            return ProblemName;
        }

        /// <summary>
        /// Gets the input file path of the planning problem.
        /// </summary>
        /// <returns>Input file path.</returns>
        public string GetInputFilePath()
        {
            return OriginalInputData.Problem.FilePath;
        }

        /// <summary>
        /// Gets the initial state of the planning problem.
        /// </summary>
        /// <returns>Initial state.</returns>
        public Planner.IState GetInitialState()
        {
            return InitialState;
        }

        /// <summary>
        /// Sets the initial state of the planning problem.
        /// </summary>
        /// <param name="state">Initial state.</param>
        public virtual void SetInitialState(Planner.IState state)
        {
            InitialState = (IState)state;
            
            // note: RigidRelations shouldn't be reseted according to the new initial state - we assume that the new state
            // is a correct state from this planning problem and such state shouldn't contain any initial rigid relations
        }

        /// <summary>
        /// Gets the goal conditions of the planning problem.
        /// </summary>
        /// <returns>Goal conditions.</returns>
        public Planner.IConditions GetGoalConditions()
        {
            return GoalConditions;
        }

        /// <summary>
        /// Sets the goal conditions of the planning problem.
        /// </summary>
        /// <param name="conditions">Goal conditions.</param>
        public virtual void SetGoalConditions(Planner.IConditions conditions)
        {
            GoalConditions = (IConditions)conditions;
        }

        /// <summary>
        /// Checks whether the specified state is meeting goal conditions of the planning problem.
        /// </summary>
        /// <param name="state">A state to be checked.</param>
        /// <returns>True if the given state is a goal state of the problem, false otherwise.</returns>
        public bool IsGoalState(Planner.IState state)
        {
            return GoalConditions.Evaluate((IState)state);
        }

        /// <summary>
        /// Checks whether the initial state of the planning problem is meeting specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions to be checked.</param>
        /// <returns>True if the given conditions are satisfied for the initial state of the problem, false otherwise.</returns>
        public bool IsStartConditions(Planner.IConditions conditions)
        {
            return conditions.Evaluate(InitialState);
        }

        /// <summary>
        /// Checks whether the initial state of the planning problem is meeting conditions specified by the given relative state.
        /// </summary>
        /// <param name="relativeState">Relative state to be checked.</param>
        /// <returns>True if the given relative state is satisfied for the initial state of the problem, false otherwise.</returns>
        public bool IsStartRelativeState(Planner.IRelativeState relativeState)
        {
            return relativeState.Evaluate(InitialState);
        }

        /// <summary>
        /// Gets the number of not accomplished goals for the specified state (forward search).
        /// </summary>
        /// <param name="state">State to be evalatuated.</param>
        /// <returns>Number of not accomplished goals.</returns>
        public int GetNotAccomplishedGoalsCount(Planner.IState state)
        {
            return GoalConditions.GetNotAccomplishedConstraintsCount((IState)state);
        }

        /// <summary>
        /// Gets the number of not accomplished goals for the specified conditions (backward search).
        /// </summary>
        /// <param name="conditions">Conditions to be evalatuated.</param>
        /// <returns>Number of not accomplished goals.</returns>
        public int GetNotAccomplishedGoalsCount(Planner.IConditions conditions)
        {
            return ((IConditions)conditions).GetNotAccomplishedConstraintsCount(InitialState);
        }

        /// <summary>
        /// Gets a collection of all possible successors (forward transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetSuccessors(Planner.IState state)
        {
            return TransitionsGenerator.Value.GetSuccessors((IState)state);
        }

        /// <summary>
        /// Enumeration method getting a list with a limited number of possible successors (forward transitions) from the specified state. The
        /// next call of this method returns new successors, until all of them are returned - then an empty collection is returned to signalize
        /// the end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <param name="numberOfSuccessors">Number of successors to be returned.</param>
        /// <returns>Lazy generated collection of successors.</returns>
        public IEnumerable<ISuccessor> GetNextSuccessors(Planner.IState state, int numberOfSuccessors)
        {
            return TransitionsGenerator.Value.GetNextSuccessors((IState)state, numberOfSuccessors);
        }

        /// <summary>
        /// Gets a random successor (forward transition) from the specified state.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Random successor from the given state. Null if no valid successor found.</returns>
        public ISuccessor GetRandomSuccessor(Planner.IState state)
        {
            return TransitionsGenerator.Value.GetRandomSuccessor((IState)state);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified conditions. Lazy generated via yield return.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(Planner.IConditions conditions)
        {
            return TransitionsGenerator.Value.GetPredecessors((IConditions)conditions);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(Planner.IState state)
        {
            return TransitionsGenerator.Value.GetPredecessors((IState)state);
        }

        /// <summary>
        /// Gets a collection of all relevant predecessors (backwards transitions) from the specified relative state. Lazy generated via yield return.
        /// </summary>
        /// <param name="relativeState">Original state.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetPredecessors(Planner.IRelativeState relativeState)
        {
            return TransitionsGenerator.Value.GetPredecessors((IRelativeState)relativeState);
        }

        /// <summary>
        /// Enumeration method getting a list with a limited number of relevant predecessors (backward transitions) from the specified conditions. The
        /// next call of this method returns new predecessors, until all of them are returned - then an empty collection is returned to signalize the
        /// end of enumeration. The next call starts the enumeration again from the beginning. The returned collection is lazy evaluated.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <param name="numberOfPredecessors">Number of predecessors to be returned.</param>
        /// <returns>Lazy generated collection of relevant predecessors.</returns>
        public IEnumerable<IPredecessor> GetNextPredecessors(Planner.IConditions conditions, int numberOfPredecessors)
        {
            return TransitionsGenerator.Value.GetNextPredecessors((IConditions)conditions, numberOfPredecessors);
        }

        /// <summary>
        /// Gets a random relevant predecessor (backwards transition) from the specified conditions.
        /// </summary>
        /// <param name="conditions">Original conditions.</param>
        /// <returns>Random relevant predecessor from the specified conditions. Null if no valid predecessor found.</returns>
        public IPredecessor GetRandomPredecessor(Planner.IConditions conditions)
        {
            return TransitionsGenerator.Value.GetRandomPredecessor((IConditions)conditions);
        }

        /// <summary>
        /// Gets a collection of all explicly enumerated successor states (created by forward applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all successor states.</returns>
        public IEnumerable<Planner.IState> GetSuccessorStates(Planner.IState state)
        {
            return TransitionsGenerator.Value.GetSuccessorStates((IState)state);
        }

        /// <summary>
        /// Gets a collection of all explicly enumerated predecessor states (created by relevant backwards applications) from the specified state. Lazy generated via yield return.
        /// </summary>
        /// <param name="state">Original state.</param>
        /// <returns>Lazy generated collection of all predecessor states.</returns>
        public IEnumerable<Planner.IState> GetPredecessorStates(Planner.IState state)
        {
            return TransitionsGenerator.Value.GetPredecessorStates((IState)state);
        }

        /// <summary>
        /// Creates the relaxed version of the current planning problem.
        /// </summary>
        /// <returns>Relaxed planning problem.</returns>
        public IRelaxedProblem GetRelaxedProblem()
        {
            return new RelaxedProblem(OriginalInputData);
        }

        /// <summary>
        /// Creates the pattern database of the abstracted planning problem.
        /// </summary>
        /// <param name="findAdditivePatterns">Should the additive patterns be automatically found?</param>
        /// <param name="patternHints">Explicitly requested patterns for the database (only if findAdditivePatterns is false).</param>
        /// <returns>Pattern database of the abstracted planning problem.</returns>
        public IPatternDatabase GetPatternDatabase(bool findAdditivePatterns = true, List<HashSet<int>> patternHints = null)
        {
            throw new NotSupportedException("Pattern database is not supported for the PDDL.");
        }

        /// <summary>
        /// Gets the initial node of the planning problem.
        /// </summary>
        /// <returns>Initial node.</returns>
        public ISearchNode GetInitialNode()
        {
            return GetInitialState();
        }

        /// <summary>
        /// Checks whether the specified node is meeting goal conditions of the planning problem.
        /// </summary>
        /// <param name="node">Node to be checked.</param>
        /// <returns>True if the given node is a goal node of the problem, false otherwise.</returns>
        public bool IsGoalNode(ISearchNode node)
        {
            return ((IStateOrConditions)node).DetermineGoalNode(this);
        }

        /// <summary>
        /// Gets an enumerator of all possible transitions from the specified node. Lazy generated via yield return.
        /// </summary>
        /// <param name="node">Original node.</param>
        /// <returns>Lazy generated collection of transitions.</returns>
        public IEnumerable<ITransition> GetTransitions(ISearchNode node)
        {
            return ((IStateOrConditions)node).DetermineTransitions(this);
        }
    }
}
