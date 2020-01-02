using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL functions.
    /// </summary>
    public class Functions : List<Function>, IVisitable
    {
        /// <summary>
        /// Checks whether the list contains the specified function. That function needs to have the same name,
        /// the terms types and the return type.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="numberOfTerms">Number of function terms.</param>
        /// <param name="returnType">Return type of the function.</param>
        /// <returns>True if the function list contains the specified function. False otherwise.</returns>
        public bool ContainsFunction(string functionName, int numberOfTerms, bool numericFunction)
        {
            return Exists(definition =>
                definition.Name.EqualsNoCase(functionName) &&
                definition.Terms.Count == numberOfTerms &&
                ((numericFunction) ? definition.IsNumbericFunction() : !definition.IsNumbericFunction()));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":functions");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(function => function.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL function.
    /// </summary>
    public class Function : IVisitable
    {
        /// <summary>
        /// Name of the function.
        /// </summary>
        public string Name { set; get; } = "";

        /// <summary>
        /// Terms of the function.
        /// </summary>
        public DefinitionTerms Terms { set; get; } = new DefinitionTerms();

        /// <summary>
        /// Type(s) of the function return value.
        /// </summary>
        public List<string> ReturnValueTypes { set; get; } = new List<string>();

        /// <summary>
        /// Checks whether the function is of numeric type (i.e. the return type is 'number').
        /// </summary>
        /// <returns>True if the function is numeric.</returns>
        public bool IsNumbericFunction()
        {
            return (ReturnValueTypes.Count == 1 && ReturnValueTypes[0].EqualsNoCase("number"));
        }

        /// <summary>
        /// Specification of a default function return type.
        /// </summary>
        public const string DEFAULT_RETURN_TYPE = "number";

        /// <summary>
        /// Constructs a function.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="returnValueTypes">Function return value type(s).</param>
        public Function(string name, string[] returnValueTypes)
        {
            Name = name;
            ReturnValueTypes.AddRange(returnValueTypes.GetTypesOrDefault(DEFAULT_RETURN_TYPE));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ((Terms.Count > 0) ? $"({Name} {Terms})" : $"({Name})") + ReturnValueTypes.ToTypeSpecString();
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Terms.Accept(visitor);
        }
    }
}
