
namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for a single pair of variable and corresponding assigned value.
    /// </summary>
    public class Assignment : IVisitable
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
        /// Constructs an empty assigned value.
        /// </summary>
        public Assignment()
        {
        }

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
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{Variable} {Value}";
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
