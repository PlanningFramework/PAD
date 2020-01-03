// ReSharper disable PossibleUnintendedReferenceComparison

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Standard implementation of an variable-value assignment.
    /// </summary>
    public class Assignment : IAssignment
    {
        /// <summary>
        /// Variable index.
        /// </summary>
        public int Variable { set; get; }

        /// <summary>
        /// Assigned value.
        /// </summary>
        public int Value { set; get; }

        /// <summary>
        /// Invalid value constant.
        /// </summary>
        public const int InvalidValue = -1;

        /// <summary>
        /// Constructs the assigned value.
        /// </summary>
        /// <param name="variable">Variable index.</param>
        /// <param name="value">Assigned value.</param>
        public Assignment(int variable, int value)
        {
            Variable = variable;
            Value = value;
        }

        /// <summary>
        /// Constructs the assigned value from the input data.
        /// </summary>
        /// <param name="assignment">Input data assignment.</param>
        public Assignment(InputData.SAS.Assignment assignment) : this(assignment.Variable, assignment.Value)
        {
        }

        /// <summary>
        /// Gets the assignment variable.
        /// </summary>
        /// <returns>Assignment variable.</returns>
        public int GetVariable()
        {
            return Variable;
        }

        /// <summary>
        /// Gets the assigned value.
        /// </summary>
        /// <returns>Assigned value.</returns>
        public int GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Makes a deep copy of the assigned value.
        /// </summary>
        /// <returns>Deep copy of the assigned value.</returns>
        public IAssignment Clone()
        {
            return new Assignment(Variable, Value);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{Variable} = {Value}";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Variable, Value);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            Assignment other = obj as Assignment;
            if (other == null)
            {
                return false;
            }

            return (Variable == other.Variable && Value == other.Value);
        }

        /// <summary>
        /// Checks the equality with other assigned value.
        /// </summary>
        /// <param name="other">Other assigned value.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(IAssignment other)
        {
            if (other == this)
            {
                return true;
            }
            return (Variable == other.GetVariable() && Value == other.GetValue());
        }
    }
}
