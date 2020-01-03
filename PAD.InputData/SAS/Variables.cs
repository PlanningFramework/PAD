using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for SAS+ variables definition.
    /// </summary>
    public class Variables : List<Variable>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines(Count, Utilities.JoinLines(this));
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(variable => variable.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single SAS+ variable.
    /// </summary>
    public class Variable : IVisitable
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Variable axiom layer. Affects the order of axiomatic inference. Equals -1 for non-axiomatic variables.
        /// </summary>
        public int AxiomLayer { set; get; }

        /// <summary>
        /// Is the variable of axiomatic type?
        /// </summary>
        /// <returns>True if the variable is axiomatic. False otherwise.</returns>
        public bool IsAxiomatic() { return AxiomLayer != -1; }

        /// <summary>
        /// Domain range of the variable, e.g. a variable with domain range of 4 gets values from {0, 1, 2, 3}.
        /// </summary>
        public int GetDomainRange() { return Values.Count; }

        /// <summary>
        /// List of available values for this variable. Strings represent a symbolic meaning of the corresponding numerical values.
        /// The number of items gives the actual domain range of the variable.
        /// </summary>
        public List<string> Values { set; get; }

        /// <summary>
        /// Constructs the variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="axiomLayer">Variable axiom layer.</param>
        /// <param name="values">List of available values.</param>
        public Variable(string name, int axiomLayer, List<string> values)
        {
            Name = name;
            AxiomLayer = axiomLayer;
            Values = values;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_variable", Name, AxiomLayer, GetDomainRange(), Utilities.JoinLines(Values), "end_variable");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
