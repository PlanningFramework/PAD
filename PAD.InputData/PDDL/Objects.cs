using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL objects.
    /// </summary>
    public class Objects : List<Object>, IVisitable
    {
        /// <summary>
        /// Adds a new object. If the given object has been defined, then only the new possible object type is added to the definition.
        /// </summary>
        /// <param name="newObject">Object definition to be added.</param>
        public new void Add(Object newObject)
        {
            var existingObject = Find(presentObject => presentObject.ObjectName.Equals(newObject.ObjectName));
            if (existingObject != null)
            {
                foreach (var objectType in newObject.TypeNames)
                {
                    if (!existingObject.TypeNames.Contains(objectType))
                    {
                        existingObject.TypeNames.Add(objectType);
                    }
                }
            }
            else
            {
                base.Add(newObject);
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":objects");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(obj => obj.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL object.
    /// </summary>
    public class Object : IVisitable
    {
        /// <summary>
        /// Name of the object.
        /// </summary>
        public string ObjectName { set; get; }

        /// <summary>
        /// Name of the object type(s).
        /// </summary>
        public List<string> TypeNames { set; get; } = new List<string>();

        /// <summary>
        /// Specification of a default type.
        /// </summary>
        public const string DefaultType = "object";

        /// <summary>
        /// Constructs an object.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="typeNames">Object type(s). Default type used if not specified.</param>
        public Object(string objectName, string[] typeNames)
        {
            ObjectName = objectName;
            TypeNames.AddRange(typeNames.GetTypesOrDefault(DefaultType));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ObjectName + TypeNames.ToTypeSpecString();
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
