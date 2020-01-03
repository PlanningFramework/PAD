using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL types.
    /// </summary>
    public class Types : List<Type>, IVisitable
    {
        /// <summary>
        /// Adds a new type. If the given type has been defined, then the base type is aggregated with the new base type.
        /// </summary>
        /// <param name="type">Type definition to be added.</param>
        public new void Add(Type type)
        {
            var existingType = Find(presentType => presentType.TypeName.Equals(type.TypeName));
            if (existingType != null)
            {
                foreach (var baseType in type.BaseTypeNames)
                {
                    if (!existingType.BaseTypeNames.Contains(baseType))
                    {
                        existingType.BaseTypeNames.Add(baseType);
                    }
                }
            }
            else
            {
                base.Add(type);
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":types");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(type => type.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL type.
    /// </summary>
    public class Type : IVisitable
    {
        /// <summary>
        /// Name of the type.
        /// </summary>
        public string TypeName { set; get; }

        /// <summary>
        /// Name of the base types, from which the type is derived. Can be a single or multiple primitive types, e.g. the form
        /// of 'typeC - (either typeA typeB)'.
        /// </summary>
        public List<string> BaseTypeNames { set; get; } = new List<string>();

        /// <summary>
        /// Specification of a default base type.
        /// </summary>
        public const string DefaultBaseType = "object";

        /// <summary>
        /// Constructs a type.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <param name="baseTypeNames">Base type(s). Default type used if not specified.</param>
        public Type(string typeName, string[] baseTypeNames = null)
        {
            TypeName = typeName;
            BaseTypeNames.AddRange(baseTypeNames.GetTypesOrDefault(DefaultBaseType));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return TypeName + BaseTypeNames.ToTypeSpecString();
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
