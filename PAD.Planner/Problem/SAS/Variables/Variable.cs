using System.Collections.Generic;
using System.Linq;
using System;
// ReSharper disable IdentifierTypo

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for a single SAS+ variable.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Variable ID.
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Variable axiom layer. Affects the order of axiomatic inference. Equals -1 for non-axiomatic variables.
        /// </summary>
        public int AxiomLayer { set; get; }

        /// <summary>
        /// List of available values for this variable. Strings represent a symbolic meaning of the corresponding numerical values.
        /// The number of items gives the actual domain range of the variable.
        /// </summary>
        public List<string> Values { set; get; }

        /// <summary>
        /// Is the variable rigid (i.e. its value is never changed)? Testing rigidity is time-demanding, so it is performed via Lazy
        /// feature only once and then stored for a later usage. Call the check via IsRigid() method.
        /// </summary>
        private Lazy<bool> Rigidity { get; }

        /// <summary>
        /// Is the variable abstracted (i.e. can have multiple values at the same time)?
        /// </summary>
        public bool Abstracted { set; get; } = false;

        /// <summary>
        /// Reference to planning problem operators.
        /// </summary>
        private Operators Operators { set; get; }

        /// <summary>
        /// Constructs the variable.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="variableId">Variable ID.</param>
        public Variable(InputData.SAS.Variable inputData, int variableId)
        {
            Name = inputData.Name;
            Id = variableId;
            AxiomLayer = inputData.AxiomLayer;
            Values = new List<string>(inputData.Values);
            Rigidity = new Lazy<bool>(() => !(Operators.Any(oper => oper.GetEffects().Any(eff => eff.GetAssignment().GetVariable() == Id))));
        }

        /// <summary>
        /// Sets the reference to the planning problem operators.
        /// </summary>
        /// <param name="operators">Operators of the planning problem.</param>
        public void SetOperators(Operators operators)
        {
            Operators = operators;
        }

        /// <summary>
        /// Is the variable of axiomatic type?
        /// </summary>
        /// <returns>True if the variable is axiomatic. False otherwise.</returns>
        public bool IsAxiomatic()
        {
            return AxiomLayer != -1;
        }

        /// <summary>
        /// Is the variable rigid (i.e. its value is never changed)?
        /// </summary>
        /// <returns>True if the variable is rigid. False otherwise.</returns>
        public bool IsRigid()
        {
            return Rigidity.Value;
        }

        /// <summary>
        /// Is the current variable abstracted?
        /// </summary>
        /// <returns>True if the current variable is abstracted, false otherwise.</returns>
        public bool IsAbstracted()
        {
            return Abstracted;
        }

        /// <summary>
        /// Domain range of the variable, e.g. a variable with domain range of 4 gets values from {0, 1, 2, 3}.
        /// </summary>
        public int GetDomainRange()
        {
            return Values.Count;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string axiomInfo = IsAxiomatic() ? $"Axiomatic({AxiomLayer}), " : "";
            return $"{Name}: {axiomInfo}DomainRange = {GetDomainRange()}, Values = ({string.Join(", ", Values)})";
        }
    }
}
