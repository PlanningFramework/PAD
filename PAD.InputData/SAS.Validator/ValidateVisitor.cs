using System.Collections.Generic;
using System;

namespace PAD.InputData.SAS.Validator
{
    /// <summary>
    /// Visitor for the SAS+ input data, checking consistency of the input data. That includes: version check, variable parameters, mutex
    /// groups and initial state consistency, operator and axiom rules ability to affect variables etc. Throws ValidationException in case
    /// of a validation failure.
    /// </summary>
    public class ValidateVisitor : BaseVisitor
    {
        /// <summary>
        /// Problem context.
        /// </summary>
        private Problem ProblemContext { set; get; }

        /// <summary>
        /// Location specification in the input data (in case of a validation failure).
        /// </summary>
        private string Location { set; get; } = "";

        /// <summary>
        /// Checks the given problem data.
        /// </summary>
        /// <param name="problem">Problem input data.</param>
        public void CheckProblem(Problem problem)
        {
            ProblemContext = problem;
            problem.Accept(this);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Version data)
        {
            Location = "Version";

            if (data.Number < 1 || data.Number > 3)
            {
                throw GetException("SAS+ input data needs to be at most version 3. Newer formats are not currently supported.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Variable data)
        {
            Location = $"Variable '{data.Name}'";

            foreach (var variable in ProblemContext.Variables)
            {
                if (variable == data)
                {
                    continue;
                }

                if (variable.Name.Equals(data.Name))
                {
                    throw GetException($"Duplicate variable name {data.Name} used.");
                }
            }

            if (data.AxiomLayer < -1)
            {
                throw GetException("Axiom layer specification needs to be a non-negative number (or -1 for non-axiomatic variable).");
            }

            if (data.GetDomainRange() == 0)
            {
                throw GetException("Variable cannot have an empty domain range.");
            }

            HashSet<string> uniqueValues = new HashSet<string>(data.Values);
            if (uniqueValues.Count != data.Values.Count)
            {
                throw GetException("Some values of the variable do not have a unique name.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(MutexGroup data)
        {
            Location = "Mutex groups";

            HashSet<Tuple<int, int>> uniqueFacts = new HashSet<Tuple<int, int>>();
            data.ForEach(mutexFact => uniqueFacts.Add(Tuple.Create(mutexFact.Variable, mutexFact.Value)));

            if (uniqueFacts.Count != data.Count)
            {
                throw GetException("Some mutex group contains duplicated mutex facts, which is invalid.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InitialState data)
        {
            Location = "Initial state";

            if (data.Count != ProblemContext.Variables.Count)
            {
                throw GetException("The number of initial values does not match the variables count.");
            }

            int variableIndex = 0;
            foreach (var value in data)
            {
                CheckValueAssignment(variableIndex++, value);
            }

            foreach (var mutexGroup in ProblemContext.MutexGroups)
            {
                bool isLocked = false;
                foreach (var mutexFact in mutexGroup)
                {
                    if (data[mutexFact.Variable] == mutexFact.Value)
                    {
                        if (isLocked)
                        {
                            throw GetException("Initial values do not comply with defined mutex groups constraints.");
                        }
                        isLocked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Operator data)
        {
            Location = $"Operator '{data.Name}'";

            if (data.Cost < 0)
            {
                throw GetException("Operator cost needs to be non-negative.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Effect data)
        {
            if (ProblemContext.Variables[data.GetAffectedVariable()].IsAxiomatic())
            {
                throw GetException($"Operator effect cannot affect an axiomatic variable {data.PrimitiveEffect.Variable}.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AxiomRule data)
        {
            Location = "Axiom rules";

            if (!ProblemContext.Variables[data.GetAffectedVariable()].IsAxiomatic())
            {
                throw GetException($"Axiom rule cannot affect a non-axiomatic variable {data.PrimitiveEffect.Variable}.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Assignment data)
        {
            CheckVariable(data.Variable);
            CheckValueAssignment(data.Variable, data.Value);
        }

        /// <summary>
        /// Checks the used variable. Throws ValidationException if the variable is not defined.
        /// </summary>
        /// <param name="variable">Variable to be checked.</param>
        private void CheckVariable(int variable)
        {
            if (variable < 0 || variable >= ProblemContext.Variables.Count)
            {
                throw GetException($"Variable {variable} is not defined.");
            }
        }

        /// <summary>
        /// Checks the used variable-value assignment. Throws ValidationException if the value cannot be assigned to the variable.
        /// </summary>
        /// <param name="variable">Variable to be assigned.</param>
        /// <param name="value">Value to be checked.</param>
        private void CheckValueAssignment(int variable, int value)
        {
            if (value < 0 || value >= ProblemContext.Variables[variable].GetDomainRange())
            {
                throw GetException($"Value {value} does not comply with the domain range of variable {variable}.");
            }
        }

        /// <summary>
        /// Gets the validation exception to be thrown, with the location in the input data (if specified).
        /// </summary>
        /// <param name="reason">Reason of a validation failure.</param>
        /// <returns>Validation exception to be thrown.</returns>
        private ValidationException GetException(string reason)
        {
            string location = (Location.Length != 0) ? $"{Location}: " : "";
            return new ValidationException(location + reason);
        }
    }
}
