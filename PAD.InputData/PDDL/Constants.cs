using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL constants.
    /// </summary>
    public class Constants : List<Constant>, IVisitable
    {
        /// <summary>
        /// Adds a new constant. If the given constant has been defined, then only the new possible constant type is added to the definition.
        /// </summary>
        /// <param name="constant">Constant definition to be added.</param>
        public new void Add(Constant constant)
        {
            var existingConstant = Find(presentConstant => presentConstant.ConstantName.Equals(constant.ConstantName));
            if (existingConstant != null)
            {
                foreach (var constType in constant.TypeNames)
                {
                    if (!existingConstant.TypeNames.Contains(constType))
                    {
                        existingConstant.TypeNames.Add(constType);
                    }
                }
            }
            else
            {
                base.Add(constant);
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":constants");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(constant => constant.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constant.
    /// </summary>
    public class Constant : IVisitable
    {
        /// <summary>
        /// Name of the constant.
        /// </summary>
        public string ConstantName { set; get; } = "";

        /// <summary>
        /// Name of the constant type(s).
        /// </summary>
        public List<string> TypeNames { set; get; } = new List<string>();

        /// <summary>
        /// Specification of a default constant type.
        /// </summary>
        public const string DEFAULT_TYPE = "object";

        /// <summary>
        /// Constructs a constant.
        /// </summary>
        /// <param name="constantName">Constant name.</param>
        /// <param name="typeNames">Constant type(s). Default type used if not specified.</param>
        public Constant(string constantName, string[] typeNames)
        {
            ConstantName = constantName;
            TypeNames.AddRange(typeNames.GetTypesOrDefault(DEFAULT_TYPE));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ConstantName + TypeNames.ToTypeSpecString();
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
