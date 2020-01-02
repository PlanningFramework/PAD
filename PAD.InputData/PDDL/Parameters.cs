using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL parameters.
    /// </summary>
    public class Parameters : List<Parameter>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString("", false);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(parameter => parameter.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL parameter.
    /// </summary>
    public class Parameter : IVisitable
    {
        /// <summary>
        /// Name of the parameter.
        /// </summary>
        public string ParameterName { set; get; } = "";

        /// <summary>
        /// Name of the parameter type(s).
        /// </summary>
        public List<string> TypeNames { set; get; } = new List<string>();

        /// <summary>
        /// Specification of a default parameter type.
        /// </summary>
        public const string DEFAULT_TYPE = "object";

        /// <summary>
        /// Constructs a parameter.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="typeNames">Parameter type(s). Default type used if not specified.</param>
        public Parameter(string parameterName, string[] typeNames)
        {
            ParameterName = parameterName;
            TypeNames.AddRange(typeNames.GetTypesOrDefault(DEFAULT_TYPE));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ParameterName + TypeNames.ToTypeSpecString();
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
